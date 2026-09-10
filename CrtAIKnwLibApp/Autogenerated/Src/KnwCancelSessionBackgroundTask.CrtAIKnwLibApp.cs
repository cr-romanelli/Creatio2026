namespace Creatio.Copilot
{
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Tasks;

	#region Class: KnwCancelSessionBackgroundTask

	/// <summary>
	/// Background task for executing knowledge source indexing session cancellation.
	/// Executes the cancellation operation in a background task context.
	/// </summary>
	public class KnwCancelSessionBackgroundTask : IBackgroundTask<KnwCancelSessionCommand>, IUserConnectionRequired
	{

		#region Fields: Private

		private UserConnection _userConnection;

		#endregion

		#region Methods: Public

		/// <summary>
		/// Sets the user connection for this background task.
		/// Called automatically by the Creatio platform before <see cref="Run"/> is invoked.
		/// </summary>
		/// <param name="userConnection">The user connection instance.</param>
		public void SetUserConnection(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		/// <summary>
		/// Executes the cancellation operation for the specified indexing session.
		/// </summary>
		/// <param name="parameters">The cancellation command containing the session and knowledge source identifiers.</param>
		public void Run(KnwCancelSessionCommand parameters) {
			var knwIndexingSessionManager = ClassFactory.Get<IKnwIndexingSessionManager>(
				new ConstructorArgument("userConnection", _userConnection));
			var knwSourceManager = ClassFactory.Get<IKnwSourceManager>(
				new ConstructorArgument("userConnection", _userConnection));

			var knwIndexingCancellationManager = ClassFactory.Get<IKnwIndexingCancellationManager>(
				new ConstructorArgument("sessionManager", knwIndexingSessionManager),
				new ConstructorArgument("sourceManager", knwSourceManager));

			knwIndexingCancellationManager.Cancel(parameters.KnwSourceId);
		}

		#endregion

	}

	#endregion

}

