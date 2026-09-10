using System;
using System.Linq;
using Common.Logging;
using Terrasoft.Core;
using Terrasoft.Core.Factories;

namespace CrtAIAgenticBridgeApp.Runtime
{
	[DefaultBinding(typeof(IEventDispatcher))]
	internal class EventDispatcher : IEventDispatcher
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private readonly IEntityChangeEventTypeProvider _provider;
		private readonly IEventSender _sender;
		private readonly IEntityChangeFilterMatcher _filterMatcher;

		public EventDispatcher(UserConnection userConnection) {
			var arg = new ConstructorArgument("userConnection", userConnection);
			_provider = ClassFactory.Get<IEntityChangeEventTypeProvider>(arg);
			_sender = ClassFactory.Get<IEventSender>(arg);
			_filterMatcher = ClassFactory.Get<IEntityChangeFilterMatcher>(arg);
		}

		public void Dispatch(EntityChangeDispatchArgs args) {
			EventTypeEntry[] eventTypeEntries = _provider.GetEventTypeEntries(
				args.EntitySchemaUId, args.ChangeType);
			if (eventTypeEntries == null || eventTypeEntries.Length == 0) {
				return;
			}
			bool canCheckFilters = args.ChangeType != Constants.ChangeTypes.Deleted;
			foreach (EventTypeEntry entry in eventTypeEntries) {
				if (canCheckFilters && !string.IsNullOrEmpty(entry.EntitySchemaFilters)) {
					if (!_filterMatcher.IsMatch(args.EntitySchemaUId, args.EntityId,
							entry.EntitySchemaFilters)) {
						continue;
					}
				}
				if (!MatchesChangedColumns(args, entry)) {
					continue;
				}
				var data = new EntityChangeData {
					EntityName = args.EntitySchemaName,
					RecordId = args.EntityId.ToString(),
					ChangeType = args.ChangeType,
					ChangedFields = BuildPayloadChangedFields(args, entry)
				};
				var request = new EventOccurrenceRequest<EntityChangeData> {
					TriggerCode = Constants.TriggerCodes.CreatioEntityChange,
					EventTypeCode = entry.Code,
					OccurredAt = args.OccurredAt,
					Data = data
				};
				try {
					_sender.Send(request);
				} catch (Exception ex) {
					Log.Error($"Failed to send event occurrence for event type " +
						$"'{entry.Code}' entity '{args.EntitySchemaName}'", ex);
				}
			}
		}

		// Payload exposes only the field names the trigger is watching, not values.
		// Wildcard trigger (no watched columns) → no field info in the payload.
		// Non-Update events → no field info in the payload.
		private static string[] BuildPayloadChangedFields(EntityChangeDispatchArgs args,
				EventTypeEntry entry) {
			if (args.ChangeType != Constants.ChangeTypes.Updated) {
				return null;
			}
			if (entry.WatchedColumnIds == null || entry.WatchedColumnIds.Count == 0) {
				return null;
			}
			if (args.ChangedFields == null || args.ChangedFields.Length == 0) {
				return null;
			}
			string[] names = args.ChangedFields
				.Where(field => field != null
					&& entry.WatchedColumnIds.Contains(field.ColumnUId))
				.Select(field => field.Name)
				.ToArray();
			return names.Length > 0 ? names : null;
		}

		private static bool MatchesChangedColumns(EntityChangeDispatchArgs args,
				EventTypeEntry entry) {
			if (args.ChangeType != Constants.ChangeTypes.Updated
					|| entry.WatchedColumnIds == null) {
				return true;
			}
			if (args.ChangedFields == null || args.ChangedFields.Length == 0) {
				return false;
			}
			return args.ChangedFields.Any(field =>
				field != null && entry.WatchedColumnIds.Contains(field.ColumnUId));
		}
	}
}

