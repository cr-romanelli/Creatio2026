namespace Creatio.Copilot {
	using System;
	using System.Runtime.Serialization;

	#region Class: KnwIndexingSessionRecord

	/// <summary>
	/// Represents a snapshot of a knowledge source indexing session, including its state and key timestamps.
	/// </summary>
	[Serializable]
	[DataContract]
	public sealed class KnwIndexingSessionRecord {

		#region Properties: Public

		/// <summary>
		/// Gets or sets the unique identifier of the indexing session record.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the associated knowledge source.
		/// </summary>
		public Guid KnwSourceId { get; set; }

		/// <summary>
		/// Gets or sets the current state of the indexing session.
		/// </summary>
		public KnwIndexingStatusEnum State { get; set; }

		/// <summary>
		/// Gets or sets the UTC date and time when the session started.
		/// </summary>
		public DateTime? StartedOn { get; set; }

		/// <summary>
		/// Gets or sets the UTC date and time when the data transfer was completed.
		/// </summary>
		public DateTime? TransferredOn { get; set; }

		/// <summary>
		/// Gets or sets the UTC date and time when the session was completed.
		/// </summary>
		public DateTime? CompletedOn { get; set; }

		/// <summary>
		/// Gets or sets the remote session identifier from the external service.
		/// </summary>
		public Guid SessionId { get; set; }

		/// <summary>
		/// Gets or sets the remote index identifier from the external service.
		/// </summary>
		public Guid IndexId { get; set; }

		/// <summary>
		/// Gets or sets the UTC date and time when the session was last modified (checked by the job).
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		#endregion

	}

	#endregion

}

