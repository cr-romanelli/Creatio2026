namespace Creatio.Copilot
{
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Tasks;

	#region Class: KnwIndexingBackgroundTask

	/// <summary>
	/// Background task for executing knowledge source indexing operations.
	/// Executes the synchronous indexing worker in a background task context
	/// to avoid blocking the message consumer thread.
	/// </summary>
	public class KnwIndexingBackgroundTask : IBackgroundTask<KnwStartIndexingCommand>, IUserConnectionRequired
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
		/// Executes the indexing operation for the specified knowledge source.
		/// </summary>
		/// <param name="parameters">The indexing command containing the knowledge source identifier.</param>
		public void Run(KnwStartIndexingCommand parameters) {
			var knwWorker = ClassFactory.Get<IKnwIndexingWorker>();
			knwWorker.Process(_userConnection, parameters);
		}

		#endregion

	}

	#endregion

}

