namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// APD-1464 S5 (R5): collects outbound trigger events fired within one
	/// <c>bulkSendPeriod</c> window and hands them off as a single bulk batch
	/// per trigger code, so a burst of record changes produces a few bulk
	/// requests instead of one request per event (which is what drove the
	/// retry storm under the 1000/min ceiling).
	///
	/// <para>
	/// Flush semantics: <see cref="TryDrainDue"/> returns the collected events
	/// grouped by trigger code once at least <c>bulkSendPeriod</c> has elapsed
	/// since the last flush (or when the buffer has reached <see cref="MaxBufferedEvents"/>,
	/// which forces an early drain so a sustained burst flushes promptly rather
	/// than waiting out the window). Each
	/// collected event is returned in exactly one drain and then removed from
	/// the buffer, giving exactly-once-per-flush delivery (R5). Pure logic with
	/// an injected clock — unit-testable without a Creatio environment.
	/// </para>
	/// </summary>
	internal sealed class EventBatchCollector
	{
		/// <summary>Default flush cadence (R7): 5 seconds.</summary>
		public static readonly TimeSpan DefaultBulkSendPeriod = TimeSpan.FromSeconds(5);

		/// <summary>
		/// Drain-trigger threshold (not a strict upper bound): once the buffer has
		/// reached this many events, the next <see cref="TryDrainDue"/> drains
		/// regardless of elapsed time so a sustained burst flushes promptly.
		/// Note <see cref="Add"/> does not block, so a burst between drain polls
		/// can momentarily push <see cref="PendingCount"/> above this value; the
		/// threshold bounds the steady-state backlog, not the instantaneous peak.
		/// </summary>
		public const int MaxBufferedEvents = 2000;

		/// <summary>
		/// Hard upper bound on the in-memory buffer (not just a drain trigger like
		/// <see cref="MaxBufferedEvents"/>). <see cref="Add"/> is non-blocking and
		/// the flush can stall for minutes under a sustained 429/5xx backoff, so
		/// without a ceiling a long flush-block during a burst would grow the buffer
		/// without bound. Once the buffer reaches this size, further <see cref="Add"/>
		/// calls drop the new event (returning <c>false</c>) and increment
		/// <see cref="DroppedEventCount"/> instead, making memory pressure
		/// deterministic with an explicit, observable drop policy. Durable
		/// requeue/persistence of dropped events is an APD-1464 follow-up; until then
		/// the bounded drop trades an unbounded memory leak (which would lose the
		/// whole buffer on OOM/crash) for a logged, bounded loss of the newest
		/// overflow. Set well above <see cref="MaxBufferedEvents"/> so it is only hit
		/// during a genuine sustained flush-block, not normal bursty traffic.
		/// </summary>
		public const int MaxBufferedEventsHardCap = 10000;

		private readonly object _gate = new object();
		private readonly List<BridgeTriggerEvent> _buffer = new List<BridgeTriggerEvent>();
		private TimeSpan _bulkSendPeriod;
		private DateTime _lastFlushUtc;
		private long _droppedEventCount;

		public EventBatchCollector(TimeSpan? bulkSendPeriod = null, DateTime? startUtc = null) {
			_bulkSendPeriod = Normalize(bulkSendPeriod ?? DefaultBulkSendPeriod);
			_lastFlushUtc = startUtc ?? DateTime.UtcNow;
		}

		/// <summary>Current flush cadence.</summary>
		public TimeSpan BulkSendPeriod {
			get { lock (_gate) { return _bulkSendPeriod; } }
		}

		/// <summary>
		/// Updates the flush cadence at runtime (driven by the bulkSendPeriod
		/// SystemSetting). A non-positive value falls back to the 5s default.
		/// </summary>
		public void SetBulkSendPeriod(TimeSpan period) {
			lock (_gate) {
				_bulkSendPeriod = Normalize(period);
			}
		}

		/// <summary>
		/// Buffers one event for the next flush window. Non-blocking. Returns
		/// <c>true</c> when the event was buffered, or <c>false</c> when the buffer
		/// is already at <see cref="MaxBufferedEventsHardCap"/> and the event was
		/// dropped (and <see cref="DroppedEventCount"/> incremented) so a stalled
		/// flush cannot grow memory without bound. The caller is responsible for
		/// logging drops — the collector stays pure (no logging dependency) so it
		/// remains unit-testable without a Creatio environment.
		/// </summary>
		public bool Add(BridgeTriggerEvent evt) {
			if (evt == null) {
				throw new ArgumentNullException(nameof(evt));
			}
			lock (_gate) {
				if (_buffer.Count >= MaxBufferedEventsHardCap) {
					_droppedEventCount++;
					return false;
				}
				_buffer.Add(evt);
				return true;
			}
		}

		/// <summary>Count of currently buffered (not-yet-flushed) events.</summary>
		public int PendingCount {
			get { lock (_gate) { return _buffer.Count; } }
		}

		/// <summary>
		/// Cumulative count of events dropped by <see cref="Add"/> because the buffer
		/// was at <see cref="MaxBufferedEventsHardCap"/>. Monotonic for the lifetime
		/// of the collector; the owning batcher logs the delta so drops stay
		/// observable.
		/// </summary>
		public long DroppedEventCount {
			get { lock (_gate) { return _droppedEventCount; } }
		}

		/// <summary>
		/// If a flush is due at <paramref name="utcNow"/> (period elapsed or
		/// buffer cap reached), removes all buffered events, groups them by
		/// trigger code, resets the flush clock, and returns the batches.
		/// Returns an empty list when no flush is due or the buffer is empty so
		/// the caller can poll cheaply. Each event is returned in exactly one
		/// drain (R5 exactly-once-per-flush).
		/// </summary>
		public IReadOnlyList<EventBatch> TryDrainDue(DateTime utcNow) {
			lock (_gate) {
				bool periodElapsed = (utcNow - _lastFlushUtc) >= _bulkSendPeriod;
				bool capReached = _buffer.Count >= MaxBufferedEvents;
				if (!periodElapsed && !capReached) {
					return EmptyBatches;
				}

				_lastFlushUtc = utcNow;
				if (_buffer.Count == 0) {
					return EmptyBatches;
				}

				var byTrigger = new Dictionary<string, List<BridgeTriggerEvent>>(StringComparer.Ordinal);
				foreach (BridgeTriggerEvent evt in _buffer) {
					string key = evt.TriggerCode ?? string.Empty;
					List<BridgeTriggerEvent> bucket;
					if (!byTrigger.TryGetValue(key, out bucket)) {
						bucket = new List<BridgeTriggerEvent>();
						byTrigger[key] = bucket;
					}
					bucket.Add(evt);
				}
				_buffer.Clear();

				var batches = new List<EventBatch>(byTrigger.Count);
				foreach (KeyValuePair<string, List<BridgeTriggerEvent>> pair in byTrigger) {
					batches.Add(new EventBatch(pair.Key, pair.Value));
				}
				return batches;
			}
		}

		/// <summary>
		/// Drains every buffered event immediately, grouped by trigger code,
		/// regardless of whether the flush period has elapsed. Resets the flush
		/// clock to <paramref name="utcNow"/>. Intended for forced flushes (e.g. a
		/// deterministic test flush or an app-shutdown drain) so nothing is left
		/// buffered. Takes the clock as a parameter (like <see cref="TryDrainDue"/>)
		/// so the shutdown-drain path stays deterministic under test. Exactly-once
		/// still holds: drained events are removed from the buffer.
		/// </summary>
		public IReadOnlyList<EventBatch> TryDrainAll(DateTime utcNow) {
			lock (_gate) {
				_lastFlushUtc = utcNow;
				if (_buffer.Count == 0) {
					return EmptyBatches;
				}
				var byTrigger = new Dictionary<string, List<BridgeTriggerEvent>>(StringComparer.Ordinal);
				foreach (BridgeTriggerEvent evt in _buffer) {
					string key = evt.TriggerCode ?? string.Empty;
					List<BridgeTriggerEvent> bucket;
					if (!byTrigger.TryGetValue(key, out bucket)) {
						bucket = new List<BridgeTriggerEvent>();
						byTrigger[key] = bucket;
					}
					bucket.Add(evt);
				}
				_buffer.Clear();
				var batches = new List<EventBatch>(byTrigger.Count);
				foreach (KeyValuePair<string, List<BridgeTriggerEvent>> pair in byTrigger) {
					batches.Add(new EventBatch(pair.Key, pair.Value));
				}
				return batches;
			}
		}

		private static readonly IReadOnlyList<EventBatch> EmptyBatches = new EventBatch[0];

		private static TimeSpan Normalize(TimeSpan period) {
			return period <= TimeSpan.Zero ? DefaultBulkSendPeriod : period;
		}
	}

	/// <summary>
	/// One buffered outbound trigger event. <see cref="EventId"/> is a
	/// bridge-assigned correlation id used to match the per-event bulk response
	/// so only rejected events are retried (exactly-once per R5 / RISK3).
	/// <see cref="Payload"/> is the serialized event-occurrence body.
	/// </summary>
	internal sealed class BridgeTriggerEvent
	{
		public BridgeTriggerEvent(string eventId, string triggerCode, object payload,
				string eventTypeCode = null, DateTime occurredAt = default(DateTime)) {
			EventId = eventId;
			TriggerCode = triggerCode;
			Payload = payload;
			EventTypeCode = eventTypeCode;
			OccurredAt = occurredAt;
		}

		public string EventId { get; }
		public string TriggerCode { get; }
		public object Payload { get; }

		// APD-1464: carried through the batch so the per-event bulk item keeps the
		// same eventTypeCode / occurredAt the single-event ingress sent. Dropping
		// them would lose the event-type binding on the bulk lane.
		public string EventTypeCode { get; }
		public DateTime OccurredAt { get; }
	}

	/// <summary>A flushed group of events sharing one trigger code.</summary>
	internal sealed class EventBatch
	{
		public EventBatch(string triggerCode, IReadOnlyList<BridgeTriggerEvent> events) {
			TriggerCode = triggerCode;
			Events = events;
		}

		public string TriggerCode { get; }
		public IReadOnlyList<BridgeTriggerEvent> Events { get; }
	}
}

