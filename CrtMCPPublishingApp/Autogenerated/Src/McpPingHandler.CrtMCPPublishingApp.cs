namespace Terrasoft.Configuration
{
	using System.Collections.Generic;

	#region Class: McpPingHandler

	/// <summary>
	/// Handles MCP <c>ping</c>. Returns an empty object per spec; clients
	/// use this to verify the connection is alive.
	/// </summary>
	internal class McpPingHandler : IMcpMethodHandler
	{

		#region Methods: Public

		public McpHandlerOutcome Handle(Dictionary<string, object> parameters) {
			return McpHandlerOutcome.Success(new Dictionary<string, object>());
		}

		#endregion

	}

	#endregion

}

