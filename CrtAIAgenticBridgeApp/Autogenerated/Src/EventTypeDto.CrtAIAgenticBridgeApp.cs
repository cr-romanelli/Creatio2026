using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "event-type")]
	public class EventTypeDto
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "entitySchemaFilters", EmitDefaultValue = false)]
		public string EntitySchemaFilters { get; set; }
	}
}

