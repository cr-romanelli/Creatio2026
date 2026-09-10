using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;

	#region Class: McpToolsCallService

	/// <summary>
	/// Implementation of <see cref="IMcpToolsCallService"/>. Owns the
	/// tool-lookup + RBAC + argument-validation + dispatch pipeline that
	/// backs MCP <c>tools/call</c>.
	/// </summary>
	[DefaultBinding(typeof(IMcpToolsCallService))]
	internal class McpToolsCallService : IMcpToolsCallService
	{

		#region Constants: Private

		private const string InvalidToolNameMessage = "Tool name is required.";
		private const string ToolUnavailableMessage = "Tool is unavailable.";
		private const string UnexpectedExecutionFailureMessage = "Tool execution failed.";

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(McpToolsCallService));
		private readonly IMcpServerRepository _serverRepository;
		private readonly IMcpToolValidator _validator;
		private readonly IMcpToolRuntime _runtime;
		private readonly Guid _serverId;

		#endregion

		#region Constructors: Public

		public McpToolsCallService(UserConnection userConnection, Guid serverId) {
			_serverRepository = ClassFactory.Get<IMcpServerRepository>(
				new ConstructorArgument("userConnection", userConnection));
			_validator = ClassFactory.Get<IMcpToolValidator>();
			_runtime = ClassFactory.Get<IMcpToolRuntime>(
				new ConstructorArgument("userConnection", userConnection));
			_serverId = serverId;
		}

		internal McpToolsCallService(IMcpServerRepository serverRepository,
				IMcpToolValidator validator, IMcpToolRuntime runtime,
				Guid serverId) {
			_serverRepository = serverRepository;
			_validator = validator;
			_runtime = runtime;
			_serverId = serverId;
		}

		#endregion

		#region Methods: Private

		private static McpToolsCallResult CreateError(string message, string errorCode,
				HttpStatusCode statusCode = HttpStatusCode.BadRequest) {
			return new McpToolsCallResult {
				IsError = true,
				ErrorMessage = message,
				ErrorCode = errorCode,
				SuggestedStatusCode = statusCode,
				Content = new List<McpToolContent> {
					new McpToolContent { Text = message }
				}
			};
		}

		#endregion

		#region Methods: Public

		public McpToolsCallResult Call(string toolName, IDictionary<string, object> arguments,
				bool suspendCapable = false) {
			try {
				if (toolName.IsNullOrWhiteSpace()) {
					return CreateError(InvalidToolNameMessage, "InvalidToolName");
				}
				// GetToolForServerByName already filters IsEnabled = true, so a null
				// result covers both "no such tool" and "tool disabled".
				McpToolModel tool = _serverRepository.GetToolForServerByName(_serverId, toolName);
				if (tool == null) {
					return CreateError($"Tool '{toolName}' not found.", "ToolNotFound", HttpStatusCode.NotFound);
				}
				if (!McpToolSourceTypes.IsSupported(tool.ToolSourceTypeId)) {
					Log.Warn($"Tool '{toolName}' has unsupported source type '{tool.ToolSourceTypeId}'.");
					return CreateError($"Tool '{toolName}' not found.", "ToolNotFound", HttpStatusCode.NotFound);
				}
				// Authorize only this tool rather than the full catalog —
				// a single tools/call needs one tool verified, not every one.
				IReadOnlyDictionary<Guid, RuntimeToolDefinition> candidateTools =
					ToolServiceRequestGuard.GetAuthorizedTools(_runtime, new[] { tool });
				if (!ToolServiceRequestGuard.TryResolveAuthorizedTool(candidateTools,
						tool.Id, out _)) {
					return CreateError($"Tool '{toolName}' not found.", "ToolNotAuthorized", HttpStatusCode.NotFound);
				}
				McpToolValidationResult validationResult = _validator.Validate(tool, candidateTools);
				if (!validationResult.IsPublishable) {
					string validationCode = validationResult.Issues.FirstOrDefault()?.Code ?? "NotPublishable";
					Log.Warn($"Tool '{toolName}' is not publishable. Validation code: {validationCode}.");
					return CreateError(ToolUnavailableMessage, validationCode, HttpStatusCode.Conflict);
				}
				RuntimeToolSchemaResult schemaResult =
					_runtime.GetInputSchema(tool);
				if (!schemaResult.IsSuccess) {
					Log.Warn($"Failed to resolve input schema for tool '{toolName}': {schemaResult.ErrorCode ?? "SchemaUnavailable"}");
					// Surface the actionable reason for UnsupportedParameterType — that
					// message is our own crafted text (names the offending parameter and
					// its type) and is safe to return. Other failures (SchemaBuildFailed)
					// carry raw exception text, so keep the generic message to avoid
					// leaking internals.
					string message = schemaResult.ErrorCode == "UnsupportedParameterType"
							&& schemaResult.ErrorMessage.IsNotNullOrWhiteSpace()
						? schemaResult.ErrorMessage
						: ToolUnavailableMessage;
					return CreateError(message, schemaResult.ErrorCode ?? "SchemaUnavailable",
						HttpStatusCode.Conflict);
				}
				McpToolInputSchema inputSchema = ToolServiceRequestGuard.BuildInputSchema(schemaResult.Schema?.Value);
				IDictionary<string, object> safeArguments = arguments ?? new Dictionary<string, object>();
				if (!ToolServiceRequestGuard.TryValidateArguments(inputSchema, safeArguments,
						out string validationErrorMessage)) {
					return CreateError(validationErrorMessage, "InvalidArguments");
				}
				RuntimeToolExecutionResult executionResult = _runtime.Execute(
					tool,
					safeArguments,
					new RuntimeToolExecutionOptions());
				if (!executionResult.IsSuccess) {
					// Log the underlying reason (server-side only — the client still gets the
					// generic message so we don't leak internals). Without this, an action that
					// throws or returns a failure surfaces as a bare "Tool execution failed."
					// with no diagnosable cause.
					Log.Warn($"Execution failed for tool '{toolName}': " +
						$"{executionResult.ErrorMessage ?? "no error detail"}");
					return CreateError(UnexpectedExecutionFailureMessage, "ExecutionFailed",
						HttpStatusCode.BadGateway);
				}
				// Source-code actions return a single Response string (no named result
				// parameters, no suspend/resume). Surface the Response verbatim as a text block,
				// plus structuredContent built by the facade to conform to the advertised
				// outputSchema: the Response parsed onto the declared Output params, or
				// { result: <Response> } when the action declares none. StructuredOutput is null
				// only when the action declared output params but returned a non-object Response.
				if (McpToolSourceTypes.ToRuntimeToolKind(tool.ToolSourceTypeId) == RuntimeToolKind.SourceCodeAction) {
					return new McpToolsCallResult {
						IsError = false,
						Content = new List<McpToolContent> {
							new McpToolContent { Text = executionResult.Response ?? string.Empty }
						},
						StructuredContent = executionResult.StructuredOutput
					};
				}
				// Collapse the BP's output parameters into:
				//   * a single MCP text block containing the JSON-serialized object —
				//     LiteLLM's MCP→OpenAI converter folds an array of blocks into a
				//     single string for OpenAI's `tool_result.content`, dropping
				//     everything past the first item; one JSON block survives that
				//     pipeline intact and is also trivial for any LLM to re-parse.
				//   * `StructuredContent` mirroring the same data as a typed object —
				//     MCP spec 2025-03-26+ field that clients (Claude Desktop, AI
				//     Studio, ChatGPT Connectors) read directly without parsing text.
				// When the tool exposes an `outputSchema`, `StructuredContent`
				// should conform to it; the BP's parameter names + types drive both.
				var structured = new Dictionary<string, object>();
				if (executionResult.Status == "suspended" && suspendCapable) {
					// §8.2 suspended envelope: snake_case top-level keys, matching the platform's
					// creatio_execute_process tool so the agent parks the run and resumes on the
					// correlation id when the backgrounded process completes. external_handle is
					// an opaque handle, so its inner processId stays camelCase per the contract.
					// Gated on suspendCapable: only a caller running under the suspend-aware
					// AgentWorkflow turn loop can persist a SuspendDirective, register a
					// correlation route, and await the Resume signal.
					structured["status"] = "suspended";
					structured["correlation_id"] = executionResult.CorrelationId;
					structured["external_handle"] = new Dictionary<string, object> {
						["processId"] = executionResult.ProcessId.ToString()
					};
					if (executionResult.ExpiresAt.HasValue) {
						structured["expires_at"] = executionResult.ExpiresAt.Value.ToString("o");
					}
				} else if (executionResult.Status == "suspended") {
					// Caller is not suspend-capable (no truthy X-Suspend-Capable header): the
					// Test / Live-preview window, or any dispatch with the suspendable-execution
					// feature OFF. Returning the §8.2 suspended envelope here would unwind the
					// runtime's ReAct loop into an uncatchable _Suspend — no correlation route is
					// registered and no Resume signal will ever arrive — surfacing to the user as
					// "Unexpected error: _Suspend(...)". Mirror the managed run_process stock
					// fallback (suspendable-agent-execution.md §8.2): the process still runs in the
					// background, but the agent receives the running handle as an ordinary result
					// instead of parking the run. The Creatio-side completion hook still fires; with
					// no correlation route the resume event is a harmless no-op.
					Log.Warn($"Tool '{toolName}' suspended but the caller is not suspend-capable; " +
						"returning the running background handle instead of a §8.2 suspended envelope.");
					structured["status"] = "running";
					structured["external_handle"] = new Dictionary<string, object> {
						["processId"] = executionResult.ProcessId.ToString()
					};
				} else if (executionResult.ResultParameters != null) {
					foreach (var parameter in executionResult.ResultParameters) {
						structured[parameter.Key] = parameter.Value;
					}
				}
				string structuredJson = JsonConvert.SerializeObject(structured);
				return new McpToolsCallResult {
					IsError = false,
					Content = new List<McpToolContent> {
						new McpToolContent { Text = structuredJson }
					},
					// MCP spec 2025-03-26+ optional field — only attach when the BP
					// actually produced output parameters. An empty dict ({}) is
					// misleading to structured-aware clients (AI Studio, Claude
					// Desktop): they treat it as "tool succeeded with an empty
					// structured object", which is different from "tool has no
					// structured output channel". When in doubt, omit the field.
					StructuredContent = structured.Count > 0 ? structured : null
				};
			} catch (Exception exception) {
				Log.Error("Unexpected exception while handling McpToolsCallService.Call.", exception);
				return CreateError(UnexpectedExecutionFailureMessage, "UnexpectedFailure",
					HttpStatusCode.InternalServerError);
			}
		}

		#endregion

	}

	#endregion

}

