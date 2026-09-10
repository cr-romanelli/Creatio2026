namespace Terrasoft.Configuration
{
	using System.Collections.Generic;

	#region Interface: IMcpToolRuntime

	/// <summary>
	/// Model-based runtime seam used by the MCP read paths (tools/list, tools/call).
	/// Unlike the per-kind facades — which are keyed on a bare identity — this carries
	/// the full <see cref="McpToolModel"/> so the implementation can dispatch by source
	/// type: business processes resolve via <see cref="McpToolModel.ProcessId"/>,
	/// source-code actions via <see cref="McpToolModel.SourceCodeAction"/> (a full type
	/// name). Candidate definitions are keyed on <see cref="McpToolModel.Id"/> so the
	/// two kinds share one uniform identity.
	/// </summary>
	internal interface IMcpToolRuntime
	{

		#region Methods: Public

		IReadOnlyCollection<RuntimeToolDefinition> GetCandidateTools(IReadOnlyCollection<McpToolModel> tools);

		RuntimeToolSchemaResult GetInputSchema(McpToolModel tool);

		RuntimeToolSchemaResult GetOutputSchema(McpToolModel tool);

		RuntimeToolExecutionResult Execute(McpToolModel tool, IDictionary<string, object> arguments,
			RuntimeToolExecutionOptions executionOptions = null);

		#endregion

	}

	#endregion

}

