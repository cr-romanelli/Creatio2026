namespace CrtAIAgenticBridgeApp
{
	using System;

	public static class Constants
	{
		public static class TriggerCodes
		{
			public const string CreatioEntityChange = "creatio_entity_change";
			public const string ProcessCompletion = "creatio_process_completion";
		}

		public static class TriggerModes
		{
			public const string Start = "Start";
			public const string Resume = "Resume";

			/// <summary>
			/// Single source of truth for whether events of a trigger code start or
			/// resume a workflow on the control plane. Resume-mode events resume a
			/// suspended workflow (correlated by id); the control-plane bulk ingress
			/// rejects them with <c>RESUME_NOT_SUPPORTED_IN_BULK</c>, so they must be
			/// delivered through the single-event endpoint rather than batched.
			/// </summary>
			public static string ForTriggerCode(string triggerCode) {
				return triggerCode == TriggerCodes.ProcessCompletion
					? Resume
					: Start;
			}

			/// <summary>
			/// True when events for <paramref name="triggerCode"/> resume a suspended
			/// workflow and therefore must not be batched onto the bulk ingress.
			/// </summary>
			public static bool IsResume(string triggerCode) {
				return ForTriggerCode(triggerCode) == Resume;
			}
		}

		public static class SourceSystems
		{
			public const string Creatio = "creatio";
		}

		public static class EntityNames
		{
			public const string AIPlatformEntityEvents = "AIPlatformEntityEvents";
		}

		public static class ColumnNames
		{
			public const string Code = "Code";
			public const string Name = "Name";
			public const string Description = "Description";
			public const string AIPlatformTriggerCode = "AIPlatformTriggerCode";
			public const string EntitySchema = "EntitySchema";
			public const string EntitySchemaFilters = "EntitySchemaFilters";
			public const string EntitySchemaChangeType = "EntitySchemaChangeType";
		}

		public static class SysSettingCodes
		{
			public const string AIStudioServiceUrl = "AIStudioServiceUrl";
			public const string AIStudioEventApiKey = "AIStudioEventApiKey";

			public const string AIStudioOAuthScope = "AIStudioOAuthScope";
			public const string AgenticProcessContinuationExpirationHours =
				"AgenticProcessContinuationExpirationHours";

			// APD-1464 S6/R7: how often the bridge flushes batched trigger
			// events to the bulk ingress endpoint. Integer seconds; default 5.
			public const string AIStudioBulkSendPeriod =
				"AIStudioBulkSendPeriod";
		}

		public static class OAuth
		{
			public const string ServiceCode = "CreatioAIStudio";
		}

		public static class Defaults
		{
			// APD-1464 R7: default bulkSendPeriod when the SystemSetting is
			// absent or non-positive.
			public const int BulkSendPeriodSeconds = 5;
		}

		public static class ApiRoutes
		{
			public const string TriggerCatalog = "/api/v1/trigger";
			public const string TriggerCatalogSync = "/api/v1/trigger/sync";

			public static string TriggerCatalogItem(string triggerCode) {
				if (string.IsNullOrWhiteSpace(triggerCode)) {
					throw new ArgumentException("Trigger code must be provided.", nameof(triggerCode));
				}
				return $"{TriggerCatalog}/{Uri.EscapeDataString(triggerCode)}";
			}

			public static string TriggerEvents(string triggerCode) {
				return $"{TriggerCatalogItem(triggerCode)}/events";
			}

			// APD-1464 R6/S3: additive bulk ingress route. The bridge POSTs a
			// batch of events here; TriggerEvents is retained only to compose this
			// bulk URL — the live outbound path now batches through the bulk route,
			// so there is no longer a per-event send path.
			public static string TriggerEventsBulk(string triggerCode) {
				return $"{TriggerEvents(triggerCode)}/bulk";
			}
		}

		public static class Http
		{
			public const int RequestTimeoutMs = 15000;
		}

		public static class ChangeTypes
		{
			public const string Inserted = "Inserted";
			public const string Updated = "Updated";
			public const string Deleted = "Deleted";
		}

		public static class ChangeTypeIds
		{
			public static readonly Guid Inserted =
				new Guid("a852c33f-0bdd-e011-92c3-00155d04c01d");
			public static readonly Guid Updated =
				new Guid("a952c33f-0bdd-e011-92c3-00155d04c01d");
			public static readonly Guid Deleted =
				new Guid("aa52c33f-0bdd-e011-92c3-00155d04c01d");
		}

		public static class RunProcessStatuses
		{
			public const string Done = "done";
			public const string Suspended = "suspended";
			public const string Failed = "failed";
			public const string Canceled = "canceled";
		}

		public static class EventTypeCodes
		{
			public const string ProcessCompleted = "process_completed";
		}

		public static class TriggerFieldDataTypes
		{
			public const string String = "string";
			public const string Enum = "enum";
			public const string Array = "array";
			public const string Object = "object";
			public const string Map = "map";
		}

		public static class OperationCodes
		{
			public const string CanManageSolution = "CanManageSolution";
		}

		public static Guid MapChangeTypeId(string changeType) {
			switch (changeType) {
				case ChangeTypes.Inserted:
					return ChangeTypeIds.Inserted;
				case ChangeTypes.Updated:
					return ChangeTypeIds.Updated;
				case ChangeTypes.Deleted:
					return ChangeTypeIds.Deleted;
				default:
					return Guid.Empty;
			}
		}
	}
}

