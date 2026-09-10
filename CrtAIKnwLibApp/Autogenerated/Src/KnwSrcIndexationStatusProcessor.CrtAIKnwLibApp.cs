namespace Creatio.Copilot
{
	using System;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSrcIndexationStatusProcessor

	/// <summary>
	/// Defines a processor for handling Knowledge Source indexation status logic.
	/// </summary>
	internal interface IKnwSrcIndexationStatusProcessor
	{

		#region Methods: Internal

		/// <summary>
		/// Processes the indexation status for Knowledge Sources.
		/// </summary>
		/// <param name="userConnection">The user connection context.</param>
		void Process(UserConnection userConnection);

		#endregion

	}

	#endregion

	#region Class: KnwSrcIndexationStatusProcessor

	/// <summary>
	/// Handles Knowledge Source indexation status processing logic.
	/// </summary>
	internal class KnwSrcIndexationStatusProcessor : IKnwSrcIndexationStatusProcessor
	{

		#region Methods: Private

		private static void ProcessSource(Guid knwSourceId,
				IKnwIndexingSessionManager sessionManager) {
			try {
				sessionManager.SyncLatestIndexingStatus(knwSourceId, CancellationToken.None);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error($"Error processing indexing status for source {knwSourceId}", e);
			}
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public void Process(UserConnection userConnection) {
			var sessionManager = ClassFactory.Get<IKnwIndexingSessionManager>();
			KnwIndexingSessionRecord[] activeSessions = sessionManager.GetActiveSessions().ToArray();
			if (!activeSessions.Any()) {
				return;
			}
			foreach (Guid sourceId in activeSessions.Select(session => session.KnwSourceId).Distinct()) {
				ProcessSource(sourceId, sessionManager);
			}
		}

		#endregion

	}

	#endregion

}

