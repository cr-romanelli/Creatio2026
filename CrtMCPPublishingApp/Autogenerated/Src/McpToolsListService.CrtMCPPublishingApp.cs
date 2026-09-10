using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;

	#region Class: McpToolsListService

	/// <summary>
	/// Implementation of <see cref="IMcpToolsListService"/>. Owns the
	/// tool + RBAC + schema-build pipeline that materializes
	/// <see cref="McpToolDescriptor"/> records for MCP <c>tools/list</c>.
	/// </summary>
	[DefaultBinding(typeof(IMcpToolsListService))]
	internal class McpToolsListService : IMcpToolsListService
	{

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(McpToolsListService));
		private readonly IMcpServerRepository _serverRepository;
		private readonly IMcpToolValidator _validator;
		private readonly IMcpToolRuntime _runtime;
		private readonly Guid _serverId;

		#endregion

		#region Constructors: Public

		public McpToolsListService(UserConnection userConnection, Guid serverId) {
			_serverRepository = ClassFactory.Get<IMcpServerRepository>(
				new ConstructorArgument("userConnection", userConnection));
			_validator = ClassFactory.Get<IMcpToolValidator>();
			_runtime = ClassFactory.Get<IMcpToolRuntime>(
				new ConstructorArgument("userConnection", userConnection));
			_serverId = serverId;
		}

		internal McpToolsListService(IMcpServerRepository serverRepository,
				IMcpToolValidator validator, IMcpToolRuntime runtime,
				Guid serverId) {
			_serverRepository = serverRepository;
			_validator = validator;
			_runtime = runtime;
			_serverId = serverId;
		}

		#endregion

		#region Methods: Private

		// Stable name for log lines: the admin-facing external name, falling back to
		// the process id when none is set.
		private static string ToolDisplayName(McpToolModel toolModel) {
			return toolModel.ExternalName ?? toolModel.ProcessId.ToString();
		}

		private static object BuildOptionalJsonPayload(string json) {
			if (json.IsNullOrWhiteSpace()) {
				return null;
			}
			return JsonConvert.DeserializeObject<object>(json);
		}

		private static string SelectJsonOverride(string baseJson, string overrideJson) {
			if (overrideJson.IsNullOrWhiteSpace()) {
				return baseJson;
			}
			return overrideJson;
		}

		private static McpToolDescriptor ToTool(McpToolModel toolModel,
				RuntimeToolDefinition toolDefinition,
				RuntimeToolSchemaResult inputSchemaResult,
				string runtimeOutputSchemaJson) {
			if (!toolDefinition.IsEnabled || !toolDefinition.CanExecute) {
				return null;
			}
			string mergedInputSchema = SelectJsonOverride(inputSchemaResult?.Schema?.Value,
				toolModel.InputSchema);
			if (mergedInputSchema.IsNullOrWhiteSpace()) {
				return null;
			}
			// outputSchema is optional in MCP, but advertising one materially helps
			// LLM clients (AI Studio, Claude Desktop) interpret tool results —
			// when absent, clients fall back to free-form text parsing and may
			// render the tool output as null. Prefer admin override; otherwise
			// auto-derive from the BP's result parameters.
			string mergedOutputSchema = SelectJsonOverride(runtimeOutputSchemaJson,
				toolModel.OutputSchema);
			return new McpToolDescriptor {
				Name = toolModel.ExternalName,
				Description = toolModel.Description.IsNullOrWhiteSpace()
					? toolDefinition.Description
					: toolModel.Description,
				InputSchema = ToolServiceRequestGuard.BuildInputSchema(mergedInputSchema),
				OutputSchema = BuildOptionalJsonPayload(mergedOutputSchema),
				Annotations = BuildOptionalJsonPayload(toolModel.Annotations)
			};
		}

		#endregion

		#region Methods: Public

		public IReadOnlyList<McpToolDescriptor> GetTools() {
			// Resolve the server's tools first, then authorize only their processes.
			// Authorizing the full process catalog here (the old behavior) made
			// tools/list O(all application processes) regardless of how few tools the
			// server actually exposes.
			IReadOnlyCollection<McpToolModel> serverTools =
				_serverRepository.GetEnabledToolsForServer(_serverId);
			IReadOnlyDictionary<Guid, RuntimeToolDefinition> candidateTools =
				ToolServiceRequestGuard.GetAuthorizedTools(_runtime, serverTools);
			var tools = new List<McpToolDescriptor>();
			foreach (McpToolModel toolModel in serverTools) {
				if (!McpToolSourceTypes.IsSupported(toolModel.ToolSourceTypeId)) {
					Log.Warn($"Tool '{ToolDisplayName(toolModel)}' was skipped: unsupported source type '{toolModel.ToolSourceTypeId}'.");
					continue;
				}
				if (!_validator.Validate(toolModel, candidateTools).IsPublishable) {
					continue;
				}
				if (!ToolServiceRequestGuard.TryResolveAuthorizedTool(candidateTools,
						toolModel.Id, out var toolDefinition)) {
					// An explicitly configured tool the admin expects to publish dropped
					// out — its process/action is missing, disabled, or not runnable for
					// the current user (or the source-code action type no longer resolves).
					// Warn (not Debug) so "my tool vanished from tools/list" is visible
					// rather than silent.
					Log.Warn($"Tool '{ToolDisplayName(toolModel)}' was excluded from MCP list: its referenced action is not an available/authorized runtime tool.");
					continue;
				}
				// Always build the runtime input schema, even when an admin override
				// exists, and exclude the tool if it fails. An override customizes the
				// *advertised* schema (see ToTool) but cannot make an un-runnable tool
				// callable: tools/call validates incoming arguments against the real BP
				// parameters, so a process with an unsupported input parameter throws
				// UnsupportedParameterType there regardless of the override. Listing such
				// a tool would advertise something that fails on every call — so the
				// runtime build is the gate for whether the tool appears at all, exactly
				// as it is on the call path.
				RuntimeToolSchemaResult inputSchemaResult =
					_runtime.GetInputSchema(toolModel);
				if (!inputSchemaResult.IsSuccess) {
					Log.Warn($"Failed to build tool schema for tool '{ToolDisplayName(toolModel)}': {inputSchemaResult.ErrorCode ?? "SchemaBuildFailed"}");
					continue;
				}
				// Output schema is optional — if derivation fails (or the runtime
				// doesn't yet support it), the tool still ships without
				// outputSchema rather than getting hidden. Only the input schema
				// is required for the tool to be usable. Skip the runtime build when
				// an override is present, since the override wins regardless.
				string runtimeOutputJson = null;
				if (toolModel.OutputSchema.IsNullOrWhiteSpace()) {
					RuntimeToolSchemaResult outputSchemaResult =
						_runtime.GetOutputSchema(toolModel);
					runtimeOutputJson = outputSchemaResult != null && outputSchemaResult.IsSuccess
						? outputSchemaResult.Schema?.Value
						: null;
					if (outputSchemaResult != null && !outputSchemaResult.IsSuccess) {
						Log.Debug($"No runtime output schema for tool '{ToolDisplayName(toolModel)}': {outputSchemaResult.ErrorCode ?? "OutputSchemaBuildFailed"}");
					}
				}
				McpToolDescriptor tool = ToTool(toolModel, toolDefinition, inputSchemaResult, runtimeOutputJson);
				if (tool == null) {
					Log.Warn($"Tool '{ToolDisplayName(toolModel)}' was excluded from MCP list because its schema could not be materialized.");
					continue;
				}
				tools.Add(tool);
			}
			return tools;
		}

		#endregion

	}

	#endregion

}

