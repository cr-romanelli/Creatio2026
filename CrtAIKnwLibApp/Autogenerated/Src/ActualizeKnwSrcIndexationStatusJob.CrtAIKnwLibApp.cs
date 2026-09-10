namespace Creatio.Copilot
{
	using System.Collections.Generic;
	using FeatureToggling;
	using Quartz;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSrcIndexationStatusJobActualizer

	/// <summary>
	/// Defines the contract for the job that actualizes Knowledge Source indexation status.
	/// </summary>
	public interface IKnwSrcIndexationStatusJobActualizer : IJobExecutor
	{

		#region Methods: Public

		/// <summary>
		/// Registers the <see cref="ActualizeKnwSrcIndexationStatusJob"/> in the scheduler.
		/// </summary>
		/// <param name="userConnection">The current user connection context.</param>
		/// <param name="appSchedulerWrapper">The application scheduler wrapper instance.</param>
		void Register(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper);

		#endregion

	}

	#endregion

	#region Class: ActualizeKnwSrcIndexationStatusJob

	/// <summary>
	/// Quartz job for periodic Knowledge Source indexation status actualization.
	/// </summary>
	[DefaultBinding(typeof(IKnwSrcIndexationStatusJobActualizer), Name = nameof(ActualizeKnwSrcIndexationStatusJob))]
	public class ActualizeKnwSrcIndexationStatusJob : IKnwSrcIndexationStatusJobActualizer
	{

		#region Constants: Private

		private const string EnableKnwRetrievalFeatureCode = "GenAIFeatures.EnableKnwRetrieval";
		private const string IndexationCheckIntervalSetting = "KnwSrcIndexationCheckInterval";

		#endregion

		#region Fields: Private

		private static readonly string _initialFrequencyParameterName = "initialFrequency";
		private static readonly string _jobName = nameof(ActualizeKnwSrcIndexationStatusJob);
		private static readonly string _jobFullName = typeof(ActualizeKnwSrcIndexationStatusJob).FullName;
		private static readonly string _jobGroupName = nameof(ActualizeKnwSrcIndexationStatusJob) + "Group";

		#endregion

		#region Properties: Private

		private static int DefaultFrequency => 1;

		#endregion

		#region Properties: Internal

		private IKnwSrcIndexationStatusProcessor _knwSrcIndexationStatusProcessor;
		internal IKnwSrcIndexationStatusProcessor KnwSrcIndexationStatusProcessor {
			get => _knwSrcIndexationStatusProcessor ??
				(_knwSrcIndexationStatusProcessor = new KnwSrcIndexationStatusProcessor());
			set => _knwSrcIndexationStatusProcessor = value;
		}

		#endregion

		#region Methods: Private

		private static IJobDetail CreateJob(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper,
				int frequency, string jobName, string jobGroup) {
			var parameters = new Dictionary<string, object> {
				{ _initialFrequencyParameterName, frequency }
			};
			return appSchedulerWrapper.CreateClassJob<ActualizeKnwSrcIndexationStatusJob>(jobName, jobGroup,
				userConnection, parameters, true);
		}

		private static ITrigger GetJobTrigger(int frequency) {
			return TriggerBuilder.Create()
				.WithSimpleSchedule(s => s.WithIntervalInMinutes(frequency).RepeatForever())
				.StartNow()
				.Build();
		}

		private static int GetIndexCheckFrequencyValue(UserConnection userConnection) {
			return SysSettings.GetValue(userConnection, IndexationCheckIntervalSetting, DefaultFrequency);
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

		/// <summary>
		/// Registers <see cref="ActualizeKnwSrcIndexationStatusJob"/> in scheduler.
		/// </summary>
		/// <param name="userConnection">User connection.</param>
		/// <param name="appSchedulerWrapper">Scheduler.</param>
		public void Register(UserConnection userConnection, IAppSchedulerWraper appSchedulerWrapper) {
			TryRemove(appSchedulerWrapper, $"{_jobName} was removed before re-scheduling.");
			if (!GetIsJobEnabled()) {
				KnwLibUtils.Logger.Info($"{_jobName} not scheduled because the {EnableKnwRetrievalFeatureCode} " +
					"feature is disabled.");
				return;
			}
			int frequency = GetIndexCheckFrequencyValue(userConnection);
			if (frequency <= 0) {
				KnwLibUtils.Logger.Info($"{_jobName} not scheduled because the value of {IndexationCheckIntervalSetting} " +
					"setting is less than or equal to zero.");
				return;
			}
			IJobDetail job = CreateJob(userConnection, appSchedulerWrapper, frequency, _jobFullName, _jobGroupName);
			ITrigger trigger = GetJobTrigger(frequency);
			appSchedulerWrapper.Instance.ScheduleJob(job, trigger);
			KnwLibUtils.Logger.Info($"{_jobName} scheduled with {frequency} minutes frequency.");
		}

		/// <inheritdoc cref="Terrasoft.Core.IJobExecutor.Execute"/>
		public void Execute(UserConnection userConnection, IDictionary<string, object> parameters) {
			if (!GetIsJobEnabled()) {
				KnwLibUtils.Logger.Info($"{_jobName} execution skipped because the {EnableKnwRetrievalFeatureCode} " +
					"feature is disabled.");
				return;
			}
			int initialFrequency = parameters.TryGetValue(_initialFrequencyParameterName, out object value)
				? (int)value
				: DefaultFrequency;
			int frequency = GetIndexCheckFrequencyValue(userConnection);
			if (frequency != initialFrequency) {
				Register(userConnection, ClassFactory.Get<IAppSchedulerWraper>());
			}
			IKnwSrcIndexationStatusProcessor processor = KnwSrcIndexationStatusProcessor;
			processor.Process(userConnection);
		}

		#endregion

	}

	#endregion

}

