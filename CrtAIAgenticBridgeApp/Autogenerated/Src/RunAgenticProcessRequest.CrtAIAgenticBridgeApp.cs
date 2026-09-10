using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Terrasoft.Core.ServiceModelContract;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices
{
	[DataContract]
	public class RunAgenticProcessRequest
	{
		[DataMember(Name = "schemaName")]
		public string SchemaName { get; set; }

		[DataMember(Name = "resultParameterNames", EmitDefaultValue = false)]
		public string[] ResultParameterNames { get; set; }

		[DataMember(Name = "parameterValues", EmitDefaultValue = false)]
		public Collection<NameValuePair> ParameterValues { get; set; }

		[DataMember(Name = "correlationId", EmitDefaultValue = false)]
		public string CorrelationId { get; set; }
	}
}

