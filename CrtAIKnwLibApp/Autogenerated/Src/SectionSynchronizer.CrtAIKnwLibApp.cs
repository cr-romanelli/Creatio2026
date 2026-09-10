namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;

	#region Interface: ISectionSynchronizer

	/// <summary>
	/// Performs synchronization of section changes for a single knowledge source.
	/// </summary>
	internal interface ISectionSynchronizer
	{

		#region Methods: Internal

		SectionSynchronizerResult Synchronize(SectionSynchronizerRequest request, CancellationToken cancellationToken = default);

		#endregion

	}

	#endregion

	#region Class: SectionSynchronizerRequest

	/// <summary>
	/// Defines parameters for section synchronization.
	/// </summary>
	internal sealed class SectionSynchronizerRequest
	{

		#region Properties: Public

		public UserConnection UserConnection { get; set; }

		public Guid SourceId { get; set; }

		public IReadOnlyCollection<KnwSrcOutboxRecord> SourceEvents { get; set; }

		public int BatchSize { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionSynchronizerResult

	/// <summary>
	/// Describes synchronization outcome for source-level execution.
	/// </summary>
	internal sealed class SectionSynchronizerResult
	{

		#region Properties: Public

		public bool IsSuccess { get; set; }

		public bool IsSkipped { get; set; }

		public bool IsTransientFailure { get; set; }

		public string Error { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionSynchronizer

	/// <summary>
	/// Default implementation of section synchronization.
	/// </summary>
	[DefaultBinding(typeof(ISectionSynchronizer))]
	internal class SectionSynchronizer : ISectionSynchronizer
	{

		#region Properties: Internal

		internal IKnwSourceManager SourceManager { get; set; }

		internal IKnwIndexingSessionManager SessionManager { get; set; }

		internal IKnwProviderFactory ProviderFactory { get; set; }

		internal IKnwServiceClient ServiceClient { get; set; }

		internal IKnwServiceProblemHelper ServiceProblemHelper { get; set; }

		#endregion

		#region Methods: Private

		private IKnwSourceManager ResolveSourceManager(UserConnection userConnection) {
			return SourceManager ?? ClassFactory.Get<IKnwSourceManager>(
				new ConstructorArgument("userConnection", userConnection));
		}

		private IKnwIndexingSessionManager ResolveSessionManager(UserConnection userConnection) {
			return SessionManager ?? ClassFactory.Get<IKnwIndexingSessionManager>(
				new ConstructorArgument("userConnection", userConnection));
		}

		private IKnwProviderFactory ResolveProviderFactory() {
			return ProviderFactory ?? ClassFactory.Get<IKnwProviderFactory>();
		}

		private IKnwServiceClient ResolveServiceClient(UserConnection userConnection) {
			return ServiceClient ?? ClassFactory.Get<IKnwServiceClient>(
				new ConstructorArgument("userConnection", userConnection),
				new ConstructorArgument("httpRequestClient", ClassFactory.Get<IHttpRequestClient>()));
		}

		private IKnwServiceProblemHelper ResolveServiceProblemHelper() {
			return ServiceProblemHelper ?? ClassFactory.Get<IKnwServiceProblemHelper>();
		}

		private static HashSet<string> BuildUpsertIds(IReadOnlyCollection<KnwSrcOutboxRecord> sourceEvents) {
			var upsertIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (KnwSrcOutboxRecord sourceEvent in sourceEvents) {
				if (sourceEvent.EventType == KnwSrcOutboxEventType.Create
						|| sourceEvent.EventType == KnwSrcOutboxEventType.Update) {
					upsertIds.Add(sourceEvent.RecordId.ToString("D"));
				}
			}
			return upsertIds;
		}

		private static HashSet<string> BuildDeleteIds(IReadOnlyCollection<KnwSrcOutboxRecord> sourceEvents) {
			var deleteIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (KnwSrcOutboxRecord sourceEvent in sourceEvents) {
				if (sourceEvent.EventType == KnwSrcOutboxEventType.Delete) {
					deleteIds.Add(sourceEvent.RecordId.ToString("D"));
				}
			}
			return deleteIds;
		}

		private static IEnumerable<List<string>> Chunk(ICollection<string> source, int size) {
			var chunk = new List<string>(size);
			foreach (string value in source) {
				chunk.Add(value);
				if (chunk.Count >= size) {
					yield return chunk;
					chunk = new List<string>(size);
				}
			}
			if (chunk.Count > 0) {
				yield return chunk;
			}
		}

		private static void TryCancelSession(KnwIndexingSessionRecord currentSession,
				IKnwIndexingSessionManager sessionManager) {
			if (currentSession == null || currentSession.SessionId == Guid.Empty || currentSession.IndexId == Guid.Empty
					|| currentSession.State.IsTerminal()) {
				return;
			}
			try {
				sessionManager.CancelSession(currentSession, CancellationToken.None);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error(
					$"Failed to cancel session {currentSession.SessionId} during section synchronization: {e.Message}", e);
			}
		}

		private static void TrySyncSessionStatus(KnwIndexingSessionRecord currentSession,
				IKnwIndexingSessionManager sessionManager) {
			if (currentSession == null || currentSession.SessionId == Guid.Empty || currentSession.IndexId == Guid.Empty) {
				return;
			}
			try {
				sessionManager.SyncIndexingStatus(currentSession.KnwSourceId, currentSession.SessionId,
					currentSession.IndexId, CancellationToken.None);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error($"Failed to synchronize authoritative status for section sync session " +
					$"{currentSession.SessionId}: {e.Message}", e);
			}
		}

		private static string BuildDetailedErrorMessage(Exception exception) {
			if (exception == null) {
				return "Synchronization failed";
			}
			Exception current = exception;
			if (current is AggregateException aggregateException) {
				AggregateException flattened = aggregateException.Flatten();
				if (flattened.InnerExceptions.Count == 1 && flattened.InnerExceptions[0] != null) {
					current = flattened.InnerExceptions[0];
				}
			}
			while (current.InnerException != null) {
				current = current.InnerException;
			}
			string message = string.IsNullOrWhiteSpace(current.Message)
				? "Synchronization failed"
				: current.Message;
			return $"{current.GetType().Name}: {message}";
		}

		private sealed class UploadStatistics
		{
			public int Uploaded { get; set; }

			public int Failed { get; set; }
		}

		private UploadStatistics UploadDiscoveredItems(IEnumerable<KnwItem> items,
				IKnwIndexingSessionManager sessionManager,
				KnwIndexingSessionRecord currentSession,
				IKnwServiceProblemHelper serviceProblemHelper,
				ISet<string> discoveredIds,
				CancellationToken cancellationToken) {
			var statistics = new UploadStatistics();
			if (items == null) {
				return statistics;
			}
			foreach (KnwItem item in items) {
				cancellationToken.ThrowIfCancellationRequested();
				if (item == null || string.IsNullOrWhiteSpace(item.Id)) {
					continue;
				}
				discoveredIds.Add(item.Id);
				try {
					bool acknowledged = sessionManager.UploadItem(currentSession, item, cancellationToken);
					if (acknowledged) {
						statistics.Uploaded++;
					} else {
						statistics.Failed++;
						KnwLibUtils.Logger.Warn($"Upload not acknowledged for item ID {item.Id} " +
							$"(session {currentSession.SessionId}, source {currentSession.KnwSourceId}).");
					}
				} catch (Exception uploadEx) {
					if (serviceProblemHelper.IsTerminalUploadFailure(uploadEx)) {
						throw;
					}
					statistics.Failed++;
					serviceProblemHelper.LogTechnicalError($"Failed to upload item ID {item.Id} " +
						$"(session {currentSession.SessionId}, source {currentSession.KnwSourceId})", uploadEx);
					serviceProblemHelper.TrySetSessionFailureDetails(sessionManager, currentSession, uploadEx,
						$"Failed to upload item ID {item.Id}",
						"Failed to set synchronization details after upload failure");
				}
			}
			return statistics;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public SectionSynchronizerResult Synchronize(SectionSynchronizerRequest request,
				CancellationToken cancellationToken = default) {
			if (request == null) {
				return new SectionSynchronizerResult {
					IsTransientFailure = false,
					Error = "Synchronization request is null"
				};
			}
			UserConnection userConnection = request.UserConnection;
			Guid sourceId = request.SourceId;
			IReadOnlyCollection<KnwSrcOutboxRecord> sourceEvents = request.SourceEvents;
			int batchSize = request.BatchSize;
			if (userConnection == null) {
				return new SectionSynchronizerResult {
					IsTransientFailure = false,
					Error = "UserConnection is null"
				};
			}
			if (batchSize <= 0) {
				return new SectionSynchronizerResult {
					IsTransientFailure = false,
					Error = "Batch size must be greater than zero"
				};
			}
			if (sourceId == Guid.Empty || sourceEvents == null || sourceEvents.Count == 0) {
				return new SectionSynchronizerResult {
					IsSuccess = true
				};
			}
			IKnwSourceManager sourceManager = ResolveSourceManager(userConnection);
			IKnwIndexingSessionManager sessionManager = ResolveSessionManager(userConnection);
			IKnwServiceClient serviceClient = ResolveServiceClient(userConnection);
			IKnwServiceProblemHelper serviceProblemHelper = ResolveServiceProblemHelper();
			KnwSourceRecord source = sourceManager.GetById(sourceId);
			if (source == null) {
				return new SectionSynchronizerResult {
					IsTransientFailure = false,
					Error = $"Knowledge source {sourceId} not found"
				};
			}
			HashSet<string> upsertIds = BuildUpsertIds(sourceEvents);
			HashSet<string> deleteIds = BuildDeleteIds(sourceEvents);
			if (upsertIds.Count == 0 && deleteIds.Count == 0) {
				return new SectionSynchronizerResult {
					IsSkipped = true,
					IsSuccess = true,
					Error = "Nothing to process"
				};
			}
			KnwIndexingSessionRecord currentSession = null;
			try {
				if (upsertIds.Count > 0) {
					KnwIndexingStatusSnapshot authoritativeStatus =
						sessionManager.ResolveLatestIndexingStatus(sourceId, cancellationToken);
					if (authoritativeStatus == null) {
						return new SectionSynchronizerResult {
							IsSkipped = true,
							IsTransientFailure = true,
							Error = "No authoritative indexing status available"
						};
					}
					if (authoritativeStatus.HasActiveIndexing) {
						throw new KnwActiveIndexingSessionExistsException(sourceId, authoritativeStatus.LatestSession);
					}
					if (!authoritativeStatus.IsAutoSyncReady) {
						return new SectionSynchronizerResult {
							IsSkipped = true,
							IsTransientFailure = true,
							Error = "No completed manual indexing session available"
						};
					}
					int uploadedCount = 0;
					int failedCount = 0;
					int totalBatches = 0;
					IKnwProvider provider = ResolveProviderFactory().CreateKnwProvider(
						new KnwProviderInitializationOptions(userConnection, source));
					if (provider == null) {
						return new SectionSynchronizerResult {
							IsTransientFailure = false,
							Error = $"Provider for source {sourceId} is not available"
						};
					}
					currentSession = sessionManager.StartAfterAuthoritativeSync(source.Id, authoritativeStatus,
						KnwIndexingSessionTypeEnum.Incremental, cancellationToken);
					if (currentSession == null) {
						return new SectionSynchronizerResult {
							IsTransientFailure = true,
							Error = "Failed to start indexing session"
						};
					}
					var discoveryOptions = new KnwProviderDiscoveryOptions {
						ItemIds = upsertIds
					};
					var discoveredIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					if (provider is IKnwBatchProvider batchProvider) {
						IEnumerable<IReadOnlyList<KnwItem>> batches =
							batchProvider.DiscoverInBatches(discoveryOptions, cancellationToken) ??
								Enumerable.Empty<IReadOnlyList<KnwItem>>();
						foreach (IReadOnlyList<KnwItem> batch in batches) {
							totalBatches++;
							KnwLibUtils.Logger.Info($"Section sync processing batch {totalBatches} with " +
								$"{(batch?.Count ?? 0)} items (session {currentSession.SessionId}, source {source.Id}).");
							UploadStatistics stats = UploadDiscoveredItems(batch, sessionManager, currentSession,
								serviceProblemHelper,
								discoveredIds, cancellationToken);
							uploadedCount += stats.Uploaded;
							failedCount += stats.Failed;
						}
					} else {
						IEnumerable<KnwItem> discoveredItems =
								provider.Discover(discoveryOptions, cancellationToken) ?? Enumerable.Empty<KnwItem>();
						UploadStatistics stats = UploadDiscoveredItems(discoveredItems, sessionManager, currentSession,
							serviceProblemHelper,
							discoveredIds, cancellationToken);
						uploadedCount += stats.Uploaded;
						failedCount += stats.Failed;
					}
					string finalizeStatus = sessionManager.FinalizeTransfer(currentSession, cancellationToken);
					KnwLibUtils.Logger.Info($"Section sync finalize status for session {currentSession.SessionId}: " +
						$"{finalizeStatus}. Uploaded={uploadedCount}, Failed={failedCount}, Batches={totalBatches}.");
					foreach (string upsertId in upsertIds) {
						if (!discoveredIds.Contains(upsertId)) {
							deleteIds.Add(upsertId);
						}
					}
				}
				if (deleteIds.Count > 0) {
					if (upsertIds.Count == 0) {
						sessionManager.EnsureNoActiveAuthoritativeSession(sourceId, cancellationToken);
					}
					foreach (List<string> chunk in Chunk(deleteIds, batchSize)) {
						serviceClient.DeleteFiles(source.Id, chunk, cancellationToken);
					}
				}
				return new SectionSynchronizerResult {
					IsSuccess = true
				};
			} catch (KnwActiveIndexingSessionExistsException) {
				return new SectionSynchronizerResult {
					IsSkipped = true,
					IsTransientFailure = true,
					Error = "Active indexing session exists"
				};
			} catch (KnwStartedSessionTerminalStateException e) {
				string detailedError = BuildDetailedErrorMessage(e);
				serviceProblemHelper.LogTechnicalError(
					$"Section synchronization failed for source {sourceId}: {detailedError}", e);
				return new SectionSynchronizerResult {
					IsTransientFailure = true,
					Error = detailedError
				};
			} catch (OperationCanceledException) {
				KnwLibUtils.Logger.Info($"Section synchronization was cancelled for source {sourceId}.");
				if (currentSession != null) {
					TryCancelSession(currentSession, sessionManager);
				}
				return new SectionSynchronizerResult {
					IsSkipped = true,
					IsTransientFailure = true,
					Error = "Synchronization cancelled"
				};
			} catch (Exception e) {
				string detailedError = BuildDetailedErrorMessage(e);
				serviceProblemHelper.LogTechnicalError(
					$"Section synchronization failed for source {sourceId}: {detailedError}", e);
				serviceProblemHelper.TrySetSessionFailureDetails(sessionManager, currentSession, e,
					$"Section synchronization failed for source {sourceId}",
					"Failed to set synchronization details after section error");
				TrySyncSessionStatus(currentSession, sessionManager);
				return new SectionSynchronizerResult {
					IsTransientFailure = true,
					Error = detailedError
				};
			}
		}

		#endregion

	}

	#endregion

}

