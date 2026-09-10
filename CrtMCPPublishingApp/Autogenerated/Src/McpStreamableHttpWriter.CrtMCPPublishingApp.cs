namespace Terrasoft.Configuration
{
	using System;
	using System.Text;
	using Newtonsoft.Json;
	using Terrasoft.Common;

	#region Class: McpStreamableHttpWriter

	/// <summary>
	/// Helper for the optional MCP Streamable HTTP transport.
	///
	/// When an MCP client sends <c>Accept: text/event-stream</c>, the server
	/// MAY respond with a Server-Sent Events stream instead of plain JSON.
	/// For the MVP we emit the entire JSON-RPC response as a single
	/// <c>message</c> event and close the stream — synchronous MCP calls
	/// don't need real streaming yet. Multi-event streams (progress
	/// notifications, partial results) are deferred until OOTB tools or
	/// background processes actually need them.
	/// </summary>
	internal static class McpStreamableHttpWriter
	{

		#region Constants: Public

		public const string SseContentType = "text/event-stream";
		public const string SseAcceptToken = "text/event-stream";

		#endregion

		#region Methods: Public

		/// <summary>
		/// True when the caller's <c>Accept</c> header includes
		/// <c>text/event-stream</c>. Matches anywhere in the comma-separated
		/// Accept list and ignores quality parameters — intentionally lenient
		/// so that <c>Accept: text/event-stream, application/json</c>
		/// (the form most MCP clients send) is detected.
		/// </summary>
		public static bool ClientPrefersSse(string acceptHeader) {
			if (acceptHeader.IsNullOrWhiteSpace()) {
				return false;
			}
			return acceptHeader.IndexOf(SseAcceptToken, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		/// <summary>
		/// Serializes one response body into the SSE framing
		/// <c>event: message\ndata: &lt;json&gt;\n\n</c>. Empty body
		/// produces an empty buffer so callers can short-circuit the
		/// no-content path without writing any framing.
		/// </summary>
		public static byte[] BuildSinglePayload(object responseBody) {
			if (responseBody == null) {
				return new byte[0];
			}
			string json = JsonConvert.SerializeObject(responseBody);
			string frame = "event: message\ndata: " + json + "\n\n";
			return Encoding.UTF8.GetBytes(frame);
		}

		#endregion

	}

	#endregion

}

