namespace Terrasoft.Configuration
{
	using System.Collections.Generic;

	#region Interface: ISourceCodeRuntimeToolFacade

	/// <summary>
	/// Resolves and executes source-code-action tools backed by Creatio Copilot's
	/// <c>Creatio.Copilot.Actions.BaseExecutableCodeAction</c> contract. The
	/// source-code counterpart of <see cref="IProcessRuntimeToolFacade"/> (the
	/// business-process facade): a tool stores the action's assembly-qualified full
	/// type name in <see cref="McpToolModel.SourceCodeAction"/>, which this facade
	/// resolves with <c>Type.GetType</c>, instantiates, and invokes. Candidate
	/// definitions are keyed on <see cref="McpToolModel.Id"/>.
	/// </summary>
	internal interface ISourceCodeRuntimeToolFacade
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

