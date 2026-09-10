using System.Runtime.Serialization;

namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto
{
	[DataContract(Name = "error-response")]
	public class ErrorResponse
	{
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }
	}
}

