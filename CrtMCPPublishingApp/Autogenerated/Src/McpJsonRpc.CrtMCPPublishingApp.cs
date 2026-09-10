namespace Terrasoft.Configuration
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	#region Class: McpJsonRpcRequest

	/// <summary>
	/// JSON-RPC 2.0 request envelope as parsed from an MCP client.
	/// This type is constructed by <see cref="ToolServiceMcp"/> via a typed
	/// mapping step over <see cref="Dictionary{TKey,TValue}"/>, so it is
	/// never directly deserialized — no JSON-binding attributes needed.
	/// <see cref="IsNotification"/> is set when the original payload had no
	/// <c>id</c> member, in which case the server MUST NOT respond.
	/// <see cref="Id"/> is <see cref="object"/> because JSON-RPC permits
	/// number, string, or null.
	/// </summary>
	internal class McpJsonRpcRequest
	{

		#region Properties: Public

		public string Jsonrpc { get; set; }

		public object Id { get; set; }

		public string Method { get; set; }

		public Dictionary<string, object> Params { get; set; }

		public bool IsNotification { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpJsonRpcResponse

	/// <summary>JSON-RPC 2.0 response envelope. Exactly one of result/error is populated per spec.</summary>
	[DataContract]
	internal class McpJsonRpcResponse
	{

		#region Constructors: Public

		public McpJsonRpcResponse() {
			Jsonrpc = McpJsonRpcConstants.Version;
		}

		#endregion

		#region Properties: Public

		[DataMember(Name = "jsonrpc")]
		public string Jsonrpc { get; set; }

		[DataMember(Name = "id")]
		public object Id { get; set; }

		[DataMember(Name = "result", EmitDefaultValue = false)]
		public object Result { get; set; }

		[DataMember(Name = "error", EmitDefaultValue = false)]
		public McpJsonRpcError Error { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpJsonRpcError

	[DataContract]
	internal class McpJsonRpcError
	{

		#region Properties: Public

		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "data", EmitDefaultValue = false)]
		public object Data { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpJsonRpcConstants

	internal static class McpJsonRpcConstants
	{
		public const string Version = "2.0";
	}

	#endregion

	#region Class: McpJsonRpcErrorCodes

	/// <summary>
	/// Standard JSON-RPC 2.0 error codes plus MCP server-defined codes
	/// (the <c>-32000..-32099</c> server-error range is reserved for
	/// implementation-defined errors).
	/// </summary>
	internal static class McpJsonRpcErrorCodes
	{
		public const int ParseError = -32700;
		public const int InvalidRequest = -32600;
		public const int MethodNotFound = -32601;
		public const int InvalidParams = -32602;
		public const int InternalError = -32603;

		// MCP / server-defined range.
		public const int Unauthenticated = -32001;
		public const int ResourceNotFound = -32002;
		public const int ServerOffline = -32003;
	}

	#endregion

}

