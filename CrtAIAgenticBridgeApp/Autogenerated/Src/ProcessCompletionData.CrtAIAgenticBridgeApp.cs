using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.Runtime
{
	[DataContract]
	public class ProcessCompletionData
	{
		[DataMember(Name = "correlationId")]
		public string CorrelationId { get; set; }

		[DataMember(Name = "processId")]
		public Guid ProcessId { get; set; }

		[DataMember(Name = "schemaName")]
		public string SchemaName { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		[DataMember(Name = "failedElement", EmitDefaultValue = false)]
		public string FailedElementName { get; set; }

		[DataMember(Name = "resultParameters", EmitDefaultValue = false)]
		public IDictionary<string, object> ResultParameters { get; set; }
	}
}

