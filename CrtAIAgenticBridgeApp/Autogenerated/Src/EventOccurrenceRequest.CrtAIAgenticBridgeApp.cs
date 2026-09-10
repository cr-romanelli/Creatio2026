using System;
using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.Runtime
{
	[DataContract]
	public class EventOccurrenceRequest<TData>
	{
		[IgnoreDataMember]
		public string TriggerCode { get; set; }

		[DataMember(Name = "eventTypeCode")]
		public string EventTypeCode { get; set; }

		[DataMember(Name = "occurredAt")]
		public DateTime OccurredAt { get; set; }

		[DataMember(Name = "data")]
		public TData Data { get; set; }
	}
}

