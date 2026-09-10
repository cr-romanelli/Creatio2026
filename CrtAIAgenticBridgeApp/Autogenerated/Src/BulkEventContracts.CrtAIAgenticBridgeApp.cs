namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// APD-1464 R6/S3: bridge-side wire contracts for the bulk trigger-events
	/// ingress endpoint. Mirrors the control-plane
	/// <c>BulkEventOccurrenceRequest</c> / <c>BulkEventOccurrenceResponseDto</c>
	/// shapes so the bridge can serialize a batch and read the per-event
	/// accept/reject result array.
	/// </summary>
	[DataContract]
	public class BulkEventOccurrenceItem<TData>
	{
		// Bridge-assigned correlation id so the per-event response can be matched
		// back and only the rejected subset retried (exactly-once per R5).
		[DataMember(Name = "eventId")]
		public string EventId { get; set; }

		[DataMember(Name = "eventTypeCode")]
		public string EventTypeCode { get; set; }

		[DataMember(Name = "occurredAt")]
		public DateTime OccurredAt { get; set; }

		[DataMember(Name = "data")]
		public TData Data { get; set; }
	}

	[DataContract]
	public class BulkEventOccurrenceRequest<TData>
	{
		[DataMember(Name = "events")]
		public List<BulkEventOccurrenceItem<TData>> Events { get; set; }
			= new List<BulkEventOccurrenceItem<TData>>();
	}

	[DataContract]
	public class BulkEventResultItem
	{
		[DataMember(Name = "eventId")]
		public string EventId { get; set; }

		[DataMember(Name = "accepted")]
		public bool Accepted { get; set; }

		[DataMember(Name = "rejectionCode")]
		public string RejectionCode { get; set; }

		[DataMember(Name = "rejectionMessage")]
		public string RejectionMessage { get; set; }

		// When false the rejection is terminal (e.g. unknown event type / invalid
		// payload) and the bridge must NOT retry it, so a poison event can never
		// drive a retry storm.
		[DataMember(Name = "retryable")]
		public bool Retryable { get; set; }
	}

	[DataContract]
	public class BulkEventOccurrenceResponseDto
	{
		[DataMember(Name = "acceptedCount")]
		public int AcceptedCount { get; set; }

		[DataMember(Name = "rejectedCount")]
		public int RejectedCount { get; set; }

		[DataMember(Name = "results")]
		public List<BulkEventResultItem> Results { get; set; }
			= new List<BulkEventResultItem>();
	}

	[DataContract]
	internal class BulkEventOccurrenceResponseEnvelope
	{
		[DataMember(Name = "data")]
		public BulkEventOccurrenceResponseDto Data { get; set; }
	}
}

