namespace Creatio.Copilot
{
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;

	#region Class: ActualizeKnwSrcEventListener

	/// <summary>
	/// Event listener for Knowledge Source indexation status actualization job.
	/// </summary>
	public sealed class ActualizeKnwSrcEventListener : IAppEventListener
	{

		#region Methods: Private

		private UserConnection GetUserConnection(AppEventContext context) {
			var appConnection = (AppConnection)context.Application["AppConnection"];
			return appConnection.SystemUserConnection;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc cref="Terrasoft.Web.Common.IAppEventListener.OnAppStart"/>
		public void OnAppStart(AppEventContext context) {
			UserConnection userConnection = GetUserConnection(context);
			var appSchedulerWrapper = ClassFactory.Get<IAppSchedulerWraper>();
			var indexationStatusJob = ClassFactory.Get<IKnwSrcIndexationStatusJobActualizer>();
			indexationStatusJob.Register(userConnection, appSchedulerWrapper);
			var deferredIndexationJob = ClassFactory.Get<IKnwDeferredIndexationJobDispatcher>();
			deferredIndexationJob.Register(userConnection, appSchedulerWrapper);
		}

		/// <inheritdoc cref="Terrasoft.Web.Common.IAppEventListener.OnAppEnd"/>
		public void OnAppEnd(AppEventContext context) {
		}

		/// <inheritdoc cref="Terrasoft.Web.Common.IAppEventListener.OnSessionStart"/>
		public void OnSessionStart(AppEventContext context) {
		}

		/// <inheritdoc cref="Terrasoft.Web.Common.IAppEventListener.OnSessionEnd"/>
		public void OnSessionEnd(AppEventContext context) {
		}

		#endregion

	}

	#endregion

}

