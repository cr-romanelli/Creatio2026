namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading;
	using Common.Logging;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Requests;

	/// <summary>
	/// APD-1464 S5: sends a batch of trigger events to the additive bulk ingress
	/// endpoint and, on a partial-failure (207) or rate-limited (429) response,
	/// retries ONLY the rejected-and-retryable subset under
	/// <see cref="RetryBackoffPolicy"/> backoff that honors the server's
	/// <c>Retry-After</c> header (R2). Outbound requests are gated by
	/// <see cref="OutboundThrottle"/> so the bridge stays under the 1000/min
	/// ceiling (R4) rather than relying on server rejection.
	///
	/// <para>
	/// The send/HTTP orchestration here depends on the Creatio runtime
	/// (UserConnection / IHttpRequestClient) so it is delivered compile-only in
	/// this stage; the decision logic it composes
	/// (<see cref="RetryBackoffPolicy"/>, <see cref="OutboundThrottle"/>,
	/// <see cref="BulkRetryPlanner"/>, <see cref="RetryAfterParser"/>) is pure
	/// and unit-tested (T1/T2/T5).
	/// </para>
	/// </summary>
	internal sealed class BulkEventSender<TData> : AIPlatformHttpSenderBase
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		// Bound the number of backoff retries so a persistently failing subset
		// cannot loop forever; after this it is logged and dropped for the cycle.
		private const int MaxRetryAttempts = 6;

		// APD-1464: the control-plane bulk ingress rejects any batch larger than
		// its MaxBulkEventsPerRequest cap (500) wholesale with HTTP 400. The
		// collector can buffer well above that within one bulkSendPeriod window
		// (MaxBufferedEvents = 2000), so a single flushed per-trigger batch can
		// exceed the cap. Split into <= MaxEventsPerBulkRequest sub-batches here —
		// the single funnel point all flush paths go through — so an oversized
		// window is sent as several conformant requests instead of one rejected
		// 400 that the backoff loop then retries forever. Must stay <= the
		// server's MaxBulkEventsPerRequest in WebTriggerEndpoints.
		internal const int MaxEventsPerBulkRequest = 500;

		private readonly OutboundThrottle _throttle;
		private readonly RetryBackoffPolicy _backoff;
		private readonly Func<DateTime> _clock;
		private readonly Action<TimeSpan> _sleep;

		public BulkEventSender(
				UserConnection userConnection,
				OutboundThrottle throttle = null,
				RetryBackoffPolicy backoff = null,
				Func<DateTime> clock = null,
				Action<TimeSpan> sleep = null)
			: base(userConnection) {
			_throttle = throttle ?? new OutboundThrottle();
			_backoff = backoff ?? new RetryBackoffPolicy();
			_clock = clock ?? (() => DateTime.UtcNow);
			_sleep = sleep ?? DefaultSleep;
		}

		/// <summary>
		/// Sends <paramref name="items"/> for <paramref name="triggerCode"/> as
		/// one bulk request, retrying the rejected-and-retryable subset under
		/// backoff until it drains or <see cref="MaxRetryAttempts"/> is reached.
		/// </summary>
		public void SendBatch(string triggerCode, IReadOnlyList<BulkEventOccurrenceItem<TData>> items) {
			SendBatch(triggerCode, items, deadlineUtc: null);
		}

		/// <summary>
		/// Sends <paramref name="items"/> for <paramref name="triggerCode"/> as one
		/// bulk request, retrying the rejected-and-retryable subset under backoff
		/// until it drains or <see cref="MaxRetryAttempts"/> is reached, bounded by
		/// an optional <paramref name="deadlineUtc"/>.
		/// </summary>
		/// <param name="deadlineUtc">
		/// Optional wall-clock budget. When set, throttle waits and backoff sleeps
		/// are clamped so the total time spent here cannot exceed the deadline;
		/// whatever is still undelivered when the deadline passes is logged (so it
		/// stays observable) and abandoned rather than blocking the caller. The
		/// shutdown drain passes a short deadline so a graceful app-pool recycle /
		/// redeploy is never stalled by outbound retries (a flush of an unhealthy
		/// endpoint could otherwise block ~<see cref="MaxRetryAttempts"/> ×
		/// <see cref="RetryBackoffPolicy.MaxDelay"/>). <c>null</c> = unbounded — the
		/// normal periodic flush, where draining reliably matters more than latency.
		/// </param>
		public void SendBatch(
				string triggerCode,
				IReadOnlyList<BulkEventOccurrenceItem<TData>> items,
				DateTime? deadlineUtc) {
			if (string.IsNullOrWhiteSpace(triggerCode) || items == null || items.Count == 0) {
				return;
			}
			if (!TryCreateBaseUrl(Log, out string baseUrl, out string apiKey)) {
				return;
			}
			string url = baseUrl.TrimEnd('/') + Constants.ApiRoutes.TriggerEventsBulk(triggerCode);

			// Split a flushed window into server-conformant sub-batches so a batch
			// larger than the control-plane cap is never POSTed as one request
			// (which the server rejects wholesale with HTTP 400). Each chunk gets
			// its own backoff/retry budget.
			if (items.Count > MaxEventsPerBulkRequest) {
				for (int offset = 0; offset < items.Count; offset += MaxEventsPerBulkRequest) {
					int count = Math.Min(MaxEventsPerBulkRequest, items.Count - offset);
					var chunk = new List<BulkEventOccurrenceItem<TData>>(count);
					for (int i = 0; i < count; i++) {
						chunk.Add(items[offset + i]);
					}
					SendChunk(url, apiKey, triggerCode, chunk, deadlineUtc);
				}
				return;
			}

			SendChunk(url, apiKey, triggerCode, items, deadlineUtc);
		}

		private void SendChunk(
				string url, string apiKey, string triggerCode,
				IReadOnlyList<BulkEventOccurrenceItem<TData>> items,
				DateTime? deadlineUtc) {
			IReadOnlyList<BulkEventOccurrenceItem<TData>> pending = items;
			int attempt = 0;
			while (pending.Count > 0 && attempt <= MaxRetryAttempts) {
				if (DeadlineExceeded(deadlineUtc)) {
					// Shutdown budget spent before this attempt — stop so the recycle
					// is not stalled; the undelivered subset is logged below.
					break;
				}
				if (!WaitForThrottleSlot(deadlineUtc)) {
					// Could not acquire a throttle slot within the shutdown budget.
					break;
				}

				BulkSendOutcome outcome = PostBatch(url, apiKey, pending);
				if (outcome.TransportFailed) {
					if (attempt >= MaxRetryAttempts) {
						break;
					}
					attempt++;
					if (!SleepBeforeRetry(_backoff.GetDelay(attempt, null), deadlineUtc)) {
						break;
					}
					continue;
				}

				if (outcome.TerminalReject) {
					// Non-429 4xx (malformed/unauthorized/forbidden): retrying the
					// same batch cannot succeed and only amplifies load during a
					// misconfiguration. PostBatch already logged the HTTP cause; log
					// the consequence here with the affected event ids so the dropped
					// triggers are observable/recoverable from the logs rather than
					// vanishing silently (no durable dead-letter store yet — tracked
					// as an APD-1464 follow-up).
					Log.Error($"Dropping {pending.Count} undelivered trigger event(s) for " +
						$"'{triggerCode}' after a terminal (non-retryable) response from " +
						$"AI Studio. Event ids: {FormatEventIds(pending)}.");
					return;
				}

				if (outcome.RateLimited) {
					// Whole batch rejected by the limiter — honor Retry-After as
					// the minimum backoff (R2), then retry the whole batch (R1).
					// Budget already spent: stop now rather than sleeping one last
					// backoff that no send would follow (the while guard would exit
					// right after). `pending` is the whole batch here, so the give-up
					// log below already reports the correct undelivered set.
					if (attempt >= MaxRetryAttempts) {
						break;
					}
					attempt++;
					if (!SleepBeforeRetry(_backoff.GetDelay(attempt, outcome.RetryAfter), deadlineUtc)) {
						break;
					}
					continue;
				}

				if (outcome.FullyAccepted) {
					// Clean 2xx with no per-event body — whole batch accepted.
					return;
				}

				// Per-event adjudication: retry only the rejected-retryable subset.
				IReadOnlyList<BulkEventOccurrenceItem<TData>> retry =
					BulkRetryPlanner.ComputeRetrySet(pending, outcome.Response, e => e.EventId);
				if (retry.Count == 0) {
					return;
				}
				// Narrow `pending` to the rejected-retryable subset BEFORE the budget
				// check so the give-up log below reports the events that were actually
				// still undelivered, not the pre-adjudication batch.
				pending = retry;
				// Budget already spent: stop now rather than sleeping one last backoff
				// that no send would follow (the while guard would exit right after).
				if (attempt >= MaxRetryAttempts) {
					break;
				}
				attempt++;
				if (!SleepBeforeRetry(_backoff.GetDelay(attempt, null), deadlineUtc)) {
					break;
				}
			}

			if (pending.Count > 0) {
				// Error (not Warn): these triggers were NOT delivered. Log the event
				// ids so the undelivered subset is observable/recoverable from the
				// logs. Durable requeue/persistence of this subset is an APD-1464
				// follow-up (the buffer is in-memory for now).
				Log.Error($"Giving up on {pending.Count} undelivered trigger event(s) for " +
					$"'{triggerCode}' after {MaxRetryAttempts} backoff attempts. " +
					$"Event ids: {FormatEventIds(pending)}.");
			}
		}

		// Compact, bounded join of event ids for a drop/give-up log line so a large
		// dropped batch cannot produce an unbounded log entry.
		private static string FormatEventIds(IReadOnlyList<BulkEventOccurrenceItem<TData>> items) {
			const int max = 50;
			int take = Math.Min(items.Count, max);
			var ids = new List<string>(take);
			for (int i = 0; i < take; i++) {
				ids.Add(items[i].EventId);
			}
			string joined = string.Join(", ", ids);
			return items.Count > max ? $"{joined}, ... (+{items.Count - max} more)" : joined;
		}

		// True once the optional shutdown budget is spent. Unbounded (null) sends
		// never expire, so the normal periodic flush keeps draining as before.
		private bool DeadlineExceeded(DateTime? deadlineUtc) {
			return deadlineUtc.HasValue && _clock() >= deadlineUtc.Value;
		}

		// Blocks until a throttle slot is free. Returns false when the shutdown
		// deadline is reached first, so the caller stops instead of waiting out the
		// rate ceiling during a recycle.
		private bool WaitForThrottleSlot(DateTime? deadlineUtc) {
			TimeSpan retryAfter;
			while (!_throttle.TryAcquire(_clock(), out retryAfter)) {
				if (!SleepBeforeRetry(retryAfter, deadlineUtc)) {
					return false;
				}
			}
			return true;
		}

		// Sleeps before the next attempt, clamped to the optional shutdown deadline.
		// Returns false when the deadline is already reached or is reached by this
		// wait, signalling the caller to stop retrying so shutdown is not stalled.
		// With no deadline (the normal flush) it always sleeps the full delay and
		// returns true, preserving the original retry behavior.
		private bool SleepBeforeRetry(TimeSpan delay, DateTime? deadlineUtc) {
			if (!deadlineUtc.HasValue) {
				Sleep(delay);
				return true;
			}
			DateTime now = _clock();
			if (now >= deadlineUtc.Value) {
				return false;
			}
			TimeSpan remaining = deadlineUtc.Value - now;
			Sleep(delay < remaining ? delay : remaining);
			return _clock() < deadlineUtc.Value;
		}

		private BulkSendOutcome PostBatch(
				string url, string apiKey, IReadOnlyList<BulkEventOccurrenceItem<TData>> items) {
			var payload = new BulkEventOccurrenceRequest<TData> {
				Events = new List<BulkEventOccurrenceItem<TData>>(items)
			};
			string jsonBody = JsonConvert.SerializeObject(payload, JsonSettings);
			try {
				HttpRequestConfig config = CreateJsonRequest(HttpRequestMethod.POST, url);
				ApplyAuthorization(config, apiKey);
				IHttpResponse response = GetHttpClient().SendWithJsonBody(config, jsonBody);

				if (response.StatusCode == (HttpStatusCode)429) {
					TimeSpan? retryAfter = RetryAfterParser.TryParse(response.Headers, _clock());
					return BulkSendOutcome.Throttled(retryAfter);
				}

				if (!IsSuccessStatusCode(response.StatusCode)
						&& response.StatusCode != (HttpStatusCode)207) {
					int statusCode = (int)response.StatusCode;
					// 404 is a special case among 4xx: the bulk ingress route does not
					// exist (yet). The most likely cause is a deploy-ordering window
					// where this bridge is live but control-plane #2071 (which adds
					// /events/bulk) is not deployed. Treat it as TRANSIENT so the
					// buffered batch is retried under backoff across the window instead
					// of being dropped on the first hit (which is exactly the
					// lost-trigger symptom APD-1464 fixes). Deploy order: the
					// control-plane bulk endpoint must be live before this path is
					// enabled (the EnableAIPlatformOutboundTriggerDelivery feature flag
					// gates the whole outbound lane and should stay off until then).
					if (statusCode == 404) {
						Log.Warn($"AI Studio bulk ingress route '{url}' returned 404 " +
							"(endpoint not deployed yet?); will retry under backoff.");
						return BulkSendOutcome.Failed();
					}
					// Any other non-429 4xx (400 malformed, 401/403 auth) is a terminal
					// client error: backoff retries can never make it succeed and would
					// only amplify load during a misconfiguration, so stop. The dropped
					// subset is logged with its event ids by the caller so lost triggers
					// stay observable.
					if (statusCode >= 400 && statusCode < 500) {
						Log.Error($"AI Studio returned terminal HTTP {statusCode} " +
							$"for bulk event send to '{url}'; not retrying this batch.");
						return BulkSendOutcome.Terminal();
					}
					Log.Warn($"AI Studio returned HTTP {statusCode} " +
						$"for bulk event send to '{url}'.");
					return BulkSendOutcome.Failed();
				}

				BulkEventOccurrenceResponseDto parsed = DeserializeResponse(response);
				// A clean 2xx (non-207) with no per-event result body means the whole
				// batch was accepted — treat it as fully accepted rather than
				// retrying it as if the transport failed. Only a 207 Multi-Status
				// carries per-event verdicts that select a rejected subset.
				if (parsed == null && response.StatusCode != (HttpStatusCode)207) {
					return BulkSendOutcome.Accepted();
				}
				return BulkSendOutcome.Adjudicated(parsed);
			} catch (Exception ex) {
				Log.Error($"HTTP request to AI Studio failed for URL '{url}'.", ex);
				return BulkSendOutcome.Failed();
			}
		}

		private static BulkEventOccurrenceResponseDto DeserializeResponse(IHttpResponse response) {
			if (string.IsNullOrWhiteSpace(response?.Content)) {
				return null;
			}
			// Deserialized without JsonSettings: the Terrasoft.NewtonsoftWrapper facade
			// the config build references (APD-2106) exposes no
			// DeserializeObject(string, JsonSerializerSettings) overload. The settings
			// carried only NullValueHandling (serialize-only) and, formerly, a camelCase
			// resolver; neither affects reads here — the wire keys are matched to the
			// [DataMember] names case-insensitively by the default resolver.
			BulkEventOccurrenceResponseEnvelope envelope =
				JsonConvert.DeserializeObject<BulkEventOccurrenceResponseEnvelope>(
					response.Content);
			if (envelope?.Data != null) {
				return envelope.Data;
			}
			return JsonConvert.DeserializeObject<BulkEventOccurrenceResponseDto>(
				response.Content);
		}

		private void Sleep(TimeSpan delay) {
			if (delay > TimeSpan.Zero) {
				_sleep(delay);
			}
		}

		private static void DefaultSleep(TimeSpan delay) {
			Thread.Sleep(delay);
		}

		private sealed class BulkSendOutcome
		{
			public bool TransportFailed { get; private set; }
			public bool TerminalReject { get; private set; }
			public bool RateLimited { get; private set; }
			public bool FullyAccepted { get; private set; }
			public TimeSpan? RetryAfter { get; private set; }
			public BulkEventOccurrenceResponseDto Response { get; private set; }

			public static BulkSendOutcome Failed() {
				return new BulkSendOutcome { TransportFailed = true };
			}

			public static BulkSendOutcome Terminal() {
				return new BulkSendOutcome { TerminalReject = true };
			}

			public static BulkSendOutcome Throttled(TimeSpan? retryAfter) {
				return new BulkSendOutcome { RateLimited = true, RetryAfter = retryAfter };
			}

			public static BulkSendOutcome Accepted() {
				return new BulkSendOutcome { FullyAccepted = true };
			}

			public static BulkSendOutcome Adjudicated(BulkEventOccurrenceResponseDto response) {
				return new BulkSendOutcome { Response = response };
			}
		}
	}
}

