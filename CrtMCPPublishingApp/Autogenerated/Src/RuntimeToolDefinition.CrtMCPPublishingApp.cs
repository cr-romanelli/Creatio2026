namespace Terrasoft.Configuration
{
	using System;

	#region Class: RuntimeToolDefinition

	internal class RuntimeToolDefinition
	{

		#region Properties: Public

		public bool CanDeriveSchema { get; set; }

		public bool CanExecute { get; set; }

		public string Description { get; set; }

		public bool IsEnabled { get; set; }

		public RuntimeToolKind Kind { get; set; }

		public string Name { get; set; }

		public string RuntimeSource { get; set; }

		public string Title { get; set; }

		public Guid ToolUId { get; set; }

		#endregion

	}

	#endregion

}

