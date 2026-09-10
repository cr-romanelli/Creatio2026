namespace CrtAIAgenticBridgeApp.TriggerDiscovery
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto;
	using CrtAIAgenticBridgeApp.Runtime;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	[DefaultBinding(typeof(ITriggerMetadataProvider))]
	public class TriggerMetadataProvider : ITriggerMetadataProvider
	{
		internal sealed class TriggerCatalogEventTypeRecord
		{
			public string TriggerCode { get; set; }
			public string Code { get; set; }
			public string DisplayName { get; set; }
			public string Description { get; set; }
			public string EntitySchemaFilters { get; set; }
		}

		private readonly UserConnection _userConnection;
		private readonly Func<IEnumerable<string>, IEnumerable<TriggerCatalogEventTypeRecord>> _queryEventTypeRecords;
		private readonly Func<string, EntitySchema> _findEntitySchemaByName;

		private static readonly Dictionary<string, TriggerCatalogPushRequest> HardcodedTriggers =
			new Dictionary<string, TriggerCatalogPushRequest> {
				[Constants.TriggerCodes.CreatioEntityChange] = new TriggerCatalogPushRequest {
					Code = Constants.TriggerCodes.CreatioEntityChange,
					DisplayName = "Creatio Entity Change",
					Description = "Fires when an entity record is created, updated, or deleted",
					SourceSystem = Constants.SourceSystems.Creatio,
					Fields = new[] {
						new TriggerFieldDto {
							Code = "entityName",
							DisplayName = "Entity Name",
							Description = "Schema name of the changed entity",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = true
						},
						new TriggerFieldDto {
							Code = "recordId",
							DisplayName = "Record Id",
							Description = "Unique identifier of the affected record",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = true
						},
						new TriggerFieldDto {
							Code = "changeType",
							DisplayName = "Change Type",
							Description = "Type of mutation",
							DataType = Constants.TriggerFieldDataTypes.Enum,
							IsRequired = true,
							EnumValues = new[] {
								Constants.ChangeTypes.Inserted,
								Constants.ChangeTypes.Updated,
								Constants.ChangeTypes.Deleted
							}
						},
						new TriggerFieldDto {
							Code = "changedFields",
							DisplayName = "Changed Fields",
							Description = "Names of watched fields that were modified " +
								"(only fields the event type subscribes to are reported)",
							DataType = Constants.TriggerFieldDataTypes.Array,
							IsRequired = false,
							Items = new TriggerFieldDto {
								Code = "changedField",
								DisplayName = "Changed Field",
								Description = "Schema name of a watched field that changed",
								DataType = Constants.TriggerFieldDataTypes.String
							}
						}
					}
				},
				[Constants.TriggerCodes.ProcessCompletion] = new TriggerCatalogPushRequest {
					Code = Constants.TriggerCodes.ProcessCompletion,
					DisplayName = "Creatio Process Completion",
					Description = "Fires when a Creatio business process completes, is canceled, or fails",
					SourceSystem = Constants.SourceSystems.Creatio,
					Fields = new[] {
						new TriggerFieldDto {
							Code = "correlationId",
							DisplayName = "Correlation Id",
							Description = "Client-supplied identifier echoed from the Run Process request to match the resume callback",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = true
						},
						new TriggerFieldDto {
							Code = "processId",
							DisplayName = "Process Id",
							Description = "Unique identifier of the executed process instance",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = true
						},
						new TriggerFieldDto {
							Code = "schemaName",
							DisplayName = "Schema Name",
							Description = "Name of the process schema",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = false
						},
						new TriggerFieldDto {
							Code = "status",
							DisplayName = "Status",
							Description = "Terminal process status",
							DataType = Constants.TriggerFieldDataTypes.Enum,
							IsRequired = true,
							EnumValues = new[] {
								Constants.RunProcessStatuses.Done,
								Constants.RunProcessStatuses.Suspended,
								Constants.RunProcessStatuses.Canceled,
								Constants.RunProcessStatuses.Failed
							}
						},
						new TriggerFieldDto {
							Code = "errorMessage",
							DisplayName = "Error Message",
							Description = "Error text when status is Failed",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = false
						},
						new TriggerFieldDto {
							Code = "failedElement",
							DisplayName = "Failed Element",
							Description = "Name of the failing process element when status is Failed",
							DataType = Constants.TriggerFieldDataTypes.String,
							IsRequired = false
						},
						new TriggerFieldDto {
							Code = "resultParameters",
							DisplayName = "Result Parameters",
							Description = "Map of requested process result parameter values",
							DataType = Constants.TriggerFieldDataTypes.Map,
							IsRequired = false
						}
					},
					EventTypes = new[] {
						new TriggerCatalogPushEventTypeRequest {
							Code = Constants.EventTypeCodes.ProcessCompleted,
							DisplayName = "Process completed",
							Description = "Fires when a Creatio process completes, is canceled, or fails",
							Mode = Constants.TriggerModes.Resume
						}
					}
				}
			};

		public TriggerMetadataProvider(UserConnection userConnection)
			: this(userConnection, null) {
		}

		internal TriggerMetadataProvider(UserConnection userConnection,
				Func<IEnumerable<string>, IEnumerable<TriggerCatalogEventTypeRecord>> queryEventTypeRecords)
			: this(userConnection, queryEventTypeRecords, null) {
		}

		internal TriggerMetadataProvider(UserConnection userConnection,
				Func<IEnumerable<string>, IEnumerable<TriggerCatalogEventTypeRecord>> queryEventTypeRecords,
				Func<string, EntitySchema> findEntitySchemaByName) {
			_userConnection = userConnection;
			_queryEventTypeRecords = queryEventTypeRecords ?? QueryEventTypeRecords;
			_findEntitySchemaByName = findEntitySchemaByName ??
				(name => _userConnection.EntitySchemaManager.FindInstanceByName(name));
		}

		public TriggerCatalogPushRequest[] GetAllTriggers() {
			IEnumerable<string> dbBackedCodes = HardcodedTriggers
				.Where(e => e.Value.EventTypes == null || e.Value.EventTypes.Length == 0)
				.Select(e => e.Key);
			Dictionary<string, TriggerCatalogPushEventTypeRequest[]> eventTypesByTriggerCode =
				GetEventTypesByTriggerCode(dbBackedCodes);
			var items = new List<TriggerCatalogPushRequest>();
			foreach (KeyValuePair<string, TriggerCatalogPushRequest> entry in HardcodedTriggers) {
				items.Add(CreateTrigger(entry.Value, entry.Key, eventTypesByTriggerCode));
			}
			return items.ToArray();
		}

		public TriggerCatalogPushRequest GetTrigger(string triggerCode) {
			if (!HardcodedTriggers.TryGetValue(triggerCode, out TriggerCatalogPushRequest trigger)) {
				return null;
			}
			IEnumerable<string> dbBackedCodes = trigger.EventTypes == null || trigger.EventTypes.Length == 0
				? new[] { triggerCode }
				: new string[0];
			return CreateTrigger(trigger, triggerCode, GetEventTypesByTriggerCode(dbBackedCodes));
		}

		public GetEventTypesResponse GetEventTypes(string triggerCode) {
			if (!HardcodedTriggers.ContainsKey(triggerCode)) {
				return null;
			}
			var records = _queryEventTypeRecords(new[] { triggerCode });
			var items = records
				.Where(r => r.TriggerCode == triggerCode)
				.Select(r => new EventTypeDto {
					Code = r.Code,
					DisplayName = r.DisplayName,
					Description = r.Description,
					EntitySchemaFilters = r.EntitySchemaFilters
				})
				.ToArray();
			return new GetEventTypesResponse {
				TriggerCode = triggerCode,
				Items = items
			};
		}

		public GetEntitySchemaColumnsResponse GetEntitySchemaColumns(string schemaName) {
			EntitySchema schema = FindEntitySchemaByName(schemaName);
			if (schema == null) {
				return null;
			}
			var items = new List<EntitySchemaColumnDto>();
			foreach (EntitySchemaColumn column in schema.Columns) {
				string caption = column.Caption?.Value;
				if (string.IsNullOrWhiteSpace(caption)) {
					continue;
				}
				items.Add(new EntitySchemaColumnDto {
					UId = column.UId,
					Caption = caption
				});
			}
			return new GetEntitySchemaColumnsResponse {
				Items = items
					.OrderBy(column => column.Caption)
					.ToArray()
			};
		}

		private TriggerCatalogPushRequest CreateTrigger(TriggerCatalogPushRequest trigger,
				string triggerCode,
				IDictionary<string, TriggerCatalogPushEventTypeRequest[]> eventTypesByTriggerCode) {
			TriggerCatalogPushEventTypeRequest[] eventTypes;
			if (trigger.EventTypes != null && trigger.EventTypes.Length > 0) {
				eventTypes = trigger.EventTypes;
			} else {
				eventTypesByTriggerCode.TryGetValue(triggerCode, out eventTypes);
			}
			return new TriggerCatalogPushRequest {
				Code = trigger.Code,
				DisplayName = trigger.DisplayName,
				Description = trigger.Description,
				SourceSystem = trigger.SourceSystem,
				Fields = trigger.Fields ?? new TriggerFieldDto[0],
				EventTypes = eventTypes ?? new TriggerCatalogPushEventTypeRequest[0]
			};
		}

		private Dictionary<string, TriggerCatalogPushEventTypeRequest[]> GetEventTypesByTriggerCode(
				IEnumerable<string> triggerCodes) {
			string[] codes = triggerCodes?.Distinct().ToArray() ?? new string[0];
			var itemsByTriggerCode =
				new Dictionary<string, List<TriggerCatalogPushEventTypeRequest>>();
			foreach (string code in codes) {
				itemsByTriggerCode[code] = new List<TriggerCatalogPushEventTypeRequest>();
			}
			if (codes.Length == 0) {
				return itemsByTriggerCode.ToDictionary(
					entry => entry.Key,
					entry => entry.Value.ToArray());
			}
			foreach (TriggerCatalogEventTypeRecord record in _queryEventTypeRecords(codes)) {
				string triggerCode = record.TriggerCode;
				if (!itemsByTriggerCode.TryGetValue(triggerCode, out List<TriggerCatalogPushEventTypeRequest> items)) {
					continue;
				}
				items.Add(new TriggerCatalogPushEventTypeRequest {
					Code = record.Code,
					DisplayName = record.DisplayName,
					Description = record.Description,
					Mode = ResolveMode(triggerCode)
				});
			}
			return itemsByTriggerCode.ToDictionary(
				entry => entry.Key,
				entry => entry.Value.ToArray());
		}

		private IEnumerable<TriggerCatalogEventTypeRecord> QueryEventTypeRecords(
				IEnumerable<string> triggerCodes) {
			string[] codes = triggerCodes?.Distinct().ToArray() ?? new string[0];
			if (codes.Length == 0 || !ConfigurationEntitySchemaExists()) {
				return new TriggerCatalogEventTypeRecord[0];
			}
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager,
				Constants.EntityNames.AIPlatformEntityEvents);
			esq.AddColumn(Constants.ColumnNames.AIPlatformTriggerCode);
			esq.AddColumn(Constants.ColumnNames.Code);
			esq.AddColumn(Constants.ColumnNames.Name);
			esq.AddColumn(Constants.ColumnNames.Description);
			esq.AddColumn(Constants.ColumnNames.EntitySchemaFilters);
			esq.Filters.Add(esq.CreateFilterWithParameters(
				FilterComparisonType.Equal,
				Constants.ColumnNames.AIPlatformTriggerCode,
				codes));
			EntityCollection entities = esq.GetEntityCollection(_userConnection);
			var records = new List<TriggerCatalogEventTypeRecord>();
			foreach (Entity entity in entities) {
				records.Add(new TriggerCatalogEventTypeRecord {
					TriggerCode = entity.GetTypedColumnValue<string>(
						Constants.ColumnNames.AIPlatformTriggerCode),
					Code = entity.GetTypedColumnValue<string>(Constants.ColumnNames.Code),
					DisplayName = entity.GetTypedColumnValue<string>(Constants.ColumnNames.Name),
					Description = entity.GetTypedColumnValue<string>(Constants.ColumnNames.Description),
					EntitySchemaFilters = entity.GetTypedColumnValue<string>(
						Constants.ColumnNames.EntitySchemaFilters),
				});
			}
			return records;
		}

		// Classifies whether a source-system event starts or resumes a workflow on
		// the AI control plane. The bridge declares this at the registration boundary
		// so the control plane no longer needs the field on event emission requests.
		// The same classification routes delivery (resume events bypass the bulk
		// ingress) — see Constants.TriggerModes for the single source of truth.
		private static string ResolveMode(string triggerCode) {
			return Constants.TriggerModes.ForTriggerCode(triggerCode);
		}

		private bool ConfigurationEntitySchemaExists() {
			return FindEntitySchemaByName(Constants.EntityNames.AIPlatformEntityEvents) != null;
		}

		private EntitySchema FindEntitySchemaByName(string schemaName) {
			try {
				return _findEntitySchemaByName(schemaName);
			} catch {
				return null;
			}
		}

	}
}

