using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Common;
	using Terrasoft.Core;

	#region Class: McpToolsCallHandler

	/// <summary>
	/// Handles MCP <c>tools/call</c>. Delegates to
	/// <see cref="IMcpToolsCallService"/> and maps the transport-agnostic
	/// <see cref="McpToolsCallResult"/> into MCP wire format.
	///
	/// Per MCP spec, execution failures from the underlying tool (validation,
	/// RBAC denial, runtime errors) are returned as a structured
	/// <c>{ content, isError: true }</c> result — NOT as JSON-RPC errors —
	/// so the LLM client can show the error to the user. Only protocol-level
	/// failures (missing <c>name</c>, malformed envelope) use the JSON-RPC
	/// error channel.
	/// </summary>
	internal class McpToolsCallHandler : IMcpMethodHandler
	{

		#region Fields: Private

		private readonly IMcpToolsCallService _callService;
		// Caller's X-Suspend-Capable advertisement, read from the request headers by
		// the transport (ToolServiceMcp) and forwarded into tools/call so the service
		// only emits the §8.2 suspended envelope to a caller that can actually resume.
		private readonly bool _suspendCapable;

		#endregion

		#region Constructors: Public

		public McpToolsCallHandler(UserConnection userConnection, Guid serverId,
				bool suspendCapable = false) {
			_callService = ClassFactory.Get<IMcpToolsCallService>(
				new ConstructorArgument("userConnection", userConnection),
				new ConstructorArgument("serverId", serverId));
			_suspendCapable = suspendCapable;
		}

		internal McpToolsCallHandler(IMcpToolsCallService callService, bool suspendCapable = false) {
			_callService = callService;
			_suspendCapable = suspendCapable;
		}

		#endregion

		#region Methods: Private

		private static IDictionary<string, object> ExtractArguments(Dictionary<string, object> parameters) {
			if (!parameters.TryGetValue("arguments", out object rawArguments) || rawArguments == null) {
				return new Dictionary<string, object>();
			}
			// Fully materialize the argument graph into plain CLR types. Newtonsoft
			// leaves NESTED objects/arrays as JObject/JArray (which downstream validation
			// and BP serialization can't read), and object / object-list tool parameters
			// carry exactly such nested values. Single-sourced in ToolServiceRequestGuard
			// so the lossless date handling and the JObject-shape note live in one place.
			return ToolServiceRequestGuard.NormalizeArguments(rawArguments);
		}

		#endregion

		#region Methods: Public

		public McpHandlerOutcome Handle(Dictionary<string, object> parameters) {
			if (parameters == null) {
				return McpHandlerOutcome.Failure(McpJsonRpcErrorCodes.InvalidParams,
					"Missing parameters for 'tools/call'.");
			}
			parameters.TryGetValue("name", out object rawName);
			string name = rawName as string;
			if (name.IsNullOrWhiteSpace()) {
				return McpHandlerOutcome.Failure(McpJsonRpcErrorCodes.InvalidParams,
					"Parameter 'name' is required and must be a non-empty string.");
			}
			IDictionary<string, object> arguments = ExtractArguments(parameters);
			McpToolsCallResult callResult = _callService.Call(name, arguments, _suspendCapable);
			var result = new Dictionary<string, object> {
				["content"] = (callResult.Content ?? new List<McpToolContent>())
					.Select(item => new Dictionary<string, object> {
						["type"] = item.Type ?? "text",
						["text"] = item.Text ?? string.Empty
					})
					.ToList(),
				["isError"] = callResult.IsError
			};
			// MCP spec 2025-03-26+ optional field — clients that understand it
			// (AI Studio, Claude Desktop) read this directly instead of parsing
			// the text channel. Only include when the service actually
			// produced a typed result; the call service emits a non-null dict
			// only when execution succeeded AND the BP returned output
			// parameters, so a null here means "no structured channel" rather
			// than "structured channel is empty".
			if (callResult.StructuredContent != null) {
				result["structuredContent"] = callResult.StructuredContent;
			}
			return McpHandlerOutcome.Success(result);
		}

		#endregion

	}

	#endregion

}

