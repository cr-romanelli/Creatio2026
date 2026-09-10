namespace Creatio.Copilot
{

	#region Enum: KnwSourceStatusEnum

	/// <summary>
	/// Represents the status of a knowledge source in the system.
	/// </summary>
	public enum KnwSourceStatusEnum
	{

		/// <summary>
		/// The knowledge source is newly created and not yet indexed.
		/// </summary>
		New = 1,

		/// <summary>
		/// The knowledge source is currently being indexed.
		/// </summary>
		Indexing = 2,

		/// <summary>
		/// The knowledge source is available for use.
		/// </summary>
		Available = 3,

		/// <summary>
		/// The indexing or processing of the knowledge source was cancelled.
		/// </summary>
		Cancelled = 4,

		/// <summary>
		/// The index is unavailable for retrieval and incremental updates.
		/// </summary>
		Unavailable = 5

	}

	#endregion

}

