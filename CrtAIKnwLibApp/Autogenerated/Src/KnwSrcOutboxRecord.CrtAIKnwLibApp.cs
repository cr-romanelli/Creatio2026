namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;

	#region Class: KnwSrcOutboxRecord

	/// <summary>
	/// Represents a synchronization outbox row for a single source record.
	/// </summary>
	internal sealed class KnwSrcOutboxRecord
	{

		#region Constructors: Internal

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwSrcOutboxRecord"/> class.
		/// </summary>
		/// <param name="id">Outbox row identifier.</param>
		/// <param name="knwSourceId">Knowledge source identifier.</param>
		/// <param name="recordId">Observed section record identifier.</param>
		/// <param name="entitySchemaUId">Observed entity schema identifier.</param>
		/// <param name="eventType">Latest merged event type.</param>
		/// <param name="changedColumns">Latest changed column set.</param>
		/// <param name="eventOn">Latest event timestamp in UTC.</param>
		/// <param name="isProcessed">Processing completion flag.</param>
		/// <param name="processedOn">Processing completion timestamp in UTC.</param>
		/// <param name="attempts">Failed processing attempts count.</param>
		/// <param name="lastError">Last processing error.</param>
		/// <param name="nextAttemptOn">Next eligible processing time in UTC.</param>
		/// <param name="leaseUntil">Current lease expiration in UTC.</param>
		/// <param name="processingToken">Lease owner token.</param>
		/// <param name="deadLetteredOn">Dead-letter timestamp in UTC.</param>
		internal KnwSrcOutboxRecord(Guid id, Guid knwSourceId, Guid recordId, Guid entitySchemaUId,
				KnwSrcOutboxEventType eventType, ISet<string> changedColumns, DateTime eventOn,
				bool isProcessed = false, DateTime? processedOn = null, int attempts = 0, string lastError = null,
				DateTime? nextAttemptOn = null, DateTime? leaseUntil = null, string processingToken = null,
				DateTime? deadLetteredOn = null) {
			Id = id;
			KnwSourceId = knwSourceId;
			RecordId = recordId;
			EntitySchemaUId = entitySchemaUId;
			EventType = eventType;
			ChangedColumns = NormalizeColumns(changedColumns);
			EventOn = eventOn;
			IsProcessed = isProcessed;
			ProcessedOn = processedOn;
			Attempts = attempts;
			LastError = lastError;
			NextAttemptOn = nextAttemptOn;
			LeaseUntil = leaseUntil;
			ProcessingToken = processingToken;
			DeadLetteredOn = deadLetteredOn;
		}

		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets outbox row identifier.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets knowledge source identifier.
		/// </summary>
		public Guid KnwSourceId { get; }

		/// <summary>
		/// Gets observed section record identifier.
		/// </summary>
		public Guid RecordId { get; }

		/// <summary>
		/// Gets observed entity schema identifier.
		/// </summary>
		public Guid EntitySchemaUId { get; private set; }

		/// <summary>
		/// Gets merged event type.
		/// </summary>
		public KnwSrcOutboxEventType EventType { get; private set; }

		/// <summary>
		/// Gets latest changed columns.
		/// </summary>
		public ISet<string> ChangedColumns { get; private set; }

		/// <summary>
		/// Gets latest event timestamp in UTC.
		/// </summary>
		public DateTime EventOn { get; private set; }

		/// <summary>
		/// Gets a value indicating whether row was processed.
		/// </summary>
		public bool IsProcessed { get; private set; }

		/// <summary>
		/// Gets processing completion timestamp in UTC.
		/// </summary>
		public DateTime? ProcessedOn { get; private set; }

		/// <summary>
		/// Gets failed processing attempts count.
		/// </summary>
		public int Attempts { get; private set; }

		/// <summary>
		/// Gets last processing error.
		/// </summary>
		public string LastError { get; private set; }

		/// <summary>
		/// Gets next eligible processing time in UTC.
		/// </summary>
		public DateTime? NextAttemptOn { get; private set; }

		/// <summary>
		/// Gets lease expiration in UTC.
		/// </summary>
		public DateTime? LeaseUntil { get; private set; }

		/// <summary>
		/// Gets lease owner token.
		/// </summary>
		public string ProcessingToken { get; private set; }

		/// <summary>
		/// Gets dead-letter timestamp in UTC.
		/// </summary>
		public DateTime? DeadLetteredOn { get; private set; }

		#endregion

		#region Methods: Private

		private static ISet<string> NormalizeColumns(ISet<string> changedColumns) {
			return changedColumns == null
				? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				: new HashSet<string>(changedColumns, StringComparer.OrdinalIgnoreCase);
		}

		private static KnwSrcOutboxEventType MergeEventType(KnwSrcOutboxEventType existingType,
				KnwSrcOutboxEventType incomingType) {
			if (incomingType == KnwSrcOutboxEventType.Delete) {
				return KnwSrcOutboxEventType.Delete;
			}
			if (incomingType == KnwSrcOutboxEventType.Update && existingType == KnwSrcOutboxEventType.Create) {
				return KnwSrcOutboxEventType.Create;
			}
			if (incomingType == KnwSrcOutboxEventType.Create && existingType == KnwSrcOutboxEventType.Update) {
				return KnwSrcOutboxEventType.Update;
			}
			if (incomingType == KnwSrcOutboxEventType.Create && existingType == KnwSrcOutboxEventType.Delete) {
				return KnwSrcOutboxEventType.Update;
			}
			return incomingType;
		}

		#endregion

		#region Methods: Internal

		/// <summary>
		/// Merges incoming event data into this record.
		/// </summary>
		internal void MergeIncomingEvent(Guid entitySchemaUId, KnwSrcOutboxEventType eventType,
				ISet<string> changedColumns, DateTime eventOnUtc) {
			EventType = MergeEventType(EventType, eventType);
			EntitySchemaUId = entitySchemaUId;
			ChangedColumns = NormalizeColumns(changedColumns);
			EventOn = eventOnUtc;
			if (IsProcessed && string.IsNullOrWhiteSpace(ProcessingToken)) {
				IsProcessed = false;
				ProcessedOn = null;
				Attempts = 0;
				LastError = null;
				NextAttemptOn = null;
				LeaseUntil = null;
			}
			if (DeadLetteredOn.HasValue && string.IsNullOrWhiteSpace(ProcessingToken)) {
				DeadLetteredOn = null;
				Attempts = 0;
				LastError = null;
				NextAttemptOn = null;
				LeaseUntil = null;
			}
		}

		/// <summary>
		/// Determines whether the record can be leased for processing.
		/// </summary>
		internal bool CanBeClaimed(DateTime utcNow) {
			return !IsProcessed
				&& !DeadLetteredOn.HasValue
				&& (!NextAttemptOn.HasValue || NextAttemptOn.Value <= utcNow)
				&& (!LeaseUntil.HasValue || LeaseUntil.Value < utcNow);
		}

		/// <summary>
		/// Marks the record as leased by a processor token.
		/// </summary>
		internal void MarkClaimed(string processingToken, DateTime leaseUntilUtc) {
			ProcessingToken = processingToken;
			LeaseUntil = leaseUntilUtc;
		}

		/// <summary>
		/// Marks the record as processed successfully.
		/// </summary>
		internal void MarkProcessed(DateTime processedOnUtc) {
			IsProcessed = true;
			ProcessedOn = processedOnUtc;
			LastError = null;
			NextAttemptOn = null;
			LeaseUntil = null;
			ProcessingToken = null;
			DeadLetteredOn = null;
		}

		/// <summary>
		/// Marks the record as failed and schedules a retry.
		/// </summary>
		internal void MarkFailed(string error, DateTime nextAttemptOnUtc) {
			Attempts++;
			LastError = error;
			NextAttemptOn = nextAttemptOnUtc;
			LeaseUntil = null;
			ProcessingToken = null;
			IsProcessed = false;
			ProcessedOn = null;
		}

		/// <summary>
		/// Marks the record as dead-lettered.
		/// </summary>
		internal void MarkDeadLettered(DateTime deadLetteredOnUtc, string error) {
			Attempts++;
			LastError = error;
			NextAttemptOn = null;
			LeaseUntil = null;
			ProcessingToken = null;
			IsProcessed = false;
			ProcessedOn = null;
			DeadLetteredOn = deadLetteredOnUtc;
		}

		/// <summary>
		/// Releases row lease without processing state change.
		/// </summary>
		internal void ReleaseLease() {
			LeaseUntil = null;
			ProcessingToken = null;
		}

		/// <summary>
		/// Creates a detached copy.
		/// </summary>
		internal KnwSrcOutboxRecord Clone() {
			return new KnwSrcOutboxRecord(Id, KnwSourceId, RecordId, EntitySchemaUId, EventType, ChangedColumns, EventOn,
				IsProcessed, ProcessedOn, Attempts, LastError, NextAttemptOn, LeaseUntil, ProcessingToken, DeadLetteredOn);
		}

		#endregion

	}

	#endregion

}

