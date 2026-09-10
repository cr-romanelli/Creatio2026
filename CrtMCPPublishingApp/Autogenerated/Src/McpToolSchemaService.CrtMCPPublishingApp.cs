namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Web.SessionState;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	/// <summary>
	/// Unified read-only schema generation for the MCP tool-management form pages,
	/// for both tool source types. Given a source type and an identifier, returns the
	/// MCP input/output JSON schemas (plus, for processes, the designer description and
	/// the set of unsupported parameters). Replaces the per-kind McpProcessSchemaService
	/// and McpSourceCodeActionService: the actual schema building is already single-
	/// sourced (process via <see cref="ProcessRuntimeToolFacade"/> statics, source code
	/// via <see cref="ISourceCodeRuntimeToolFacade"/>, both over <c>ToolSchemaBuilder</c>),
	/// so this is one transport endpoint over that shared logic. Internal admin tool —
	/// gated by <c>CanManageMCPServersContent</c>, not exposed to the portal.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class McpToolSchemaService : BaseService, IReadOnlySessionState
	{

		#region Constants: Private

		private const string ManageContentOperation = "CanManageMCPServersContent";
		// Source-type discriminators the client sends. Anything other than the
		// source-code value is treated as a process (the original default).
		private const string SourceTypeSourceCode = "sourcecode";

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(McpToolSchemaService));

		#endregion

		#region Methods: Public

		/// <summary>
		/// Returns <c>{ inputSchema, outputSchema, description, unsupportedParameters }</c>
		/// for a tool source.
		/// </summary>
		/// <param name="sourceType">"process" (default) or "sourcecode".</param>
		/// <param name="identifier">Process schema name (process) or the action's
		/// assembly-qualified full type name (source code).</param>
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
		public string GetToolSchemas(string sourceType, string identifier) {
			var result = new Dictionary<string, object> {
				["inputSchema"] = string.Empty,
				["outputSchema"] = string.Empty,
				["description"] = string.Empty,
				["unsupportedParameters"] = new List<Dictionary<string, object>>()
			};
			if (!ToolServiceRequestGuard.IsAuthenticated(UserConnection)) {
				result["error"] = "Authentication is required.";
				return JsonConvert.SerializeObject(result);
			}
			if (!UserConnection.DBSecurityEngine.GetCanExecuteOperation(ManageContentOperation)) {
				result["error"] = "Access denied.";
				return JsonConvert.SerializeObject(result);
			}
			try {
				if (string.Equals(sourceType, SourceTypeSourceCode, StringComparison.OrdinalIgnoreCase)) {
					FillSourceCodeSchemas(identifier, result);
				} else {
					FillProcessSchemas(identifier, result);
				}
			} catch (Exception exception) {
				// Log the detail server-side; do not leak exception text to the caller.
				Log.Error($"GetToolSchemas failed for '{sourceType}' identifier '{identifier}'.", exception);
				result["error"] = "Failed to build tool schemas.";
			}
			return JsonConvert.SerializeObject(result);
		}

		#endregion

		#region Methods: Private

		// Source code: resolve and build via the source-code facade (the same code that
		// derives the schema at tools/list time). Description and unsupportedParameters
		// are not applicable here — the picker already supplies the action's description,
		// and unsupported parameters surface as a schema-build error, not a soft warning.
		private void FillSourceCodeSchemas(string actionTypeName, Dictionary<string, object> result) {
			ISourceCodeRuntimeToolFacade facade = ClassFactory.Get<ISourceCodeRuntimeToolFacade>(
				new ConstructorArgument("userConnection", UserConnection));
			var tool = new McpToolModel {
				SourceCodeAction = actionTypeName,
				ToolSourceTypeId = McpToolSourceTypes.SourceCodeAction
			};
			RuntimeToolSchemaResult inputSchema = facade.GetInputSchema(tool);
			if (inputSchema == null || !inputSchema.IsSuccess || inputSchema.Schema == null) {
				// The action type no longer resolves (renamed/removed) or its schema
				// can't be built. Signal failure so the caller doesn't overwrite the
				// stored schema with an empty value or report a false success.
				result["error"] = "Source code action could not be resolved.";
				return;
			}
			result["inputSchema"] = inputSchema.Schema.Value;
			RuntimeToolSchemaResult outputSchema = facade.GetOutputSchema(tool);
			if (outputSchema != null && outputSchema.IsSuccess && outputSchema.Schema != null) {
				result["outputSchema"] = outputSchema.Schema.Value;
			}
		}

		private void FillProcessSchemas(string processName, Dictionary<string, object> result) {
			List<ProcessSchemaParameter> parameters = ReadProcessParameters(processName);
			result["inputSchema"] = BuildSchema(parameters, isInput: true);
			result["outputSchema"] = BuildSchema(parameters, isInput: false);
			// The process' own description (set in the Process Designer) lives in the
			// schema metadata, NOT in SysSchema.Description / VwProcessLib.Description
			// (that DB column is virtually always empty).
			result["description"] = ReadProcessDescription(processName);
			// Input parameters whose type can't be represented as an MCP scalar — surfaced
			// so the form can warn the admin instead of failing only at call time.
			result["unsupportedParameters"] = GetUnsupportedInputParameters(parameters);
		}

		private List<ProcessSchemaParameter> ReadProcessParameters(string processName) {
			var manager = UserConnection.ProcessSchemaManager;
			var schema = manager.GetInstanceByName(processName) as ProcessSchema;
			if (schema == null) {
				return new List<ProcessSchemaParameter>();
			}
			// schema.Parameters (the declared collection), NOT ForceGetParameters() —
			// the latter injects synthetic execution-only params. Same source the
			// runtime publishing path reads, so the preview matches what runs.
			return schema.Parameters
				.Where(p => !string.IsNullOrWhiteSpace(p.Name))
				.ToList();
		}

		// Reads the description authored in the Process Designer (a LocalizableString on
		// the schema), distinct from the DB-level SysSchema.Description. Falls back to the
		// schema caption when no description was provided.
		private string ReadProcessDescription(string processName) {
			var manager = UserConnection.ProcessSchemaManager;
			var schema = manager.GetInstanceByName(processName) as ProcessSchema;
			if (schema == null) {
				return string.Empty;
			}
			try {
				var description = schema.Description != null ? schema.Description.ToString() : string.Empty;
				if (!string.IsNullOrWhiteSpace(description)) {
					return description;
				}
				var caption = schema.Caption != null ? schema.Caption.ToString() : string.Empty;
				return caption ?? string.Empty;
			} catch {
				return string.Empty;
			}
		}

		// Delegates the per-parameter mapping to ProcessRuntimeToolFacade — the single
		// source of truth — so this admin preview is byte-for-byte what tools/list
		// advertises. throwOnUnsupported:false so an unsupported parameter still renders a
		// best-effort preview; the genuinely unsupported set is surfaced separately.
		private string BuildSchema(List<ProcessSchemaParameter> parameters, bool isInput) {
			var properties = new Dictionary<string, object>();
			var required = new List<string>();
			foreach (ProcessSchemaParameter prm in parameters) {
				if (isInput ? !ProcessRuntimeToolFacade.IsInputParameter(prm) : !ProcessRuntimeToolFacade.IsOutputParameter(prm)) {
					continue;
				}
				properties[prm.Name] =
					ProcessRuntimeToolFacade.BuildParameterPropertySchema(prm, throwOnUnsupported: false, lookupAsObject: !isInput);
				if (isInput && prm.IsRequired) {
					required.Add(prm.Name);
				}
			}
			var schema = new Dictionary<string, object>();
			schema["type"] = "object";
			schema["properties"] = properties;
			// Match the runtime input schema: input is locked to declared arguments;
			// output stays open (advisory).
			if (isInput) {
				schema["additionalProperties"] = false;
			}
			if (required.Count > 0) {
				schema["required"] = required;
			}
			return JsonConvert.SerializeObject(schema);
		}

		private List<Dictionary<string, object>> GetUnsupportedInputParameters(List<ProcessSchemaParameter> parameters) {
			var unsupported = new List<Dictionary<string, object>>();
			foreach (ProcessSchemaParameter prm in parameters) {
				if (!ProcessRuntimeToolFacade.IsInputParameter(prm)) {
					continue;
				}
				if (!ProcessRuntimeToolFacade.IsSupportedParameterType(prm)) {
					unsupported.Add(new Dictionary<string, object> {
						["name"] = prm.Name,
						["type"] = prm.DataValueType?.Name ?? "unknown"
					});
				}
			}
			return unsupported;
		}

		#endregion

	}
}

