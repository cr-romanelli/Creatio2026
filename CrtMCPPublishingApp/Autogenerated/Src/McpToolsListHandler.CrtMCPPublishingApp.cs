using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using Terrasoft.Core;

	#region Class: McpToolsListHandler

	/// <summary>
	/// Handles MCP <c>tools/list</c>. Delegates to
	/// <see cref="IMcpToolsListService"/> and returns the MCP wire shape
	/// <c>{ "tools": [ ... ] }</c>. Each <see cref="McpToolDescriptor"/> is decorated
	/// with <see cref="System.Runtime.Serialization.DataContractAttribute"/> +
	/// <see cref="System.Runtime.Serialization.DataMemberAttribute"/> so
	/// Newtonsoft serializes it to <c>{ name, description, inputSchema,
	/// outputSchema?, annotations? }</c> matching the MCP tool descriptor.
	/// </summary>
	internal class McpToolsListHandler : IMcpMethodHandler
	{

		#region Fields: Private

		private readonly IMcpToolsListService _listService;

		#endregion

		#region Constructors: Public

		public McpToolsListHandler(UserConnection userConnection, Guid serverId) {
			_listService = ClassFactory.Get<IMcpToolsListService>(
				new ConstructorArgument("userConnection", userConnection),
				new ConstructorArgument("serverId", serverId));
		}

		internal McpToolsListHandler(IMcpToolsListService listService) {
			_listService = listService;
		}

		#endregion

		#region Methods: Public

		public McpHandlerOutcome Handle(Dictionary<string, object> parameters) {
			IReadOnlyList<McpToolDescriptor> tools = _listService.GetTools();
			var result = new Dictionary<string, object> {
				["tools"] = tools
			};
			return McpHandlerOutcome.Success(result);
		}

		#endregion

	}

	#endregion

}

