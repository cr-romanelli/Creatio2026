using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Core;

	#region Class: McpToolRuntime

	/// <summary>
	/// Dispatcher implementation of <see cref="IMcpToolRuntime"/>. Routes each tool to
	/// the business-process facade (<see cref="IProcessRuntimeToolFacade"/>) or the
	/// source-code facade (<see cref="ISourceCodeRuntimeToolFacade"/>) based on
	/// <c>ToolSourceTypeId</c>, and re-keys business-process candidates from process
	/// SysSchema.Id onto the tool's own <c>McpTool.Id</c> so both kinds present a
	/// uniform <c>ToolUId</c>.
	/// </summary>
	[DefaultBinding(typeof(IMcpToolRuntime))]
	internal class McpToolRuntime : IMcpToolRuntime
	{

		#region Fields: Private

		private readonly IProcessRuntimeToolFacade _processFacade;
		private readonly ISourceCodeRuntimeToolFacade _sourceCodeFacade;

		#endregion

		#region Constructors: Public

		public McpToolRuntime(UserConnection userConnection) {
			_processFacade = ClassFactory.Get<IProcessRuntimeToolFacade>(
				new ConstructorArgument("userConnection", userConnection));
			_sourceCodeFacade = ClassFactory.Get<ISourceCodeRuntimeToolFacade>(
				new ConstructorArgument("userConnection", userConnection));
		}

		internal McpToolRuntime(IProcessRuntimeToolFacade processFacade,
				ISourceCodeRuntimeToolFacade sourceCodeFacade) {
			_processFacade = processFacade;
			_sourceCodeFacade = sourceCodeFacade;
		}

		#endregion

		#region Methods: Private

		private static bool IsSourceCode(McpToolModel tool) {
			return tool != null &&
				McpToolSourceTypes.ToRuntimeToolKind(tool.ToolSourceTypeId) == RuntimeToolKind.SourceCodeAction;
		}

		#endregion

		#region Methods: Public

		// Pure routing: split the tools by source type and let each facade build its own
		// definitions (already keyed by McpTool.Id), then concatenate. No filtering or
		// mapping lives here — that is the facades' responsibility.
		public IReadOnlyCollection<RuntimeToolDefinition> GetCandidateTools(IReadOnlyCollection<McpToolModel> tools) {
			List<McpToolModel> toolList = (tools ?? Enumerable.Empty<McpToolModel>())
				.Where(tool => tool != null)
				.ToList();
			var result = new List<RuntimeToolDefinition>();
			result.AddRange(_processFacade.GetCandidateTools(toolList.Where(tool => !IsSourceCode(tool)).ToList()));
			result.AddRange(_sourceCodeFacade.GetCandidateTools(toolList.Where(IsSourceCode).ToList()));
			return result;
		}

		public RuntimeToolSchemaResult GetInputSchema(McpToolModel tool) {
			return IsSourceCode(tool)
				? _sourceCodeFacade.GetInputSchema(tool)
				: _processFacade.GetInputSchema(tool.ProcessId);
		}

		public RuntimeToolSchemaResult GetOutputSchema(McpToolModel tool) {
			return IsSourceCode(tool)
				? _sourceCodeFacade.GetOutputSchema(tool)
				: _processFacade.GetOutputSchema(tool.ProcessId);
		}

		public RuntimeToolExecutionResult Execute(McpToolModel tool, IDictionary<string, object> arguments,
				RuntimeToolExecutionOptions executionOptions = null) {
			return IsSourceCode(tool)
				? _sourceCodeFacade.Execute(tool, arguments, executionOptions)
				: _processFacade.Execute(tool.ProcessId, arguments, executionOptions);
		}

		#endregion

	}

	#endregion

}

