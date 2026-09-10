namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;

	#region Interface: IMcpServerRepository

	/// <summary>
	/// Persistence boundary for MCP servers (<c>McpServer</c>) and their
	/// scoped tools (<c>McpTool</c>, 1:N via <c>McpServer</c>). Tools are
	/// projected onto <see cref="McpToolModel"/> (a 1:1 mirror of the
	/// <c>McpTool</c> columns) for both the runtime <c>tools/list</c> /
	/// <c>tools/call</c> pipeline and management.
	/// </summary>
	internal interface IMcpServerRepository
	{

		#region Methods: Public

		McpServerModel GetServerByCode(string code);

		IReadOnlyCollection<McpToolModel> GetEnabledToolsForServer(Guid serverId);

		McpToolModel GetToolForServerByName(Guid serverId, string externalToolName);

		#endregion

	}

	#endregion

}

