namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Threading;
	using Common.Logging;
	using Terrasoft.Core;

	/// <summary>
	/// APD-1464 live-wiring (RISK-bridge-live-wiring): process-wide front-end that
	/// makes the batched / backoff outbound path active end-to-end. The
	/// per-request <see cref="EventSender"/> no longer POSTs one HTTP request per
	/// event; instead it hands each event to this singleton, which buffers events
	/// into a single shared <see cref="EventBatchCollector"/> and flushes them on
	/// the <c>bulkSendPeriod</c> cadence (R7) through a single shared
	/// <see cref="BulkEventSender{TData}"/> — so the
	/// <see cref="OutboundThrottle"/> (R4, &lt;= 1000/min), the
	/// <see cref="RetryBackoffPolicy"/> (R1) and the <c>Retry-After</c> floor (R2)
	/// all apply across the whole process rather than per dispatch.
	///
	/// <para>
	/// Why a singleton: every entity change is dispatched on its own background
	/// task with its own short-lived <see cref="UserConnection"/>, so a per-call
	/// collector / throttle could never enforce a global rate ceiling or batch
	/// events from different changes together. One static instance gives one
	/// buffer, one sliding-window throttle, and one periodic flush timer for the
	/// whole application — which is exactly what stops the 429 retry storm.
	/// </para>
	///
	/// <para>
	/// The flush timer (a <see cref="Timer"/>) ticks at the bulkSendPeriod and
	/// drains any due batch. The HTTP send needs a <see cref="UserConnection"/>
	/// for SysSettings + the HTTP client. The per-call connection handed to
	/// <see cref="Enqueue"/> belongs to the entity-change dispatch background task
	/// (see <c>DispatchEntityChangeAsyncOperation</c>) and is disposed when that
	/// task ends — but the flush timer fires asynchronously, long afterward. So
	/// the batcher must not reuse that per-task connection. On the first enqueue
	/// it resolves the app-lifetime system connection
	/// (<c>AppConnection.SystemUserConnection</c>, the same one
	/// <c>TriggerCatalogAppEventListener</c> uses) and the timer thread always
	/// flushes on that long-lived connection.
	/// </para>
	///
	/// <para>
	/// The flush timer is a SELF-REARMING ONE-SHOT, not a periodic timer.
	/// <see cref="Timer"/> does not serialize callbacks, so a periodic timer would
	/// fire overlapping ticks on fresh threadpool threads while a flush is blocked
	/// in <see cref="BulkEventSender{TData}"/> backoff (<c>Thread.Sleep</c>, up to
	/// ~60s × attempts under sustained 429/5xx). Re-arming a one-shot only on tick
	/// exit (<see cref="ArmNextTick"/>) guarantees the next flush is scheduled
	/// after the current one returns, so flush threads cannot accumulate.
	/// </para>
	///
	/// <para>
	/// Scope caveat: the <see cref="OutboundThrottle"/> ceiling (R4, 1000/min) is
	/// PER APP INSTANCE. In a multi-node / web-farm deployment, N nodes admit up to
	/// N×1000/min, so the control plane can still observe more than 1000/min in
	/// aggregate and return 429 — which the backoff + Retry-After path then absorbs.
	/// A cluster-wide ceiling would require a shared (distributed) limiter and is
	/// out of scope for APD-1464.
	/// </para>
	/// </summary>
	internal sealed class OutboundEventBatcher
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		// Floor on the timer tick so a tiny / misconfigured bulkSendPeriod cannot
		// spin the flush thread; the collector still gates the actual flush on the
		// configured period, this only bounds how often we poll it.
		private static readonly TimeSpan MinTimerInterval = TimeSpan.FromSeconds(1);

		// Bounded budget for the shutdown drain so a graceful app-pool recycle /
		// redeploy / stop is never stalled by outbound throttle + backoff sleeps.
		// OnAppEnd runs FlushPendingForShutdown synchronously, and a flush of an
		// unhealthy endpoint would otherwise block ~MaxRetryAttempts × MaxDelay
		// (minutes) — long enough to trip the platform shutdown timeout. Past this
		// budget BulkEventSender logs the undelivered subset (observable) instead of
		// holding up shutdown. The periodic flush stays unbounded (null deadline).
		private static readonly TimeSpan ShutdownFlushBudget = TimeSpan.FromSeconds(5);

		private static OutboundEventBatcher _current = new OutboundEventBatcher();

		public static OutboundEventBatcher Current => _current;

		/// <summary>
		/// Test-only: replaces the process-wide instance with a fresh one so unit
		/// tests do not leak buffered events, a captured connection, or the flush
		/// timer across cases. The previous instance's timer is disposed.
		/// </summary>
		internal static void ResetForTests() {
			OutboundEventBatcher previous = _current;
			// No-op sleep + no flush timer so tests drive flushing deterministically
			// via FlushNow and never block on real backoff sleeps.
			_current = new OutboundEventBatcher(sleep: _ => { }, startTimer: false);
			previous?._flushTimer?.Dispose();
		}

		private readonly object _gate = new object();
		private readonly EventBatchCollector _collector;
		private readonly OutboundThrottle _throttle;
		private readonly Action<TimeSpan> _sleep;
		private readonly bool _startTimer;
		private volatile UserConnection _lastUserConnection;
		private BulkEventSender<object> _sender;
		private Timer _flushTimer;
		private TimeSpan _timerInterval;
		// Read once outside _gate as a fast-path in EnsureStarted; volatile so the
		// lock-free read sees the write published under _gate on weak memory models.
		private volatile bool _started;
		// Last DroppedEventCount we logged, so overflow drops are reported once per
		// flush window (as a delta) rather than once per dropped event — the buffer
		// can drop thousands/sec under a sustained block, which would flood the log.
		private long _lastReportedDropCount;

		private OutboundEventBatcher(Action<TimeSpan> sleep = null, bool startTimer = true) {
			_collector = new EventBatchCollector(startUtc: DateTime.UtcNow);
			_throttle = new OutboundThrottle();
			_sleep = sleep;
			_startTimer = startTimer;
		}

		/// <summary>
		/// Buffers one outbound trigger event and ensures the periodic flush timer
		/// is running. Safe to call from any dispatch background task; never
		/// throws into the caller (the previous per-event path swallowed send
		/// errors too, so a transient outbound problem must not fail the entity
		/// save that produced the change).
		/// </summary>
		public void Enqueue<TData>(EventOccurrenceRequest<TData> request, UserConnection userConnection) {
			if (request == null || userConnection == null) {
				return;
			}
			try {
				UserConnection flushConnection = ResolveLongLivedConnection(userConnection);
				_lastUserConnection = flushConnection;
				EnsureStarted(flushConnection);
				string eventId = Guid.NewGuid().ToString("N");
				// Drop is accounted on the collector (DroppedEventCount) and logged
				// as a bounded per-window delta from the flush tick, so a sustained
				// overflow does not flood the log with one line per dropped event.
				_collector.Add(new BridgeTriggerEvent(
					eventId,
					request.TriggerCode,
					request.Data,
					request.EventTypeCode,
					request.OccurredAt));
			} catch (Exception ex) {
				Log.Error("Failed to enqueue outbound trigger event for batching.", ex);
			}
		}

		private void EnsureStarted(UserConnection userConnection) {
			if (_started) {
				return;
			}
			lock (_gate) {
				if (_started) {
					return;
				}
				_sender = new BulkEventSender<object>(
					userConnection, throttle: _throttle, sleep: _sleep);
				TimeSpan period = AIPlatformHttpSenderBase.GetBulkSendPeriod(userConnection);
				_collector.SetBulkSendPeriod(period);
				_timerInterval = period < MinTimerInterval ? MinTimerInterval : period;
				if (_startTimer) {
					// Self-rearming one-shot (period = InfiniteTimeSpan): OnFlushTick
					// re-arms it on exit so ticks never overlap (see class remarks).
					// The collector itself decides whether a flush is actually due, so
					// an early tick is a cheap no-op.
					_flushTimer = new Timer(OnFlushTick, null, _timerInterval, Timeout.InfiniteTimeSpan);
				}
				_started = true;
				Log.Info($"Outbound trigger-event batcher started; flush cadence {period.TotalSeconds:0.##}s.");
			}
		}

		/// <summary>
		/// Test/diagnostic hook: drains and sends every buffered batch right now,
		/// regardless of whether the bulkSendPeriod has elapsed, using the most
		/// recently captured connection. Lets unit tests exercise the live
		/// enqueue → bulk-send path deterministically without waiting on the
		/// timer. No-op when nothing has been enqueued yet.
		/// </summary>
		internal void FlushNow() {
			// Diagnostic/test path: drain fully with no deadline, matching the
			// unbounded periodic flush.
			DrainAndSendAll(deadlineUtc: null);
		}

		/// <summary>
		/// Drains and sends every buffered batch immediately, regardless of the
		/// bulkSendPeriod window. Invoked from the application-end hook
		/// (<c>TriggerCatalogAppEventListener.OnAppEnd</c>) so a buffered window is
		/// not silently lost on a graceful app-pool recycle / redeploy / stop.
		/// Best-effort and time-bounded: the send path still applies throttle +
		/// backoff, but the drain is capped at <see cref="ShutdownFlushBudget"/> so
		/// it cannot stall the app-pool recycle if the endpoint is unhealthy — on a
		/// healthy shutdown this is a single quick flush. Anything not delivered
		/// inside the budget is logged by <see cref="BulkEventSender{TData}"/> rather
		/// than held onto. No-op when nothing has been enqueued. The buffer is
		/// in-memory, so an abrupt crash (no OnAppEnd) can still lose the current
		/// window — durable persistence is an APD-1464 follow-up.
		/// </summary>
		internal void FlushPendingForShutdown() {
			DrainAndSendAll(deadlineUtc: DateTime.UtcNow + ShutdownFlushBudget);
		}

		private void DrainAndSendAll(DateTime? deadlineUtc) {
			UserConnection userConnection = _lastUserConnection;
			BulkEventSender<object> sender = _sender;
			if (userConnection == null || sender == null) {
				return;
			}
			ReportBufferDropsIfAny();
			foreach (EventBatch batch in _collector.TryDrainAll(DateTime.UtcNow)) {
				FlushBatch(sender, batch, deadlineUtc);
			}
		}

		// Log overflow drops once per drain as a delta against the last reported
		// count. The collector accounts the drops (DroppedEventCount); the batcher
		// owns logging so the collector stays a pure, environment-free unit. Error
		// (not Warn): a non-empty delta means buffered triggers were lost because
		// the flush stalled long enough to fill the buffer to its hard cap.
		//
		// A flush-timer tick and a shutdown drain can call this concurrently, so the
		// delta computation and the advance of the _lastReportedDropCount high-water
		// mark must be atomic — a lock-free read/update would race and double-count
		// or miss drops, and a plain Interlocked.Exchange could regress the mark when
		// a thread that captured a smaller total writes after one that captured a
		// larger one. Compute under _gate; log outside it so the lock is not held
		// across the I/O.
		private void ReportBufferDropsIfAny() {
			long total = _collector.DroppedEventCount;
			long delta;
			lock (_gate) {
				delta = total - _lastReportedDropCount;
				if (delta <= 0) {
					return;
				}
				_lastReportedDropCount = total;
			}
			Log.Error($"Outbound trigger-event buffer at hard cap " +
				$"({EventBatchCollector.MaxBufferedEventsHardCap}); dropped {delta} " +
				$"trigger event(s) this window ({total} total since start). The flush " +
				"is stalled (sustained 429/5xx or unhealthy endpoint); durable requeue " +
				"of dropped events is an APD-1464 follow-up.");
		}

		private void OnFlushTick(object state) {
			try {
				UserConnection userConnection = _lastUserConnection;
				if (userConnection == null) {
					return;
				}
				// Re-read the SysSetting each tick so an operator change to
				// bulkSendPeriod takes effect without an app restart (R7). The timer
				// re-arms itself with _timerInterval in the finally below, so updating
				// the interval here is enough — the new cadence applies from the next
				// tick without churning the timer mid-flush.
				TimeSpan period = AIPlatformHttpSenderBase.GetBulkSendPeriod(userConnection);
				_collector.SetBulkSendPeriod(period);
				UpdateTimerInterval(period);
				BulkEventSender<object> sender = _sender;
				if (sender == null) {
					return;
				}
				ReportBufferDropsIfAny();
				foreach (EventBatch batch in _collector.TryDrainDue(DateTime.UtcNow)) {
					// Periodic flush is unbounded (null): under sustained 429/5xx the
					// non-reentrant timer waits out the backoff so the window drains,
					// rather than racing a deadline. Only the shutdown path is bounded.
					FlushBatch(sender, batch, deadlineUtc: null);
				}
			} catch (Exception ex) {
				Log.Error("Outbound trigger-event flush tick failed.", ex);
			} finally {
				// Schedule the next tick only now that this flush has returned, so
				// flush callbacks never overlap even when a flush blocks in backoff.
				ArmNextTick();
			}
		}

		// R7: keep the flush cadence in sync with the current bulkSendPeriod so a
		// runtime change actually alters how often we flush. The self-rearming
		// timer picks up the new interval on its next arm (OnFlushTick's finally).
		private void UpdateTimerInterval(TimeSpan period) {
			TimeSpan interval = period < MinTimerInterval ? MinTimerInterval : period;
			if (interval == _timerInterval) {
				return;
			}
			_timerInterval = interval;
			Log.Info($"Outbound trigger-event batcher flush cadence updated to {interval.TotalSeconds:0.##}s.");
		}

		// Re-arm the self-rearming one-shot flush timer for the next tick. Called
		// from OnFlushTick's finally so the next tick is scheduled only after the
		// current flush completes — System.Threading.Timer does not serialize
		// callbacks, so this is what keeps flushes non-reentrant.
		private void ArmNextTick() {
			if (!_startTimer) {
				return;
			}
			lock (_gate) {
				_flushTimer?.Change(_timerInterval, Timeout.InfiniteTimeSpan);
			}
		}

		private static void FlushBatch(BulkEventSender<object> sender, EventBatch batch, DateTime? deadlineUtc) {
			var items = new System.Collections.Generic.List<BulkEventOccurrenceItem<object>>(
				batch.Events.Count);
			foreach (BridgeTriggerEvent evt in batch.Events) {
				items.Add(new BulkEventOccurrenceItem<object> {
					EventId = evt.EventId,
					EventTypeCode = evt.EventTypeCode,
					OccurredAt = evt.OccurredAt,
					Data = evt.Payload,
				});
			}
			sender.SendBatch(batch.TriggerCode, items, deadlineUtc);
		}

		// The dispatch task's UserConnection is disposed when the task completes,
		// so the asynchronous flush timer cannot safely reuse it. Resolve the
		// app-lifetime system connection instead; fall back to the supplied
		// connection only if the AppConnection is unexpectedly unavailable.
		private static UserConnection ResolveLongLivedConnection(UserConnection userConnection) {
			AppConnection appConnection = userConnection.AppConnection;
			return appConnection?.SystemUserConnection ?? userConnection;
		}
	}
}

