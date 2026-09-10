namespace Terrasoft.Configuration
{
	using System.Collections.Generic;

	#region Class: McpInitializeHandler

	/// <summary>
	/// Handles MCP <c>initialize</c>. Returns server identity, protocol
	/// version, and capability set. The composable app advertises only the
	/// <c>tools</c> capability today; <c>prompts</c> and <c>resources</c>
	/// are deferred (see APD-92, closed Won't Fix for the MVP slice).
	/// </summary>
	internal class McpInitializeHandler : IMcpMethodHandler
	{

		#region Fields: Private

		private readonly string _serverName;

		#endregion

		#region Constructors: Public

		public McpInitializeHandler() {
		}

		/// <summary>
		/// Per-server <c>initialize</c>: advertises the specific MCP server's
		/// title as <c>serverInfo.name</c> so each <c>/{code}/v1/mcp</c>
		/// endpoint identifies itself distinctly to clients.
		/// </summary>
		public McpInitializeHandler(string serverName) {
			_serverName = serverName;
		}

		#endregion

		#region Methods: Public

		public McpHandlerOutcome Handle(Dictionary<string, object> parameters) {
			var result = new Dictionary<string, object> {
				["protocolVersion"] = McpServerInfo.ProtocolVersion,
				["capabilities"] = new Dictionary<string, object> {
					["tools"] = new Dictionary<string, object> {
						["listChanged"] = false
					}
				},
				["serverInfo"] = new Dictionary<string, object> {
					["name"] = string.IsNullOrWhiteSpace(_serverName) ? McpServerInfo.Name : _serverName,
					["version"] = McpServerInfo.Version
				}
			};
			return McpHandlerOutcome.Success(result);
		}

		#endregion

	}

	#endregion

}

