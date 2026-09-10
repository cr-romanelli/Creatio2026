namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwIndexingWorker

	/// <summary>
	/// Defines the contract for indexing worker operations.
	/// </summary>
	public interface IKnwIndexingWorker
	{

		#region Methods: Public

		/// <summary>
		/// Processes the indexing command using the provided user connection context.
		/// </summary>
		/// <param name="userConnection">The user connection context.</param>
		/// <param name="message">The indexing command message to process.</param>
		void Process(UserConnection userConnection, KnwStartIndexingCommand message);

		#endregion

	}

	#endregion

	#region Class: KnwIndexingWorker

	/// <summary>
	/// Default implementation of <see cref="IKnwIndexingWorker"/> for processing indexing commands.
	/// </summary>
	[DefaultBinding(typeof(IKnwIndexingWorker))]
	internal class KnwIndexingWorker : IKnwIndexingWorker
	{

		#region Properties: Private

		private IKnwIndexingCancellationManager _cancellationManager;
		private IKnwIndexingCancellationManager CancellationManager => _cancellationManager ?? (_cancellationManager =
			ClassFactory.Get<IKnwIndexingCancellationManager>());
		private IKnwServiceProblemHelper _serviceProblemHelper;
		private IKnwServiceProblemHelper ServiceProblemHelper => _serviceProblemHelper ?? (_serviceProblemHelper =
			ClassFactory.Get<IKnwServiceProblemHelper>());

		#endregion

		#region Methods: Private

		private static IKnwProvider GetProvider(UserConnection userConnection, KnwSourceRecord knwSource) {
			var providerFactory = ClassFactory.Get<IKnwProviderFactory>();
			var options = new KnwProviderInitializationOptions(userConnection, knwSource);
			return providerFactory.CreateKnwProvider(options);
		}

		private void TryCancelSession(KnwIndexingSessionRecord currentSession, IKnwIndexingSessionManager sessionManager) {
			if (currentSession == null || currentSession.SessionId == Guid.Empty || currentSession.IndexId == Guid.Empty
					|| currentSession.State.IsTerminal()) {
				return;
			}
			try {
				sessionManager.CancelSession(currentSession, CancellationToken.None);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error($"Failed to cancel session {currentSession.SessionId}: {e.Message}", e);
			}
		}

		private void TrySyncSessionStatus(KnwIndexingSessionRecord currentSession,
				IKnwIndexingSessionManager sessionManager) {
			if (currentSession == null || currentSession.SessionId == Guid.Empty || currentSession.IndexId == Guid.Empty) {
				return;
			}
			try {
				sessionManager.SyncIndexingStatus(currentSession.KnwSourceId, currentSession.SessionId,
					currentSession.IndexId, CancellationToken.None);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error($"Failed to synchronize authoritative status for session " +
					$"{currentSession.SessionId}: {e.Message}", e);
			}
		}

		private void ProcessWithBatchProvider(
					IKnwBatchProvider batchProvider,
					KnwSourceRecord knwSource,
					KnwIndexingSessionRecord currentSession,
					KnwProviderDiscoveryOptions options,
					IKnwIndexingSessionManager sessionManager,
					CancellationToken cancellationToken) {
			int uploadedCount = 0;
			int failedCount = 0;
			int batchNumber = 0;
			List<IReadOnlyList<KnwItem>> batches =
				batchProvider.DiscoverInBatches(options, cancellationToken)?.ToList() ??
				new List<IReadOnlyList<KnwItem>>();
			int totalBatches = batches.Count;
			foreach (IReadOnlyList<KnwItem> batch in batches) {
				batchNumber++;
				KnwLibUtils.Logger.Info($"Processing batch {batchNumber}/{totalBatches} with {batch.Count} items " +
					$"(batch size {batch.Count}) from source ID: {knwSource.Id}");
				foreach (KnwItem document in batch) {
					cancellationToken.ThrowIfCancellationRequested();
					try {
						if (document == null) {
							continue;
						}
						KnwLibUtils.Logger.Debug($"Indexing document ID: {document.Id} from source ID: {knwSource.Id}");
						bool acknowledged =
							sessionManager.UploadItem(currentSession, document, cancellationToken);
						if (acknowledged) {
							uploadedCount++;
						} else {
							failedCount++;
							KnwLibUtils.Logger.Warn($"Upload not acknowledged for document ID {document.Id} " +
								$"(session {currentSession.SessionId}).");
						}
					} catch (Exception docEx) {
						if (ServiceProblemHelper.IsTerminalUploadFailure(docEx)) {
							throw;
						}
						failedCount++;
						ServiceProblemHelper.LogTechnicalError($"Failed to upload document ID {document?.Id} in session " +
							$"{currentSession.SessionId}", docEx);
						ServiceProblemHelper.TrySetSessionFailureDetails(sessionManager, currentSession, docEx,
							$"Failed to upload document ID {document?.Id}",
							"Failed to set indexing session details after upload failure");
					}
				}
			}
			string finalizeStatus = sessionManager.FinalizeTransfer(currentSession, cancellationToken);
			KnwLibUtils.Logger.Info($"Finalize status for session {currentSession.SessionId}: {finalizeStatus}. " +
				$"TotalBatches={batchNumber}, TotalUploaded={uploadedCount}, TotalFailed={failedCount}");
		}

		private void ProcessWithStandardProvider(
					IKnwProvider provider,
					KnwSourceRecord knwSource,
					KnwIndexingSessionRecord currentSession,
					KnwProviderDiscoveryOptions options,
					IKnwIndexingSessionManager sessionManager,
					CancellationToken cancellationToken) {
			int uploadedCount = 0;
			int failedCount = 0;
			foreach (KnwItem document in provider.Discover(options, cancellationToken)) {
				cancellationToken.ThrowIfCancellationRequested();
				try {
					if (document == null) {
						continue;
					}
					KnwLibUtils.Logger.Debug($"Indexing document ID: {document.Id} from source ID: {knwSource.Id}");
					bool acknowledged =
						sessionManager.UploadItem(currentSession, document, cancellationToken);
					if (acknowledged) {
						uploadedCount++;
					} else {
						failedCount++;
						KnwLibUtils.Logger.Warn($"Upload not acknowledged for document ID {document.Id} " +
							$"(session {currentSession.SessionId}).");
					}
				} catch (Exception docEx) {
					if (ServiceProblemHelper.IsTerminalUploadFailure(docEx)) {
						throw;
					}
					failedCount++;
					ServiceProblemHelper.LogTechnicalError($"Failed to upload document ID {document?.Id} in session " +
						$"{currentSession.SessionId}", docEx);
					ServiceProblemHelper.TrySetSessionFailureDetails(sessionManager, currentSession, docEx,
						$"Failed to upload document ID {document?.Id}",
						"Failed to set indexing session details after upload failure");
				}
			}
			string finalizeStatus =
				sessionManager.FinalizeTransfer(currentSession, cancellationToken);
			KnwLibUtils.Logger.Info($"Finalize status for session {currentSession.SessionId}: {finalizeStatus}. " +
			$"Uploaded={uploadedCount}, Failed={failedCount}");
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public void Process(UserConnection userConnection, KnwStartIndexingCommand message) {
			IKnwIndexingSessionManager sessionManager = ClassFactory.Get<IKnwIndexingSessionManager>(
				new ConstructorArgument("userConnection", userConnection));
			KnwIndexingSessionRecord currentSession = null;
			KnwSourceRecord knwSource = null;
			CancellationToken activeCancellationToken = CancellationToken.None;
			try {
				IKnwSourceManager sourceManager = ClassFactory.Get<IKnwSourceManager>(
					new ConstructorArgument("userConnection", userConnection));
				knwSource = sourceManager.GetById(message.KnwSourceId);
				if (knwSource == null) {
					KnwLibUtils.Logger.Error($"Knowledge source with ID {message.KnwSourceId} not found.");
					return;
				}
				knwSource.Validate();
				IKnwProvider provider = GetProvider(userConnection, knwSource);
				if (provider == null) {
					KnwLibUtils.Logger.Error($"Knowledge provider with code {knwSource.KnwProvider.Code} not found");
					return;
				}
				KnwIndexingSessionTypeEnum sessionType = knwSource.LastIndexedOn.HasValue
					? KnwIndexingSessionTypeEnum.Incremental
					: KnwIndexingSessionTypeEnum.Initial;

				currentSession = sessionManager.Start(knwSource.Id, sessionType, CancellationToken.None);
				if (currentSession == null) {
					KnwLibUtils.Logger.Error("Failed to start a new indexing session.");
					return;
				}
				using (var cancellationContext = CancellationManager.RegisterCancellation(currentSession.Id)) {
					var cancellationToken = cancellationContext.CancellationToken;
					activeCancellationToken = cancellationToken;
					var options = new KnwProviderDiscoveryOptions {
						ModifiedSince = null // TODO change to knwSource.LastIndexedOn once supported by microservice
					};
					if (provider is IKnwBatchProvider batchProvider) {
						KnwLibUtils.Logger.Info($"Using batch processing for provider {provider.GetType().Name}");
						ProcessWithBatchProvider(batchProvider, knwSource, currentSession, options,
							sessionManager, cancellationToken);
					} else {
						KnwLibUtils.Logger.Info($"Using standard processing for provider {provider.GetType().Name}");
						ProcessWithStandardProvider(provider, knwSource, currentSession, options, sessionManager, cancellationToken);
					}
				}
			} catch (KnwActiveIndexingSessionExistsException ex) {
				KnwIndexingSessionSnapshot activeSession = ex.Session;
				string sessionInfo = activeSession == null
					? "Authoritative status reports active indexing work."
					: $"Active session {activeSession.SessionId} is already in progress with state {activeSession.State}.";
				KnwLibUtils.Logger.Warn(
					$"Knowledge source with ID {message.KnwSourceId} is already being indexed. {sessionInfo}");
			} catch (KnwStartedSessionTerminalStateException ex) {
				KnwIndexingSessionRecord session = ex.Session;
				KnwLibUtils.Logger.Error(session == null
					? $"Newly started session for knowledge source {message.KnwSourceId} resolved to terminal state."
					: $"Newly started session {session.SessionId} is in terminal state {session.State}.");
			} catch (OperationCanceledException ex) {
				string innerType = ex.InnerException?.GetType().Name ?? "none";
				string innerMessage = ex.InnerException?.Message ?? "none";
				KnwLibUtils.Logger.Info($"Indexing was cancelled for knowledge source ID {message.KnwSourceId}. " +
					$"TokenCancellationRequested={activeCancellationToken.IsCancellationRequested}, " +
				$"ExceptionType={ex.GetType().Name}, Message={ex.Message}, " +
					$"InnerType={innerType}, InnerMessage={innerMessage}");
				if (currentSession != null) {
					TryCancelSession(currentSession, sessionManager);
				}
			} catch (Exception e) {
				ServiceProblemHelper.LogTechnicalError(
					$"Error processing knowledge source ID {message.KnwSourceId}", e);
				ServiceProblemHelper.TrySetSessionFailureDetails(sessionManager, currentSession, e,
					$"Indexing failed for source {message.KnwSourceId}",
					"Failed to set indexing session details after source processing error");
				TrySyncSessionStatus(currentSession, sessionManager);
			}
		}

		#endregion

	}

	#endregion

}

