namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using Common.Logging;
	using Creatio.FeatureToggling;
	using Terrasoft.Core;
	using Terrasoft.Web.Common;

	public class TriggerCatalogAppEventListener : AppEventListenerBase
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		public override void OnAppStart(AppEventContext context) {
			try {
				var isFeatureEnabled = Features.GetIsEnabled<EnableAIPlatformOutboundTriggerDelivery>();
				if (!isFeatureEnabled) {
					return;
				}
				UserConnection userConnection = GetSystemUserConnection(context);
				EnqueueTriggerCatalogSync(userConnection);
			} catch (Exception ex) {
				Log.Error("Failed to synchronize trigger catalog on application start.", ex);
			}
		}

		public override void OnAppEnd(AppEventContext context) {
			try {
				// APD-1464: drain any events still buffered in the outbound batcher so
				// a graceful app-pool recycle / redeploy / stop does not silently lose
				// the current bulkSendPeriod window. This runs synchronously on app end,
				// so the drain is time-bounded (ShutdownFlushBudget) inside the batcher:
				// an unhealthy endpoint cannot stall shutdown on outbound backoff —
				// undelivered events are logged instead. No-op when the feature is off
				// or nothing has been enqueued.
				OutboundEventBatcher.Current.FlushPendingForShutdown();
			} catch (Exception ex) {
				Log.Error("Failed to drain outbound trigger-event batcher on application end.", ex);
			}
		}

		internal virtual UserConnection GetSystemUserConnection(AppEventContext context) {
			AppConnection appConnection = (AppConnection)context.Application["AppConnection"];
			return appConnection.SystemUserConnection;
		}

		internal virtual void EnqueueTriggerCatalogSync(UserConnection userConnection) {
			TriggerCatalogStartupSyncJob.Schedule(userConnection);
		}
	}
}

