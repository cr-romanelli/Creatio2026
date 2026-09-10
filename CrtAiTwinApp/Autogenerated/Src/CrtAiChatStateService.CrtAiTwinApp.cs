namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Web.SessionState;
	using Creatio.FeatureToggling;
	using global::Common.Logging;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	/// <summary>
	/// REST service that resolves whether the new AI chat is enabled for the
	/// current user.
	///
	/// Resolution rules:
	///   1. Look up every <c>AIManagement</c> row whose <c>SysAdminUnit</c>
	///      equals the current user id OR a role id the current user belongs
	///      to.
	///   2. Direct user-row matches always win over role-row matches,
	///      regardless of priority.
	///   3. Within the winning group, the row with the lowest <c>Priority</c>
	///      wins (ascending order — smaller number = higher priority);
	///      <c>Id</c> is added as a deterministic tie-breaker so rows that
	///      share a priority resolve in a stable order across requests.
	///   4. When no row matches, the resolved state is <c>false</c>.
	///
	/// Reads run through <c>SystemUserConnection</c> because the
	/// <c>AIManagement</c> entity grants Read/Edit/Delete only to the
	/// "System administrators" role; every non-admin caller would otherwise
	/// see zero rows and fall back to the default. The write endpoint
	/// (<c>DeleteAiManagementRows</c>) keeps the same admin boundary by
	/// requiring <c>CanManageAdministration</c> before executing writes
	/// through <c>SystemUserConnection</c>.
	///
	/// Route: <c>GET /0/rest/CrtAiChatStateService/IsNewChatEnabled</c>.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CrtAiChatStateService : BaseService, IReadOnlySessionState
	{
		private const string AIManagementSchemaName = "AIManagement";
		private const string SysUserInRoleSchemaName = "SysUserInRole";
		private const string SysAdminUnitColumnName = "SysAdminUnit";
		private const string PriorityColumnName = "Priority";
		private const string NewAIEnabledColumnName = "NewAIEnabled";
		private const string IdColumnName = "Id";
		private const string CanManageAdministrationOperation = "CanManageAdministration";

		internal const string ResolvedFromUserRow = "user";
		internal const string ResolvedFromRoleRow = "role";
		internal const string ResolvedFromDefault = "default";

		private static readonly global::Common.Logging.ILog Log =
			global::Common.Logging.LogManager.GetLogger(typeof(CrtAiChatStateService));

		// APD-4399: per-user cache of the resolved state, gated by the
		// CrtAiTwinApiResultCache feature. Every IsNewChatEnabled call otherwise
		// runs two ESQs (user roles + AIManagement rows) — once per tab per user
		// across the whole organization. The TTL keeps admin changes visible
		// without a recycle; DeleteAiManagementRows clears the cache eagerly.
		private static readonly TimeSpan StateCacheTtl = TimeSpan.FromMinutes(5);
		private static readonly ConcurrentDictionary<Guid, CachedAiChatState> StateCache =
			new ConcurrentDictionary<Guid, CachedAiChatState>();

		[OperationContract]
		[WebInvoke(
			Method = "GET",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public AiChatStateResponse IsNewChatEnabled()
		{
			try
			{
				if (!Features.GetIsEnabled<CrtAiTwinApiResultCache>())
				{
					return ResolveState(UserConnection);
				}

				Guid currentUserId = UserConnection.CurrentUser.Id;
				CachedAiChatState cached;
				if (StateCache.TryGetValue(currentUserId, out cached))
				{
					if (cached.ExpiresAtUtc > DateTime.UtcNow)
					{
						return cached.Response;
					}
					// Reclaim the expired entry eagerly so the map stays bounded
					// by recently active users instead of growing one stale entry
					// per user for the whole app-domain lifetime.
					StateCache.TryRemove(currentUserId, out _);
				}

				AiChatStateResponse response = ResolveState(UserConnection);
				StateCache[currentUserId] = new CachedAiChatState
				{
					Response = response,
					ExpiresAtUtc = DateTime.UtcNow.Add(StateCacheTtl)
				};
				return response;
			}
			catch (Exception ex)
			{
				// Log server-side; never echo the exception to the HTTP
				// response — DB / ESQ messages can leak schema or internal
				// hostnames and aren't actionable for the browser.
				Log.Error("CrtAiChatStateService.IsNewChatEnabled failed.", ex);
				return new AiChatStateResponse
				{
					Enabled = false,
					ResolvedFrom = ResolvedFromDefault
				};
			}
		}

		[OperationContract]
		[WebInvoke(
			Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public DeleteAiManagementRowsResponse DeleteAiManagementRows(DeleteAiManagementRowsRequest request)
		{
			try
			{
				if (request == null)
				{
					throw new ArgumentNullException("request");
				}

				if (request.RecordIds == null || !request.RecordIds.Exists(recordId => recordId != Guid.Empty))
				{
					throw new ArgumentException("At least one record id is required.", "request");
				}

				EnsureCanManageAssistantState(UserConnection);
				UserConnection systemConnection = UserConnection.AppConnection.SystemUserConnection;
				int affectedRows = DeleteAiManagementRows(systemConnection, request.RecordIds);
				// APD-4399: the mutation changes resolution inputs for an unknown
				// set of users (role rows) — drop the whole state cache eagerly
				// instead of waiting out the TTL.
				StateCache.Clear();
				return new DeleteAiManagementRowsResponse
				{
					Success = true,
					AffectedRows = affectedRows
				};
			}
			catch (Exception ex)
			{
				Log.Error("CrtAiChatStateService.DeleteAiManagementRows failed.", ex);
				return new DeleteAiManagementRowsResponse
				{
					Success = false,
					AffectedRows = 0
				};
			}
		}

		private static AiChatStateResponse ResolveState(UserConnection userConnection)
		{
			Guid currentUserId = userConnection.CurrentUser.Id;
			UserConnection systemConnection = userConnection.AppConnection.SystemUserConnection;

			HashSet<Guid> roleIds = LoadCurrentUserRoleIds(systemConnection, currentUserId);
			List<AiChatStateRow> matchingRows = LoadMatchingRows(systemConnection, currentUserId, roleIds);

			AiChatStateRow winner = SelectWinner(matchingRows, currentUserId);
			return BuildResponse(winner, currentUserId);
		}

		/// <summary>
		/// Pure resolution pass over the in-memory matching rows so the
		/// algorithm is testable without an <see cref="EntitySchemaQuery"/>
		/// round-trip. Caller is responsible for pre-loading and pre-ordering
		/// the row set; this method only encodes the user-vs-role precedence
		/// and the lowest-priority winner rule.
		/// </summary>
		internal static AiChatStateRow SelectWinner(
			IEnumerable<AiChatStateRow> matchingRows,
			Guid currentUserId)
		{
			if (matchingRows == null)
			{
				return null;
			}

			AiChatStateRow userRow = null;
			AiChatStateRow bestRoleRow = null;

			foreach (AiChatStateRow row in matchingRows)
			{
				if (row == null)
				{
					continue;
				}

				if (row.SysAdminUnitId == currentUserId)
				{
					if (userRow == null || row.Priority < userRow.Priority)
					{
						userRow = row;
					}
					continue;
				}

				if (bestRoleRow == null || row.Priority < bestRoleRow.Priority)
				{
					bestRoleRow = row;
				}
			}

			return userRow ?? bestRoleRow;
		}

		/// <summary>
		/// Maps a resolved row (or <c>null</c> for "no match") into the
		/// outbound REST contract. Kept internal so tests can pin the
		/// `default` / `user` / `role` projection without spinning up the
		/// WCF host.
		/// </summary>
		internal static AiChatStateResponse BuildResponse(AiChatStateRow winner, Guid currentUserId)
		{
			if (winner == null)
			{
				return new AiChatStateResponse
				{
					Enabled = false,
					ResolvedFrom = ResolvedFromDefault
				};
			}

			return new AiChatStateResponse
			{
				Enabled = winner.NewAIEnabled,
				ResolvedFrom = winner.SysAdminUnitId == currentUserId
					? ResolvedFromUserRow
					: ResolvedFromRoleRow,
				MatchedSysAdminUnitId = winner.SysAdminUnitId,
				Priority = winner.Priority
			};
		}

		/// <summary>
		/// Loads the role ids the given user belongs to. Exposed as
		/// <c>internal</c> (alongside <see cref="SelectWinner"/> /
		/// <see cref="BuildResponse"/>) so the ESQ data-loading path is
		/// directly testable with a mocked connection instead of only through
		/// the WCF entrypoint.
		/// </summary>
		internal static HashSet<Guid> LoadCurrentUserRoleIds(UserConnection systemConnection, Guid currentUserId)
		{
			var roleIds = new HashSet<Guid>();

			var esq = new EntitySchemaQuery(systemConnection.EntitySchemaManager, SysUserInRoleSchemaName);
			string roleIdColumn = esq.AddColumn("SysRole.Id").Name;
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysUser", currentUserId));

			EntityCollection entities = esq.GetEntityCollection(systemConnection);
			foreach (Entity entity in entities)
			{
				Guid roleId = entity.GetTypedColumnValue<Guid>(roleIdColumn);
				if (roleId != Guid.Empty)
				{
					roleIds.Add(roleId);
				}
			}

			return roleIds;
		}

		/// <summary>
		/// Loads the <c>AIManagement</c> rows that target the user or one of
		/// the supplied role ids, pre-ordered by priority then id. Exposed as
		/// <c>internal</c> so the query shape and row mapping are testable with
		/// a mocked connection (see <see cref="LoadCurrentUserRoleIds"/>).
		/// </summary>
		internal static List<AiChatStateRow> LoadMatchingRows(
			UserConnection systemConnection,
			Guid currentUserId,
			HashSet<Guid> roleIds)
		{
			var subjectIds = new List<Guid>(roleIds.Count + 1) { currentUserId };
			subjectIds.AddRange(roleIds);

			var esq = new EntitySchemaQuery(systemConnection.EntitySchemaManager, AIManagementSchemaName);
			var idColumnDef = esq.AddColumn(IdColumnName);
			string idColumn = idColumnDef.Name;
			string sysAdminUnitIdColumn = esq.AddColumn(SysAdminUnitColumnName + ".Id").Name;
			var priorityColumnDef = esq.AddColumn(PriorityColumnName);
			string priorityColumn = priorityColumnDef.Name;
			string newAIEnabledColumn = esq.AddColumn(NewAIEnabledColumnName).Name;

			// Stable ordering: lowest Priority wins; Id breaks ties so the
			// resolver is deterministic when two rows share a priority.
			priorityColumnDef.OrderByAsc();
			idColumnDef.OrderByAsc();

			var filterGroup = new EntitySchemaQueryFilterCollection(
				esq,
				Terrasoft.Common.LogicalOperationStrict.Or);
			foreach (Guid subjectId in subjectIds)
			{
				filterGroup.Add(esq.CreateFilterWithParameters(
					FilterComparisonType.Equal,
					SysAdminUnitColumnName,
					subjectId));
			}
			esq.Filters.Add(filterGroup);

			EntityCollection entities = esq.GetEntityCollection(systemConnection);

			var rows = new List<AiChatStateRow>(entities.Count);
			foreach (Entity entity in entities)
			{
				rows.Add(new AiChatStateRow
				{
					Id = entity.GetTypedColumnValue<Guid>(idColumn),
					SysAdminUnitId = entity.GetTypedColumnValue<Guid>(sysAdminUnitIdColumn),
					Priority = entity.GetTypedColumnValue<int>(priorityColumn),
					NewAIEnabled = entity.GetTypedColumnValue<bool>(newAIEnabledColumn)
				});
			}

			return rows;
		}

		private static void EnsureCanManageAssistantState(UserConnection userConnection)
		{
			userConnection.DBSecurityEngine.CheckCanExecuteOperation(CanManageAdministrationOperation);
		}

		internal static int DeleteAiManagementRows(
			UserConnection systemConnection,
			IEnumerable<Guid> recordIds)
		{
			var uniqueRecordIds = new HashSet<Guid>();
			if (recordIds != null)
			{
				foreach (Guid recordId in recordIds)
				{
					if (recordId != Guid.Empty)
					{
						uniqueRecordIds.Add(recordId);
					}
				}
			}

			if (uniqueRecordIds.Count == 0)
			{
				return 0;
			}

			return new Delete(systemConnection)
				.From(AIManagementSchemaName)
				.Where(IdColumnName).In(Column.Parameters(uniqueRecordIds))
				.Execute();
		}

		internal sealed class AiChatStateRow
		{
			public Guid Id { get; set; }
			public Guid SysAdminUnitId { get; set; }
			public int Priority { get; set; }
			public bool NewAIEnabled { get; set; }
		}

		private sealed class CachedAiChatState
		{
			public AiChatStateResponse Response { get; set; }
			public DateTime ExpiresAtUtc { get; set; }
		}
	}

	[DataContract]
	public class AiChatStateResponse
	{
		[DataMember(Name = "enabled")]
		public bool Enabled { get; set; }

		[DataMember(Name = "resolvedFrom")]
		public string ResolvedFrom { get; set; }

		[DataMember(Name = "matchedSysAdminUnitId", EmitDefaultValue = false)]
		public Guid? MatchedSysAdminUnitId { get; set; }

		[DataMember(Name = "priority", EmitDefaultValue = false)]
		public int? Priority { get; set; }
	}

	[DataContract]
	public class DeleteAiManagementRowsRequest
	{
		[DataMember(Name = "recordIds")]
		public List<Guid> RecordIds { get; set; }
	}

	[DataContract]
	public class DeleteAiManagementRowsResponse
	{
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		[DataMember(Name = "affectedRows")]
		public int AffectedRows { get; set; }
	}

	/// <summary>
	/// Gates the API-result caching introduced by APD-4399: the per-user
	/// server-side cache of <c>CrtAiChatStateService.IsNewChatEnabled</c> and the
	/// <c>cacheable</c> hint in the <c>CrtIdentityAssertionService.EnsureUser</c>
	/// response that lets the shell cache the gate outcome client-side.
	///
	/// Lives in this schema (not Files/src/cs) because schema sources are also
	/// compiled into Terrasoft.Configuration on platform integration builds,
	/// where the package Files assembly and its CrtAiTwinApp namespace do not
	/// exist — schemas may only reference types that compile there.
	/// </summary>
	public class CrtAiTwinApiResultCache : FeatureMetadata
	{
		// IsEnabled is intentionally left unset here: the default (disabled) state
		// is carried by the AdminUnitFeatureState_CrtAiTwinApiResultCache data
		// record for "All employees", so it stays admin-toggleable without a
		// code deploy.
		public CrtAiTwinApiResultCache()
		{
			Description = "Enables caching of AI Twin package API results (EnsureUser client cache hint, IsNewChatEnabled server cache) to prevent REST call overload.";
		}
	}
}

