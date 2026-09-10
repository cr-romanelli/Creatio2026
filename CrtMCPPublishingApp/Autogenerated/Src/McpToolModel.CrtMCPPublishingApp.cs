namespace Terrasoft.Configuration
{
	using System;
	using Terrasoft.Common;

	#region Class: McpToolModel

	/// <summary>
	/// Single internal projection of an <c>McpTool</c> row, shared by both the
	/// runtime (<c>tools/list</c> / <c>tools/call</c>) and management paths.
	/// Property names mirror the <c>McpTool</c> entity columns 1:1 — the DB is the
	/// source of truth. <see cref="ToolSourceTypeId"/> is the tool-source discriminator
	/// (business process or source-code action); it decides which reference is
	/// authoritative: <see cref="ProcessId"/> (the process's raw <c>SysSchema.Id</c>,
	/// Id not UId) for a business process, or <see cref="SourceCodeAction"/> (the
	/// action's assembly-qualified type name) for a source-code action.
	/// </summary>
	internal class McpToolModel
	{

		#region Properties: Public

		public Guid Id { get; set; }

		public Guid McpServerId { get; set; }

		public Guid ProcessId { get; set; }

		public Guid ToolSourceTypeId { get; set; }

		// For SourceCodeAction tools: the assembly-qualified full type name of the
		// BaseExecutableCodeAction class to run (e.g.
		// "Creatio.ComponentCopilot.FindLookupValueAction, CrtComponentCopilot").
		// Empty for BusinessProcess tools, which use ProcessId instead. The
		// ToolSourceTypeId discriminator decides which of the two is authoritative.
		public string SourceCodeAction { get; set; }

		public string Title { get; set; }

		public string ExternalName { get; set; }

		public string Description { get; set; }

		public string InputSchema { get; set; }

		public string OutputSchema { get; set; }

		public string Annotations { get; set; }

		public bool IsEnabled { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpToolSourceTypes

	/// <summary>
	/// Well-known <c>McpToolSourceType</c> lookup ids. The source type selects how a
	/// tool's reference is resolved into an executable runtime tool: a business process
	/// (from <see cref="McpToolModel.ProcessId"/>) or a source-code action (from
	/// <see cref="McpToolModel.SourceCodeAction"/>).
	/// </summary>
	internal static class McpToolSourceTypes
	{

		#region Fields: Public

		public static readonly Guid BusinessProcess =
			new Guid("8dfb191a-a0af-433d-b088-fa1797ad2e50");

		// "Source code action" McpToolSourceType row. A tool of this type stores the
		// assembly-qualified full type name of a Creatio.Copilot.Actions.BaseExecutableCodeAction
		// class in McpTool.SourceCodeAction; the runtime resolves it via Type.GetType and
		// invokes Execute.
		public static readonly Guid SourceCodeAction =
			new Guid("ebed4287-7fd3-4d75-843f-c2ea0b795e66");

		#endregion

		#region Methods: Public

		/// <summary>
		/// True when a tool of the given source type can be resolved by the current
		/// runtime. An empty id is treated as the default (business process) for
		/// back-compat with rows created before the source-type column existed.
		/// </summary>
		public static bool IsSupported(Guid toolSourceTypeId) {
			return toolSourceTypeId.IsEmpty() ||
				toolSourceTypeId == BusinessProcess ||
				toolSourceTypeId == SourceCodeAction;
		}

		/// <summary>
		/// Maps an <c>McpToolSourceType</c> lookup id to the runtime tool kind that
		/// selects which facade resolves and executes the tool. An empty id defaults
		/// to business process (back-compat with rows predating the source-type column).
		/// </summary>
		public static RuntimeToolKind ToRuntimeToolKind(Guid toolSourceTypeId) {
			if (toolSourceTypeId == SourceCodeAction) {
				return RuntimeToolKind.SourceCodeAction;
			}
			return RuntimeToolKind.BusinessProcess;
		}

		#endregion

	}

	#endregion

}

