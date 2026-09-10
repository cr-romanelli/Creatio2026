namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using Common.Logging;
	using Creatio.FeatureToggling;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Tasks;

	internal sealed class TriggerCatalogDispatchArgs
	{
		public string TriggerCode { get; set; }

		public static TriggerCatalogDispatchArgs CreateUpdateTrigger(string triggerCode) {
			return new TriggerCatalogDispatchArgs {
				TriggerCode = triggerCode
			};
		}
	}

	internal class TriggerCatalogDispatchAsyncOperation
		: IBackgroundTask<TriggerCatalogDispatchArgs>, IUserConnectionRequired
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private UserConnection _userConnection;

		internal static void Schedule(TriggerCatalogDispatchArgs arguments) {
			Task.StartNewWithUserConnection<TriggerCatalogDispatchAsyncOperation,
				TriggerCatalogDispatchArgs>(arguments);
		}

		public void Run(TriggerCatalogDispatchArgs arguments) {
			try {
				if (!Features.GetIsEnabled<EnableAIPlatformOutboundTriggerDelivery>()) {
					return;
				}
				var sender = ClassFactory.Get<ITriggerCatalogSender>(
					new ConstructorArgument("userConnection", _userConnection));
				string triggerCode = arguments?.TriggerCode;
				if (!string.IsNullOrWhiteSpace(triggerCode)) {
					sender.UpdateTrigger(triggerCode);
				}
			} catch (Exception ex) {
				Log.Error("Failed to dispatch trigger catalog update.", ex);
			}
		}

		public void SetUserConnection(UserConnection userConnection) {
			_userConnection = userConnection;
		}
	}
}

