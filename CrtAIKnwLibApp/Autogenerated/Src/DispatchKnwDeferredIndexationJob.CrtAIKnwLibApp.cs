namespace Creatio.Copilot
{
	using FeatureToggling;
	using Quartz;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwDeferredIndexationJobDispatcher

	/// <summary>
	/// Defines registration and execution contract for deferred indexation queue processing scheduler job.
	/// </summary>
	internal interface IKnwDeferredIndexationJobDispatcher : IJobExecutor
	{

		#region Methods: Internal

		void Register(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper);

		#endregion

	}

	#endregion

	#region Class: DispatchKnwDeferredIndexationJob

	/// <summary>
	/// Processes queued knowledge-source indexing requests created during installation flows.
	/// </summary>
	[DefaultBinding(typeof(IKnwDeferredIndexationJobDispatcher), Name = nameof(DispatchKnwDeferredIndexationJob))]
	internal class DispatchKnwDeferredIndexationJob : IKnwDeferredIndexationJobDispatcher
	{

		#region Constants: Private

		private const string EnableKnwRetrievalFeatureCode = "GenAIFeatures.EnableKnwRetrieval";
		private const string QueueCheckIntervalSetting = "KnwSrcIndexationCheckInterval";
		private const string InitialFrequencyParameterName = "initialFrequency";
		private const string KnwDeferredIndexationQueueSchemaName = "KnwDeferredIndexationQueue";

		#endregion

		#region Fields: Private

		private static readonly string _jobName = nameof(DispatchKnwDeferredIndexationJob);
		private static readonly string _jobFullName = typeof(DispatchKnwDeferredIndexationJob).FullName;
		private static readonly string _jobGroupName = nameof(DispatchKnwDeferredIndexationJob) + "Group";

		#endregion

		#region Properties: Internal

		internal Func<UserConnection, IKnwIndexingSessionManager> SessionManagerProvider { get; set; } =
			uc => ClassFactory.Get<IKnwIndexingSessionManager>(new ConstructorArgument("userConnection", uc));

		#endregion

		#region Methods: Private

		private static IJobDetail CreateJob(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper,
				int frequency, string jobName, string jobGroup) {
			var parameters = new Dictionary<string, object> {
				{ InitialFrequencyParameterName, frequency }
			};
			return appSchedulerWrapper.CreateClassJob<DispatchKnwDeferredIndexationJob>(jobName, jobGroup, userConnection,
				parameters, true);
		}

		private static ITrigger CreateTrigger(int frequency) {
			return TriggerBuilder.Create()
				.WithSimpleSchedule(s => s.WithIntervalInMinutes(frequency).RepeatForever())
				.StartNow()
				.Build();
		}

		private static int GetFrequency(UserConnection userConnection) {
			return SysSettings.GetValue(userConnection, QueueCheckIntervalSetting, 1);
		}

		private static bool GetIsJobEnabled() {
			return Features.GetIsEnabled(EnableKnwRetrievalFeatureCode);
		}

		private static bool HasActiveAuthoritativeSessions(IKnwIndexingSessionManager sessionManager) {
			foreach (Guid sourceId in sessionManager.GetActiveSessions()
					.Select(session => session.KnwSourceId)
					.Distinct()) {
				try {
					KnwIndexingStatusSnapshot statusSnapshot = sessionManager.ResolveLatestIndexingStatus(sourceId);
					if (statusSnapshot?.HasActiveIndexing == true) {
						return true;
					}
				} catch (Exception e) {
					KnwLibUtils.Logger.Warn($"Failed to resolve active session for source {sourceId} before " +
						$"deferred indexation dispatch. Queue processing is postponed.", e);
					return true;
				}
			}
			return false;
		}

		private void TryProcessDeferredIndexationQueue(UserConnection userConnection) {
			var sessionManager = SessionManagerProvider(userConnection);
			if (HasActiveAuthoritativeSessions(sessionManager)) {
				return;
			}
			EntitySchema queueSchema = userConnection.EntitySchemaManager
				.FindInstanceByName(KnwDeferredIndexationQueueSchemaName);
			if (queueSchema == null) {
				return;
			}
			var esq = new EntitySchemaQuery(queueSchema) {
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn("KnwSourceId");
			esq.AddColumn("CreatedOn").OrderByAsc();
			esq.RowCount = 1;
			Entity entry = esq.GetEntityCollection(userConnection).FirstOrDefault();
			if (entry == null) {
				return;
			}
			Guid entryId = entry.PrimaryColumnValue;
			Guid knwSourceId = entry.GetTypedColumnValue<Guid>("KnwSourceId");
			KnwLibUtils.Logger.Info($"[{_jobName}] Starting indexing for deferred-queued KnwSource {knwSourceId}.");
			ClassFactory.Get<IKnwIndexingRunner>().StartIndexing(knwSourceId, userConnection);
			new Delete(userConnection)
				.From(KnwDeferredIndexationQueueSchemaName)
				.Where("Id").IsEqual(Column.Parameter(entryId))
				.Execute();
		}

		private bool TryRemove(IAppSchedulerWraper appSchedulerWrapper, string message) {
			if (appSchedulerWrapper.DoesJobExist(_jobFullName, _jobGroupName)) {
				appSchedulerWrapper.RemoveJob(_jobFullName, _jobGroupName);
				KnwLibUtils.Logger.Info(message);
				return true;
			}
			return false;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public void Register(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper) {
			TryRemove(appSchedulerWrapper, $"{_jobName} was removed before re-scheduling.");
			if (!GetIsJobEnabled()) {
				KnwLibUtils.Logger.Info($"{_jobName} not scheduled because the {EnableKnwRetrievalFeatureCode} " +
					"feature is disabled.");
				return;
			}
			int frequency = GetFrequency(userConnection);
			if (frequency <= 0) {
				KnwLibUtils.Logger.Info($"{_jobName} not scheduled because the value of {QueueCheckIntervalSetting} " +
					"setting is less than or equal to zero.");
				return;
			}
			IJobDetail job = CreateJob(userConnection, appSchedulerWrapper, frequency, _jobFullName, _jobGroupName);
			ITrigger trigger = CreateTrigger(frequency);
			appSchedulerWrapper.Instance.ScheduleJob(job, trigger);
			KnwLibUtils.Logger.Info($"{_jobName} scheduled with {frequency} minutes frequency.");
		}

		/// <inheritdoc />
		public void Execute(UserConnection userConnection, IDictionary<string, object> parameters) {
			if (!GetIsJobEnabled()) {
				KnwLibUtils.Logger.Info($"{_jobName} execution skipped because the {EnableKnwRetrievalFeatureCode} " +
					"feature is disabled.");
				return;
			}
			int frequency = GetFrequency(userConnection);
			int initialFrequency = parameters != null
					&& parameters.TryGetValue(InitialFrequencyParameterName, out object value)
				? (int)value
				: frequency;
			if (frequency != initialFrequency) {
				Register(userConnection, ClassFactory.Get<IAppSchedulerWraper>());
			}
			TryProcessDeferredIndexationQueue(userConnection);
		}

		#endregion

	}

	#endregion
}

