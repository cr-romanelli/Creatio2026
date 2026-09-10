using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "get-triggers-response")]
	public class GetTriggersResponse
	{
		[DataMember(Name = "items")]
		public TriggerMetadataDto[] Items { get; set; }
	}
}

