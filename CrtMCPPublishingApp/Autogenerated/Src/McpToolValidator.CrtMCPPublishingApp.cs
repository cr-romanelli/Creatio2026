using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using Terrasoft.Common;

	#region Class: McpToolValidator

	[DefaultBinding(typeof(IMcpToolValidator))]
	internal class McpToolValidator : IMcpToolValidator
	{

		#region Constructors: Public

		// Stateless: membership is decided against the authorized set the caller
		// passes to Validate, so the validator no longer holds a runtime facade or
		// re-queries the process catalog per tool.
		public McpToolValidator() {
		}

		#endregion

		#region Methods: Private

		private static void AddIssue(McpToolValidationResult validationResult, string code, string message,
				string fieldName = null, bool isPublishBlocking = true) {
			validationResult.Issues.Add(new McpToolValidationIssue {
				Code = code,
				FieldName = fieldName,
				IsPublishBlocking = isPublishBlocking,
				Message = message
			});
		}

		private static bool IsValidJson(string value) {
			if (value.IsNullOrWhiteSpace()) {
				return true;
			}
			try {
				JsonConvert.DeserializeObject<object>(value);
				return true;
			} catch {
				return false;
			}
		}

		#endregion

		#region Methods: Public

		public McpToolValidationResult Validate(McpToolModel tool,
				IReadOnlyDictionary<Guid, RuntimeToolDefinition> authorizedTools) {
			var validationResult = new McpToolValidationResult();
			if (tool == null) {
				AddIssue(validationResult, "ToolMissing", "Tool record was not provided.");
				return validationResult;
			}
			// Per-kind reference requirement: a business-process tool must reference a
			// process; a source-code-action tool must reference an action type name. The
			// kind also selects which field validation issues point at (so the form
			// highlights the right control).
			bool isSourceCode = McpToolSourceTypes.ToRuntimeToolKind(tool.ToolSourceTypeId) == RuntimeToolKind.SourceCodeAction;
			string referenceFieldName = isSourceCode ? nameof(tool.SourceCodeAction) : nameof(tool.ProcessId);
			if (isSourceCode) {
				if (tool.SourceCodeAction.IsNullOrWhiteSpace()) {
					AddIssue(validationResult, "ActionMissing",
						"Tool record must reference a source code action.", referenceFieldName);
				}
			} else if (tool.ProcessId.IsEmpty()) {
				AddIssue(validationResult, "ActionMissing",
					"Tool record must reference a process.", referenceFieldName);
			}
			if (tool.ExternalName.IsNullOrWhiteSpace()) {
				AddIssue(validationResult, "ToolNameMissing",
					"Tool record must define an external MCP tool name.", nameof(tool.ExternalName));
			}
			// Tool-name charset/length validation is delegated to the MCP client
			// (control-plane ToolNameValidator, {1,64}) — the single authoritative gate.
			// Filtering here would silently drop tools from tools/list (removed on purpose);
			// the form page gives authoring-time feedback with the same rule.
			if (!IsValidJson(tool.Annotations)) {
				AddIssue(validationResult, "AnnotationsJsonInvalid", "Annotations JSON is invalid.",
					nameof(tool.Annotations));
			}
			if (!IsValidJson(tool.InputSchema)) {
				AddIssue(validationResult, "InputSchemaOverrideInvalid", "Input schema JSON is invalid.",
					nameof(tool.InputSchema));
			}
			if (!IsValidJson(tool.OutputSchema)) {
				AddIssue(validationResult, "OutputSchemaInvalid", "Output schema JSON is invalid.",
					nameof(tool.OutputSchema));
			}
			// Per-server external-name uniqueness is a write-time concern. This
			// validator runs on the runtime read path, where a global duplicate
			// check is neither relevant nor correct under the 1:N model.
			// Membership is decided against the authorized set the caller already
			// resolved once per request (keyed by process SysSchema.Id, the same
			// identity McpTool.ProcessId stores). The validator no longer re-queries
			// the runtime per tool — that turned a list of N tools into N catalog
			// scans. authorizedTools only ever holds executable + enabled processes,
			// and TryResolveAuthorizedTool re-checks both defensively.
			// Membership is decided against the authorized set, keyed on McpTool.Id (the
			// uniform identity the dispatcher advertises for both kinds). An absent entry
			// covers a missing/disabled process, an unresolvable source-code action type
			// (renamed class/namespace), and a disabled action alike.
			bool runtimeToolExists = authorizedTools != null &&
				ToolServiceRequestGuard.TryResolveAuthorizedTool(authorizedTools, tool.Id, out _);
			if (!runtimeToolExists) {
				AddIssue(validationResult, "ActionUnavailable",
					"Referenced action is not available as a tool.", referenceFieldName);
			}
			return validationResult;
		}

		#endregion

	}

	#endregion

}

