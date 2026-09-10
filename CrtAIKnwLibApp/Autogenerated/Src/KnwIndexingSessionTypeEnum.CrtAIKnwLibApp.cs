namespace Creatio.Copilot
{

	#region Enum: KnwIndexingSessionTypeEnum

	/// <summary>
	/// Represents indexing session type for remote session creation.
	/// </summary>
	public enum KnwIndexingSessionTypeEnum
	{

		/// <summary>
		/// Full indexing session, used for manual indexing.
		/// </summary>
		Initial = 1,

		/// <summary>
		/// Incremental indexing session, used for sync updates.
		/// </summary>
		Incremental = 2

	}

	#endregion

}

