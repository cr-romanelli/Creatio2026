namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// APD-1464 S5 (R4): client-side outbound limiter that keeps the bridge's
	/// request rate to AI Studio at or under the configured ceiling
	/// (1000 requests / minute in the affected environment) instead of relying
	/// on the server to reject excess with 429.
	///
	/// <para>
	/// Implemented as a sliding window: timestamps of granted requests within
	/// the trailing window are retained, and a request is admitted only while
	/// fewer than <c>maxRequests</c> remain in that window. When the window is
	/// full the throttle reports the wait until the oldest in-window request
	/// ages out, so the caller can defer (queue/batch) rather than drop or
	/// hot-retry. Pure logic — the clock is injected — so it is unit-testable
	/// without a Creatio environment or real time.
	/// </para>
	/// </summary>
	internal sealed class OutboundThrottle
	{
		private readonly int _maxRequests;
		private readonly TimeSpan _window;
		private readonly Queue<DateTime> _grants = new Queue<DateTime>();
		private readonly object _gate = new object();

		/// <summary>
		/// 1000 requests per minute — the configured ceiling for the affected
		/// environment. Kept here as the bridge-side mirror so the throttle and
		/// the server limit are derived from the same number.
		/// </summary>
		public const int DefaultMaxRequestsPerMinute = 1000;

		public OutboundThrottle(
				int maxRequests = DefaultMaxRequestsPerMinute,
				TimeSpan? window = null) {
			if (maxRequests < 1) {
				maxRequests = 1;
			}
			_maxRequests = maxRequests;
			_window = window ?? TimeSpan.FromMinutes(1);
		}

		/// <summary>
		/// Attempts to admit one outbound request at <paramref name="utcNow"/>.
		/// On success records the grant and returns <c>true</c> with
		/// <paramref name="retryAfter"/> = <see cref="TimeSpan.Zero"/>. When the
		/// window is saturated returns <c>false</c> and sets
		/// <paramref name="retryAfter"/> to the wait until the next slot frees.
		/// </summary>
		public bool TryAcquire(DateTime utcNow, out TimeSpan retryAfter) {
			lock (_gate) {
				Evict(utcNow);
				if (_grants.Count < _maxRequests) {
					_grants.Enqueue(utcNow);
					retryAfter = TimeSpan.Zero;
					return true;
				}

				DateTime oldest = _grants.Peek();
				TimeSpan wait = (oldest + _window) - utcNow;
				retryAfter = wait > TimeSpan.Zero ? wait : TimeSpan.FromMilliseconds(1);
				return false;
			}
		}

		/// <summary>
		/// Number of grants currently counted within the trailing window at
		/// <paramref name="utcNow"/>. Exposed for assertions / observability.
		/// </summary>
		public int CountInWindow(DateTime utcNow) {
			lock (_gate) {
				Evict(utcNow);
				return _grants.Count;
			}
		}

		private void Evict(DateTime utcNow) {
			DateTime cutoff = utcNow - _window;
			while (_grants.Count > 0 && _grants.Peek() <= cutoff) {
				_grants.Dequeue();
			}
		}
	}
}

