namespace Terrasoft.Configuration
{
	using System;
	using Terrasoft.Core.ServiceModelContract;

	#region Class: RuntimeToolExecutionResult

	internal class RuntimeToolExecutionResult
	{

		#region Properties: Public

		public string ErrorMessage { get; set; }

		public bool IsSuccess { get; set; }

		public Guid ProcessId { get; set; }

		public string Status { get; set; }

		// Source-code-action result payload: a single response string returned by
		// BaseExecutableCodeAction.Execute (CopilotActionExecutionResult.Response).
		// Null for BusinessProcess tools, which return named ResultParameters instead.
		public string Response { get; set; }

		// Source-code-action structured output, built to conform to the advertised
		// outputSchema: the Response parsed as the declared object when the action
		// declares Output params, or { result: <Response> } when it declares none.
		// Null for BusinessProcess tools (they use ResultParameters).
		public System.Collections.Generic.Dictionary<string, object> StructuredOutput { get; set; }

		public string CorrelationId { get; set; }

		public DateTime? ExpiresAt { get; set; }

		public ProcessParameterValuesCollection ResultParameters { get; set; }

		#endregion

	}

	#endregion

}

