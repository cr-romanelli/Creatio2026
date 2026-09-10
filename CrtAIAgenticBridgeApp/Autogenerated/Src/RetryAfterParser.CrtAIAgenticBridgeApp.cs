namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	/// <summary>
	/// APD-1464 S4 (R2): parses the HTTP <c>Retry-After</c> header the control
	/// plane now emits on 429 responses for the trigger lane.
	///
	/// <para>
	/// Per RFC 7231 the value is either a delta in seconds (e.g. <c>"42"</c>) or
	/// an HTTP-date. The control plane emits the seconds form; the HTTP-date
	/// form is handled defensively so a proxy that rewrites the header cannot
	/// blind the bridge's backoff. Returns <c>null</c> when the header is absent
	/// or unparseable, in which case <see cref="RetryBackoffPolicy"/> falls back
	/// to pure exponential backoff (R1).
	/// </para>
	/// </summary>
	internal static class RetryAfterParser
	{
		private const string HeaderName = "Retry-After";

		/// <summary>
		/// Extracts the Retry-After hint from the response headers relative to
		/// <paramref name="utcNow"/> (injected for deterministic testing of the
		/// HTTP-date branch). Negative or zero deltas are normalised to
		/// <c>null</c> so the caller falls back to computed backoff rather than
		/// retrying instantly.
		/// </summary>
		public static TimeSpan? TryParse(
				IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
				DateTime utcNow) {
			string raw = FindHeaderValue(headers);
			if (string.IsNullOrWhiteSpace(raw)) {
				return null;
			}

			raw = raw.Trim();

			// Delta-seconds form (what the control plane emits).
			if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture,
					out long seconds)) {
				return seconds > 0 ? TimeSpan.FromSeconds(seconds) : (TimeSpan?)null;
			}

			// HTTP-date form (defensive — proxies may rewrite the header).
			if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture,
					DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
					out DateTimeOffset when)) {
				TimeSpan delta = when.UtcDateTime - DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
				return delta > TimeSpan.Zero ? delta : (TimeSpan?)null;
			}

			return null;
		}

		private static string FindHeaderValue(
				IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers) {
			if (headers == null) {
				return null;
			}
			foreach (KeyValuePair<string, IEnumerable<string>> header in headers) {
				if (string.Equals(header.Key, HeaderName, StringComparison.OrdinalIgnoreCase)) {
					return header.Value?.FirstOrDefault();
				}
			}
			return null;
		}
	}
}

