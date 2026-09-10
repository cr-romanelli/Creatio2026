namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using Common.Logging;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;
	using Terrasoft.Core.Process.Hooks;
	using Terrasoft.Core.Tasks;

	public class AgenticPlatformProcessCompletionHookArgs
	{
		public string CorrelationId { get; set; }
		public string CallbackEventTypeCode { get; set; }
		public string[] ResultParameterNames { get; set; }
	}

	/// <summary>
	/// Mutable container passed to <c>ExecuteProcessOptions.WithHook(..., context)</c>. Framework forwards
	/// it as <c>ProcessExecutionHookArgs.InlineExecutionContext</c> only when hook fires inline within
	/// <c>IProcessExecutor.Execute</c> (synchronous completion). Background-thread fires receive null.
	/// Hook populates this object inline so the caller can return terminal status/result parameters
	/// directly without going through the resume event pipeline.
	/// </summary>
	public sealed class InlineProcessCompletionContext
	{
		public bool CompletedInline { get; set; }
		public string Status { get; set; }
		public Guid ProcessId { get; set; }
		public string SchemaName { get; set; }
		public IDictionary<string, object> ResultParameters { get; set; }
		public string ErrorMessage { get; set; }
		public string FailedElementName { get; set; }
	}

	public class AgenticPlatformProcessCompletionHook
		: IBackgroundTask<ProcessExecutionHookArgs<AgenticPlatformProcessCompletionHookArgs>>
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		public void Run(ProcessExecutionHookArgs<AgenticPlatformProcessCompletionHookArgs> parameters) {
			try {
				AgenticPlatformProcessCompletionHookArgs args = parameters?.Arguments;
				BaseProcessHookArgs hookInfo = parameters?.ProcessHookInfo;
				if (hookInfo == null) {
					Log.Warn("ProcessHookInfo is null in AgenticPlatformProcessCompletionHook.");
					return;
				}
				string status = MapStatus(hookInfo.HookEvent);
				string errorMessage = null;
				string failedElementName = null;
				if (hookInfo is ProcessExecutionErrorHookArgs errorInfo) {
					errorMessage = errorInfo.ErrorMessage;
					failedElementName = errorInfo.ElementName;
				}
				IDictionary<string, object> resultParameters =
					hookInfo.HookEvent.HasFlag(ProcessExecutionHookEvents.ElementFailed)
						? null
						: ReadResultParameters(hookInfo, args?.ResultParameterNames);
				resultParameters = EnrichLookupValues(resultParameters, hookInfo, parameters.UserConnection);
				if (parameters.InlineExecutionContext is InlineProcessCompletionContext inlineCtx) {
					inlineCtx.CompletedInline = true;
					inlineCtx.Status = status;
					inlineCtx.ProcessId = hookInfo.Process?.Descriptor?.UId ?? Guid.Empty;
					inlineCtx.SchemaName = hookInfo.Process?.Schema?.Name;
					inlineCtx.ResultParameters = resultParameters;
					inlineCtx.ErrorMessage = errorMessage;
					inlineCtx.FailedElementName = failedElementName;
					return;
				}
				if (args == null || string.IsNullOrWhiteSpace(args.CorrelationId)) {
					return;
				}
				var data = new ProcessCompletionData {
					CorrelationId = args.CorrelationId,
					ProcessId = hookInfo.Process?.Descriptor?.UId ?? Guid.Empty,
					SchemaName = hookInfo.Process?.Schema?.Name,
					Status = status,
					ResultParameters = resultParameters,
					ErrorMessage = errorMessage,
					FailedElementName = failedElementName
				};
				string eventTypeCode = string.IsNullOrWhiteSpace(args.CallbackEventTypeCode)
					? Constants.EventTypeCodes.ProcessCompleted
					: args.CallbackEventTypeCode;
				var request = new EventOccurrenceRequest<ProcessCompletionData> {
					TriggerCode = Constants.TriggerCodes.ProcessCompletion,
					EventTypeCode = eventTypeCode,
					OccurredAt = DateTime.UtcNow,
					Data = data
				};
				DispatchResumeEvent(request);
			} catch (Exception ex) {
				Log.Error("AgenticPlatformProcessCompletionHook failed.", ex);
			}
		}

		/// <summary>
		/// APD-2470: hands the process-completion resume event off to a Creatio background
		/// task instead of sending it inline. When a suspended process completes right after
		/// an interactive element, this hook fires on the user-facing process-resume thread;
		/// sending the trigger inline ran the single-event transport's bounded retry/back-off
		/// (up to ~1 minute against a failing or unreachable AI Studio) on that thread and froze
		/// the UI. Offloading makes the resume return immediately so a delivery problem can no
		/// longer affect the UI (the expected behavior in the ticket).
		/// <para>
		/// The payload is serialized here — where the process parameters are still readable —
		/// and carried as plain strings (<see cref="ProcessCompletionSendArgs"/>) because the
		/// background-task argument transport cannot serialize the typed request (its
		/// <c>IDictionary&lt;string, object&gt;</c> result parameters break that serializer).
		/// The send itself is always a resume-mode single-event call, so it goes straight to
		/// <see cref="SingleEventSender"/> rather than back through the bulk/single router.
		/// Marked <c>internal virtual</c> as a unit-test seam.
		/// </para>
		/// </summary>
		internal virtual void DispatchResumeEvent(EventOccurrenceRequest<ProcessCompletionData> request) {
			ProcessCompletionSendArgs sendArgs = BuildSendArgs(request);
			Task.StartNewWithUserConnection<ProcessCompletionSendAsyncOperation,
				ProcessCompletionSendArgs>(sendArgs);
		}

		/// <summary>
		/// Builds the MessagePack-safe background-task argument from the typed request by
		/// serializing the body up front (camelCase, per <see cref="AIPlatformHttpSenderBase.JsonSettings"/>).
		/// Extracted from <see cref="DispatchResumeEvent"/> so the request-to-JSON step is
		/// unit-testable without the Creatio task scheduler.
		/// <para>
		/// Note: the resulting <see cref="ProcessCompletionSendArgs.JsonBody"/> may contain
		/// arbitrary process result parameters — never log it.
		/// </para>
		/// </summary>
		internal static ProcessCompletionSendArgs BuildSendArgs(
				EventOccurrenceRequest<ProcessCompletionData> request) {
			return new ProcessCompletionSendArgs {
				TriggerCode = request.TriggerCode,
				JsonBody = JsonConvert.SerializeObject(request, AIPlatformHttpSenderBase.JsonSettings)
			};
		}

		private static string MapStatus(ProcessExecutionHookEvents hookEvent) {
			if (hookEvent.HasFlag(ProcessExecutionHookEvents.ElementFailed)) {
				return Constants.RunProcessStatuses.Failed;
			}
			if (hookEvent.HasFlag(ProcessExecutionHookEvents.ProcessCanceled)) {
				return Constants.RunProcessStatuses.Canceled;
			}
			return Constants.RunProcessStatuses.Done;
		}

		private static IDictionary<string, object> ReadResultParameters(BaseProcessHookArgs hookInfo,
				string[] names) {
			if (names == null || names.Length == 0 || hookInfo?.Process == null) {
				return null;
			}
			var result = new Dictionary<string, object>(names.Length);
			foreach (string name in names) {
				if (string.IsNullOrWhiteSpace(name)) {
					continue;
				}
				try {
					ParameterValueDescriptor descriptor = hookInfo.Process.GetParameterValue(name);
					if (descriptor != null && descriptor.IsValueDefined) {
						result[name] = descriptor.Value;
					}
				} catch (Exception ex) {
					Log.Warn($"Failed to read result parameter '{name}' in process completion hook.", ex);
				}
			}
			return result.Count == 0 ? null : result;
		}

		// Enriches lookup result values so MCP / AI-platform consumers receive a stable { value } object
		// (gaining displayValue when enrichment is enabled and the id resolves) instead of a bare identifier,
		// recursing into composite results. When enrichment runs, wrapping is unconditional — disabling the
		// feature only skips the display-value reads, never reverting to bare ids. Enrichment is skipped and
		// values pass through unenriched only when the schema parameters or user connection are unavailable,
		// or if the wrapping step throws; a display-value resolution failure keeps the wrapped shape, and a
		// failed DI resolution falls back to the default enricher. Never throws into the hook.
		private IDictionary<string, object> EnrichLookupValues(IDictionary<string, object> resultParameters,
				BaseProcessHookArgs hookInfo, UserConnection userConnection) {
			IEnumerable<ProcessSchemaParameter> schemaParameters = GetResultSchemaParameters(hookInfo);
			if (resultParameters == null || schemaParameters == null || userConnection == null) {
				return resultParameters;
			}
			ILookupResponseEnricher enricher;
			try {
				enricher = ClassFactory.Get<ILookupResponseEnricher>(
					new ConstructorArgument("userConnection", userConnection));
			} catch (Exception exception) {
				// DI resolution failed — fall back to the default implementation so lookups are still
				// wrapped ({ value }) instead of reverting to bare identifiers (preserves no drift).
				Log.Warn("Could not resolve ILookupResponseEnricher; using the default " +
					"LookupResponseEnricher.", exception);
				enricher = new LookupResponseEnricher(userConnection);
			}
			try {
				return enricher.Enrich(resultParameters, schemaParameters);
			} catch (Exception exception) {
				Log.Warn("Lookup enrichment failed in process completion hook.", exception);
				return resultParameters;
			}
		}

		// The OUT/result schema parameters carrying the lookup type metadata enrichment reads. Pulled
		// behind an internal virtual seam (like DispatchResumeEvent) so the hook -> enricher wiring can
		// be unit-tested without a fully built ProcessSchema, which requires an ISchemaManager and
		// exposes no public parameter-add API.
		internal virtual IEnumerable<ProcessSchemaParameter> GetResultSchemaParameters(
				BaseProcessHookArgs hookInfo) {
			return (hookInfo?.Process?.Schema as ProcessSchema)?.Parameters;
		}
	}
}

