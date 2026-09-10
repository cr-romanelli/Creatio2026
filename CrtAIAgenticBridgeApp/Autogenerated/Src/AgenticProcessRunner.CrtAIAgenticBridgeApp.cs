namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Security;
	using Common.Logging;
	using CrtAIAgenticBridgeApp.EntryPoints.WebServices;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;
	using Terrasoft.Core.Process.Hooks;
	using Terrasoft.Core.ServiceModelContract;

	/// <summary>
	/// Runs an agentic business process and returns its terminal result inline when the process
	/// completes synchronously, or a suspended status otherwise. Resolvable through the factory so
	/// callers outside the web endpoint can reuse the same execution path. Authorization is the
	/// caller's responsibility.
	/// </summary>
	public interface IAgenticProcessRunner
	{
		RunAgenticProcessResponse RunProcess(RunAgenticProcessRequest request);
	}

	[DefaultBinding(typeof(IAgenticProcessRunner))]
	internal class AgenticProcessRunner : IAgenticProcessRunner
	{
		private const int DefaultContinuationExpirationHours = 6;
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private readonly UserConnection _userConnection;

		public AgenticProcessRunner(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		public RunAgenticProcessResponse RunProcess(RunAgenticProcessRequest request) {
			try {
				if (request == null || string.IsNullOrWhiteSpace(request.SchemaName)) {
					return Failure(HttpStatusCode.BadRequest, "schemaName is required.");
				}
				var inlineContext = new InlineProcessCompletionContext();
				ExecuteProcessOptions options = BuildOptions(request, inlineContext);
				IProcessExecutor executor = _userConnection.ProcessEngine.ProcessExecutor;
				ProcessDescriptor descriptor = executor.Execute(options);
				if (inlineContext.CompletedInline) {
					return BuildInlineResponse(inlineContext, request);
				}
				return BuildResponse(descriptor, request, _userConnection);
			} catch (ItemNotFoundException ex) {
				Log.Warn("Process schema not found.", ex);
				return Failure(HttpStatusCode.NotFound, ex.Message);
			} catch (ProcessSchemaIsNotPublishedException ex) {
				Log.Warn("Process schema is not published.", ex);
				return Failure(HttpStatusCode.BadRequest, ex.Message);
			} catch (SecurityException ex) {
				Log.Warn("Run process permission denied.", ex);
				return Failure(HttpStatusCode.Forbidden, ex.Message);
			} catch (Exception ex) {
				Log.Error("Run process failed.", ex);
				return Failure(HttpStatusCode.InternalServerError, "Internal server error");
			}
		}

		private static ExecuteProcessOptions BuildOptions(RunAgenticProcessRequest request,
				InlineProcessCompletionContext inlineContext) {
			ExecuteProcessOptions options = ExecuteProcessOptions.RunBySchemaName(request.SchemaName);
			Dictionary<string, string> parameters = ConvertParameters(request.ParameterValues);
			if (parameters != null) {
				options.ParameterValues = parameters;
			}
			options.WithParseSerializableObjectAsJson();
			var hookArgs = new AgenticPlatformProcessCompletionHookArgs {
				CorrelationId = request.CorrelationId,
				CallbackEventTypeCode = Constants.EventTypeCodes.ProcessCompleted,
				ResultParameterNames = request.ResultParameterNames
			};
			options.WithHook<AgenticPlatformProcessCompletionHook,
				AgenticPlatformProcessCompletionHookArgs>(hookArgs,
					ProcessExecutionHookEvents.ProcessCompleted
						| ProcessExecutionHookEvents.ProcessCanceled
						| ProcessExecutionHookEvents.ElementFailed,
					inlineContext);
			return options;
		}

		private static Dictionary<string, string> ConvertParameters(
				System.Collections.ObjectModel.Collection<NameValuePair> parameterValues) {
			if (parameterValues == null || parameterValues.Count == 0) {
				return null;
			}
			var result = new Dictionary<string, string>(parameterValues.Count);
			foreach (NameValuePair pair in parameterValues) {
				if (pair == null || string.IsNullOrEmpty(pair.Name)) {
					continue;
				}
				result[pair.Name] = pair.Value;
			}
			return result;
		}

		private static RunAgenticProcessResponse BuildInlineResponse(InlineProcessCompletionContext ctx,
				RunAgenticProcessRequest request) {
			return new RunAgenticProcessResponse {
				Success = true,
				Status = ctx.Status,
				CorrelationId = request.CorrelationId,
				ResultParameterValues = ToParameterValues(ctx.ResultParameters),
				ErrorMessage = ctx.ErrorMessage,
				FailedElement = ctx.FailedElementName,
				ProcessId = ctx.ProcessId
			};
		}

		private static RunAgenticProcessResponse BuildResponse(ProcessDescriptor descriptor,
				RunAgenticProcessRequest request, UserConnection userConnection) {
			string status = MapStatus(descriptor);
			DateTime? expiresAt = status == Constants.RunProcessStatuses.Suspended
				? GetContinuationExpiresAt(userConnection)
				: (DateTime?)null;
			return new RunAgenticProcessResponse {
				Success = true,
				Status = status,
				CorrelationId = request.CorrelationId,
				ExpiresAt = expiresAt,
				ProcessId = descriptor?.UId ?? Guid.Empty
			};
		}

		private static Dictionary<string, object> ToParameterValues(
				IDictionary<string, object> values) {
			if (values == null || values.Count == 0) {
				return null;
			}
			return new Dictionary<string, object>(values);
		}

		private static string MapStatus(ProcessDescriptor descriptor) {
			if (descriptor == null || descriptor.UId.IsEmpty()) {
				return Constants.RunProcessStatuses.Suspended;
			}
			switch (descriptor.ProcessStatus) {
				case ProcessStatus.Done:
					return Constants.RunProcessStatuses.Done;
				case ProcessStatus.Cancelled:
					return Constants.RunProcessStatuses.Canceled;
				case ProcessStatus.Error:
					return Constants.RunProcessStatuses.Failed;
				default:
					return Constants.RunProcessStatuses.Suspended;
			}
		}

		private static DateTime GetContinuationExpiresAt(UserConnection userConnection) {
			int hours = Terrasoft.Core.Configuration.SysSettings.GetValue(
				userConnection,
				Constants.SysSettingCodes.AgenticProcessContinuationExpirationHours,
				DefaultContinuationExpirationHours);
			return DateTime.UtcNow.AddHours(hours);
		}

		private static RunAgenticProcessResponse Failure(HttpStatusCode statusCode, string message) {
			return new RunAgenticProcessResponse {
				Success = false,
				ErrorInfo = new ErrorInfo {
					Message = message,
					ErrorCode = ((int)statusCode).ToString()
				}
			};
		}
	}
}

