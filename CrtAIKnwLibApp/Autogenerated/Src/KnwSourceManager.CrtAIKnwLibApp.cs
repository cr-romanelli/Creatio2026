namespace Creatio.Copilot
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSourceManager

	/// <summary>
	/// Provides methods for accessing and updating knowledge source records of the knowledge library.
	/// </summary>
	internal interface IKnwSourceManager
	{

		#region Methods: Internal

		/// <summary>
		/// Gets a knowledge source record by its unique identifier.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		/// <returns>The <see cref="KnwSourceRecord"/> corresponding to the specified identifier.</returns>
		KnwSourceRecord GetById(Guid knwSourceId);

		/// <summary>
		/// Updates the status of a knowledge source.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		/// <param name="status">The new status to set for the knowledge source.</param>
		void UpdateStatus(Guid knwSourceId, KnwSourceStatusEnum status);

		/// <summary>
		/// Updates the status of a knowledge source.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		/// <param name="status">The new status to set for the knowledge source.</param>
		/// <param name="lastIndexedOn">The date and time when the knowledge source was last indexed. Optional.</param>
		void UpdateStatus(Guid knwSourceId, KnwSourceStatusEnum status, DateTime? lastIndexedOn);

		/// <summary>
		/// Lists knowledge sources visible to the current user that are ready for retrieval.
		/// </summary>
		/// <returns>Visible and available knowledge sources.</returns>
		IReadOnlyCollection<KnwSourceRecord> GetAvailableSources();

		#endregion

	}

	#endregion

	#region Class: KnwSourceManager

	/// <summary>
	/// Default implementation of <see cref="IKnwSourceManager"/> for managing knowledge source records.
	/// </summary>
	[DefaultBinding(typeof(IKnwSourceManager))]
	internal class KnwSourceManager: IKnwSourceManager
	{

		#region Fields: Private

		private static readonly ConcurrentDictionary<KnwSourceStatusEnum, Guid> _statusCache =
			new ConcurrentDictionary<KnwSourceStatusEnum, Guid>();
		private static int _statusCacheState;
		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwSourceManager"/> class.
		/// </summary>
		/// <param name="userConnection">The user connection context.</param>
		public KnwSourceManager(UserConnection userConnection) {
			_userConnection = userConnection;
			EnsureStatusCacheInitialized();
		}

		#endregion

		#region Methods: Private

		private Guid GetKnowledgeSourceStatusId(KnwSourceStatusEnum value) {
			EnsureStatusCacheInitialized();
			return _statusCache.TryGetValue(value, out var id) ? id : Guid.Empty;
		}

		private void EnsureStatusCacheInitialized() {
			if (Volatile.Read(ref _statusCacheState) == 1) {
				return;
			}
			if (Interlocked.CompareExchange(ref _statusCacheState, 1, 0) == 0) {
				var schema = _userConnection.EntitySchemaManager.GetInstanceByName("KnwSourceStatus");
				var esq = new EntitySchemaQuery(schema) { PrimaryQueryColumn = { IsAlwaysSelect = true } };
				esq.AddColumn("Value");
				var collection = esq.GetEntityCollection(_userConnection);
				foreach (var e in collection) {
					var value = (KnwSourceStatusEnum)e.GetTypedColumnValue<int>("Value");
					var id = e.PrimaryColumnValue;
					_statusCache.AddOrUpdate(value, id, (_, __) => id);
				}
			} else {
				var spin = new SpinWait();
				while (Volatile.Read(ref _statusCacheState) != 1) {
					spin.SpinOnce();
				}
			}
		}

		private EntitySchemaQuery CreateKnwSourceQuery() {
			var entitySchemaManager = _userConnection.EntitySchemaManager;
			var entitySchema = entitySchemaManager.GetInstanceByName("KnwSource");
			var esq = new EntitySchemaQuery(entitySchema) {
				UnmaskColumnValues = true,
				IgnoreDisplayValues = true
			};
			esq.PrimaryQueryColumn.IsAlwaysSelect = true;
			esq.AddColumn("Name");
			esq.AddColumn("Description");
			esq.AddColumn("LastIndexedOn");
			esq.AddColumn("KnwSourceConfig");
			esq.AddColumn("KnwSourceStatus.Id");
			esq.AddColumn("KnwSourceStatus.Value");
			esq.AddColumn("KnwProvider.Id");
			esq.AddColumn("KnwProvider.Name");
			esq.AddColumn("KnwProvider.Code");
			esq.AddColumn("KnwProvider.Description");
			EntitySchemaQueryColumn connectionIdColumn = esq.AddColumn("KnwProviderConnection.Id");
			connectionIdColumn.SetForcedSourceAlias("KPC");
			esq.AddColumn("KnwProviderConnection.Name").SetForcedSourceAlias("KPC");
			esq.AddColumn("KnwProviderConnection.Description").SetForcedSourceAlias("KPC");
			esq.AddColumn("KnwProviderConnection.ConnectionParams").SetForcedSourceAlias("KPC");
			return esq;
		}

		private EntitySchemaQuery CreateAvailableSourcesQuery() {
			EntitySchemaQuery esq = CreateKnwSourceQuery();
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "KnwSourceStatus.Value",
				(int)KnwSourceStatusEnum.Available));
			return esq;
		}

		private static KnwSourceRecord ReadKnwSourceRecord(Entity entity) {
			var id = entity.PrimaryColumnValue;
			var name = entity.GetTypedColumnValue<string>("Name");
			var description = entity.GetTypedColumnValue<string>("Description");
			var status = (KnwSourceStatusEnum)entity.GetTypedColumnValue<int>("KnwSourceStatus_Value");
			var providerId = entity.GetTypedColumnValue<Guid>("KnwProvider_Id");
			DateTime? lastIndexedOn = null;
			object lastIndexedOnValue = entity.GetColumnValue("LastIndexedOn");
			if (lastIndexedOnValue is DateTime value) {
				lastIndexedOn = value;
			}
			var knwSourceConfig = entity.GetTypedColumnValue<string>("KnwSourceConfig") ?? string.Empty;
			KnwProviderRecord provider = null;
			if (providerId != Guid.Empty) {
				var providerName = entity.GetTypedColumnValue<string>("KnwProvider_Name");
				var providerDescription = entity.GetTypedColumnValue<string>("KnwProvider_Description");
				var providerCode = entity.GetTypedColumnValue<string>("KnwProvider_Code");
				provider = new KnwProviderRecord(providerId, providerName, providerDescription, providerCode);
			}
			var connectionId = entity.GetTypedColumnValue<Guid>("KnwProviderConnection_Id");
			KnwProviderConnectionRecord connection = null;
			if (connectionId != Guid.Empty) {
				var connectionName = entity.GetTypedColumnValue<string>("KnwProviderConnection_Name");
				var connectionParams = entity.GetTypedColumnValue<string>("KnwProviderConnection_ConnectionParams");
				connection = new KnwProviderConnectionRecord(connectionId, connectionName, providerId,
					connectionParams);
			}
			return new KnwSourceRecord(id, name, description, lastIndexedOn, status, provider, connection, knwSourceConfig);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public KnwSourceRecord GetById(Guid knwSourceId) {
			EntitySchemaQuery esq = CreateKnwSourceQuery();
			var entity = esq.GetEntity(_userConnection, knwSourceId);
			return entity == null ? null : ReadKnwSourceRecord(entity);
		}

		/// <inheritdoc/>
		public void UpdateStatus(Guid knwSourceId, KnwSourceStatusEnum status) {
			UpdateStatus(knwSourceId, status, null);
		}

		/// <inheritdoc/>
		public void UpdateStatus(Guid knwSourceId, KnwSourceStatusEnum status, DateTime? lastIndexedOn) {
			if (knwSourceId == Guid.Empty) {
				throw new ArgumentException("KnwSourceId must not be empty", nameof(knwSourceId));
			}
			var statusId = GetKnowledgeSourceStatusId(status);
			if (statusId == Guid.Empty) {
				throw new InvalidOperationException($"KnwSourceStatus lookup not found for value {(int)status} ({status}).");
			}
			var schema = _userConnection.EntitySchemaManager.GetInstanceByName("KnwSource");
			var entity = schema.CreateEntity(_userConnection);
			if (!entity.FetchFromDB(knwSourceId)) {
				throw new InvalidOperationException($"KnwSource entity not found by Id {knwSourceId}");
			}
			entity.SetColumnValue("KnwSourceStatus", statusId);
			if (lastIndexedOn.HasValue) {
				entity.SetColumnValue("LastIndexedOn", lastIndexedOn);
			}
			entity.Save();
		}

		/// <inheritdoc/>
		public IReadOnlyCollection<KnwSourceRecord> GetAvailableSources() {
			EntitySchemaQuery esq = CreateAvailableSourcesQuery();
			EntityCollection entities = esq.GetEntityCollection(_userConnection);
			IReadOnlyCollection<KnwSourceRecord> result = entities.Select(ReadKnwSourceRecord)
				.OrderBy(record => record.Name)
				.ToList();
			KnwLibUtils.Logger.Info(
				$"[{nameof(KnwSourceManager)}] Available knowledge sources query completed. " +
				$"CurrentUserId={_userConnection.CurrentUser.Id}, CurrentUserName={_userConnection.CurrentUser.Name}, " +
				$"SourceCount={result.Count}, SourceIds=[{string.Join(", ", result.Select(source => source.Id.ToString()))}]");
			return result;
		}

		#endregion

	}

	#endregion

}

