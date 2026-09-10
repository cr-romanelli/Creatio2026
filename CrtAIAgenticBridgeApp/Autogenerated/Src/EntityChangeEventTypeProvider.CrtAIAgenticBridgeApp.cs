namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Logging;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Class: EntityChangeEventTypeProvider

	[DefaultBinding(typeof(IEntityChangeEventTypeProvider))]
	internal class EntityChangeEventTypeProvider : IEntityChangeEventTypeProvider
	{

		#region Class: CacheEntry

		private sealed class CacheEntry
		{

			#region Fields: Public

			public EventTypeEntry[] Entries;
			public DateTime ExpiresAtUtc;

			#endregion

		}

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");
		private static readonly ConcurrentDictionary<Guid, CacheEntry> _cache =
			new ConcurrentDictionary<Guid, CacheEntry>();
		private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		public EntityChangeEventTypeProvider(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Public

		public void InvalidateCache() => _cache.Clear();

		public EventTypeEntry[] GetEventTypeEntries(Guid entitySchemaUId, string changeType) {
			Guid changeTypeId = Constants.MapChangeTypeId(changeType);
			CacheEntry current = _cache.AddOrUpdate(
				entitySchemaUId,
				_ => CreateCacheEntry(entitySchemaUId),
				(_, existing) => existing.ExpiresAtUtc > DateTime.UtcNow
					? existing
					: CreateCacheEntry(entitySchemaUId));
			return Array.FindAll(current.Entries,
				entry => entry.ChangeTypeId == Guid.Empty || entry.ChangeTypeId == changeTypeId);
		}

		#endregion

		#region Methods: Private

		private CacheEntry CreateCacheEntry(Guid entitySchemaUId) {
			return new CacheEntry {
				Entries = QueryEventTypeEntries(entitySchemaUId),
				ExpiresAtUtc = DateTime.UtcNow.Add(CacheTtl)
			};
		}

		private EventTypeEntry[] QueryEventTypeEntries(Guid entitySchemaUId) {
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager,
				Constants.EntityNames.AIPlatformEntityEvents);
			esq.AddColumn(Constants.ColumnNames.Code);
			esq.AddColumn(Constants.ColumnNames.EntitySchemaFilters);
			esq.AddColumn(Constants.ColumnNames.EntitySchemaChangeType);
			esq.AddColumn("ChangedColumns");
			esq.Filters.Add(esq.CreateFilterWithParameters(
				FilterComparisonType.Equal,
				Constants.ColumnNames.AIPlatformTriggerCode,
				Constants.TriggerCodes.CreatioEntityChange));
			esq.Filters.Add(esq.CreateFilterWithParameters(
				FilterComparisonType.Equal,
				Constants.ColumnNames.EntitySchema,
				entitySchemaUId.ToString()));
			EntityCollection entities = esq.GetEntityCollection(_userConnection);
			var entries = new List<EventTypeEntry>();
			foreach (Entity entity in entities) {
				string code = entity.GetTypedColumnValue<string>(Constants.ColumnNames.Code);
				if (!string.IsNullOrEmpty(code)) {
					entries.Add(new EventTypeEntry {
						Code = code,
						EntitySchemaFilters = entity.GetTypedColumnValue<string>(
							Constants.ColumnNames.EntitySchemaFilters),
						ChangeTypeId = entity.GetTypedColumnValue<Guid>(
							Constants.ColumnNames.EntitySchemaChangeType + "Id"),
						WatchedColumnIds = ParseChangedColumns(
							entity.GetTypedColumnValue<string>("ChangedColumns"))
					});
				}
			}
			return entries.ToArray();
		}

		internal static HashSet<Guid> ParseChangedColumns(string changedColumnsJson) {
			if (string.IsNullOrWhiteSpace(changedColumnsJson)) {
				return null;
			}
			try {
				Guid[] parsed = JsonConvert.DeserializeObject<Guid[]>(changedColumnsJson);
				if (parsed == null) {
					return null;
				}
				Guid[] filtered = parsed.Where(uid => uid != Guid.Empty).Distinct().ToArray();
				return filtered.Length > 0 ? new HashSet<Guid>(filtered) : null;
			} catch (Exception ex) {
				Log.Warn($"Failed to parse ChangedColumns JSON '{changedColumnsJson}'. " +
					"The trigger will fire on any field update (wildcard fallback).", ex);
				return null;
			}
		}

		#endregion

	}

	#endregion

}

