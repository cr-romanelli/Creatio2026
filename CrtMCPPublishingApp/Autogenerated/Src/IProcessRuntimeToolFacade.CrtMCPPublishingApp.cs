 namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;

	#region Interface: IProcessRuntimeToolFacade

	/// <summary>
	/// Temporary local runtime seam until the shared Creatio.Copilot.Abstractions
	/// package in this workspace exposes the same contract.
	/// </summary>
	internal interface IProcessRuntimeToolFacade
	{

		#region Methods: Public

		/// <summary>
		/// Resolves and authorizes the business-process tools in <paramref name="tools"/>
		/// (only their processes are scanned — O(server tools), not O(all processes)) and
		/// returns one <see cref="RuntimeToolDefinition"/> per tool, keyed on
		/// <see cref="McpToolModel.Id"/>. The caller (the dispatcher) passes only
		/// business-process tools; the result is already keyed by tool id, so the
		/// dispatcher just concatenates it with the source-code facade's output.
		/// </summary>
		IReadOnlyCollection<RuntimeToolDefinition> GetCandidateTools(IReadOnlyCollection<McpToolModel> tools);

		RuntimeToolSchemaResult GetInputSchema(Guid toolUId);

		/// <summary>
		/// Returns a JSON-Schema describing the tool's structured output, derived
		/// from the BP's result parameters (or empty / failure when none are
		/// declared). Used by <see cref="McpToolsListService"/> to populate the
		/// MCP <c>outputSchema</c> field on tool descriptors, which in turn lets
		/// LLM clients (AI Studio, Claude Desktop) interpret the typed payload
		/// returned in <c>tools/call</c> <c>structuredContent</c>.
		/// </summary>
		RuntimeToolSchemaResult GetOutputSchema(Guid toolUId);

		RuntimeToolExecutionResult Execute(Guid toolUId, IDictionary<string, object> arguments,
			RuntimeToolExecutionOptions executionOptions = null);

		#endregion

	}

	#endregion

}

