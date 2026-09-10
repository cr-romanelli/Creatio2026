namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;

	/// <summary>
	/// APD-1464 S4 (R1/R2): computes the delay before the next retry of an
	/// outbound trigger send after AI Studio returns 429.
	///
	/// <para>
	/// Replaces the previous immediate resend (the retry storm tracked by
	/// APD-1464). The delay grows exponentially with the attempt number and
	/// carries full jitter so a fleet of bridges does not resynchronise into a
	/// thundering herd. When the 429 carried a <c>Retry-After</c> header the
	/// server's value is honored as the MINIMUM wait (R2); the computed
	/// backoff is used as the floor only when the header is absent (R1). Every
	/// delay is clamped to <see cref="MaxDelay"/> so repeated 429s settle into a
	/// bounded poll rather than an ever-growing or tight loop.
	/// </para>
	///
	/// <para>
	/// Pure, deterministic logic (the only non-determinism — jitter — is
	/// injected as a 0..1 sampler) so it is unit-testable without a Creatio
	/// environment.
	/// </para>
	/// </summary>
	internal sealed class RetryBackoffPolicy
	{
		// Base unit doubled each attempt: 1s, 2s, 4s, 8s, ... before jitter.
		private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

		// Ceiling so the window never grows without bound; aligns with the
		// 1-minute rate-limit window the control plane uses.
		internal static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(60);

		// Cap the exponent so 2^attempt cannot overflow when many retries pile
		// up; beyond this the clamp to MaxDelay dominates anyway.
		private const int MaxExponent = 16;

		private readonly Func<double> _jitterSampler;

		/// <summary>
		/// Creates a policy. <paramref name="jitterSampler"/> returns a value in
		/// [0,1); production passes a thread-safe random sampler, tests pass a
		/// deterministic stub. Defaults to <see cref="ThreadSafeRandom"/> (a
		/// thread-static per-thread <see cref="Random"/>), not a shared instance.
		/// </summary>
		public RetryBackoffPolicy(Func<double> jitterSampler = null) {
			_jitterSampler = jitterSampler ?? DefaultJitter;
		}

		/// <summary>
		/// Computes the delay before the next attempt.
		/// </summary>
		/// <param name="attempt">
		/// 1-based attempt number that just failed (1 = first failure).
		/// </param>
		/// <param name="retryAfter">
		/// The server-supplied Retry-After, or <c>null</c> when the 429 omitted
		/// it. When present it is the minimum wait; the jittered exponential
		/// backoff is used only if it is larger.
		/// </param>
		public TimeSpan GetDelay(int attempt, TimeSpan? retryAfter) {
			if (attempt < 1) {
				attempt = 1;
			}

			TimeSpan backoff = ComputeJitteredBackoff(attempt);

			// R2: honor Retry-After as the floor; never wait less than the
			// server asked for, but still cap the overall wait at MaxDelay.
			if (retryAfter.HasValue && retryAfter.Value > backoff) {
				backoff = retryAfter.Value;
			}

			return backoff > MaxDelay ? MaxDelay : backoff;
		}

		private TimeSpan ComputeJitteredBackoff(int attempt) {
			int exponent = Math.Min(attempt - 1, MaxExponent);
			double cappedSeconds = Math.Min(
				BaseDelay.TotalSeconds * Math.Pow(2, exponent),
				MaxDelay.TotalSeconds);

			// Full jitter: scale the cap by a uniform [0,1) sample, then clamp up to
			// a small positive floor (below) — so the delay lands in (0, cappedSeconds],
			// never exactly 0. Spreads retries so concurrent bridges do not realign on
			// the same instant, while still pausing when jitter≈0.
			double jitter = SampleJitter();
			double delaySeconds = cappedSeconds * jitter;

			// Keep a small positive floor so attempt 1 with jitter≈0 still
			// introduces a real (non-zero) pause instead of an effective resend.
			double minSeconds = Math.Min(BaseDelay.TotalSeconds, cappedSeconds);
			if (delaySeconds < minSeconds) {
				delaySeconds = minSeconds;
			}

			return TimeSpan.FromSeconds(delaySeconds);
		}

		private double SampleJitter() {
			double sample = _jitterSampler();
			if (sample < 0) {
				return 0;
			}
			return sample >= 1 ? 0.9999999 : sample;
		}

		private static double DefaultJitter() {
			return ThreadSafeRandom.NextDouble();
		}

		// Lightweight thread-safe random for jitter; net472-compatible (no
		// Random.Shared). Per-instance avoids the shared-Random tearing bug.
		private static class ThreadSafeRandom
		{
			[ThreadStatic]
			private static Random _local;

			public static double NextDouble() {
				Random rng = _local;
				if (rng == null) {
					_local = rng = new Random(
						unchecked(Environment.TickCount * 31
							+ System.Threading.Thread.CurrentThread.ManagedThreadId));
				}
				return rng.NextDouble();
			}
		}
	}
}

