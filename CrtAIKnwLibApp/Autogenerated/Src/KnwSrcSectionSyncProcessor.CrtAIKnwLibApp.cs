namespace Creatio.Copilot
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSrcSectionSyncProcessor

	/// <summary>
	/// Executes synchronization pipeline for outbox events.
	/// </summary>
	internal interface IKnwSrcSectionSyncProcessor
	{

		#region Methods: Internal

		void Process(UserConnection userConnection, CancellationToken cancellationToken);

		#endregion

	}

	#endregion

	#region Class: KnwSrcSectionSyncProcessor

	/// <summary>
	/// Orchestrates outbox leasing, source synchronization and state transitions.
	/// </summary>
	[DefaultBinding(typeof(IKnwSrcSectionSyncProcessor))]
	internal class KnwSrcSectionSyncProcessor : IKnwSrcSectionSyncProcessor
	{

		#region Class: CircuitState

		private sealed class CircuitState
		{
			public int ConsecutiveFailures { get; set; }

			public DateTime? OpenUntilUtc { get; set; }
		}

		#endregion

		#region Constants: Private

		private const int DefaultMaxRetries = 3;
		private const int DefaultMaxOutboxAttempts = 10;
		private const int DefaultLeaseDurationMinutes = 5;
		private const int DefaultCircuitCooldownMinutes = 10;
		private const int DefaultRetryAfterMinutes = 5;
		private const int DefaultCleanupRetentionDays = 7;
		private const int DefaultBatchSize = 250;

		private const string KnowledgeIndexingBatchSizeSetting = "KnowledgeIndexingBatchSize";
		private const string MaxRetriesSetting = "KnwSrcAutoSyncMaxRetries";
		private const string MaxOutboxAttemptsSetting = "KnwSrcAutoSyncMaxOutboxAttempts";
		private const string LeaseDurationMinutesSetting = "KnwSrcAutoSyncLeaseDurationMinutes";
		private const string CircuitCooldownMinutesSetting = "KnwSrcAutoSyncCircuitCooldownMinutes";
		private const string RetryAfterMinutesSetting = "KnwSrcAutoSyncRetryAfterMinutes";
		private const string CleanupRetentionDaysSetting = "KnwSrcAutoSyncCleanupRetentionDays";

		#endregion

		#region Fields: Private

		private static readonly ConcurrentDictionary<Guid, CircuitState> _circuitStates =
			new ConcurrentDictionary<Guid, CircuitState>();

		private ISectionSynchronizer _sectionSynchronizer;
		private IKnwSrcOutboxRepository _outboxRepository;

		#endregion

		#region Properties: Internal

		internal ISectionSynchronizer SectionSynchronizer {
			get => _sectionSynchronizer;
			set => _sectionSynchronizer = value;
		}

		internal IKnwSrcOutboxRepository OutboxRepository {
			get => _outboxRepository;
			set => _outboxRepository = value;
		}

		internal Action<TimeSpan> DelayAction { get; set; } = span => Thread.Sleep(span);

		#endregion

		#region Methods: Private

		private ISectionSynchronizer ResolveSynchronizer() {
			return _sectionSynchronizer ?? (_sectionSynchronizer = ClassFactory.Get<ISectionSynchronizer>());
		}

		private IKnwSrcOutboxRepository ResolveRepository(UserConnection userConnection) {
			return _outboxRepository ?? (_outboxRepository = ClassFactory.Get<IKnwSrcOutboxRepository>(
				new ConstructorArgument("userConnection", userConnection)));
		}

		private static TimeSpan GetRetryDelay(int attempt) {
			switch (attempt) {
				case 1:
					return TimeSpan.FromSeconds(1);
				case 2:
					return TimeSpan.FromSeconds(3);
				default:
					return TimeSpan.FromSeconds(9);
			}
		}

		private bool IsCircuitOpen(Guid sourceId, DateTime utcNow) {
			if (!_circuitStates.TryGetValue(sourceId, out CircuitState state)) {
				return false;
			}
			if (!state.OpenUntilUtc.HasValue) {
				return false;
			}
			if (state.OpenUntilUtc.Value > utcNow) {
				KnwLibUtils.Logger.Debug($"Circuit breaker is open for source {sourceId} until " +
					$"{state.OpenUntilUtc.Value:o}.");
				return true;
			}
			KnwLibUtils.Logger.Info($"Circuit breaker cooldown elapsed for source {sourceId}. " +
				"Resetting circuit state.");
			state.OpenUntilUtc = null;
			state.ConsecutiveFailures = 0;
			return false;
		}

		private void RegisterSuccess(Guid sourceId) {
			_circuitStates.TryRemove(sourceId, out _);
			KnwLibUtils.Logger.Debug($"Registered synchronization success for source {sourceId}. " +
				"Circuit state cleared.");
		}

		private void RegisterFailure(Guid sourceId, DateTime utcNow, TimeSpan circuitCooldown) {
			CircuitState state = _circuitStates.GetOrAdd(sourceId, _ => new CircuitState());
			state.ConsecutiveFailures++;
			KnwLibUtils.Logger.Debug($"Registered synchronization failure for source {sourceId}. " +
				$"Consecutive failures: {state.ConsecutiveFailures}.");
			if (state.ConsecutiveFailures >= 3) {
				state.OpenUntilUtc = utcNow.Add(circuitCooldown);
				state.ConsecutiveFailures = 0;
				KnwLibUtils.Logger.Info($"Circuit breaker opened for source {sourceId} until " +
					$"{state.OpenUntilUtc.Value:o}.");
			}
		}

		private SectionSynchronizerResult ExecuteWithRetries(UserConnection userConnection, Guid sourceId,
				List<KnwSrcOutboxRecord> sourceEvents, int maxRetries, int batchSize,
				CancellationToken cancellationToken) {
			SectionSynchronizerResult lastResult = null;
			var request = new SectionSynchronizerRequest {
				UserConnection = userConnection,
				SourceId = sourceId,
				SourceEvents = sourceEvents,
				BatchSize = batchSize
			};
			for (int attempt = 1; attempt <= maxRetries; attempt++) {
				cancellationToken.ThrowIfCancellationRequested();
				KnwLibUtils.Logger.Debug($"Starting synchronization attempt {attempt}/{maxRetries} for source " +
					$"{sourceId} with {sourceEvents.Count} outbox events.");
				lastResult = ResolveSynchronizer().Synchronize(request, cancellationToken);
				if (lastResult == null) {
					lastResult = new SectionSynchronizerResult {
						Error = "Unknown synchronization error",
						IsTransientFailure = true
					};
				}
				KnwLibUtils.Logger.Debug($"Synchronization attempt {attempt}/{maxRetries} for source {sourceId} " +
					$"finished. Success={lastResult.IsSuccess}, Skipped={lastResult.IsSkipped}, " +
					$"TransientFailure={lastResult.IsTransientFailure}, Error={lastResult.Error ?? "<none>"}.");
				if (lastResult.IsSuccess || lastResult.IsSkipped || !lastResult.IsTransientFailure) {
					return lastResult;
				}
				if (attempt < maxRetries) {
					TimeSpan retryDelay = GetRetryDelay(attempt);
					KnwLibUtils.Logger.Debug($"Transient failure for source {sourceId}. Next retry in " +
						$"{retryDelay.TotalSeconds:0} seconds.");
					DelayAction(retryDelay);
				}
			}
			return lastResult ?? new SectionSynchronizerResult {
				Error = "Synchronization failed",
				IsTransientFailure = true
			};
		}

		private static int GetBatchSize(UserConnection userConnection) {
			int batchSize = SysSettings.GetValue(userConnection, KnowledgeIndexingBatchSizeSetting, DefaultBatchSize);
			return batchSize <= 0 ? DefaultBatchSize : batchSize;
		}

		private static int GetPositiveSetting(UserConnection userConnection, string settingCode, int defaultValue) {
			int value = SysSettings.GetValue(userConnection, settingCode, defaultValue);
			if (value <= 0) {
				KnwLibUtils.Logger.Warn($"Setting '{settingCode}' has invalid value '{value}'. " +
					$"Using default '{defaultValue}'.");
				return defaultValue;
			}
			return value;
		}

		private static int GetMaxRetries(UserConnection userConnection) {
			return GetPositiveSetting(userConnection, MaxRetriesSetting, DefaultMaxRetries);
		}

		private static int GetMaxOutboxAttempts(UserConnection userConnection) {
			return GetPositiveSetting(userConnection, MaxOutboxAttemptsSetting, DefaultMaxOutboxAttempts);
		}

		private static TimeSpan GetLeaseDuration(UserConnection userConnection) {
			int minutes = GetPositiveSetting(userConnection, LeaseDurationMinutesSetting,
				DefaultLeaseDurationMinutes);
			return TimeSpan.FromMinutes(minutes);
		}

		private static TimeSpan GetCircuitCooldown(UserConnection userConnection) {
			int minutes = GetPositiveSetting(userConnection, CircuitCooldownMinutesSetting,
				DefaultCircuitCooldownMinutes);
			return TimeSpan.FromMinutes(minutes);
		}

		private static int GetRetryAfterMinutes(UserConnection userConnection) {
			return GetPositiveSetting(userConnection, RetryAfterMinutesSetting, DefaultRetryAfterMinutes);
		}

		private static int GetCleanupRetentionDays(UserConnection userConnection) {
			return GetPositiveSetting(userConnection, CleanupRetentionDaysSetting, DefaultCleanupRetentionDays);
		}

		private static List<KnwSrcOutboxRecord> DeduplicateSourceEvents(IEnumerable<KnwSrcOutboxRecord> sourceEvents) {
			return sourceEvents
				.GroupBy(x => x.RecordId)
				.Select(group => group
					.OrderByDescending(item => item.EventOn)
					.ThenByDescending(item => item.Id)
					.First())
				.ToList();
		}

		#endregion

		#region Methods: Public

		internal static void ResetCircuitBreakerState() {
			_circuitStates.Clear();
		}

		/// <inheritdoc />
		public void Process(UserConnection userConnection, CancellationToken cancellationToken) {
			IKnwSrcOutboxRepository repository = ResolveRepository(userConnection);
			string processingToken = Guid.NewGuid().ToString("N");
			DateTime startUtc = DateTime.UtcNow;
			int batchSize = GetBatchSize(userConnection);
			int maxRetries = GetMaxRetries(userConnection);
			int maxOutboxAttempts = GetMaxOutboxAttempts(userConnection);
			TimeSpan leaseDuration = GetLeaseDuration(userConnection);
			TimeSpan circuitCooldown = GetCircuitCooldown(userConnection);
			int retryAfterMinutes = GetRetryAfterMinutes(userConnection);
			int cleanupRetentionDays = GetCleanupRetentionDays(userConnection);
			int cleanupBatchSize = batchSize;
			KnwLibUtils.Logger.Debug($"Starting section sync processor run. Token={processingToken}, " +
				$"BatchSize={batchSize}, LeaseMinutes={leaseDuration.TotalMinutes:0}, " +
				$"MaxRetries={maxRetries}, MaxOutboxAttempts={maxOutboxAttempts}, " +
				$"RetryAfterMinutes={retryAfterMinutes}, CircuitCooldownMinutes={circuitCooldown.TotalMinutes:0}, " +
				$"CleanupRetentionDays={cleanupRetentionDays}.");
			int totalClaimed = 0;
			int batchNumber = 0;
			while (true) {
				cancellationToken.ThrowIfCancellationRequested();
				DateTime utcNow = DateTime.UtcNow;
				IList<KnwSrcOutboxRecord> claimed = repository.ClaimPendingBatch(batchSize, leaseDuration,
					processingToken, utcNow);
				batchNumber++;
				KnwLibUtils.Logger.Debug($"Claimed {claimed.Count} outbox records with token {processingToken} " +
					$"(batch {batchNumber}).");
				if (!claimed.Any()) {
					break;
				}
				totalClaimed += claimed.Count;
				foreach (IGrouping<Guid, KnwSrcOutboxRecord> sourceGroup in claimed.GroupBy(x => x.KnwSourceId)) {
					cancellationToken.ThrowIfCancellationRequested();
					List<Guid> sourceOutboxIds = sourceGroup.Select(x => x.Id).ToList();
					KnwLibUtils.Logger.Debug($"Processing source {sourceGroup.Key}. Claimed rows: " +
						$"{sourceOutboxIds.Count}.");
					if (IsCircuitOpen(sourceGroup.Key, utcNow)) {
						// Important: do NOT release the lease here. When processing all batches in one run,
						// releasing would cause the same rows to be reclaimed and skipped again in a tight loop.
						KnwLibUtils.Logger.Info($"Skipping source {sourceGroup.Key} because circuit breaker is open. " +
							$"Keeping lease for {sourceOutboxIds.Count} rows until expiration.");
						continue;
					}
					List<KnwSrcOutboxRecord> sourceEvents = DeduplicateSourceEvents(sourceGroup);
					KnwLibUtils.Logger.Debug($"Source {sourceGroup.Key} deduplicated events count: " +
						$"{sourceEvents.Count}.");
					SectionSynchronizerResult result = ExecuteWithRetries(userConnection, sourceGroup.Key, sourceEvents,
						maxRetries, batchSize, cancellationToken);
					if (result.IsSuccess) {
						try {
							repository.MarkProcessed(sourceOutboxIds, processingToken, DateTime.UtcNow);
						} catch (Exception e) {
							KnwLibUtils.Logger.Error($"Failed to mark outbox rows as processed for source " +
								$"{sourceGroup.Key}. Releasing lease for retry.", e);
							repository.ReleaseLease(sourceOutboxIds, processingToken);
							continue;
						}
						KnwLibUtils.Logger.Info($"Section synchronization succeeded for source {sourceGroup.Key}. " +
							$"Marked {sourceOutboxIds.Count} outbox rows as processed.");
						RegisterSuccess(sourceGroup.Key);
						continue;
					}
					if (result.IsSkipped) {
						// Important: do NOT release the lease here. Otherwise, the same rows will be reclaimed again
						// in this run when draining all batches.
						KnwLibUtils.Logger.Info($"Section synchronization skipped for source {sourceGroup.Key}. " +
							$"Keeping lease for {sourceOutboxIds.Count} rows until expiration. " +
							$"Reason: {result.Error ?? "<none>"}.");
						continue;
					}
					DateTime failureTime = DateTime.UtcNow;
					DateTime nextAttemptOnUtc = failureTime.AddMinutes(retryAfterMinutes);
					string error = result.Error ?? "Synchronization failed";
					// Enforce max outbox attempts: rows that reached the limit are dead-lettered.
					List<Guid> deadLetterIds = sourceGroup
						.Where(x => x.Attempts + 1 >= maxOutboxAttempts)
						.Select(x => x.Id)
						.Distinct()
						.ToList();
					List<Guid> retryIds = sourceGroup
						.Where(x => x.Attempts + 1 < maxOutboxAttempts)
						.Select(x => x.Id)
						.Distinct()
						.ToList();
					try {
						if (deadLetterIds.Count > 0) {
							repository.MarkDeadLettered(deadLetterIds, processingToken,
								$"Attempts limit reached ({maxOutboxAttempts}). {error}", failureTime);
							KnwLibUtils.Logger.Warn($"Section synchronization failed for source {sourceGroup.Key}. " +
								$"Dead-lettered {deadLetterIds.Count} rows because attempts limit reached " +
								$"({maxOutboxAttempts}).");
						}
						if (retryIds.Count > 0) {
							repository.MarkFailed(retryIds, processingToken, error, nextAttemptOnUtc);
							KnwLibUtils.Logger.Info($"Section synchronization failed for source {sourceGroup.Key}. " +
								$"Marked {retryIds.Count} rows for retry on {nextAttemptOnUtc:o}. " +
								$"Error: {error}.");
						}
					} catch (Exception e) {
						KnwLibUtils.Logger.Error($"Failed to update outbox rows after synchronization failure for source " +
							$"{sourceGroup.Key}. Releasing lease.", e);
						repository.ReleaseLease(sourceOutboxIds, processingToken);
						RegisterFailure(sourceGroup.Key, DateTime.UtcNow, circuitCooldown);
						continue;
					}
					RegisterFailure(sourceGroup.Key, DateTime.UtcNow, circuitCooldown);
				}
			}
			int cleanupRemoved = repository.CleanupProcessed(DateTime.UtcNow.AddDays(-cleanupRetentionDays),
				cleanupBatchSize);
			KnwLibUtils.Logger.Debug($"Section sync processor run completed. Token={processingToken}, " +
				$"TotalClaimed={totalClaimed}, TotalBatches={batchNumber - 1}, " +
				$"DurationSeconds={(DateTime.UtcNow - startUtc).TotalSeconds:0}. " +
				$"Cleanup removed {cleanupRemoved} rows.");
		}

		#endregion

	}

	#endregion

}

