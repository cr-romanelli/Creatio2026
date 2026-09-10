using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "trigger-field")]
	public class TriggerFieldDto
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "dataType")]
		public string DataType { get; set; }

		[DataMember(Name = "isRequired")]
		public bool IsRequired { get; set; }

		[DataMember(Name = "enumValues", EmitDefaultValue = false)]
		public string[] EnumValues { get; set; }

		[DataMember(Name = "items", EmitDefaultValue = false)]
		public TriggerFieldDto Items { get; set; }

		[DataMember(Name = "fields", EmitDefaultValue = false)]
		public TriggerFieldDto[] Fields { get; set; }
	}
}

