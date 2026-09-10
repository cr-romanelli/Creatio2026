namespace Terrasoft.Configuration
{
	using System.Collections.Generic;
	using System.Linq;

	#region Class: McpToolValidationResult

	internal class McpToolValidationResult
	{

		#region Constructors: Public

		public McpToolValidationResult() {
			Issues = new List<McpToolValidationIssue>();
		}

		#endregion

		#region Properties: Public

		public bool IsPublishable => Issues.All(issue => !issue.IsPublishBlocking);

		public List<McpToolValidationIssue> Issues { get; }

		#endregion

	}

	#endregion

}

