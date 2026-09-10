using System;
using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "get-event-types-response")]
	public class GetEventTypesResponse
	{
		[DataMember(Name = "triggerCode")]
		public string TriggerCode { get; set; }

		[DataMember(Name = "items")]
		public EventTypeDto[] Items { get; set; }
	}

	[DataContract(Name = "entity-schema-column")]
	public class EntitySchemaColumnDto
	{
		[DataMember(Name = "uId")]
		public Guid UId { get; set; }

		[DataMember(Name = "caption")]
		public string Caption { get; set; }
	}

	[DataContract(Name = "get-entity-schema-columns-response")]
	public class GetEntitySchemaColumnsResponse
	{
		[DataMember(Name = "items")]
		public EntitySchemaColumnDto[] Items { get; set; }
	}
}

