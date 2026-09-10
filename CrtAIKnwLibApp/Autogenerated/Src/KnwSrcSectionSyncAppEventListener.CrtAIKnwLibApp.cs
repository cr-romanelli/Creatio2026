namespace Creatio.Copilot
{
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;

	#region Class: KnwSrcSectionSyncAppEventListener

	/// <summary>
	/// Registers section synchronization job on application startup.
	/// </summary>
	public sealed class KnwSrcSectionSyncAppEventListener : IAppEventListener
	{

		#region Methods: Private

		private UserConnection GetUserConnection(AppEventContext context) {
			var appConnection = (AppConnection)context.Application["AppConnection"];
			return appConnection.SystemUserConnection;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public void OnAppStart(AppEventContext context) {
			UserConnection userConnection = GetUserConnection(context);
			var appSchedulerWrapper = ClassFactory.Get<IAppSchedulerWraper>();
			var syncJob = ClassFactory.Get<IKnwSrcSectionSyncJobActualizer>();
			syncJob.Register(userConnection, appSchedulerWrapper);
		}

		/// <inheritdoc />
		public void OnAppEnd(AppEventContext context) {
		}

		/// <inheritdoc />
		public void OnSessionStart(AppEventContext context) {
		}

		/// <inheritdoc />
		public void OnSessionEnd(AppEventContext context) {
		}

		#endregion

	}

	#endregion

}

