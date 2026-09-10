using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.Runtime
{
	[DataContract]
	public class EntityChangeData
	{
		[DataMember(Name = "entityName")]
		public string EntityName { get; set; }

		[DataMember(Name = "recordId")]
		public string RecordId { get; set; }

		[DataMember(Name = "changeType")]
		public string ChangeType { get; set; }

		[DataMember(Name = "changedFields", EmitDefaultValue = false)]
		public string[] ChangedFields { get; set; }
	}
}

