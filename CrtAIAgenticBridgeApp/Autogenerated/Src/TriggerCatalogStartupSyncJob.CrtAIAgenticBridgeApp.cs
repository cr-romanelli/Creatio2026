namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using Common.Logging;
	using Creatio.FeatureToggling;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;

	public class TriggerCatalogStartupSyncJob : IJobExecutor
	{
		private static ILog Log => LogManager.GetLogger("CrtAIAgenticBridgeApp");
		private const string JobName = "AIPlatformTriggerCatalogStartupSync";
		private const string JobGroup = "AIPlatformTriggerCatalog";

		internal static void Schedule(UserConnection userConnection) {
			var schedulerWrapper = ClassFactory.Get<IAppSchedulerWraper>();
			if (schedulerWrapper.DoesJobExist(JobName, JobGroup)) {
				return;
			}
			schedulerWrapper.ScheduleImmediateJob<TriggerCatalogStartupSyncJob>(
				JobName,
				JobGroup,
				userConnection.CurrentUser.Name,
				null,
				isSystemUser: true);
		}

		public void Execute(UserConnection userConnection, IDictionary<string, object> parameters) {
			try {
				if (!Features.GetIsEnabled<EnableAIPlatformOutboundTriggerDelivery>()) {
					return;
				}
				var sender = ClassFactory.Get<ITriggerCatalogSender>(
					new ConstructorArgument("userConnection", userConnection));
				TriggerPushSyncResponseDto syncResult = sender.SyncAll();
				if (syncResult != null) {
					Log.Info($"Startup trigger catalog synchronization finished at " +
						$"{syncResult.SyncedAt:O}. Triggers added/updated: " +
						$"{syncResult.TriggersUpserted}, removed: {syncResult.TriggersRemoved}. " +
						$"Event types added/updated: {syncResult.EventTypesUpserted}, removed: " +
						$"{syncResult.EventTypesRemoved}.");
				}
			} catch (Exception ex) {
				Log.Error("Failed to synchronize trigger catalog in the startup background job.", ex);
			}
		}
	}
}

