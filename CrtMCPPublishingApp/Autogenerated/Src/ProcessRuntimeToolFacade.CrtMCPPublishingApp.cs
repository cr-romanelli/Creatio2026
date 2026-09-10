using Terrasoft.Core.DB;
using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Linq;
	using System.Security;
	using CrtAIAgenticBridgeApp.EntryPoints.WebServices;
	using CrtAIAgenticBridgeApp.Runtime;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Process;
	using Terrasoft.Core.ServiceModelContract;

	#region Class: ProcessRuntimeToolFacade

	[DefaultBinding(typeof(IProcessRuntimeToolFacade))]
	internal class ProcessRuntimeToolFacade : IProcessRuntimeToolFacade
	{

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(ProcessRuntimeToolFacade));
		private readonly UserConnection _userConnection;
		// Per-instance cache for SysSchema.Id -> process schema UId. tools/list resolves
		// input + output schemas per tool, and tools/call resolves the same tool twice
		// (GetInputSchema + Execute); without this each resolution is a separate SysSchema
		// query (an N+1 pattern). The facade is request-scoped, so the cache is short-lived.
		private readonly Dictionary<Guid, Guid> _schemaUIdBySysSchemaId = new Dictionary<Guid, Guid>();
		// Per-instance cache for SysSchema.Id -> resolved active ProcessSchema. The
		// authorize pass, GetInputSchema and GetOutputSchema all resolve the same
		// schema for a tool within one request (and tools/call resolves it again for
		// Execute); this collapses those to a single GetItemByUId/GetActiveVersionItem
		// lookup. Primed by the candidate scan so the schema is resolved once per tool.
		private readonly Dictionary<Guid, ProcessSchema> _schemaByToolUId = new Dictionary<Guid, ProcessSchema>();
		private const string RunProcessOperationName = "CanRunBusinessProcesses";

		#endregion

		#region Constructors: Public

		public ProcessRuntimeToolFacade(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Private

		private static JsonRaw BuildInputSchema(ProcessSchema schema) {
			schema.CheckArgumentNull(nameof(schema));
			return JsonRaw.Create(JsonConvert.SerializeObject(
				ToolSchemaBuilder.BuildSchemaObject(AdaptParameters(schema.Parameters), isInput: true)));
		}

		private static JsonRaw BuildOutputSchema(ProcessSchema schema) {
			schema.CheckArgumentNull(nameof(schema));
			return JsonRaw.Create(JsonConvert.SerializeObject(
				ToolSchemaBuilder.BuildSchemaObject(AdaptParameters(schema.Parameters), isInput: false)));
		}

		// Adapt the declared parameter collection (NOT ForceGetParameters() — that injects
		// synthetic execution-only params) to the source-agnostic IToolParameter the shared
		// ToolSchemaBuilder consumes.
		private static IEnumerable<IToolParameter> AdaptParameters(IEnumerable<ProcessSchemaParameter> parameters) {
			return (parameters ?? Enumerable.Empty<ProcessSchemaParameter>())
				.Where(parameter => parameter != null)
				.Select(parameter => (IToolParameter)new ProcessSchemaParameterToolParameter(parameter));
		}

		private IReadOnlyCollection<RuntimeToolDefinition> BuildToolDefinitions(
				IEnumerable<ISchemaManagerItem<ProcessSchema>> items) {
			List<ISchemaManagerItem<ProcessSchema>> itemList = items.ToList();
			IReadOnlyDictionary<Guid, Guid> schemaUIdToSysSchemaId =
				ResolveSysSchemaIdsByUId(itemList.Select(item => item.UId).Distinct().ToList());
			return itemList
				.Select(item => ToToolDefinition(item, schemaUIdToSysSchemaId))
				.ToList();
		}

		// Looks up only the supplied tools' process schemas (by SysSchema.Id) instead of
		// walking the whole ProcessSchemaManager catalog. Applies the publish gates —
		// runnable Tag, active version, manual-run availability + RBAC (see
		// TryGetAcceptableActiveItem) — keeping the candidate scan O(server tools).
		private IEnumerable<ISchemaManagerItem<ProcessSchema>> GetAvailableProcessItemsByIds(
				IReadOnlyCollection<Guid> sysSchemaIds) {
			ProcessSchemaManager manager = _userConnection.ProcessSchemaManager;
			// Resolve every Id -> UId in one query rather than a single-row SysSchema
			// SELECT per tool, then the per-id ResolveSchemaUId calls below are all
			// cache hits.
			PrimeSchemaUIds(sysSchemaIds);
			var seenSchemaUIds = new HashSet<Guid>();
			foreach (Guid sysSchemaId in sysSchemaIds.Distinct()) {
				if (sysSchemaId == Guid.Empty) {
					continue;
				}
				Guid schemaUId = ResolveSchemaUId(sysSchemaId);
				ISchemaManagerItem<ProcessSchema> item;
				try {
					item = manager.GetItemByUId(schemaUId);
				} catch (Exception exception) {
					Log.Debug($"Failed to resolve process schema item for tool '{sysSchemaId}'.", exception);
					continue;
				}
				if (!TryGetAcceptableActiveItem(item, seenSchemaUIds, out ISchemaManagerItem<ProcessSchema> activeItem)) {
					continue;
				}
				// Prime the schema cache so GetInputSchema/GetOutputSchema reuse this
				// resolution instead of re-running GetItemByUId/GetActiveVersionItem.
				if (activeItem.Instance != null) {
					_schemaByToolUId[sysSchemaId] = activeItem.Instance;
				}
				yield return activeItem;
			}
		}

		// Single source of truth for the publish gate: the item must carry a runnable Tag,
		// resolve to an active version not already seen this pass, and pass the manual-run
		// availability + RBAC check.
		private bool TryGetAcceptableActiveItem(ISchemaManagerItem<ProcessSchema> item,
				HashSet<Guid> seenSchemaUIds, out ISchemaManagerItem<ProcessSchema> activeItem) {
			activeItem = null;
			if (item == null ||
					item.ExtraProperties.FindValueByName("Tag", string.Empty).IsNullOrWhiteSpace()) {
				return false;
			}
			ISchemaManagerItem<ProcessSchema> resolved;
			try {
				resolved = _userConnection.ProcessSchemaManager.GetActiveVersionItem(item);
			} catch (Exception exception) {
				Log.Debug($"Failed to get active version item for process schema '{item.UId}'.", exception);
				return false;
			}
			if (resolved == null || !seenSchemaUIds.Add(resolved.UId)) {
				return false;
			}
			if (!IsAvailableForManualRun(resolved)) {
				return false;
			}
			activeItem = resolved;
			return true;
		}

		// Resolves SysSchema.Id -> UId for the whole set in a single query and primes
		// _schemaUIdBySysSchemaId. Ids with no SysSchema row fall back to themselves
		// (legacy data where the stored value is already a UId).
		private void PrimeSchemaUIds(IReadOnlyCollection<Guid> sysSchemaIds) {
			var pending = sysSchemaIds
				.Where(id => id != Guid.Empty && !_schemaUIdBySysSchemaId.ContainsKey(id))
				.Distinct()
				.ToList();
			if (pending.Count == 0) {
				return;
			}
			var select = (Terrasoft.Core.DB.Select)new Terrasoft.Core.DB.Select(_userConnection)
				.Column("Id")
				.Column("UId")
				.From("SysSchema");
			select.Where("Id").In(Terrasoft.Core.DB.Column.Parameters(pending));
			using (Terrasoft.Core.DB.DBExecutor dbExecutor = _userConnection.EnsureDBConnection()) {
				using (System.Data.IDataReader reader = select.ExecuteReader(dbExecutor)) {
					while (reader.Read()) {
						if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) {
							_schemaUIdBySysSchemaId[reader.GetGuid(0)] = reader.GetGuid(1);
						}
					}
				}
			}
			foreach (Guid id in pending) {
				if (!_schemaUIdBySysSchemaId.ContainsKey(id)) {
					_schemaUIdBySysSchemaId[id] = id;
				}
			}
		}

		private static string GetDescription(ProcessSchema schema) {
			schema.CheckArgumentNull(nameof(schema));
			return schema.Caption != null && schema.Caption.ToString().IsNotNullOrWhiteSpace()
				? schema.Caption.ToString()
				: schema.Name;
		}

		private static IReadOnlyDictionary<string, string> GetParameterValues(IDictionary<string, object> arguments) {
			if (arguments == null) {
				return new Dictionary<string, string>();
			}
			var result = new Dictionary<string, string>(arguments.Count);
			foreach (KeyValuePair<string, object> argument in arguments) {
				result[argument.Key] = SerializeArgumentValue(argument.Value);
			}
			return result;
		}

		// The bridge transports every parameter value as a string and the BP value
		// initializer re-parses it by the declared parameter type. Scalars keep the
		// invariant string form; complex values (Dictionary/List graphs) are emitted as
		// JSON so the bridge's WithParseSerializableObjectAsJson() can deserialize them.
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

		private static Collection<NameValuePair> ToNameValuePairs(IReadOnlyDictionary<string, string> values) {
			var result = new Collection<NameValuePair>();
			foreach (KeyValuePair<string, string> pair in values) {
				result.Add(new NameValuePair { Name = pair.Key, Value = pair.Value });
			}
			return result;
		}

		private static IEnumerable<string> GetResultParameterNames(ProcessSchema schema,
				RuntimeToolExecutionOptions executionOptions) {
			schema.CheckArgumentNull(nameof(schema));
			// Mirror the output schema: read back Out + Variable (bidirectional) + IsResult
			// parameters so the values returned match the advertised output schema.
			var resultParameterNames = schema.Parameters
				.Where(parameter =>
					parameter.IsResult ||
					parameter.Direction == ProcessSchemaParameterDirection.Out ||
					parameter.Direction == ProcessSchemaParameterDirection.Variable)
				.Select(parameter => parameter.Name)
				.Where(name => name.IsNotNullOrWhiteSpace());
			IEnumerable<string> requestedNames = executionOptions?.ResultParameterNames;
			if (requestedNames.IsNullOrEmpty()) {
				return resultParameterNames.Distinct().ToArray();
			}
			// Caller-supplied names are passed to ProcessParameterValueReceiver.Verify just
			// like the derived ones, so they must also exist in schema.Parameters.
			var declaredNames = new HashSet<string>(
				schema.Parameters
					.Select(parameter => parameter.Name)
					.Where(name => name.IsNotNullOrWhiteSpace()));
			IEnumerable<string> validRequestedNames = requestedNames
				.Where(name => name.IsNotNullOrWhiteSpace() && declaredNames.Contains(name));
			return resultParameterNames.Concat(validRequestedNames).Distinct().ToArray();
		}

		private bool IsAvailableForManualRun(ISchemaManagerItem<ProcessSchema> item) {
			item.CheckArgumentNull(nameof(item));
			try {
				var processItem = (IBaseProcessSchemaManagerItem)item;
				if (!processItem.GetIsEnabled()) {
					return false;
				}
				processItem.Verify(ProcessStartType.Manual);
				DBSecurityEngine dbSecurityEngine = _userConnection.DBSecurityEngine;
				if (dbSecurityEngine.GetCanExecuteOperation(RunProcessOperationName)) {
					return true;
				}
				dbSecurityEngine.CheckCanExecute(item.UId, _userConnection.CurrentUser.Id);
				return true;
			} catch (SecurityException) {
				return false;
			} catch (Exception exception) {
				Log.Warn($"Failed to evaluate process availability for schema '{item?.UId}'.", exception);
				return false;
			}
		}

		private ProcessSchema ResolveSchema(Guid toolUId) {
			if (_schemaByToolUId.TryGetValue(toolUId, out ProcessSchema cachedSchema)) {
				return cachedSchema;
			}
			ProcessSchemaManager manager = _userConnection.ProcessSchemaManager;
			// A tool's process is stored as a SysSchema lookup whose FK value is the
			// SysSchema.Id, but the process schema manager keys items by UId (Id != UId
			// for SysSchema). Resolve Id -> UId before looking the schema up.
			Guid schemaUId = ResolveSchemaUId(toolUId);
			ISchemaManagerItem<ProcessSchema> schemaItem = manager.GetItemByUId(schemaUId);
			ProcessSchema schema = manager.GetActiveVersionItem(schemaItem)?.Instance ?? schemaItem?.Instance;
			if (schema == null) {
				throw new InvalidOperationException($"Process schema '{toolUId}' was not found.");
			}
			_schemaByToolUId[toolUId] = schema;
			return schema;
		}

		private Guid ResolveSchemaUId(Guid sysSchemaId) {
			if (sysSchemaId == Guid.Empty) {
				return sysSchemaId;
			}
			if (_schemaUIdBySysSchemaId.TryGetValue(sysSchemaId, out Guid cachedUId)) {
				return cachedUId;
			}
			// Fallback: the value may already be a process schema UId (legacy data).
			Guid resolvedUId = sysSchemaId;
			var select = (Terrasoft.Core.DB.Select)new Terrasoft.Core.DB.Select(_userConnection)
				.Column("UId")
				.From("SysSchema")
				.Where("Id").IsEqual(Terrasoft.Core.DB.Column.Parameter(sysSchemaId));
			using (Terrasoft.Core.DB.DBExecutor dbExecutor = _userConnection.EnsureDBConnection()) {
				using (System.Data.IDataReader reader = select.ExecuteReader(dbExecutor)) {
					if (reader.Read() && !reader.IsDBNull(0)) {
						resolvedUId = reader.GetGuid(0);
					}
				}
			}
			_schemaUIdBySysSchemaId[sysSchemaId] = resolvedUId;
			return resolvedUId;
		}

		private static RuntimeToolDefinition ToToolDefinition(ISchemaManagerItem<ProcessSchema> item,
				IReadOnlyDictionary<Guid, Guid> schemaUIdToSysSchemaId) {
			ProcessSchema schema = item.Instance;
			// A tool stores its process as a SysSchema lookup whose FK value is the
			// SysSchema.Id (Id != UId). The candidate must advertise the SysSchema.Id here
			// — not the schema UId — so the match path finds it. Fall back to the UId for
			// legacy data whose SysSchema row can't be resolved.
			Guid sysSchemaId = schemaUIdToSysSchemaId.TryGetValue(item.UId, out Guid resolvedId)
				? resolvedId
				: item.UId;
			return new RuntimeToolDefinition {
				CanDeriveSchema = true,
				CanExecute = true,
				Description = GetDescription(schema),
				IsEnabled = true,
				Kind = RuntimeToolKind.BusinessProcess,
				Name = schema.Name,
				RuntimeSource = "ProcessSchema",
				Title = schema.Caption?.ToString(),
				ToolUId = sysSchemaId
			};
		}

		private IReadOnlyDictionary<Guid, Guid> ResolveSysSchemaIdsByUId(IReadOnlyCollection<Guid> schemaUIds) {
			var map = new Dictionary<Guid, Guid>();
			if (schemaUIds == null || schemaUIds.Count == 0) {
				return map;
			}
			var select = (Terrasoft.Core.DB.Select)new Terrasoft.Core.DB.Select(_userConnection)
				.Column("Id")
				.Column("UId")
				.From("SysSchema");
			select.Where("UId").In(Terrasoft.Core.DB.Column.Parameters(schemaUIds));
			using (Terrasoft.Core.DB.DBExecutor dbExecutor = _userConnection.EnsureDBConnection()) {
				using (System.Data.IDataReader reader = select.ExecuteReader(dbExecutor)) {
					while (reader.Read()) {
						if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) {
							// key by UId -> value SysSchema.Id
							map[reader.GetGuid(1)] = reader.GetGuid(0);
						}
					}
				}
			}
			return map;
		}

		// Copy a process-derived definition but re-key it onto the tool's own Id. The scan
		// keys definitions by process SysSchema.Id; the runtime addresses tools by McpTool.Id.
		private static RuntimeToolDefinition WithToolUId(RuntimeToolDefinition definition, Guid toolUId) {
			return new RuntimeToolDefinition {
				CanDeriveSchema = definition.CanDeriveSchema,
				CanExecute = definition.CanExecute,
				Description = definition.Description,
				IsEnabled = definition.IsEnabled,
				Kind = definition.Kind,
				Name = definition.Name,
				RuntimeSource = definition.RuntimeSource,
				Title = definition.Title,
				ToolUId = toolUId
			};
		}

		#endregion

		#region Methods: Internal

		// Thin delegations preserved for McpToolSchemaService's process preview, which works
		// directly with ProcessSchemaParameter. The real logic lives in ToolSchemaBuilder; the
		// adapter bridges the two so the preview and the runtime publishing path stay aligned.
		internal static bool IsInputParameter(ProcessSchemaParameter parameter) {
			return new ProcessSchemaParameterToolParameter(parameter).IsInput;
		}

		internal static bool IsOutputParameter(ProcessSchemaParameter parameter) {
			return new ProcessSchemaParameterToolParameter(parameter).IsOutput;
		}

		internal static Dictionary<string, object> BuildParameterPropertySchema(ProcessSchemaParameter parameter,
				bool throwOnUnsupported, bool lookupAsObject = false) {
			return ToolSchemaBuilder.BuildParameterPropertySchema(
				new ProcessSchemaParameterToolParameter(parameter), throwOnUnsupported, lookupAsObject);
		}

		internal static bool IsSupportedParameterType(ProcessSchemaParameter parameter) {
			return ToolSchemaBuilder.IsSupportedParameterType(new ProcessSchemaParameterToolParameter(parameter));
		}

		// Process-parameter entry point over the shared builder (adapts each parameter to
		// IToolParameter). Single-sources the mapping for the admin preview and keeps a
		// ProcessSchemaParameter-based surface for callers that work with raw process params.
		internal static Dictionary<string, object> BuildSchemaObject(
				IEnumerable<ProcessSchemaParameter> parameters, bool isInput) {
			return ToolSchemaBuilder.BuildSchemaObject(AdaptParameters(parameters), isInput);
		}

		#endregion

		#region Methods: Public

		public IReadOnlyCollection<RuntimeToolDefinition> GetCandidateTools(
				IReadOnlyCollection<McpToolModel> tools) {
			List<McpToolModel> toolList = (tools ?? Enumerable.Empty<McpToolModel>())
				.Where(tool => tool != null)
				.ToList();
			if (toolList.Count == 0) {
				return Array.Empty<RuntimeToolDefinition>();
			}
			List<Guid> sysSchemaIds = toolList
				.Select(tool => tool.ProcessId)
				.Where(id => !id.IsEmpty())
				.Distinct()
				.ToList();
			// Scan resolves one definition per process (keyed by the process SysSchema.Id).
			Dictionary<Guid, RuntimeToolDefinition> definitionsByProcessId =
				BuildToolDefinitions(GetAvailableProcessItemsByIds(sysSchemaIds))
					.Where(definition => definition != null)
					.GroupBy(definition => definition.ToolUId)
					.ToDictionary(group => group.Key, group => group.First());
			// Emit one definition per tool, re-keyed onto the tool's own Id — the uniform
			// identity both kinds advertise. Two tools may share a process; each still gets
			// its own definition.
			var result = new List<RuntimeToolDefinition>();
			foreach (McpToolModel tool in toolList) {
				if (definitionsByProcessId.TryGetValue(tool.ProcessId, out RuntimeToolDefinition definition)) {
					result.Add(WithToolUId(definition, tool.Id));
				}
			}
			return result;
		}

		public RuntimeToolSchemaResult GetInputSchema(Guid toolUId) {
			try {
				return new RuntimeToolSchemaResult {
					IsSuccess = true,
					Schema = BuildInputSchema(ResolveSchema(toolUId))
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

		public RuntimeToolSchemaResult GetOutputSchema(Guid toolUId) {
			try {
				return new RuntimeToolSchemaResult {
					IsSuccess = true,
					Schema = BuildOutputSchema(ResolveSchema(toolUId))
				};
			} catch (Exception exception) {
				return new RuntimeToolSchemaResult {
					ErrorCode = "OutputSchemaBuildFailed",
					ErrorMessage = exception.Message,
					IsSuccess = false
				};
			}
		}

		public RuntimeToolExecutionResult Execute(Guid toolUId, IDictionary<string, object> arguments,
				RuntimeToolExecutionOptions executionOptions = null) {
			try {
				ProcessSchema schema = ResolveSchema(toolUId);
				IReadOnlyDictionary<string, string> parameterValues = GetParameterValues(arguments);
				IEnumerable<string> resultParameterNames = GetResultParameterNames(schema, executionOptions);
				// Per-call correlation id (minted like creatio_execute_process): a suspended
				// process resumes against this unique handle when the bridge reports back.
				string correlationId = Guid.NewGuid().ToString();
				var request = new RunAgenticProcessRequest {
					SchemaName = schema.Name,
					ParameterValues = ToNameValuePairs(parameterValues),
					ResultParameterNames = resultParameterNames.ToArray(),
					CorrelationId = correlationId
				};
				RunAgenticProcessResponse response = ClassFactory
					.Get<IAgenticProcessRunner>(new ConstructorArgument("userConnection", _userConnection))
					.RunProcess(request);
				return new RuntimeToolExecutionResult {
					IsSuccess = response.Success && response.Status != "failed" && response.Status != "canceled",
					ErrorMessage = response.ErrorMessage ?? response.ErrorInfo?.Message,
					ProcessId = response.ProcessId,
					Status = response.Status,
					// Fall back to the minted id when the bridge does not echo one, so the
					// suspended envelope always carries a resolvable correlation id.
					CorrelationId = response.CorrelationId.IsNotNullOrWhiteSpace()
						? response.CorrelationId
						: correlationId,
					ExpiresAt = response.ExpiresAt,
					ResultParameters = response.ResultParameterValues != null
						? ProcessParameterValuesCollection.Create(response.ResultParameterValues)
						: null
				};
			} catch (Exception exception) {
				return new RuntimeToolExecutionResult {
					ErrorMessage = exception.Message,
					IsSuccess = false
				};
			}
		}

		#endregion

		#region Class: ProcessSchemaParameterToolParameter

		// Adapts a business-process ProcessSchemaParameter to the shared IToolParameter.
		// Direction split matches core (ProcessSchemaParameterDirectionUtils): In/Variable
		// are inputs; Out/Variable/IsResult are outputs.
		private sealed class ProcessSchemaParameterToolParameter : IToolParameter
		{

			private readonly ProcessSchemaParameter _parameter;

			public ProcessSchemaParameterToolParameter(ProcessSchemaParameter parameter) {
				_parameter = parameter;
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
				_parameter.Direction == ProcessSchemaParameterDirection.In ||
				_parameter.Direction == ProcessSchemaParameterDirection.Variable;

			public bool IsOutput =>
				_parameter.IsResult ||
				_parameter.Direction == ProcessSchemaParameterDirection.Out ||
				_parameter.Direction == ProcessSchemaParameterDirection.Variable;

			public Terrasoft.Core.DataValueType DataValueType => _parameter.DataValueType;

			public IReadOnlyList<IToolParameter> ItemProperties {
				get {
					ProcessSchemaNestedParameterCollection itemProperties = _parameter.ItemProperties;
					if (itemProperties == null) {
						return Array.Empty<IToolParameter>();
					}
					var list = new List<IToolParameter>();
					foreach (ProcessSchemaParameter nested in itemProperties) {
						list.Add(new ProcessSchemaParameterToolParameter(nested));
					}
					return list;
				}
			}

		}

		#endregion

	}

	#endregion

}

