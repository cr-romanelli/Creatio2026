namespace Creatio.Copilot
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.ExceptionServices;
	using System.Threading;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;

	#region Class: KnwIndexingSessionSnapshot

	/// <summary>
	/// Represents authoritative indexing session data resolved from the external service.
	/// </summary>
	public sealed class KnwIndexingSessionSnapshot
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the knowledge source identifier.
		/// </summary>
		public Guid KnwSourceId { get; set; }

		/// <summary>
		/// Gets or sets the remote indexing session identifier.
		/// </summary>
		public Guid SessionId { get; set; }

		/// <summary>
		/// Gets or sets the remote index identifier.
		/// </summary>
		public Guid IndexId { get; set; }

		/// <summary>
		/// Gets or sets the authoritative session state.
		/// </summary>
		public KnwIndexingStatusEnum State { get; set; }

		/// <summary>
		/// Gets or sets the session start timestamp.
		/// </summary>
		public DateTime? StartedOn { get; set; }

		/// <summary>
		/// Gets or sets the transfer completion timestamp.
		/// </summary>
		public DateTime? TransferredOn { get; set; }

		/// <summary>
		/// Gets or sets the session completion timestamp.
		/// </summary>
		public DateTime? CompletedOn { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwIndexingStatusSnapshot

	/// <summary>
	/// Represents authoritative indexing status resolved from the external service without local persistence.
	/// </summary>
	public sealed class KnwIndexingStatusSnapshot
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the knowledge source identifier.
		/// </summary>
		public Guid KnowledgeSourceId { get; set; }

		/// <summary>
		/// Gets or sets the authoritative knowledge source status.
		/// </summary>
		public KnwSourceStatusEnum SourceStatus { get; set; }

		/// <summary>
		/// Gets or sets the authoritative last indexed timestamp.
		/// </summary>
		public DateTime? LastIndexedOn { get; set; }

		/// <summary>
		/// Gets or sets the latest authoritative indexing session when the service reports one.
		/// </summary>
		public KnwIndexingSessionSnapshot LatestSession { get; set; }

		/// <summary>
		/// Gets a value indicating whether authoritative status reports active indexing work.
		/// </summary>
		public bool HasActiveIndexing => (SourceStatus == KnwSourceStatusEnum.Indexing && LatestSession == null)
			|| LatestSession?.State.IsActive() == true;

		/// <summary>
		/// Gets a value indicating whether automatic incremental synchronization can start.
		/// </summary>
		public bool IsAutoSyncReady => SourceStatus == KnwSourceStatusEnum.Available && LastIndexedOn.HasValue;

		#endregion

	}

	#endregion

	#region Class: KnwActiveIndexingSessionExistsException

	/// <summary>
	/// Represents an attempt to start indexing while authoritative work already exists.
	/// </summary>
	internal sealed class KnwActiveIndexingSessionExistsException : InvalidOperationException
	{

		#region Constructors: Public

		public KnwActiveIndexingSessionExistsException(Guid knowledgeSourceId, KnwIndexingSessionSnapshot session)
			: base($"Active indexing session already exists for knowledge source {knowledgeSourceId}.") {
			KnowledgeSourceId = knowledgeSourceId;
			Session = session;
		}

		#endregion

		#region Properties: Public

		public Guid KnowledgeSourceId { get; }

		public KnwIndexingSessionSnapshot Session { get; }

		#endregion

	}

	#endregion

	#region Class: KnwStartedSessionTerminalStateException

	/// <summary>
	/// Represents a newly created session resolving to a terminal authoritative state.
	/// </summary>
	internal sealed class KnwStartedSessionTerminalStateException : InvalidOperationException
	{

		#region Constructors: Public

		public KnwStartedSessionTerminalStateException(KnwIndexingSessionRecord session)
			: base($"Newly started session {session?.SessionId} is in terminal state {session?.State}.") {
			Session = session;
		}

		#endregion

		#region Properties: Public

		public KnwIndexingSessionRecord Session { get; }

		#endregion

	}

	#endregion

	#region Class: KnwIndexingSessionManager

	/// <summary>
	/// Manages lifecycle of knowledge source indexing sessions persisted in the database.
	/// Active sessions are those whose status is not Completed / Failed / Cancelled.
	/// </summary>
	[DefaultBinding(typeof(IKnwIndexingSessionManager))]
	public class KnwIndexingSessionManager : IKnwIndexingSessionManager
	{

		#region Constants: Private

		private const string KnwIndexingSessionSchemaName = "KnwIndexingSession";
		private const string KnwIndexingStatusSchemaName = "KnwIndexingStatus";
		private const int FinalizeRetryCount = 3;
		private const int StartSyncRetryCount = 3;
		private static readonly TimeSpan StartSyncRetryDelay = TimeSpan.FromMilliseconds(500);

		#endregion

		#region Fields: Private

		private static readonly ConcurrentDictionary<KnwIndexingStatusEnum, Guid> _writeStatusCache =
			new ConcurrentDictionary<KnwIndexingStatusEnum, Guid>();
		private static readonly ConcurrentDictionary<Guid, KnwIndexingStatusEnum> _readStatusCache =
			new ConcurrentDictionary<Guid, KnwIndexingStatusEnum>();
		private readonly UserConnection _userConnection;
		private IKnwServiceClient _serviceClient;
		private IKnwServiceProblemHelper _serviceProblemHelper;
		private IKnwSourceManager _sourceManager;
		private static int _statusCacheState;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwIndexingSessionManager"/> class.
		/// </summary>
		/// <param name="userConnection">The user connection context.</param>
		public KnwIndexingSessionManager(UserConnection userConnection) {
			_userConnection = userConnection;
			EnsureStatusCacheInitialized();
		}

		#endregion

		#region Properties: Private

		private IKnwServiceClient ServiceClient {
			get {
				if (_serviceClient == null) {
					_serviceClient = ClassFactory.Get<IKnwServiceClient>(
						new ConstructorArgument("userConnection", _userConnection),
						new ConstructorArgument("httpRequestClient", ClassFactory.Get<IHttpRequestClient>()));
				}
				return _serviceClient;
			}
		}

		private IKnwServiceProblemHelper ServiceProblemHelper =>
			_serviceProblemHelper ?? (_serviceProblemHelper = ClassFactory.Get<IKnwServiceProblemHelper>());

		private IKnwSourceManager SourceManager =>
			_sourceManager ?? (_sourceManager = ClassFactory.Get<IKnwSourceManager>(
				new ConstructorArgument("userConnection", _userConnection)));

		#endregion

		#region Methods: Private

		private static bool HasServingIndex(KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			return statusResponse.ServingIndex != null && statusResponse.ServingIndex.IndexId != Guid.Empty;
		}

		private static DateTime? NormalizeTimestamp(DateTimeOffset? value) {
			return value?.UtcDateTime;
		}

		private static bool HasRemoteIdentifiers(KnwIndexingSessionRecord session) {
			return session != null && session.SessionId != Guid.Empty && session.IndexId != Guid.Empty;
		}

		private static bool IsCancellationLike(Exception exception) {
			if (exception is OperationCanceledException || exception is TimeoutException) {
				return true;
			}
			return exception?.Message?.IndexOf("A task was canceled", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static string GetExceptionSummary(Exception exception) {
			if (exception == null) {
				return "none";
			}
			string innerType = exception.InnerException?.GetType().Name ?? "none";
			string innerMessage = exception.InnerException?.Message ?? "none";
			return $"Type={exception.GetType().Name}, Message={exception.Message}, InnerType={innerType}, InnerMessage={innerMessage}";
		}

		private static string GetFinalizeFailureReason(Exception exception, CancellationToken cancellationToken) {
			string reasonCategory;
			if (exception is OperationCanceledException) {
				reasonCategory = "OperationCanceledException";
			} else if (exception is TimeoutException) {
				reasonCategory = "Timeout";
			} else if (exception?.Message?.IndexOf("A task was canceled", StringComparison.OrdinalIgnoreCase) >= 0) {
				reasonCategory = "TaskCanceledMessage";
			} else {
				reasonCategory = "UnexpectedFinalizeFailure";
			}
			return $"ReasonCategory={reasonCategory}, TokenCancellationRequested={cancellationToken.IsCancellationRequested}, " +
				GetExceptionSummary(exception);
		}

		private static KnwSourceStatusEnum ResolveSourceStatus(KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			if (statusResponse.Work != null) {
				return KnwSourceStatusEnum.Indexing;
			}
			if (HasServingIndex(statusResponse)) {
				return KnwSourceStatusEnum.Available;
			}
			string summaryStatus = statusResponse.SummaryStatus?.Trim();
			if (string.IsNullOrWhiteSpace(summaryStatus)) {
				throw new InvalidOperationException("Knowledge source indexing status summaryStatus is empty.");
			}
			switch (summaryStatus.ToLowerInvariant()) {
				case "ready":
					return KnwSourceStatusEnum.Available;
				case "building":
					return KnwSourceStatusEnum.Indexing;
				case "updating":
				case "rebuilding":
					return KnwSourceStatusEnum.Indexing;
				case "failed":
					return KnwSourceStatusEnum.Unavailable;
				case "cancelled":
					return KnwSourceStatusEnum.Cancelled;
				case "unavailable":
					return KnwSourceStatusEnum.Unavailable;
				default:
					throw new InvalidOperationException($"Unknown knowledge source summary status '{summaryStatus}'.");
			}
		}

		private static KnwIndexingStatusEnum ResolveSessionState(KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			KnwKnowledgeSourceSessionSnapshotResponse latestSession = statusResponse.LatestSession;
			if (latestSession == null) {
				throw new InvalidOperationException("Knowledge source indexing status latestSession is missing.");
			}
			if (statusResponse.Work != null) {
				return KnwIndexingStatusEnum.Indexing;
			}
			string sessionStatus = latestSession.SessionStatus?.Trim();
			if (string.IsNullOrWhiteSpace(sessionStatus)) {
				throw new InvalidOperationException("Knowledge source indexing status latestSession.sessionStatus is empty.");
			}
			switch (sessionStatus.ToLowerInvariant()) {
				case "created":
					return KnwIndexingStatusEnum.Created;
				case "running":
					return KnwIndexingStatusEnum.Sending;
				case "failed":
					return KnwIndexingStatusEnum.Failed;
				case "cancelled":
					return KnwIndexingStatusEnum.Cancelled;
				case "completed":
				case "done":
					switch (statusResponse.SummaryStatus?.Trim().ToLowerInvariant()) {
						case "ready":
							return KnwIndexingStatusEnum.Completed;
						case "failed":
						case "unavailable":
							return KnwIndexingStatusEnum.Failed;
						case "cancelled":
							return KnwIndexingStatusEnum.Cancelled;
						case "building":
						case "rebuilding":
						case "updating":
							throw new InvalidOperationException(
								$"Knowledge source indexing status returned terminal session status '{sessionStatus}' " +
								$"for in-progress summary status '{statusResponse.SummaryStatus}' without active work.");
						default:
							throw new InvalidOperationException(
								$"Unknown knowledge source summary status '{statusResponse.SummaryStatus}'.");
					}
				default:
					throw new InvalidOperationException(
						$"Unknown knowledge source latestSession.sessionStatus '{latestSession.SessionStatus}'.");
			}
		}

		private static DateTime? ResolveCompletedOn(KnwKnowledgeSourceIndexingStatusResponse statusResponse,
				KnwIndexingStatusEnum state) {
			switch (state) {
				case KnwIndexingStatusEnum.Completed:
					return NormalizeTimestamp(statusResponse.LatestSession?.SessionCompletedAt);
				case KnwIndexingStatusEnum.Failed:
				case KnwIndexingStatusEnum.Cancelled:
					return NormalizeTimestamp(statusResponse.LatestSession?.SessionCompletedAt);
				default:
					return null;
			}
		}

		private void ValidateAuthoritativeStatus(Guid knowledgeSourceId, Guid expectedSessionId, Guid expectedIndexId,
				KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			if (statusResponse.KnowledgeSourceId == Guid.Empty) {
				throw new InvalidOperationException("Knowledge source indexing status knowledgeSourceId is empty.");
			}
			if (statusResponse.KnowledgeSourceId != knowledgeSourceId) {
				throw new InvalidOperationException(
					$"Knowledge source indexing status returned unexpected knowledgeSourceId '{statusResponse.KnowledgeSourceId}'.");
			}
			if (string.IsNullOrWhiteSpace(statusResponse.SummaryStatus)) {
				throw new InvalidOperationException("Knowledge source indexing status summaryStatus is empty.");
			}
			KnwKnowledgeSourceSessionSnapshotResponse latestSession = statusResponse.LatestSession;
			if (latestSession == null) {
				throw new InvalidOperationException("Knowledge source indexing status latestSession is missing.");
			}
			if (latestSession.SessionId == Guid.Empty) {
				throw new InvalidOperationException("Knowledge source indexing status latestSession.sessionId is empty.");
			}
			if (latestSession.IndexId == Guid.Empty) {
				throw new InvalidOperationException("Knowledge source indexing status latestSession.indexId is empty.");
			}
			if (string.IsNullOrWhiteSpace(latestSession.SessionStatus)) {
				throw new InvalidOperationException(
					"Knowledge source indexing status latestSession.sessionStatus is empty.");
			}
			if (!latestSession.SessionCreatedAt.HasValue) {
				throw new InvalidOperationException(
					"Knowledge source indexing status latestSession.sessionCreatedAt is empty.");
			}
			if (expectedSessionId != Guid.Empty && latestSession.SessionId != expectedSessionId) {
				throw new InvalidOperationException(
					$"Knowledge source indexing status returned unexpected sessionId '{latestSession.SessionId}'.");
			}
			if (expectedIndexId != Guid.Empty && latestSession.IndexId != expectedIndexId) {
				throw new InvalidOperationException(
					$"Knowledge source indexing status returned unexpected indexId '{latestSession.IndexId}'.");
			}
		}

		private void ValidateLatestAuthoritativeStatus(Guid knowledgeSourceId,
				KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			if (statusResponse.KnowledgeSourceId == Guid.Empty) {
				throw new InvalidOperationException("Knowledge source indexing status knowledgeSourceId is empty.");
			}
			if (statusResponse.KnowledgeSourceId != knowledgeSourceId) {
				throw new InvalidOperationException(
					$"Knowledge source indexing status returned unexpected knowledgeSourceId '{statusResponse.KnowledgeSourceId}'.");
			}
			if (string.IsNullOrWhiteSpace(statusResponse.SummaryStatus)) {
				throw new InvalidOperationException("Knowledge source indexing status summaryStatus is empty.");
			}
		}

		private void TryCancelRemoteSession(Guid knowledgeSourceId, Guid sessionId) {
			if (sessionId == Guid.Empty) {
				return;
			}
			try {
				ServiceClient.CancelSession(sessionId, CancellationToken.None);
			} catch (Exception ex) {
				KnwLibUtils.Logger.Warn($"Failed to cancel remote session {sessionId} for source {knowledgeSourceId} " +
					$"after local persistence error. {GetExceptionSummary(ex)}", ex);
			}
		}

		private KnwIndexingSessionSnapshot BuildSessionSnapshot(Guid knowledgeSourceId,
				KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			KnwIndexingStatusEnum state = ResolveSessionState(statusResponse);
			KnwKnowledgeSourceSessionSnapshotResponse latestSession = statusResponse.LatestSession;
			return new KnwIndexingSessionSnapshot {
				KnwSourceId = knowledgeSourceId,
				SessionId = latestSession.SessionId,
				IndexId = latestSession.IndexId,
				State = state,
				StartedOn = NormalizeTimestamp(latestSession.SessionCreatedAt),
				TransferredOn = NormalizeTimestamp(latestSession.SessionCompletedAt),
				CompletedOn = ResolveCompletedOn(statusResponse, state),
			};
		}

		private static KnwIndexingSessionRecord BuildSessionRecord(Guid recordId,
				KnwIndexingSessionSnapshot sessionSnapshot) {
			return new KnwIndexingSessionRecord {
				Id = recordId,
				KnwSourceId = sessionSnapshot.KnwSourceId,
				SessionId = sessionSnapshot.SessionId,
				IndexId = sessionSnapshot.IndexId,
				State = sessionSnapshot.State,
				StartedOn = sessionSnapshot.StartedOn,
				TransferredOn = sessionSnapshot.TransferredOn,
				CompletedOn = sessionSnapshot.CompletedOn,
				ModifiedOn = DateTime.UtcNow
			};
		}

		private KnwIndexingStatusSnapshot BuildStatusSnapshot(Guid knowledgeSourceId,
				KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			KnwIndexingSessionSnapshot latestSession = null;
			KnwKnowledgeSourceSessionSnapshotResponse latestSessionResponse = statusResponse.LatestSession;
			if (latestSessionResponse != null && latestSessionResponse.SessionId != Guid.Empty
					&& latestSessionResponse.IndexId != Guid.Empty) {
				latestSession = BuildSessionSnapshot(knowledgeSourceId, statusResponse);
			}
			return new KnwIndexingStatusSnapshot {
				KnowledgeSourceId = knowledgeSourceId,
				SourceStatus = ResolveSourceStatus(statusResponse),
				LastIndexedOn = HasServingIndex(statusResponse)
					? NormalizeTimestamp(statusResponse.ServingIndex.UpdatedAt)
					: null,
				LatestSession = latestSession
			};
		}

		private void ApplySourceStatus(Guid knowledgeSourceId, KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			KnwSourceStatusEnum sourceStatus = ResolveSourceStatus(statusResponse);
			DateTime? lastIndexedOn = HasServingIndex(statusResponse)
				? NormalizeTimestamp(statusResponse.ServingIndex.UpdatedAt)
				: null;
			SourceManager.UpdateStatus(knowledgeSourceId, sourceStatus, lastIndexedOn);
		}

		private KnwIndexingSessionRecord PersistStatusSnapshot(KnwIndexingStatusSnapshot statusSnapshot) {
			SourceManager.UpdateStatus(statusSnapshot.KnowledgeSourceId, statusSnapshot.SourceStatus,
				statusSnapshot.LastIndexedOn);
			if (statusSnapshot.LatestSession == null) {
				return null;
			}
			return UpsertSessionRecord(BuildSessionRecord(Guid.Empty, statusSnapshot.LatestSession));
		}

		private KnwIndexingSessionRecord SyncAuthoritativeSession(Guid knowledgeSourceId,
				KnwKnowledgeSourceIndexingStatusResponse statusResponse) {
			ApplySourceStatus(knowledgeSourceId, statusResponse);
			KnwKnowledgeSourceSessionSnapshotResponse latestSession = statusResponse.LatestSession;
			if (latestSession == null || latestSession.SessionId == Guid.Empty || latestSession.IndexId == Guid.Empty) {
				return null;
			}
			return UpsertSessionRecord(BuildSessionRecord(Guid.Empty, BuildSessionSnapshot(knowledgeSourceId,
				statusResponse)));
		}

		private static void ValidateNoActiveAuthoritativeSession(Guid knowledgeSourceId,
				KnwIndexingSessionSnapshot latestAuthoritativeSession) {
			if (latestAuthoritativeSession == null) {
				return;
			}
			if (latestAuthoritativeSession.KnwSourceId != knowledgeSourceId) {
				throw new InvalidOperationException(
					$"Authoritative session belongs to unexpected knowledge source '{latestAuthoritativeSession.KnwSourceId}'.");
			}
			if (latestAuthoritativeSession.State.IsActive()) {
				throw new KnwActiveIndexingSessionExistsException(knowledgeSourceId, latestAuthoritativeSession);
			}
		}

		private static void ValidateAuthoritativeStatusSnapshot(Guid knowledgeSourceId,
				KnwIndexingStatusSnapshot authoritativeStatus) {
			if (authoritativeStatus == null) {
				return;
			}
			if (authoritativeStatus.KnowledgeSourceId != knowledgeSourceId) {
				throw new InvalidOperationException(
					$"Authoritative status belongs to unexpected knowledge source '{authoritativeStatus.KnowledgeSourceId}'.");
			}
			if (authoritativeStatus.HasActiveIndexing && authoritativeStatus.LatestSession == null) {
				throw new KnwActiveIndexingSessionExistsException(knowledgeSourceId, null);
			}
			ValidateNoActiveAuthoritativeSession(knowledgeSourceId, authoritativeStatus.LatestSession);
		}

		private EntitySchema GetSessionSchema() {
			EntitySchemaManager entitySchemaManager = _userConnection.EntitySchemaManager;
			return entitySchemaManager.GetInstanceByName(KnwIndexingSessionSchemaName);
		}

		private KnwIndexingSessionRecord ReadSessionRecord(Entity entity) {
			Guid knwSourceId = entity.GetTypedColumnValue<Guid>("KnwSourceId");
			Guid stateId = entity.GetTypedColumnValue<Guid>("KnwIndexingStatusId");
			KnwIndexingStatusEnum state = GetKnowledgeSourceStatusValue(stateId);
			DateTime? startedOn = GetDateTime(entity, "StartedOn");
			DateTime? transferredOn = GetDateTime(entity, "TransferredOn");
			DateTime? completedOn = GetDateTime(entity, "CompletedOn");
			DateTime? modifiedOn = GetDateTime(entity, "ModifiedOn");
			EntitySchemaColumnCollection columns = entity.Schema.Columns;
			Guid sessionId = columns.FindByName("SessionId") != null
				? entity.GetTypedColumnValue<Guid>("SessionId")
				: Guid.Empty;
			Guid indexId = columns.FindByName("IndexId") != null
				? entity.GetTypedColumnValue<Guid>("IndexId")
				: Guid.Empty;
			return new KnwIndexingSessionRecord {
				Id = entity.PrimaryColumnValue,
				KnwSourceId = knwSourceId,
				State = state,
				StartedOn = startedOn,
				TransferredOn = transferredOn,
				CompletedOn = completedOn,
				SessionId = sessionId,
				IndexId = indexId,
				ModifiedOn = modifiedOn
			};
		}

		private KnwIndexingSessionRecord FindBySourceAndSession(Guid knowledgeSourceId, Guid sessionId) {
			if (knowledgeSourceId == Guid.Empty || sessionId == Guid.Empty) {
				return null;
			}
			EntitySchemaQuery esq = GetSessionEntitySchemaQuery(includeOnlyActive: false);
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "KnwSource", knowledgeSourceId));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SessionId", sessionId));
			esq.RowCount = 1;
			Entity entity = esq.GetEntityCollection(_userConnection).FirstOrDefault();
			return entity == null ? null : ReadSessionRecord(entity);
		}

		private EntitySchemaQuery GetSessionEntitySchemaQuery(bool includeOnlyActive = true) {
			EntitySchema schema = GetSessionSchema();
			var esq = new EntitySchemaQuery(schema) {
				IgnoreDisplayValues = true,
				UnmaskColumnValues = true,
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn("KnwSource");
			esq.AddColumn("KnwIndexingStatus");
			esq.AddColumn("KnwIndexingStatus.Value");
			esq.AddColumn("StartedOn").OrderByDesc();
			esq.AddColumn("TransferredOn");
			esq.AddColumn("CompletedOn");
			esq.AddColumn("ModifiedOn");
			esq.AddColumn("SessionId");
			esq.AddColumn("IndexId");
			esq.AddColumn("IsCompleted");
			if (includeOnlyActive) {
				IEntitySchemaQueryFilterItem activeFilter = esq.CreateFilterWithParameters(FilterComparisonType.NotEqual,
					"IsCompleted", true);
				esq.Filters.Add(activeFilter);
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.NotEqual, "SessionId", Guid.Empty));
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.NotEqual, "IndexId", Guid.Empty));
			}
			return esq;
		}

		private DateTime? GetDateTime(Entity entity, string columnName) {
			object dateTimeObj = entity.GetColumnValue(columnName);
			if (dateTimeObj == null) {
				return null;
			}
			var dateTime = (DateTime)dateTimeObj;
			return TimeZoneUtilities.ConvertToUtc(_userConnection, dateTime);
		}

		private void EnsureStatusCacheInitialized() {
			if (Volatile.Read(ref _statusCacheState) == 1) {
				return;
			}
			if (Interlocked.CompareExchange(ref _statusCacheState, 1, 0) == 0) {
				EntitySchemaManager entitySchemaManager = _userConnection.EntitySchemaManager;
				EntitySchema schema = entitySchemaManager.GetInstanceByName(KnwIndexingStatusSchemaName);
				var esq = new EntitySchemaQuery(schema) {
					PrimaryQueryColumn = {
						IsAlwaysSelect = true
					}
				};
				esq.AddColumn("Value");
				EntityCollection collection = esq.GetEntityCollection(_userConnection);
				foreach (Entity entity in collection) {
					var value = (KnwIndexingStatusEnum)entity.GetTypedColumnValue<int>("Value");
					Guid id = entity.PrimaryColumnValue;
					_writeStatusCache.AddOrUpdate(value, id, (_, __) => id);
					_readStatusCache.AddOrUpdate(id, value, (_, __) => value);
				}
			} else {
				var spin = new SpinWait();
				while (Volatile.Read(ref _statusCacheState) != 1) {
					spin.SpinOnce();
				}
			}
		}

		private Guid GetKnowledgeSourceStatusId(KnwIndexingStatusEnum value) {
			EnsureStatusCacheInitialized();
			return _writeStatusCache.TryGetValue(value, out Guid id) ? id : Guid.Empty;
		}

		private KnwIndexingStatusEnum GetKnowledgeSourceStatusValue(Guid id) {
			EnsureStatusCacheInitialized();
			return _readStatusCache.TryGetValue(id, out KnwIndexingStatusEnum value) ? value : 0;
		}

		private void UpdateSessionInternal(KnwIndexingSessionRecord session, Entity entity) {
			Guid statusId = GetKnowledgeSourceStatusId(session.State);
			entity.SetColumnValue("KnwIndexingStatusId", statusId);
			entity.SetColumnValue("StartedOn", session.StartedOn);
			entity.SetColumnValue("TransferredOn", session.TransferredOn);
			entity.SetColumnValue("CompletedOn", session.CompletedOn);
			entity.SetColumnValue("SessionId", session.SessionId);
			entity.SetColumnValue("IndexId", session.IndexId);
			entity.SetColumnValue("ModifiedOn", session.ModifiedOn);
			if (session.State != KnwIndexingStatusEnum.Failed) {
				entity.SetColumnValue("Details", null);
			}
			entity.SetColumnValue("IsCompleted", session.State.IsTerminal());
			entity.Save(false);
		}

		private KnwIndexingSessionRecord CreateSession(KnwIndexingSessionRecord session) {
			EntitySchema schema = GetSessionSchema();
			Entity entity = schema.CreateEntity(_userConnection);
			entity.SetColumnValue("KnwSourceId", session.KnwSourceId);
			entity.SetColumnValue("KnwIndexingStatusId", GetKnowledgeSourceStatusId(session.State));
			entity.SetColumnValue("StartedOn", session.StartedOn);
			entity.SetColumnValue("TransferredOn", session.TransferredOn);
			entity.SetColumnValue("CompletedOn", session.CompletedOn);
			entity.SetColumnValue("ModifiedOn", session.ModifiedOn);
			entity.SetColumnValue("SessionId", session.SessionId);
			entity.SetColumnValue("IndexId", session.IndexId);
			entity.SetColumnValue("Details", null);
			entity.SetColumnValue("IsCompleted", session.State.IsTerminal());
			entity.Save(false);
			session.Id = entity.PrimaryColumnValue;
			return session;
		}

		private KnwIndexingSessionRecord UpsertSessionRecord(KnwIndexingSessionRecord sessionRecord) {
			KnwIndexingSessionRecord existingSession = FindBySourceAndSession(sessionRecord.KnwSourceId,
				sessionRecord.SessionId);
			if (existingSession != null) {
				sessionRecord.Id = existingSession.Id;
				Update(sessionRecord);
				return sessionRecord;
			}
			try {
				return CreateSession(sessionRecord);
			} catch (Exception) {
				KnwIndexingSessionRecord concurrentSession = FindBySourceAndSession(sessionRecord.KnwSourceId,
					sessionRecord.SessionId);
				if (concurrentSession == null) {
					throw;
				}
				sessionRecord.Id = concurrentSession.Id;
				Update(sessionRecord);
				return sessionRecord;
			}
		}

		private string TryCancelSessionOnService(KnwIndexingSessionRecord session, CancellationToken cancellationToken) {
			if (session.State == KnwIndexingStatusEnum.Indexing) {
				KnwIndexStateResponse response = ServiceClient.CancelIndexingProcess(session.IndexId,
					cancellationToken);
				return response.Status;
			}
			return ServiceClient.CancelSession(session.SessionId, cancellationToken);
		}

		private bool TryRecoverSessionStateFromAuthoritativeStatus(KnwIndexingSessionRecord session,
				string operationContext, out KnwIndexingStatusEnum recoveredState) {
			recoveredState = default(KnwIndexingStatusEnum);
			try {
				KnwKnowledgeSourceIndexingStatusResponse statusResponse =
					ServiceClient.GetKnowledgeSourceIndexingStatus(session.KnwSourceId, CancellationToken.None);
				ValidateAuthoritativeStatus(session.KnwSourceId, session.SessionId, session.IndexId, statusResponse);
				recoveredState = ResolveSessionState(statusResponse);
				try {
					SyncAuthoritativeSession(session.KnwSourceId, statusResponse);
				} catch (Exception syncException) {
					KnwLibUtils.Logger.Warn($"Failed to persist recovered {operationContext} status for session " +
						$"{session.SessionId}. {GetExceptionSummary(syncException)}", syncException);
				}
				return true;
			} catch (Exception recoveryException) {
				KnwLibUtils.Logger.Warn($"Failed to recover {operationContext} status from authoritative sync for " +
					$"session {session.SessionId}. {GetExceptionSummary(recoveryException)}", recoveryException);
				return false;
			}
		}

		private string FinalizeSessionWithRetry(KnwIndexingSessionRecord session, CancellationToken cancellationToken,
				out bool recoveredFromAuthoritativeStatus, out bool synchronizedAuthoritativeStatus) {
			recoveredFromAuthoritativeStatus = false;
			synchronizedAuthoritativeStatus = false;
			for (int attempt = 1; attempt <= FinalizeRetryCount + 1; attempt++) {
				try {
					return ServiceClient.FinalizeSession(session.SessionId, CancellationToken.None);
				} catch (Exception ex) {
					if (!IsCancellationLike(ex)) {
						throw;
					}
					KnwIndexingStatusEnum recoveredState;
					if (TryRecoverSessionStateFromAuthoritativeStatus(session, "finalize", out recoveredState)) {
						synchronizedAuthoritativeStatus = true;
						if (recoveredState.IsTerminal()) {
							recoveredFromAuthoritativeStatus = true;
							return recoveredState.ToString();
						}
						KnwLibUtils.Logger.Warn($"Finalize recovery for session {session.SessionId} resolved " +
							$"non-terminal state {recoveredState}. Retrying finalize.");
					}
					if (attempt > FinalizeRetryCount) {
						ExceptionDispatchInfo.Capture(ex).Throw();
						throw;
					}
					KnwLibUtils.Logger.Warn($"Finalize attempt {attempt} failed for session {session.SessionId} " +
						$"(source {session.KnwSourceId}). Retrying. {GetFinalizeFailureReason(ex, cancellationToken)}");
					CancellationToken.None.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
				}
			}
			throw new InvalidOperationException($"Finalize failed after {FinalizeRetryCount + 1} attempts.");
		}

		private KnwIndexingSessionRecord StartInternal(Guid knowledgeSourceId, KnwIndexingSessionTypeEnum sessionType,
				CancellationToken cancellationToken) {
			KnwStartSessionResponse sessionResponse;
			try {
				sessionResponse = ServiceClient.CreateIndexingSession(knowledgeSourceId, sessionType, cancellationToken);
			} catch (OperationCanceledException) {
				throw;
			} catch (Exception ex) {
				ServiceProblemHelper.LogTechnicalError($"Failed to create remote session for source {knowledgeSourceId}",
					ex);
				throw;
			}
			if (sessionResponse.SessionId == Guid.Empty) {
				throw new InvalidOperationException("Create session response sessionId is empty.");
			}
			if (sessionResponse.IndexId == Guid.Empty) {
				throw new InvalidOperationException("Create session response indexId is empty.");
			}
			KnwIndexingSessionRecord startedSession;
			try {
				startedSession = SyncCreatedSessionStatus(knowledgeSourceId, sessionResponse.SessionId, sessionResponse.IndexId,
					cancellationToken);
			} catch (Exception syncException) {
				TryCancelRemoteSession(knowledgeSourceId, sessionResponse.SessionId);
				try {
					SyncIndexingStatus(knowledgeSourceId, sessionResponse.SessionId, sessionResponse.IndexId,
						CancellationToken.None);
				} catch (Exception finalSyncException) {
					KnwLibUtils.Logger.Warn($"Failed to synchronize authoritative status after cancelling created session " +
						$"{sessionResponse.SessionId}. {GetExceptionSummary(finalSyncException)}", finalSyncException);
				}
				ServiceProblemHelper.LogTechnicalError(
					$"Failed to synchronize authoritative status for created session on source {knowledgeSourceId}",
					syncException);
				throw;
			}
			if (startedSession.State.IsTerminal()) {
				throw new KnwStartedSessionTerminalStateException(startedSession);
			}
			return startedSession;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public KnwIndexingSessionRecord Start(Guid knowledgeSourceId,
				KnwIndexingSessionTypeEnum sessionType = KnwIndexingSessionTypeEnum.Initial,
				CancellationToken cancellationToken = default) {
			if (knowledgeSourceId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			if (cancellationToken.IsCancellationRequested) {
				throw new OperationCanceledException("Indexing session start was cancelled before initiation.");
			}
			KnwLibUtils.Logger.Info($"Starting new indexing session for source {knowledgeSourceId}. " +
				$"SessionType={sessionType}.");
			EnsureNoActiveAuthoritativeSession(knowledgeSourceId, cancellationToken);
			return StartInternal(knowledgeSourceId, sessionType, cancellationToken);
		}

		/// <inheritdoc />
		public KnwIndexingSessionRecord StartAfterAuthoritativeSync(Guid knowledgeSourceId,
				KnwIndexingStatusSnapshot authoritativeStatus,
				KnwIndexingSessionTypeEnum sessionType = KnwIndexingSessionTypeEnum.Initial,
				CancellationToken cancellationToken = default) {
			if (knowledgeSourceId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			if (cancellationToken.IsCancellationRequested) {
				throw new OperationCanceledException("Indexing session start was cancelled before initiation.");
			}
			KnwLibUtils.Logger.Info($"Starting new indexing session for source {knowledgeSourceId} after " +
				$"authoritative resolve. SessionType={sessionType}.");
			ValidateAuthoritativeStatusSnapshot(knowledgeSourceId, authoritativeStatus);
			return StartInternal(knowledgeSourceId, sessionType, cancellationToken);
		}

		/// <inheritdoc />
		public KnwIndexingSessionRecord GetById(Guid id) {
			if (id == Guid.Empty) {
				return null;
			}
			EntitySchemaQuery esq = GetSessionEntitySchemaQuery(includeOnlyActive: false);
			IEntitySchemaQueryFilterItem sourceFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal,
				"Id", id);
			esq.Filters.Add(sourceFilter);
			esq.RowCount = 1;
			Entity entity = esq.GetEntityCollection(_userConnection).FirstOrDefault();
			return entity == null ? null : ReadSessionRecord(entity);
		}

		/// <inheritdoc />
		public IEnumerable<KnwIndexingSessionRecord> GetActiveSessions() {
			EntitySchemaQuery esq = GetSessionEntitySchemaQuery();
			return esq.GetEntityCollection(_userConnection).Select(ReadSessionRecord)
				.Where(HasRemoteIdentifiers);
		}

		/// <inheritdoc />
		public KnwIndexingStatusSnapshot ResolveLatestIndexingStatus(Guid knowledgeSourceId,
				CancellationToken cancellationToken = default) {
			if (knowledgeSourceId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			KnwKnowledgeSourceIndexingStatusResponse statusResponse =
				ServiceClient.GetKnowledgeSourceIndexingStatus(knowledgeSourceId, cancellationToken);
			ValidateLatestAuthoritativeStatus(knowledgeSourceId, statusResponse);
			return BuildStatusSnapshot(knowledgeSourceId, statusResponse);
		}

		/// <inheritdoc />
		public KnwIndexingSessionRecord SyncLatestIndexingStatus(Guid knowledgeSourceId,
				CancellationToken cancellationToken = default) {
			KnwIndexingStatusSnapshot statusSnapshot = ResolveLatestIndexingStatus(knowledgeSourceId, cancellationToken);
			return PersistStatusSnapshot(statusSnapshot);
		}

		/// <inheritdoc />
		public KnwIndexingSessionSnapshot EnsureNoActiveAuthoritativeSession(Guid knowledgeSourceId,
				CancellationToken cancellationToken = default) {
			KnwIndexingStatusSnapshot statusSnapshot = ResolveLatestIndexingStatus(knowledgeSourceId, cancellationToken);
			ValidateAuthoritativeStatusSnapshot(knowledgeSourceId, statusSnapshot);
			return statusSnapshot.LatestSession;
		}

		/// <inheritdoc />
		public void Update(KnwIndexingSessionRecord session) {
			if (session == null) {
				throw new ArgumentNullOrEmptyException(nameof(session));
			}
			if (session.KnwSourceId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(session.KnwSourceId));
			}
			EntitySchema schema = GetSessionSchema();
			Entity entity = schema.CreateEntity(_userConnection);
			if (!entity.FetchFromDB(session.Id)) {
				throw new InvalidOperationException($"KnwIndexingSession entity not found by Id {session.Id}");
			}
			UpdateSessionInternal(session, entity);
		}

		/// <inheritdoc />
		public bool UploadItem(KnwIndexingSessionRecord session, KnwItem item, CancellationToken cancellationToken) {
			if (session == null) {
				throw new ArgumentNullOrEmptyException(nameof(session));
			}
			if (item == null) {
				throw new ArgumentNullOrEmptyException(nameof(item));
			}
			if (session.SessionId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(session.SessionId));
			}
			return ServiceClient.UploadItem(session.SessionId, item, cancellationToken);
		}

		/// <inheritdoc />
		public string FinalizeTransfer(KnwIndexingSessionRecord session, CancellationToken cancellationToken) {
			if (session == null) {
				throw new ArgumentNullOrEmptyException(nameof(session));
			}
			if (session.SessionId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(session.SessionId));
			}
			try {
				bool recoveredFromAuthoritativeStatus;
				bool synchronizedAuthoritativeStatus;
				string status = FinalizeSessionWithRetry(session, cancellationToken, out recoveredFromAuthoritativeStatus,
					out synchronizedAuthoritativeStatus);
				if (!recoveredFromAuthoritativeStatus && !synchronizedAuthoritativeStatus) {
					try {
						SyncIndexingStatus(session.KnwSourceId, session.SessionId, session.IndexId, CancellationToken.None);
					} catch (Exception syncException) {
						KnwLibUtils.Logger.Warn($"Failed to refresh authoritative status after finalize for session " +
							$"{session.SessionId}. {GetExceptionSummary(syncException)}", syncException);
					}
				}
				return status;
			} catch (Exception ex) {
				if (!IsCancellationLike(ex)) {
					KnwLibUtils.Logger.Error($"Finalize request failed for session {session.SessionId} " +
						$"(source {session.KnwSourceId}). {GetFinalizeFailureReason(ex, cancellationToken)}", ex);
				}
				throw;
			}
		}

		/// <inheritdoc />
		public KnwIndexingSessionRecord SyncIndexingStatus(Guid knowledgeSourceId, Guid sessionId, Guid indexId,
				CancellationToken cancellationToken = default) {
			if (knowledgeSourceId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			if (sessionId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(sessionId));
			}
			if (indexId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(indexId));
			}
			KnwKnowledgeSourceIndexingStatusResponse statusResponse =
				ServiceClient.GetKnowledgeSourceIndexingStatus(knowledgeSourceId, cancellationToken);
			ValidateAuthoritativeStatus(knowledgeSourceId, sessionId, indexId, statusResponse);
			return SyncAuthoritativeSession(knowledgeSourceId, statusResponse);
		}

		private KnwIndexingSessionRecord SyncCreatedSessionStatus(Guid knowledgeSourceId, Guid sessionId, Guid indexId,
				CancellationToken cancellationToken) {
			Exception lastException = null;
			for (int attempt = 1; attempt <= StartSyncRetryCount; attempt++) {
				try {
					cancellationToken.ThrowIfCancellationRequested();
					return SyncIndexingStatus(knowledgeSourceId, sessionId, indexId, cancellationToken);
				} catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
					throw;
				} catch (Exception ex) {
					lastException = ex;
					if (attempt == StartSyncRetryCount) {
						break;
					}
					KnwLibUtils.Logger.Warn($"Authoritative indexing status is not ready yet for created session " +
						$"{sessionId} (source {knowledgeSourceId}, attempt {attempt}/{StartSyncRetryCount}). " +
						$"{GetExceptionSummary(ex)}", ex);
					if (cancellationToken.WaitHandle.WaitOne(StartSyncRetryDelay)) {
						throw new OperationCanceledException(cancellationToken);
					}
				}
			}
			throw new InvalidOperationException($"Failed to synchronize authoritative status for created session {sessionId}.",
				lastException);
		}

		/// <inheritdoc />
		public string CancelSession(KnwIndexingSessionRecord session, CancellationToken cancellationToken) {
			if (session == null) {
				throw new ArgumentNullOrEmptyException(nameof(session));
			}
			if (session.SessionId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(session.SessionId));
			}
			try {
				string status = TryCancelSessionOnService(session, cancellationToken);
				try {
					SyncIndexingStatus(session.KnwSourceId, session.SessionId, session.IndexId, CancellationToken.None);
				} catch (Exception ex) {
					KnwLibUtils.Logger.Warn($"Failed to refresh authoritative cancellation status for session " +
						$"{session.SessionId}. {GetExceptionSummary(ex)}", ex);
				}
				return status;
			} catch (Exception ex) {
				KnwLibUtils.Logger.Error(
					$"Error cancelling session {session.SessionId} for source {session.KnwSourceId}: {ex.Message}", ex);
				KnwIndexingStatusEnum recoveredState;
				if (TryRecoverSessionStateFromAuthoritativeStatus(session, "cancellation", out recoveredState)) {
					return recoveredState.ToString();
				}
				ExceptionDispatchInfo.Capture(ex).Throw();
				throw;
			}
		}

		/// <inheritdoc />
		public void SetSessionFailureDetails(Guid sessionId, string errorCode, string fallbackMessage) {
			if (sessionId == Guid.Empty) {
				throw new ArgumentNullOrEmptyException(nameof(sessionId));
			}
			try {
				string sessionDetails = KnwExternalApiMessage.ResolveLocalizedMessage(_userConnection, errorCode,
					fallbackMessage);
				EntitySchema sessionSchema = GetSessionSchema();
				Entity sessionEntity = sessionSchema.CreateEntity(_userConnection);
				if (sessionEntity.FetchFromDB(sessionId)) {
					sessionEntity.SetColumnValue("Details", sessionDetails);
					sessionEntity.Save(false);
				}
			} catch (Exception ex) {
				KnwLibUtils.Logger.Warn($"Failed to set indexing session details for session {sessionId}.", ex);
			}
		}

		#endregion

	}

	#endregion

}

