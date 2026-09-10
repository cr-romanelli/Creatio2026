namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSrcOutboxRepository

	/// <summary>
	/// Defines outbox persistence operations for active section synchronization.
	/// </summary>
	internal interface IKnwSrcOutboxRepository
	{

		#region Methods: Internal

		void Upsert(Guid knwSourceId, Guid recordId, Guid entitySchemaUId, KnwSrcOutboxEventType eventType,
			ISet<string> changedColumns, DateTime eventOnUtc);

		IList<KnwSrcOutboxRecord> ClaimPendingBatch(int batchSize, TimeSpan leaseDuration, string processingToken,
			DateTime utcNow);

		void MarkProcessed(ICollection<Guid> outboxIds, string processingToken, DateTime processedOnUtc);

		void MarkFailed(ICollection<Guid> outboxIds, string processingToken, string error, DateTime nextAttemptOnUtc);

		void MarkDeadLettered(ICollection<Guid> outboxIds, string processingToken, string error,
			DateTime deadLetteredOnUtc);

		void ReleaseLease(ICollection<Guid> outboxIds, string processingToken);

		int CleanupProcessed(DateTime olderThanUtc, int batchSize);

		#endregion

	}

	#endregion

	#region Class: KnwSrcOutboxRepository

	/// <summary>
	/// Database-backed implementation of synchronization outbox persistence.
	/// </summary>
	[DefaultBinding(typeof(IKnwSrcOutboxRepository))]
	internal class KnwSrcOutboxRepository : IKnwSrcOutboxRepository
	{

		#region Constructors: Public

		public KnwSrcOutboxRepository(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Fields: Private

		private readonly UserConnection _userConnection;

		#endregion

		#region Methods: Private

		private EntitySchema GetOutboxSchema() {
			return _userConnection.EntitySchemaManager.GetInstanceByName(KnwSrcEventsOutboxSchema.Name);
		}

		private static DateTime EnsureUtc(DateTime dateTime) {
			if (dateTime.Kind == DateTimeKind.Utc) {
				return dateTime;
			}
			if (dateTime.Kind == DateTimeKind.Local) {
				return dateTime.ToUniversalTime();
			}
			return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
		}

		private DateTime ToUtc(DateTime dateTime) {
			return EnsureUtc(TimeZoneUtilities.ConvertToUtc(_userConnection, dateTime));
		}

		private DateTime? GetNullableDateTime(Entity entity, string columnName) {
			object value = entity.GetColumnValue(columnName);
			if (value == null || value == DBNull.Value) {
				return null;
			}
			return ToUtc((DateTime)value);
		}

		private static ISet<string> DeserializeChangedColumns(string changedColumnsJson) {
			if (changedColumnsJson.IsNullOrWhiteSpace()) {
				return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			try {
				IList<string> values = JsonConvert.DeserializeObject<IList<string>>(changedColumnsJson);
				return values == null
					? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
					: new HashSet<string>(values.Where(x => !x.IsNullOrWhiteSpace()),
						StringComparer.OrdinalIgnoreCase);
			} catch {
				string[] values = changedColumnsJson
					.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				return new HashSet<string>(values.Select(x => x.Trim()).Where(x => !x.IsNullOrWhiteSpace()),
					StringComparer.OrdinalIgnoreCase);
			}
		}

		private static string SerializeChangedColumns(ISet<string> changedColumns) {
			if (changedColumns == null || changedColumns.Count == 0) {
				return "[]";
			}
			List<string> normalized = changedColumns
				.Where(x => !x.IsNullOrWhiteSpace())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList();
			return JsonConvert.SerializeObject(normalized);
		}

		private KnwSrcOutboxRecord ReadRecord(Entity entity) {
			return new KnwSrcOutboxRecord(
				entity.PrimaryColumnValue,
				entity.GetTypedColumnValue<Guid>("KnwSourceId"),
				entity.GetTypedColumnValue<Guid>("RecordId"),
				entity.GetTypedColumnValue<Guid>("EntitySchemaUId"),
				(KnwSrcOutboxEventType)entity.GetTypedColumnValue<int>("EventType"),
				DeserializeChangedColumns(entity.GetTypedColumnValue<string>("ChangedColumns")),
				ToUtc(entity.GetTypedColumnValue<DateTime>("EventOn")),
				entity.GetTypedColumnValue<bool>("IsProcessed"),
				GetNullableDateTime(entity, "ProcessedOn"),
				entity.GetTypedColumnValue<int>("Attempts"),
				entity.GetTypedColumnValue<string>("LastError"),
				GetNullableDateTime(entity, "NextAttemptOn"),
				GetNullableDateTime(entity, "LeaseUntil"),
				entity.GetTypedColumnValue<string>("ProcessingToken"),
				GetNullableDateTime(entity, "DeadLetteredOn"));
		}

		private void ApplyRecord(Entity entity, KnwSrcOutboxRecord record) {
			entity.SetColumnValue("KnwSourceId", record.KnwSourceId);
			entity.SetColumnValue("RecordId", record.RecordId);
			entity.SetColumnValue("EntitySchemaUId", record.EntitySchemaUId);
			entity.SetColumnValue("EventType", (int)record.EventType);
			entity.SetColumnValue("ChangedColumns", SerializeChangedColumns(record.ChangedColumns));
			entity.SetColumnValue("EventOn", record.EventOn);
			entity.SetColumnValue("IsProcessed", record.IsProcessed);
			entity.SetColumnValue("ProcessedOn", record.ProcessedOn.HasValue
				? (object)record.ProcessedOn.Value
				: null);
			entity.SetColumnValue("Attempts", record.Attempts);
			entity.SetColumnValue("LastError", record.LastError);
			entity.SetColumnValue("NextAttemptOn", record.NextAttemptOn.HasValue
				? (object)record.NextAttemptOn.Value
				: null);
			entity.SetColumnValue("LeaseUntil", record.LeaseUntil.HasValue
				? (object)record.LeaseUntil.Value
				: null);
			entity.SetColumnValue("ProcessingToken", record.ProcessingToken);
			entity.SetColumnValue("DeadLetteredOn", record.DeadLetteredOn.HasValue
				? (object)record.DeadLetteredOn.Value
				: null);
		}

		private EntitySchemaQuery CreateReadEsq() {
			EntitySchema schema = GetOutboxSchema();
			var esq = new EntitySchemaQuery(schema) {
				IgnoreDisplayValues = true,
				UnmaskColumnValues = true,
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn("KnwSource");
			esq.AddColumn("RecordId");
			esq.AddColumn("EntitySchemaUId");
			esq.AddColumn("EventType");
			esq.AddColumn("ChangedColumns");
			esq.AddColumn("EventOn");
			esq.AddColumn("IsProcessed");
			esq.AddColumn("ProcessedOn");
			esq.AddColumn("Attempts");
			esq.AddColumn("LastError");
			esq.AddColumn("NextAttemptOn");
			esq.AddColumn("LeaseUntil");
			esq.AddColumn("ProcessingToken");
			esq.AddColumn("DeadLetteredOn");
			esq.AddColumn("ModifiedOn");
			return esq;
		}

		private Entity FindBySourceAndRecord(Guid knwSourceId, Guid recordId) {
			EntitySchemaQuery esq = CreateReadEsq();
			esq.RowCount = 1;
			esq.Columns.FindByName("EventOn")?.OrderByDesc();
			esq.Columns.FindByName("ModifiedOn")?.OrderByDesc();
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "KnwSource", knwSourceId));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "RecordId", recordId));
			return esq.GetEntityCollection(_userConnection).FirstOrDefault();
		}

		private bool TryClaim(KnwSrcOutboxRecord candidate, string processingToken, DateTime utcNow,
				DateTime leaseUntilUtc, out KnwSrcOutboxRecord claimedRecord) {
			claimedRecord = null;
			if (candidate == null || candidate.Id == Guid.Empty) {
				return false;
			}
			int affected = new Update(_userConnection, KnwSrcEventsOutboxSchema.Name)
				.Set("ProcessingToken", Column.Parameter(processingToken))
				.Set("LeaseUntil", Column.Parameter(leaseUntilUtc))
				.Where("Id").IsEqual(Column.Parameter(candidate.Id))
				.And("IsProcessed").IsEqual(Column.Parameter(false))
				.And("DeadLetteredOn").IsNull()
				.And().OpenBlock("NextAttemptOn").IsNull()
					.Or("NextAttemptOn").IsLessOrEqual(Column.Parameter(utcNow)).CloseBlock()
				.And().OpenBlock("LeaseUntil").IsNull()
					.Or("LeaseUntil").IsLess(Column.Parameter(utcNow)).CloseBlock()
				.Execute();
			if (affected <= 0) {
				return false;
			}
			KnwSrcOutboxRecord updated = candidate.Clone();
			updated.MarkClaimed(processingToken, leaseUntilUtc);
			claimedRecord = updated;
			return true;
		}

		private static bool IsUniqueConstraintViolation(Exception exception) {
			Exception current = exception;
			while (current != null) {
				string message = current.Message ?? string.Empty;
				if (message.IndexOf("duplicate key", StringComparison.OrdinalIgnoreCase) >= 0
						|| message.IndexOf("unique constraint", StringComparison.OrdinalIgnoreCase) >= 0
						|| message.IndexOf("23505", StringComparison.OrdinalIgnoreCase) >= 0) {
					return true;
				}
				current = current.InnerException;
			}
			return false;
		}

		private static List<Guid> NormalizeIds(ICollection<Guid> outboxIds) {
			return outboxIds == null
				? new List<Guid>()
				: outboxIds.Where(x => x != Guid.Empty).Distinct().ToList();
		}

		private int ExecuteUpdateWithTokenOwnership(ICollection<Guid> outboxIds, string processingToken,
				Action<Update> configureUpdate) {
			List<Guid> ids = NormalizeIds(outboxIds);
			if (ids.Count == 0) {
				return 0;
			}
			var update = new Update(_userConnection, KnwSrcEventsOutboxSchema.Name);
			configureUpdate(update);
			update.Where("Id").In(Column.Parameters(ids))
				.And("ProcessingToken").IsEqual(Column.Parameter(processingToken));
			return update.Execute();
		}

		private int ExecuteUpdateByIds(ICollection<Guid> outboxIds, Action<Update> configureUpdate) {
			List<Guid> ids = NormalizeIds(outboxIds);
			if (ids.Count == 0) {
				return 0;
			}
			var update = new Update(_userConnection, KnwSrcEventsOutboxSchema.Name);
			configureUpdate(update);
			update.Where("Id").In(Column.Parameters(ids));
			return update.Execute();
		}

		private QueryColumnExpression GetTypedNullParameter(string columnName) {
			EntitySchema schema = GetOutboxSchema();
			EntitySchemaColumn schemaColumn = schema?.Columns?.FindByName(columnName);
			DataValueType dataValueType = schemaColumn?.DataValueType;
			return dataValueType == null
				? Column.Parameter(null)
				: Column.Parameter(null, dataValueType);
		}

		// NOTE: Candidate row count is intentionally limited to the requested batch size.
		// Undersampling under contention is acceptable and will be picked up by next processor run.
		private static int GetClaimCandidateRowCount(int batchSize) {
			return batchSize <= 0 ? 0 : batchSize;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public void Upsert(Guid knwSourceId, Guid recordId, Guid entitySchemaUId, KnwSrcOutboxEventType eventType,
				ISet<string> changedColumns, DateTime eventOnUtc) {
			if (knwSourceId == Guid.Empty || recordId == Guid.Empty || entitySchemaUId == Guid.Empty) {
				return;
			}
			eventOnUtc = EnsureUtc(eventOnUtc);
			const int maxAttempts = 3;
			for (int attempt = 0; attempt < maxAttempts; attempt++) {
				Entity existing = FindBySourceAndRecord(knwSourceId, recordId);
				if (existing != null) {
					KnwSrcOutboxRecord merged = ReadRecord(existing);
					merged.MergeIncomingEvent(entitySchemaUId, eventType, changedColumns, eventOnUtc);
					ApplyRecord(existing, merged);
					existing.Save(false);
					return;
				}
				var created = new KnwSrcOutboxRecord(Guid.NewGuid(), knwSourceId, recordId, entitySchemaUId,
					eventType, changedColumns, eventOnUtc);
				Entity insertEntity = GetOutboxSchema().CreateEntity(_userConnection);
				insertEntity.SetColumnValue("Id", created.Id);
				ApplyRecord(insertEntity, created);
				try {
					insertEntity.Save(false);
					return;
				} catch (Exception e) {
					if (!IsUniqueConstraintViolation(e) || attempt == maxAttempts - 1) {
						throw;
					}
				}
			}
			Entity fallback = FindBySourceAndRecord(knwSourceId, recordId);
			if (fallback != null) {
				KnwSrcOutboxRecord merged = ReadRecord(fallback);
				merged.MergeIncomingEvent(entitySchemaUId, eventType, changedColumns, eventOnUtc);
				ApplyRecord(fallback, merged);
				fallback.Save(false);
			}
		}

		/// <inheritdoc />
		public IList<KnwSrcOutboxRecord> ClaimPendingBatch(int batchSize, TimeSpan leaseDuration, string processingToken,
				DateTime utcNow) {
			if (batchSize <= 0 || string.IsNullOrWhiteSpace(processingToken)) {
				return new List<KnwSrcOutboxRecord>();
			}
			DateTime claimNow = EnsureUtc(utcNow);
			EntitySchemaQuery esq = CreateReadEsq();
			esq.RowCount = GetClaimCandidateRowCount(batchSize);
			// Ensure DB applies ordering before RowCount truncation.
			esq.Columns.FindByName("EventOn")?.OrderByAsc();
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "IsProcessed", false));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNull, "DeadLetteredOn"));
			var nextAttemptFilter = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.Or);
			nextAttemptFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNull, "NextAttemptOn"));
			nextAttemptFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.LessOrEqual, "NextAttemptOn",
				claimNow));
			esq.Filters.Add(nextAttemptFilter);
			var leaseFilter = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.Or);
			leaseFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNull, "LeaseUntil"));
			leaseFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.Less, "LeaseUntil", claimNow));
			esq.Filters.Add(leaseFilter);
			List<KnwSrcOutboxRecord> candidates = esq.GetEntityCollection(_userConnection)
				.Select(ReadRecord)
				.Where(x => x.CanBeClaimed(claimNow))
				.OrderBy(x => x.EventOn)
				.ThenBy(x => x.Id)
				.ToList();
			var claimed = new List<KnwSrcOutboxRecord>(batchSize);
			DateTime leaseUntil = claimNow.Add(leaseDuration);
			foreach (KnwSrcOutboxRecord candidate in candidates) {
				if (claimed.Count >= batchSize) {
					break;
				}
				if (TryClaim(candidate, processingToken, claimNow, leaseUntil, out KnwSrcOutboxRecord claimedRecord)) {
					claimed.Add(claimedRecord);
				}
			}
			return claimed;
		}

		/// <inheritdoc />
		public void MarkProcessed(ICollection<Guid> outboxIds, string processingToken, DateTime processedOnUtc) {
			if (outboxIds == null || outboxIds.Count == 0 || string.IsNullOrWhiteSpace(processingToken)) {
				return;
			}
			Action<Update> configureUpdate = update => update
				.Set("IsProcessed", Column.Parameter(true))
				.Set("ProcessedOn", Column.Parameter(processedOnUtc))
				.Set("LastError", GetTypedNullParameter("LastError"))
				.Set("NextAttemptOn", GetTypedNullParameter("NextAttemptOn"))
				.Set("LeaseUntil", GetTypedNullParameter("LeaseUntil"))
				.Set("ProcessingToken", GetTypedNullParameter("ProcessingToken"))
				.Set("DeadLetteredOn", GetTypedNullParameter("DeadLetteredOn"));
			int affected = ExecuteUpdateWithTokenOwnership(outboxIds, processingToken, configureUpdate);
			if (affected > 0) {
				return;
			}
			KnwLibUtils.Logger.Warn($"[{nameof(KnwSrcOutboxRepository)}] MarkProcessed updated 0 rows by token " +
				$"ownership. Falling back to ID-only update for {NormalizeIds(outboxIds).Count} rows.");
			ExecuteUpdateByIds(outboxIds, configureUpdate);
		}

		/// <inheritdoc />
		public void MarkFailed(ICollection<Guid> outboxIds, string processingToken, string error, DateTime nextAttemptOnUtc) {
			if (outboxIds == null || outboxIds.Count == 0 || string.IsNullOrWhiteSpace(processingToken)) {
				return;
			}
			nextAttemptOnUtc = EnsureUtc(nextAttemptOnUtc);
			Action<Update> configureUpdate = update => update
				.Set("Attempts", Column.SourceColumn("Attempts") + Column.Parameter(1))
				.Set("LastError", Column.Parameter(error))
				.Set("NextAttemptOn", Column.Parameter(nextAttemptOnUtc))
				.Set("LeaseUntil", GetTypedNullParameter("LeaseUntil"))
				.Set("ProcessingToken", GetTypedNullParameter("ProcessingToken"))
				.Set("IsProcessed", Column.Parameter(false))
				.Set("ProcessedOn", GetTypedNullParameter("ProcessedOn"))
				.Set("DeadLetteredOn", GetTypedNullParameter("DeadLetteredOn"));
			int affected = ExecuteUpdateWithTokenOwnership(outboxIds, processingToken, configureUpdate);
			if (affected > 0) {
				return;
			}
			KnwLibUtils.Logger.Warn($"[{nameof(KnwSrcOutboxRepository)}] MarkFailed updated 0 rows by token " +
				$"ownership. Falling back to ID-only update for {NormalizeIds(outboxIds).Count} rows.");
			ExecuteUpdateByIds(outboxIds, configureUpdate);
		}

		/// <inheritdoc />
		public void MarkDeadLettered(ICollection<Guid> outboxIds, string processingToken, string error,
				DateTime deadLetteredOnUtc) {
			if (outboxIds == null || outboxIds.Count == 0 || string.IsNullOrWhiteSpace(processingToken)) {
				return;
			}
			Action<Update> configureUpdate = update => update
				.Set("Attempts", Column.SourceColumn("Attempts") + Column.Parameter(1))
				.Set("LastError", Column.Parameter(error))
				.Set("NextAttemptOn", GetTypedNullParameter("NextAttemptOn"))
				.Set("LeaseUntil", GetTypedNullParameter("LeaseUntil"))
				.Set("ProcessingToken", GetTypedNullParameter("ProcessingToken"))
				.Set("IsProcessed", Column.Parameter(false))
				.Set("ProcessedOn", GetTypedNullParameter("ProcessedOn"))
				.Set("DeadLetteredOn", Column.Parameter(deadLetteredOnUtc));
			int affected = ExecuteUpdateWithTokenOwnership(outboxIds, processingToken, configureUpdate);
			if (affected > 0) {
				return;
			}
			KnwLibUtils.Logger.Warn($"[{nameof(KnwSrcOutboxRepository)}] MarkDeadLettered updated 0 rows by token " +
				$"ownership. Falling back to ID-only update for {NormalizeIds(outboxIds).Count} rows.");
			ExecuteUpdateByIds(outboxIds, configureUpdate);
		}

		/// <inheritdoc />
		public void ReleaseLease(ICollection<Guid> outboxIds, string processingToken) {
			if (outboxIds == null || outboxIds.Count == 0 || string.IsNullOrWhiteSpace(processingToken)) {
				return;
			}
			Action<Update> configureUpdate = update => update
				.Set("LeaseUntil", GetTypedNullParameter("LeaseUntil"))
				.Set("ProcessingToken", GetTypedNullParameter("ProcessingToken"));
			int affected = ExecuteUpdateWithTokenOwnership(outboxIds, processingToken, configureUpdate);
			if (affected > 0) {
				return;
			}
			KnwLibUtils.Logger.Warn($"[{nameof(KnwSrcOutboxRepository)}] ReleaseLease updated 0 rows by token " +
				$"ownership. Falling back to ID-only update for {NormalizeIds(outboxIds).Count} rows.");
			ExecuteUpdateByIds(outboxIds, configureUpdate);
		}

		/// <inheritdoc />
		public int CleanupProcessed(DateTime olderThanUtc, int batchSize) {
			if (batchSize <= 0) {
				return 0;
			}
			olderThanUtc = EnsureUtc(olderThanUtc);
			const int maxInClauseParameterCount = 2000;
			int totalRemoved = 0;
			while (true) {
				EntitySchemaQuery esq = CreateReadEsq();
				esq.RowCount = batchSize;
				var processedFilter = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.And);
				processedFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "IsProcessed", true));
				processedFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.Less, "ProcessedOn", olderThanUtc));
				var cleanupFilter = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.Or);
				cleanupFilter.Add(processedFilter);
				var deadLetteredFilter = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.And);
				deadLetteredFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNotNull, "DeadLetteredOn"));
				deadLetteredFilter.Add(esq.CreateFilterWithParameters(FilterComparisonType.Less, "DeadLetteredOn",
					olderThanUtc));
				cleanupFilter.Add(deadLetteredFilter);
				esq.Filters.Add(cleanupFilter);
				List<Guid> ids = esq.GetEntityCollection(_userConnection)
					.Select(x => x.PrimaryColumnValue)
					.Where(x => x != Guid.Empty)
					.Distinct()
					.ToList();
				if (ids.Count == 0) {
					break;
				}
				int removedThisBatch = 0;
				for (int i = 0; i < ids.Count; i += maxInClauseParameterCount) {
					List<Guid> chunk = ids.Skip(i).Take(maxInClauseParameterCount).ToList();
					removedThisBatch += new Delete(_userConnection)
						.From(KnwSrcEventsOutboxSchema.Name)
						.Where("Id").In(Column.Parameters(chunk))
						.Execute();
				}
				totalRemoved += removedThisBatch;
				if (removedThisBatch <= 0) {
					KnwLibUtils.Logger.Warn($"[{nameof(KnwSrcOutboxRepository)}] CleanupProcessed deleted 0 rows " +
						$"for {ids.Count} candidates; stopping to avoid potential infinite loop.");
					break;
				}
			}
			return totalRemoved;
		}

		#endregion

	}

	#endregion

}

