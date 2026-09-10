namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Threading;
	using Common.Logging;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;

	/// <summary>
	/// Transport for the single-event ingress route
	/// (<c>/api/v1/trigger/{triggerCode}/events</c>). Used for resume-mode triggers
	/// (e.g. <c>creatio_process_completion</c>), which the control-plane bulk ingress
	/// rejects with <c>RESUME_NOT_SUPPORTED_IN_BULK</c>. <see cref="EventSender"/>
	/// resolves and routes to this transport; the bulk counterpart is
	/// <see cref="BulkEventSender{TData}"/>.
	///
	/// <para>
	/// A resume event resumes a suspended workflow, so silently dropping one on a
	/// transient failure would leave the workflow stuck until it expires. The send
	/// therefore applies a bounded retry with exponential backoff + jitter
	/// (<see cref="RetryBackoffPolicy"/>) that honors the server <c>Retry-After</c>
	/// (<see cref="RetryAfterParser"/>) on 429, 5xx, and transport failures —
	/// mirroring <see cref="BulkEventSender{TData}"/>'s resilience for this lane.
	/// A non-429 4xx is terminal (a retry cannot change the outcome) and is not
	/// retried. These events are low-volume (one per process completion), so this
	/// path does not need the bulk path's shared throttle.
	/// </para>
	/// </summary>
	[DefaultBinding(typeof(SingleEventSender))]
	internal class SingleEventSender : AIPlatformHttpSenderBase
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		// Bound the backoff retries so a persistently failing endpoint cannot loop
		// forever; matches BulkEventSender. After this the event is logged and dropped.
		internal const int MaxRetryAttempts = 6;

		private readonly RetryBackoffPolicy _backoff;
		private readonly Func<DateTime> _clock;
		private readonly Action<TimeSpan> _sleep;

		public SingleEventSender(UserConnection userConnection)
			: this(userConnection, backoff: null, clock: null, sleep: null) {
		}

		// Test seam: inject backoff / clock / sleep so the retry path is exercised
		// deterministically without real Thread.Sleep.
		internal SingleEventSender(
				UserConnection userConnection,
				RetryBackoffPolicy backoff,
				Func<DateTime> clock,
				Action<TimeSpan> sleep)
			: base(userConnection) {
			_backoff = backoff ?? new RetryBackoffPolicy();
			_clock = clock ?? (() => DateTime.UtcNow);
			_sleep = sleep ?? DefaultSleep;
		}

		public void Send<TData>(EventOccurrenceRequest<TData> request) {
			SendSerialized(request.TriggerCode, JsonConvert.SerializeObject(request, JsonSettings));
		}

		/// <summary>
		/// Sends a pre-serialized event body to the single-event ingress route for
		/// <paramref name="triggerCode"/>. Splitting this from <see cref="Send{TData}"/>
		/// lets a caller serialize the payload up front and carry it as a plain string
		/// across a boundary that cannot serialize the typed request — e.g. the
		/// background-task argument transport used by the process-completion resume send
		/// (APD-2470) — while reusing the exact same URL building, bearer auth and bounded
		/// retry/back-off resilience.
		/// </summary>
		internal void SendSerialized(string triggerCode, string jsonBody) {
			if (!TryCreateBaseUrl(Log, out string baseUrl, out string apiKey)) {
				return;
			}
			string url = baseUrl.TrimEnd('/')
				+ Constants.ApiRoutes.TriggerEvents(triggerCode);

			int attempt = 0;
			while (true) {
				SendOutcome outcome = Post(url, apiKey, jsonBody);
				if (outcome.Succeeded) {
					return;
				}
				if (outcome.Terminal) {
					// Non-429 4xx (malformed / unauthorized / forbidden / unknown trigger):
					// retrying cannot change the result, so stop now. The status code is
					// logged here so the dropped event stays diagnosable.
					Log.Error($"AI Studio returned terminal HTTP {outcome.StatusCode} " +
						$"for event send to '{url}'; not retrying.");
					return;
				}
				if (attempt >= MaxRetryAttempts) {
					break;
				}
				attempt++;
				// Transient (429 / 5xx / transport): back off — honoring Retry-After on a
				// 429 — and retry so a brief outage or rate-limit does not drop a
				// workflow-resuming event.
				Sleep(_backoff.GetDelay(attempt, outcome.RetryAfter));
			}

			// Error (not Warn): the resume event was NOT delivered, so the suspended
			// workflow will not resume until it is re-sent. Durable requeue is an
			// APD-1464 follow-up (the bulk lane has the same limitation).
			Log.Error($"Giving up on an undelivered resume event for trigger " +
				$"'{triggerCode}' after {MaxRetryAttempts} backoff attempts; the " +
				"suspended workflow will not resume until the event is re-sent.");
		}

		private SendOutcome Post(string url, string apiKey, string jsonBody) {
			try {
				HttpRequestConfig config = CreateJsonRequest(HttpRequestMethod.POST, url);
				ApplyAuthorization(config, apiKey);
				IHttpResponse response = GetHttpClient().SendWithJsonBody(config, jsonBody);
				int statusCode = (int)response.StatusCode;
				if (IsSuccessStatusCode(response.StatusCode)) {
					return SendOutcome.Success();
				}
				if (statusCode == 429) {
					TimeSpan? retryAfter = RetryAfterParser.TryParse(response.Headers, _clock());
					return SendOutcome.Transient(statusCode, retryAfter);
				}
				if (statusCode >= 400 && statusCode < 500) {
					return SendOutcome.TerminalReject(statusCode);
				}
				// 5xx (or any other non-success): transient — retry under backoff.
				Log.Warn($"AI Studio returned HTTP {statusCode} for event send to '{url}'.");
				return SendOutcome.Transient(statusCode, null);
			} catch (Exception ex) {
				Log.Error($"HTTP request to AI Studio failed for URL '{url}'.", ex);
				return SendOutcome.Transient(0, null);
			}
		}

		private void Sleep(TimeSpan delay) {
			if (delay > TimeSpan.Zero) {
				_sleep(delay);
			}
		}

		private static void DefaultSleep(TimeSpan delay) {
			Thread.Sleep(delay);
		}

		private sealed class SendOutcome
		{
			public bool Succeeded { get; private set; }
			public bool Terminal { get; private set; }
			public int StatusCode { get; private set; }
			public TimeSpan? RetryAfter { get; private set; }

			public static SendOutcome Success() {
				return new SendOutcome { Succeeded = true };
			}

			public static SendOutcome TerminalReject(int statusCode) {
				return new SendOutcome { Terminal = true, StatusCode = statusCode };
			}

			public static SendOutcome Transient(int statusCode, TimeSpan? retryAfter) {
				return new SendOutcome { StatusCode = statusCode, RetryAfter = retryAfter };
			}
		}
	}
}

