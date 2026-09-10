using Common.Logging;
using Creatio.FeatureToggling;
using Terrasoft.Core;
using Terrasoft.Core.Factories;
using Terrasoft.Core.Tasks;

namespace CrtAIAgenticBridgeApp.Runtime
{
	internal class DispatchEntityChangeAsyncOperation
		: IBackgroundTask<EntityChangeDispatchArgs>, IUserConnectionRequired
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");
		private UserConnection _userConnection;

		public void Run(EntityChangeDispatchArgs arguments) {
			var isFeatureEnabled = Features.GetIsEnabled<EnableAIPlatformOutboundTriggerDelivery>();
			if (!isFeatureEnabled) {
				Log.Debug("Feature disabled, skipping dispatch in async operation.");
				return;
			}
			var dispatcher = ClassFactory.Get<IEventDispatcher>(
				new ConstructorArgument("userConnection", _userConnection));
			dispatcher.Dispatch(arguments);
		}

		public void SetUserConnection(UserConnection userConnection) {
			_userConnection = userConnection;
		}
	}
}

