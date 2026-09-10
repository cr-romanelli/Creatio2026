using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terrasoft.Core.ServiceModelContract;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices
{
	[DataContract]
	public class RunAgenticProcessResponse : BaseResponse
	{
		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "correlationId", EmitDefaultValue = false)]
		public string CorrelationId { get; set; }

		[DataMember(Name = "expiresAt", EmitDefaultValue = false)]
		public DateTime? ExpiresAt { get; set; }

		[DataMember(Name = "resultParameterValues", EmitDefaultValue = false)]
		public Dictionary<string, object> ResultParameterValues { get; set; }

		[DataMember(Name = "processId", EmitDefaultValue = false)]
		public Guid ProcessId { get; set; }

		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		[DataMember(Name = "failedElement", EmitDefaultValue = false)]
		public string FailedElement { get; set; }

		/// <summary>
		/// Newtonsoft ignores <c>EmitDefaultValue = false</c> for value types, so this mirrors it for
		/// <see cref="ProcessId"/> — an empty id is omitted from the serialized payload.
		/// </summary>
		public bool ShouldSerializeProcessId() {
			return ProcessId != Guid.Empty;
		}
	}
}

