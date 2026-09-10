namespace Terrasoft.Configuration
{
	using System;

	#region Class: McpServerModel

	/// <summary>
	/// Transport-agnostic data shape for a configured MCP server
	/// (<c>McpServer</c>). A server owns its own tool list (1:N via
	/// <c>McpTool.McpServer</c>); consumers connect to a server by its
	/// <see cref="Code"/> path segment (<c>/{Code}/v1/mcp</c>).
	/// </summary>
	internal class McpServerModel
	{

		#region Properties: Public

		public Guid UId { get; set; }

		public string Title { get; set; }

		public string Code { get; set; }

		public string Description { get; set; }

		public bool IsOnline { get; set; }

		#endregion

	}

	#endregion

}

