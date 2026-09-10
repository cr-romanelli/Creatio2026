using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "trigger-metadata")]
	public class TriggerMetadataDto
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "fields")]
		public TriggerFieldDto[] Fields { get; set; }
	}
}

