namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	#region Interface: IKnwServiceClient

	/// <summary>
	/// Defines methods for interacting with the knowledge base service client.
	/// </summary>
	internal interface IKnwServiceClient
	{

		#region Methods: Internal

		/// <summary>
		/// Retrieves knowledge base items by their IDs using the specified search request.
		/// </summary>
		/// <param name="request">The search request containing item IDs and other parameters.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a collection of knowledge base item responses.</returns>
		IEnumerable<KnwBaseItemResponse> GetKnowledgeBaseItemsByIds(KnwSearchQuery request,
			CancellationToken cancellationToken);

		/// <summary>
		/// Creates a new indexing session for a knowledge source.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source unique identifier.</param>
		/// <param name="sessionType">Indexing session type.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Session create response containing session and index ids.</returns>
		KnwStartSessionResponse CreateIndexingSession(Guid knowledgeSourceId, KnwIndexingSessionTypeEnum sessionType,
			CancellationToken cancellationToken);

		/// <summary>
		/// Uploads a knowledge item to the remote indexing session.
		/// Large files are automatically chunked according to the KnowledgeServiceUploadChunkSize system setting.
		/// Each chunk is sent with an incrementing sequence number and includes a hash for validation.
		/// Failed chunks are automatically retried up to 2 times before the operation fails.
		/// </summary>
		/// <param name="sessionId">Session identifier returned by create.</param>
		/// <param name="item">Knowledge item to transfer.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>True if all chunks were acknowledged; otherwise, false.</returns>
		bool UploadItem(Guid sessionId, KnwItem item, CancellationToken cancellationToken);

		/// <summary>
		/// Finalizes a remote indexing session transfer phase.
		/// </summary>
		/// <param name="sessionId">Session identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Status string returned by the service.</returns>
		string FinalizeSession(Guid sessionId, CancellationToken cancellationToken);

		/// <summary>
		/// Cancels a remote indexing session.
		/// </summary>
		/// <param name="sessionId">Session identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Status string returned by the service.</returns>
		string CancelSession(Guid sessionId, CancellationToken cancellationToken);

		/// <summary>
		/// Cancels an active indexing process on the remote index.
		/// </summary>
		/// <param name="indexId">Index identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// Returns <see cref="KnwIndexStateResponse"/> object containing status and progress.
		/// </returns>
		KnwIndexStateResponse CancelIndexingProcess(Guid indexId, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the information of an indexing session by its session ID.
		/// </summary>
		/// <param name="sessionId">The unique identifier of the session.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// Returns <see cref="KnwSessionResponse"/> object containing session details.
		/// </returns>
		KnwSessionResponse GetSession(Guid sessionId, CancellationToken cancellationToken);

		/// <summary>
		/// Deletes multiple files from the knowledge source.
		/// </summary>
		/// <param name="knowledgeSourceId">The unique identifier of the knowledge source.</param>
		/// <param name="fileIds">The list of file identifiers to delete.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		void DeleteFiles(Guid knowledgeSourceId, List<string> fileIds, CancellationToken cancellationToken);

		/// <summary>
		/// Invalidates a knowledge source index snapshot.
		/// </summary>
		/// <param name="knowledgeSourceId">The unique identifier of the knowledge source.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		void InvalidateKnowledgeSource(Guid knowledgeSourceId, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the state of an indexing operation by its index ID.
		/// </summary>
		/// <param name="indexId">The unique identifier of the index.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// Returns <see cref="KnwIndexStateResponse"/> object containing progress and status.
		/// </returns>
		KnwIndexStateResponse GetIndexState(Guid indexId, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the authoritative indexing status for a knowledge source.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Authoritative indexing status snapshot.</returns>
		KnwKnowledgeSourceIndexingStatusResponse GetKnowledgeSourceIndexingStatus(Guid knowledgeSourceId,
			CancellationToken cancellationToken);

		/// <summary>
		/// Resolves the effective configuration for a specific knowledge source.
		/// </summary>
		/// <param name="knowledgeSourceId">Knowledge source identifier.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Resolved configuration response.</returns>
		Task<ConfigurationScopeResponse> GetResolvedConfigurationAsync(Guid knowledgeSourceId,
			CancellationToken ct = default);

		/// <summary>
		/// Creates a new configuration scope.
		/// </summary>
		/// <param name="request">Configuration scope request.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Created configuration response.</returns>
		Task<ConfigurationScopeResponse> CreateConfigurationAsync(ConfigurationScopeRequest request,
			CancellationToken ct = default);

		/// <summary>
		/// Updates an existing configuration scope.
		/// </summary>
		/// <param name="id">Configuration identifier.</param>
		/// <param name="request">Configuration update request.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Updated configuration response.</returns>
		Task<ConfigurationScopeResponse> UpdateConfigurationAsync(Guid id, ConfigurationScopeRequest request,
			CancellationToken ct = default);

		/// <summary>
		/// Deletes an existing configuration scope.
		/// </summary>
		/// <param name="id">Configuration identifier.</param>
		/// <param name="ct">Cancellation token.</param>
		Task DeleteConfigurationAsync(Guid id, CancellationToken ct = default);

		/// <summary>
		/// Lists configuration scopes with optional knowledge source filtering.
		/// </summary>
		/// <param name="knowledgeSourceId">Optional knowledge source identifier filter.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Configuration scope collection.</returns>
		Task<IEnumerable<ConfigurationScopeResponse>> ListConfigurationsAsync(Guid? knowledgeSourceId = null,
			CancellationToken ct = default);

		/// <summary>
		/// Verifies that the knowledge management microservice is reachable and healthy.
		/// Throws if the service is unreachable, returns a non-success status, or the base URL is not configured.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		void CheckServiceHealth(CancellationToken cancellationToken);

		#endregion

	}

	#endregion

}

