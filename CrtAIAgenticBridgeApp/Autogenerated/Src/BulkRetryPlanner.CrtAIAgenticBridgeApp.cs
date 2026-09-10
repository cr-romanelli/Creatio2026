namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// APD-1464 S5 (R5 / RISK3): given the events the bridge sent in a bulk
	/// request and the per-event response from the control plane, decides which
	/// events to retry. Only the rejected-and-retryable subset is retried;
	/// accepted events and terminal (non-retryable) rejections are dropped from
	/// the retry set so a poison event cannot loop and accepted events are never
	/// re-sent (exactly-once).
	///
	/// <para>Pure logic — unit-testable without a Creatio environment.</para>
	/// </summary>
	internal static class BulkRetryPlanner
	{
		/// <summary>
		/// Computes the subset of <paramref name="sent"/> to retry given the
		/// bulk <paramref name="response"/>. An event is retried only if the
		/// response carries a result for its EventId with
		/// <c>Accepted == false</c> and <c>Retryable == true</c>. Events with no
		/// matching result (the server dropped them from the response) are
		/// retried defensively so nothing is silently lost.
		/// </summary>
		public static IReadOnlyList<TItem> ComputeRetrySet<TItem>(
				IReadOnlyList<TItem> sent,
				BulkEventOccurrenceResponseDto response,
				Func<TItem, string> eventIdSelector) {
			if (sent == null || sent.Count == 0) {
				return EmptyList<TItem>();
			}
			if (eventIdSelector == null) {
				throw new ArgumentNullException(nameof(eventIdSelector));
			}

			// No usable response (null / empty results) → retry the whole batch
			// under backoff; the transport likely failed before per-event
			// adjudication, so we cannot assume anything was accepted.
			if (response == null || response.Results == null || response.Results.Count == 0) {
				return new List<TItem>(sent);
			}

			var resultByEventId = new Dictionary<string, BulkEventResultItem>(StringComparer.Ordinal);
			foreach (BulkEventResultItem result in response.Results) {
				if (result != null && !string.IsNullOrEmpty(result.EventId)) {
					resultByEventId[result.EventId] = result;
				}
			}

			var retry = new List<TItem>();
			foreach (TItem item in sent) {
				string eventId = eventIdSelector(item);
				BulkEventResultItem result;
				if (eventId != null && resultByEventId.TryGetValue(eventId, out result)) {
					// Retry only rejected-and-retryable; accepted and terminal
					// rejections are excluded (exactly-once / no poison loop).
					if (!result.Accepted && result.Retryable) {
						retry.Add(item);
					}
				} else {
					// Server returned no verdict for this event → retry it so it
					// is not silently dropped.
					retry.Add(item);
				}
			}
			return retry;
		}

		private static IReadOnlyList<T> EmptyList<T>() {
			return new T[0];
		}
	}
}

