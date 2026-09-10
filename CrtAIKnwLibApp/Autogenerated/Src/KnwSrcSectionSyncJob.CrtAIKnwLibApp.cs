namespace Creatio.Copilot
{
	using System.Collections.Generic;
	using System.Threading;
	using FeatureToggling;
	using Quartz;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSrcSectionSyncJobActualizer

	/// <summary>
	/// Defines registration and execution contract for section synchronization scheduler job.
	/// </summary>
	internal interface IKnwSrcSectionSyncJobActualizer : IJobExecutor
	{

		#region Methods: Internal

		void Register(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper);

		#endregion

	}

	#endregion

	#region Class: KnwSrcSectionSyncJob

	/// <summary>
	/// Executes periodic synchronization of section changes accumulated in outbox.
	/// </summary>
	[DefaultBinding(typeof(IKnwSrcSectionSyncJobActualizer), Name = nameof(KnwSrcSectionSyncJob))]
	internal class KnwSrcSectionSyncJob : IKnwSrcSectionSyncJobActualizer
	{

		#region Constants: Private

		private const string EnableKnwRetrievalFeatureCode = "GenAIFeatures.EnableKnwRetrieval";
		private const string SyncIntervalSetting = "KnwSrcAutoSyncIntervalSeconds";
		private const string InitialFrequencyParameterName = "initialFrequency";

		#endregion

		#region Fields: Private

		private static readonly string _jobName = nameof(KnwSrcSectionSyncJob);
		private static readonly string _jobFullName = typeof(KnwSrcSectionSyncJob).FullName;
		private static readonly string _jobGroupName = nameof(KnwSrcSectionSyncJob) + "Group";
		private static readonly SemaphoreSlim _runGate = new SemaphoreSlim(1, 1);
		private IKnwSrcSectionSyncProcessor _syncProcessor;

		#endregion

		#region Properties: Internal

		internal IKnwSrcSectionSyncProcessor SyncProcessor {
			get => _syncProcessor ?? (_syncProcessor = new KnwSrcSectionSyncProcessor());
			set => _syncProcessor = value;
		}

		#endregion

		#region Methods: Private

		private static IJobDetail CreateJob(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper,
				int frequency, string jobName, string jobGroup) {
			var parameters = new Dictionary<string, object> {
				{ InitialFrequencyParameterName, frequency }
			};
			return appSchedulerWrapper.CreateClassJob<KnwSrcSectionSyncJob>(jobName, jobGroup, userConnection,
				parameters, true);
		}

		private static ITrigger CreateTrigger(int frequency) {
			return TriggerBuilder.Create()
				.WithSimpleSchedule(s => s.WithIntervalInSeconds(frequency).RepeatForever())
				.StartNow()
				.Build();
		}

		private static int GetFrequency(UserConnection userConnection) {
			return SysSettings.GetValue(userConnection, SyncIntervalSetting, 0);
		}

		private static bool GetIsJobEnabled() {
			return Features.GetIsEnabled(EnableKnwRetrievalFeatureCode);
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
				KnwLibUtils.Logger.Info($"{_jobName} not scheduled because the value of {SyncIntervalSetting} " +
					"setting is less than or equal to zero.");
				return;
			}
			IJobDetail job = CreateJob(userConnection, appSchedulerWrapper, frequency, _jobFullName, _jobGroupName);
			ITrigger trigger = CreateTrigger(frequency);
			appSchedulerWrapper.Instance.ScheduleJob(job, trigger);
			KnwLibUtils.Logger.Info($"{_jobName} scheduled with {frequency} seconds frequency.");
		}

		/// <inheritdoc />
		public void Execute(UserConnection userConnection, IDictionary<string, object> parameters) {
			if (!GetIsJobEnabled()) {
				KnwLibUtils.Logger.Info($"{_jobName} execution skipped because the {EnableKnwRetrievalFeatureCode} " +
					"feature is disabled.");
				return;
			}
			if (!_runGate.Wait(0)) {
				KnwLibUtils.Logger.Info($"{_jobName} execution skipped because previous run is still in progress.");
				return;
			}
			try {
				int frequency = GetFrequency(userConnection);
				int initialFrequency = parameters != null
						&& parameters.TryGetValue(InitialFrequencyParameterName, out object value)
					? (int)value
					: frequency;
				if (frequency != initialFrequency) {
					Register(userConnection, ClassFactory.Get<IAppSchedulerWraper>());
				}
				SyncProcessor.Process(userConnection, CancellationToken.None);
			} finally {
				_runGate.Release();
			}
		}

		#endregion

	}

	#endregion

}

