namespace Creatio.Copilot
{

	#region Enum: KnwIndexingStatusEnum

	/// <summary>
	/// Represents lifecycle states of an indexing session. Integer values MUST match KnwIndexingStatus.Value in DB.
	/// Adjust explicit values here only if the database lookup uses different numeric codes.
	/// </summary>
	public enum KnwIndexingStatusEnum
	{
		/// <summary>
		/// The indexing session has been created but not started.
		/// </summary>
		Created = 0,

		/// <summary>
		/// Data is currently being transferred for indexing.
		/// </summary>
		Sending = 1,

		/// <summary>
		/// The indexing is in progress.
		/// </summary>
		Indexing = 2,

		/// <summary>
		/// The indexing session has completed successfully.
		/// </summary>
		Completed = 3,

		/// <summary>
		/// The indexing session has failed.
		/// </summary>
		Failed = 4,

		/// <summary>
		/// The indexing session was cancelled.
		/// </summary>
		Cancelled = 5

	}

	#endregion

	#region Class: KnwIndexingStatusEnumExtensions

	/// <summary>
	/// Provides lifecycle helpers for <see cref="KnwIndexingStatusEnum"/>.
	/// </summary>
	public static class KnwIndexingStatusEnumExtensions
	{

		#region Methods: Public

		/// <summary>
		/// Determines whether indexing session state is terminal.
		/// </summary>
		/// <param name="state">Indexing session state.</param>
		/// <returns>True when the state is terminal.</returns>
		public static bool IsTerminal(this KnwIndexingStatusEnum state) {
			return state == KnwIndexingStatusEnum.Completed
				|| state == KnwIndexingStatusEnum.Failed
				|| state == KnwIndexingStatusEnum.Cancelled;
		}

		/// <summary>
		/// Determines whether indexing session state is active.
		/// </summary>
		/// <param name="state">Indexing session state.</param>
		/// <returns>True when the state is active.</returns>
		public static bool IsActive(this KnwIndexingStatusEnum state) {
			return !state.IsTerminal();
		}

		#endregion

	}

	#endregion

}

