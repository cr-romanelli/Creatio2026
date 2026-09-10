namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	#region Interface: IKnwIndexingSessionManager

	public interface IKnwIndexingSessionManager
	{
		/// <summary>
		/// Starts a new indexing session by creating a remote session and persisting local record.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="sessionType">Requested indexing session type.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Created indexing session record.</returns>
		KnwIndexingSessionRecord Start(Guid knowledgeSourceId,
			KnwIndexingSessionTypeEnum sessionType = KnwIndexingSessionTypeEnum.Initial,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Starts a new indexing session after the caller has already resolved authoritative state.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="authoritativeStatus">Authoritative status snapshot resolved without local persistence.</param>
		/// <param name="sessionType">Requested indexing session type.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Created indexing session record.</returns>
		KnwIndexingSessionRecord StartAfterAuthoritativeSync(Guid knowledgeSourceId,
			KnwIndexingStatusSnapshot authoritativeStatus,
			KnwIndexingSessionTypeEnum sessionType = KnwIndexingSessionTypeEnum.Initial,
			CancellationToken cancellationToken = default);


		/// <summary>
		/// Gets the indexing session record by its identifier.
		/// </summary>
		/// <param name="sessionId">Identifier of the session</param>
		/// <returns>The indexing session record.</returns>
		KnwIndexingSessionRecord GetById(Guid sessionId);

		/// <summary>
		/// Retrieves local active indexing session records as candidates for authoritative synchronization.
		/// </summary>
		/// <returns>A collection of local active indexing session records.</returns>
		IEnumerable<KnwIndexingSessionRecord> GetActiveSessions();

		/// <summary>
		/// Resolves latest authoritative knowledge-source indexing status without local persistence.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Authoritative indexing status snapshot.</returns>
		KnwIndexingStatusSnapshot ResolveLatestIndexingStatus(Guid knowledgeSourceId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Refreshes local source/session records from the latest authoritative knowledge-source indexing status.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Refreshed latest indexing session record.</returns>
		KnwIndexingSessionRecord SyncLatestIndexingStatus(Guid knowledgeSourceId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Resolves authoritative state and throws when the latest resolved session is active.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Latest authoritative session snapshot when the latest session is terminal.</returns>
		KnwIndexingSessionSnapshot EnsureNoActiveAuthoritativeSession(Guid knowledgeSourceId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates the specified indexing session record in the local store or remote service.
		/// </summary>
		/// <param name="session">The indexing session record to update.</param>
		void Update(KnwIndexingSessionRecord session);

		/// <summary>
		/// Uploads a knowledge item stream to the active indexing session.
		/// </summary>
		/// <param name="session">Session record.</param>
		/// <param name="item">Knowledge item.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>True if remote service acknowledged the upload.</returns>
		bool UploadItem(KnwIndexingSessionRecord session, KnwItem item, CancellationToken cancellationToken);

		/// <summary>
		/// Finalizes the session data transfer phase on remote service.
		/// </summary>
		/// <param name="session">Session record.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Status string provided by the service.</returns>
		string FinalizeTransfer(KnwIndexingSessionRecord session, CancellationToken cancellationToken);

		/// <summary>
		/// Refreshes local source/session records from the authoritative knowledge-source indexing status endpoint.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="sessionId">Expected remote session identifier.</param>
		/// <param name="indexId">Expected remote index identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Refreshed indexing session record.</returns>
		KnwIndexingSessionRecord SyncIndexingStatus(Guid knowledgeSourceId, Guid sessionId, Guid indexId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Cancels the indexing session on remote service and refreshes local state from the
		/// authoritative knowledge-source indexing status endpoint.
		/// </summary>
		/// <param name="session">Session record.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Status string returned by the remote cancel endpoint.</returns>
		string CancelSession(KnwIndexingSessionRecord session, CancellationToken cancellationToken);

		/// <summary>
		/// Stores user-facing failure details for the specified indexing session.
		/// </summary>
		/// <param name="sessionId">Session identifier.</param>
		/// <param name="errorCode">Optional machine-readable error code.</param>
		/// <param name="fallbackMessage">Fallback message when localized message is not available.</param>
		void SetSessionFailureDetails(Guid sessionId, string errorCode, string fallbackMessage);

	}

	#endregion

}

