namespace Terrasoft.Configuration
{
	#region Class: McpServerInfo

	/// <summary>
	/// Server identity reported in the MCP <c>initialize</c> response.
	/// Protocol version follows the MCP 2025-03-26 spec (Streamable HTTP
	/// transport with JSON-RPC 2.0 framing).
	/// </summary>
	internal static class McpServerInfo
	{
		public const string Name = "Creatio MCP Publishing";
		public const string Version = "1.0.0";
		public const string ProtocolVersion = "2025-03-26";
	}

	#endregion

}

