namespace Creatio.Copilot
{

	#region Enum: KnwSrcOutboxEventType

	/// <summary>
	/// Describes section change event type persisted in synchronization outbox.
	/// </summary>
	internal enum KnwSrcOutboxEventType
	{

		/// <summary>
		/// New record was created.
		/// </summary>
		Create = 1,

		/// <summary>
		/// Existing record was updated.
		/// </summary>
		Update = 2,

		/// <summary>
		/// Existing record was deleted.
		/// </summary>
		Delete = 3

	}

	#endregion

}

