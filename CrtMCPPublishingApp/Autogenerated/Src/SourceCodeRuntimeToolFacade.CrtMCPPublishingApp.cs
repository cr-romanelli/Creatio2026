using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Creatio.Copilot.Actions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.ServiceModelContract;

	#region Class: SourceCodeRuntimeToolFacade

	/// <summary>
	/// Default <see cref="ISourceCodeRuntimeToolFacade"/> implementation. Reuses Creatio
	/// Copilot's <see cref="BaseExecutableCodeAction"/>: a tool stores the action's
	/// assembly-qualified full type name in <see cref="McpToolModel.SourceCodeAction"/>;
	/// this facade resolves it with <c>Type.GetType</c> (a keyed lookup — no reflection
	/// scan), instantiates it, injects the <see cref="UserConnection"/> when required,
	/// and invokes <c>Execute</c>. Input and output schemas are derived from the action's
	/// declared <c>Parameters</c> via the shared <see cref="ToolSchemaBuilder"/> (so complex
	/// object / object-list / lookup params map identically to the business-process path).
	/// Execution is synchronous (no suspend/resume — that stays exclusive to the
	/// business-process bridge path in <see cref="ProcessRuntimeToolFacade"/>).
	/// </summary>
	[DefaultBinding(typeof(ISourceCodeRuntimeToolFacade))]
	internal class SourceCodeRuntimeToolFacade : ISourceCodeRuntimeToolFacade
	{

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(SourceCodeRuntimeToolFacade));
		// Property name for the synthetic output of an action that declares no Output
		// params: both the fallback output schema and the runtime structured value use it.
		private const string ResultPropertyName = "result";
		// Execution authorization gate. Creatio has no per-source-code-action execute
		// right (unlike business processes, which also support per-schema rights), so we
		// reuse the run-automation operation the business-process MCP tools already
		// require — giving both tool kinds a single, admin-manageable permission. Without
		// it, any authenticated session could invoke any published source-code action.
		private const string RunActionOperationName = "CanRunBusinessProcesses";
		private readonly UserConnection _userConnection;
		// Per-request cache of resolved actions keyed by McpTool.Id. tools/list resolves
		// each tool 3x (GetCandidateTools + GetInputSchema + GetOutputSchema); this
		// collapses the Type.GetType + Activator.CreateInstance to once per tool. The
		// facade is request-scoped, so the cache is short-lived. Only the read/schema
		// paths reuse it — Execute resolves a fresh instance so a run never reuses an
		// instance already used elsewhere.
		private readonly Dictionary<Guid, BaseExecutableCodeAction> _actionByToolId =
			new Dictionary<Guid, BaseExecutableCodeAction>();

		#endregion

		#region Constructors: Public

		public SourceCodeRuntimeToolFacade(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Private

		// useCache: reuse a per-request resolved instance keyed by McpTool.Id (safe for the
		// read/schema paths). Execute passes false so a run never reuses an instance.
		private bool TryResolveAction(McpToolModel tool, bool useCache, out BaseExecutableCodeAction action,
				out string error) {
			action = null;
			error = null;
			Guid cacheKey = tool != null ? tool.Id : Guid.Empty;
			if (useCache && cacheKey != Guid.Empty && _actionByToolId.TryGetValue(cacheKey, out action)) {
				return true;
			}
			string typeName = tool != null ? tool.SourceCodeAction : null;
			if (typeName.IsNullOrWhiteSpace()) {
				error = "Tool does not reference a source code action.";
				return false;
			}
			Type type;
			try {
				type = Type.GetType(typeName, false);
			} catch (Exception exception) {
				Log.Debug($"Failed to resolve source code action type '{typeName}'.", exception);
				error = "Source code action type could not be resolved.";
				return false;
			}
			if (type == null) {
				error = $"Source code action type '{typeName}' was not found.";
				return false;
			}
			if (type.IsAbstract || !typeof(BaseExecutableCodeAction).IsAssignableFrom(type)) {
				error = $"Type '{typeName}' is not an executable code action.";
				return false;
			}
			try {
				var instance = (BaseExecutableCodeAction)Activator.CreateInstance(type);
				if (instance is IUserConnectionRequired userConnectionRequired) {
					userConnectionRequired.SetUserConnection(_userConnection);
				}
				action = instance;
				if (useCache && cacheKey != Guid.Empty) {
					_actionByToolId[cacheKey] = instance;
				}
				return true;
			} catch (Exception exception) {
				Log.Debug($"Failed to instantiate source code action '{typeName}'.", exception);
				error = "Source code action could not be instantiated.";
				return false;
			}
		}

		// Adapt the action's declared parameters to the shared IToolParameter so the same
		// ToolSchemaBuilder used for business processes maps them (scalars, composite objects,
		// object lists, lookups, nested item properties).
		private IEnumerable<IToolParameter> AdaptParameters(BaseExecutableCodeAction action) {
			return (action.Parameters ?? (IReadOnlyList<SourceCodeActionParameter>)Array.Empty<SourceCodeActionParameter>())
				.Where(parameter => parameter != null)
				.Select(parameter => (IToolParameter)new SourceCodeActionParameterToolParameter(parameter, _userConnection));
		}

		// Same string-form transport as the BP path (ProcessRuntimeToolFacade): scalars keep
		// their invariant string form, complex graphs are emitted as JSON.
		private static Dictionary<string, string> GetParameterValues(IDictionary<string, object> arguments) {
			var result = new Dictionary<string, string>();
			if (arguments == null) {
				return result;
			}
			foreach (KeyValuePair<string, object> argument in arguments) {
				result[argument.Key] = SerializeArgumentValue(argument.Value);
			}
			return result;
		}

		private static string SerializeArgumentValue(object value) {
			if (value == null) {
				return null;
			}
			if (value is string stringValue) {
				return stringValue;
			}
			if (value is IDictionary || value is IEnumerable) {
				return JsonConvert.SerializeObject(value);
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		// Builds the structured output to conform to the advertised outputSchema (see
		// GetOutputSchema): for an action that declares Output params, the Response parsed
		// as the declared JSON object; for one that declares none, { result: <Response> }.
		private Dictionary<string, object> BuildStructuredOutput(BaseExecutableCodeAction action, string response) {
			if (AdaptParameters(action).Any(parameter => parameter.IsOutput)) {
				Dictionary<string, object> parsed = TryParseJsonObject(response);
				if (parsed == null) {
					// The action declares an output contract but its Response is not a JSON
					// object, so it can't be mapped onto the advertised schema. Leave structured
					// output empty (text block still carries the raw Response) — author's bug.
					Log.Warn("Source code action declares output parameters but its Response " +
						"is not a JSON object; returning no structuredContent.");
				}
				return parsed;
			}
			return new Dictionary<string, object> { [ResultPropertyName] = response ?? string.Empty };
		}

		// Parses text into a dictionary when it is a JSON object; null otherwise (array,
		// scalar, or non-JSON). Lenient — no strict validation against the output schema.
		private static Dictionary<string, object> TryParseJsonObject(string text) {
			if (text.IsNullOrWhiteSpace()) {
				return null;
			}
			try {
				if (JToken.Parse(text) is JObject jObject) {
					return jObject.ToObject<Dictionary<string, object>>();
				}
			} catch {
				// Not JSON (or not an object).
			}
			return null;
		}

		#endregion

		#region Methods: Public

		public IReadOnlyCollection<RuntimeToolDefinition> GetCandidateTools(IReadOnlyCollection<McpToolModel> tools) {
			var definitions = new List<RuntimeToolDefinition>();
			// Authorization gate, mirroring the business-process facade
			// (IsAvailableForManualRun): a user without the run-automation operation gets
			// no source-code candidates, so such tools are excluded from tools/list and
			// rejected by tools/call. The check is user-scoped, so evaluate it once.
			if (!_userConnection.DBSecurityEngine.GetCanExecuteOperation(RunActionOperationName)) {
				return definitions;
			}
			foreach (McpToolModel tool in tools ?? Enumerable.Empty<McpToolModel>()) {
				if (!TryResolveAction(tool, useCache: true, out BaseExecutableCodeAction action, out string error)) {
					Log.Warn($"Source code action tool '{tool?.ExternalName ?? tool?.Id.ToString()}' " +
						$"was skipped: {error}");
					continue;
				}
				LocalizableString caption = action.GetCaption();
				LocalizableString description = action.GetDescription();
				definitions.Add(new RuntimeToolDefinition {
					CanDeriveSchema = true,
					CanExecute = true,
					Description = description != null ? description.ToString() : null,
					IsEnabled = action.IsEnabled,
					Kind = RuntimeToolKind.SourceCodeAction,
					Name = tool.ExternalName,
					RuntimeSource = "SourceCodeAction",
					Title = caption != null ? caption.ToString() : null,
					ToolUId = tool.Id
				});
			}
			return definitions;
		}

		public RuntimeToolSchemaResult GetInputSchema(McpToolModel tool) {
			try {
				if (!TryResolveAction(tool, useCache: true, out BaseExecutableCodeAction action, out string error)) {
					return new RuntimeToolSchemaResult {
						ErrorCode = "ActionTypeNotFound",
						ErrorMessage = error,
						IsSuccess = false
					};
				}
				return new RuntimeToolSchemaResult {
					IsSuccess = true,
					Schema = JsonRaw.Create(JsonConvert.SerializeObject(
						ToolSchemaBuilder.BuildSchemaObject(AdaptParameters(action), isInput: true)))
				};
			} catch (NotSupportedException exception) {
				return new RuntimeToolSchemaResult {
					ErrorCode = "UnsupportedParameterType",
					ErrorMessage = exception.Message,
					IsSuccess = false
				};
			} catch (Exception exception) {
				return new RuntimeToolSchemaResult {
					ErrorCode = "SchemaBuildFailed",
					ErrorMessage = exception.Message,
					IsSuccess = false
				};
			}
		}

		// An action MAY declare Output/Var parameters (SourceCodeActionParameter.Direction) —
		// these describe the shape the action puts in its CopilotActionExecutionResult.Response
		// (the framework doesn't bind them; it's the author's declared output contract). When
		// present we derive an outputSchema from them via the shared builder, just like inputs.
		// When the action declares no output parameters (e.g. FindLookupValueAction) we advertise
		// a canonical single-string { result } schema — the shape the runtime's structuredContent
		// ({ result: <Response> }) conforms to — so the tool always ships with an outputSchema.
		// The admin can still override it via McpTool.OutputSchema.
		public RuntimeToolSchemaResult GetOutputSchema(McpToolModel tool) {
			try {
				if (!TryResolveAction(tool, useCache: true, out BaseExecutableCodeAction action, out string error)) {
					return new RuntimeToolSchemaResult {
						ErrorCode = "ActionTypeNotFound",
						ErrorMessage = error,
						IsSuccess = false
					};
				}
				List<IToolParameter> parameters = AdaptParameters(action).ToList();
				if (!parameters.Any(parameter => parameter.IsOutput)) {
					// No declared output contract: advertise a canonical single-string result so
					// the tool still has an outputSchema that the runtime { result: <Response> }
					// structuredContent conforms to.
					var fallback = new Dictionary<string, object> {
						["type"] = "object",
						["properties"] = new Dictionary<string, object> {
							[ResultPropertyName] = new Dictionary<string, object> { ["type"] = "string" }
						}
					};
					return new RuntimeToolSchemaResult {
						IsSuccess = true,
						Schema = JsonRaw.Create(JsonConvert.SerializeObject(fallback))
					};
				}
				return new RuntimeToolSchemaResult {
					IsSuccess = true,
					// lookupAsObject:false — a source-code action has no bridge enricher, so an
					// output lookup stays a bare uuid string rather than implying the
					// { value, displayValue } shape the action would not actually produce. An
					// author who enriches must declare that shape (e.g. McpTool.OutputSchema).
					Schema = JsonRaw.Create(JsonConvert.SerializeObject(
						ToolSchemaBuilder.BuildSchemaObject(parameters, isInput: false, lookupAsObject: false)))
				};
			} catch (Exception exception) {
				return new RuntimeToolSchemaResult {
					ErrorCode = "OutputSchemaBuildFailed",
					ErrorMessage = exception.Message,
					IsSuccess = false
				};
			}
		}

		public RuntimeToolExecutionResult Execute(McpToolModel tool, IDictionary<string, object> arguments,
				RuntimeToolExecutionOptions executionOptions = null) {
			try {
				// Defense in depth: the call path authorizes via GetCandidateTools first,
				// but re-check here so Execute can never run an action for a user without
				// the run-automation operation.
				if (!_userConnection.DBSecurityEngine.GetCanExecuteOperation(RunActionOperationName)) {
					return new RuntimeToolExecutionResult {
						IsSuccess = false,
						ErrorMessage = "Not authorized to run source code actions."
					};
				}
				if (!TryResolveAction(tool, useCache: false, out BaseExecutableCodeAction action, out string error)) {
					return new RuntimeToolExecutionResult { IsSuccess = false, ErrorMessage = error };
				}
				CopilotActionExecutionResult result = action.Execute(new ActionExecutionOptions {
					ParameterValues = GetParameterValues(arguments)
				});
				if (result == null) {
					// A well-behaved action returns a CopilotActionExecutionResult; null
					// signals a misbehaving action. Don't mask it silently — log it, then
					// surface an empty completed result (no Response to return).
					Log.Warn($"Source code action '{tool?.SourceCodeAction}' returned a null result.");
				}
				string response = result != null ? result.Response : null;
				bool isSuccess = result == null || result.Status != CopilotActionExecutionStatus.Failed;
				return new RuntimeToolExecutionResult {
					IsSuccess = isSuccess,
					Status = isSuccess ? "completed" : "failed",
					ErrorMessage = result != null ? result.ErrorMessage : null,
					Response = response,
					StructuredOutput = BuildStructuredOutput(action, response)
				};
			} catch (Exception exception) {
				Log.Error("Unexpected exception while executing source code action tool.", exception);
				return new RuntimeToolExecutionResult { IsSuccess = false, ErrorMessage = exception.Message };
			}
		}

		#endregion

		#region Class: SourceCodeActionParameterToolParameter

		// Adapts a Copilot SourceCodeActionParameter to the shared IToolParameter. Direction
		// split: Input/Var are inputs, Output/Var are outputs. The DataValueTypeUId is resolved
		// to a concrete DataValueType via the manager, so ToolSchemaBuilder's complex-type
		// dispatch (which keys off the DataValueType class) works exactly as for processes.
		private sealed class SourceCodeActionParameterToolParameter : IToolParameter
		{

			private readonly SourceCodeActionParameter _parameter;
			private readonly UserConnection _userConnection;
			// The shared ToolSchemaBuilder reads DataValueType several times per parameter
			// (value type, lookup check, complex-type dispatch, format); memoize the
			// manager lookup so it resolves once per adapter instance.
			private Terrasoft.Core.DataValueType _dataValueType;
			private bool _dataValueTypeResolved;

			public SourceCodeActionParameterToolParameter(SourceCodeActionParameter parameter,
					UserConnection userConnection) {
				_parameter = parameter;
				_userConnection = userConnection;
			}

			public string Name => _parameter.Name;

			public string Description {
				get {
					string description = _parameter.Description?.ToString();
					return description.IsNullOrWhiteSpace() ? _parameter.Caption?.ToString() : description;
				}
			}

			public bool IsRequired => _parameter.IsRequired;

			public bool IsInput =>
				_parameter.Direction == ParameterDirection.Input ||
				_parameter.Direction == ParameterDirection.Var;

			public bool IsOutput =>
				_parameter.Direction == ParameterDirection.Output ||
				_parameter.Direction == ParameterDirection.Var;

			public Terrasoft.Core.DataValueType DataValueType {
				get {
					if (_dataValueTypeResolved) {
						return _dataValueType;
					}
					try {
						_dataValueType = _userConnection.DataValueTypeManager.GetInstanceByUId(_parameter.DataValueTypeUId);
					} catch {
						_dataValueType = null;
					}
					_dataValueTypeResolved = true;
					return _dataValueType;
				}
			}

			public IReadOnlyList<IToolParameter> ItemProperties {
				get {
					IReadOnlyList<SourceCodeActionParameter> itemProperties = _parameter.ItemProperties;
					if (itemProperties == null) {
						return Array.Empty<IToolParameter>();
					}
					var list = new List<IToolParameter>();
					foreach (SourceCodeActionParameter nested in itemProperties) {
						list.Add(new SourceCodeActionParameterToolParameter(nested, _userConnection));
					}
					return list;
				}
			}

		}

		#endregion

	}

	#endregion

}

