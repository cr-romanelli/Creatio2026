namespace Terrasoft.Configuration
{
	using System.Collections.Generic;
	using System.Net;
	using System.Runtime.Serialization;

	#region Class: McpToolContent

	/// <summary>
	/// MCP <c>tools/call</c> content block. Per the MCP spec, the result
	/// <c>content</c> field is an array of typed blocks; this app emits a
	/// single <c>"type": "text"</c> block whose <c>text</c> is the
	/// JSON-serialized output (mirrored as a typed object in
	/// <see cref="McpToolsCallResult.StructuredContent"/>). Using a single
	/// block matters for LiteLLM's MCP→OpenAI bridge, which folds the array
	/// into one string and drops everything past the first item.
	/// </summary>
	[DataContract]
	public class McpToolContent
	{

		#region Properties: Public

		[DataMember(Name = "type")]
		public string Type { get; set; } = "text";

		[DataMember(Name = "text")]
		public string Text { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpToolsCallResult

	/// <summary>
	/// Transport-agnostic outcome of a tool execution. The MCP transport
	/// (APD-1374) translates this into the MCP <c>tools/call</c> wire format.
	/// <see cref="SuggestedStatusCode"/> is a hint for HTTP transports —
	/// JSON-RPC mappers ignore it and use <see cref="ErrorCode"/> +
	/// <see cref="ErrorMessage"/> instead.
	///
	/// <see cref="StructuredContent"/> mirrors MCP spec 2025-03-26's optional
	/// <c>structuredContent</c> field on <c>tools/call</c> results — a typed
	/// object alongside the human-readable <see cref="Content"/> array.
	/// Clients that understand it (Claude Desktop, AI Studio) consume it
	/// directly without parsing the text channel; older clients fall back
	/// to the JSON-serialized single content block.
	/// </summary>
	internal class McpToolsCallResult
	{

		#region Constructors: Public

		public McpToolsCallResult() {
			Content = new List<McpToolContent>();
			SuggestedStatusCode = HttpStatusCode.OK;
		}

		#endregion

		#region Properties: Public

		public bool IsError { get; set; }

		public string ErrorMessage { get; set; }

		public string ErrorCode { get; set; }

		public List<McpToolContent> Content { get; set; }

		public Dictionary<string, object> StructuredContent { get; set; }

		public HttpStatusCode SuggestedStatusCode { get; set; }

		#endregion

	}

	#endregion

	#region Interface: IMcpToolsCallService

	/// <summary>
	/// Transport-agnostic execution of an exposed tool by external name.
	/// Mirrors <see cref="IMcpToolsListService"/> and shares the same
	/// projection + RBAC + argument-validation rules.
	/// </summary>
	internal interface IMcpToolsCallService
	{

		#region Methods: Public

		/// <summary>
		/// Executes the named tool. <paramref name="suspendCapable"/> carries the
		/// caller's <c>X-Suspend-Capable</c> advertisement (suspendable-agent-execution
		/// design §8.2): only a suspend-capable caller may receive the §8.2 suspended
		/// envelope. A non-capable caller (Test / Live-preview window, or any dispatch
		/// with the suspendable-execution feature OFF) gets the running background
		/// handle instead, so a backgrounded process never unwinds the runtime ReAct
		/// loop into an uncatchable <c>_Suspend</c>.
		/// </summary>
		McpToolsCallResult Call(string toolName, IDictionary<string, object> arguments,
			bool suspendCapable = false);

		#endregion

	}

	#endregion

}

