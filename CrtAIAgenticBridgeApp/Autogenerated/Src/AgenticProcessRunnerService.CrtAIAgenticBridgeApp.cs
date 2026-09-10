namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices
{
	using System;
	using System.IO;
	using System.Net;
	using System.Security;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Text;
	using System.Web.SessionState;
	using Common.Logging;
	using CrtAIAgenticBridgeApp.Runtime;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.ServiceModelContract;
	using Terrasoft.Web.Common;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AgenticProcessRunnerService : BaseService, IReadOnlySessionState
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		[OperationContract]
		[WebInvoke(Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream RunProcess(RunAgenticProcessRequest request) {
			RunAgenticProcessResponse response = Execute(request);
			string json = JsonConvert.SerializeObject(response, AIPlatformHttpSenderBase.JsonSettings);
			SetJsonContentType();
			return new MemoryStream(Encoding.UTF8.GetBytes(json));
		}

		internal RunAgenticProcessResponse Execute(RunAgenticProcessRequest request) {
			try {
				if (request == null || string.IsNullOrWhiteSpace(request.SchemaName)) {
					return Failure(HttpStatusCode.BadRequest, "schemaName is required.");
				}
				UserConnection.DBSecurityEngine.CheckCanExecuteOperation(
					Constants.OperationCodes.CanManageSolution);
				return ClassFactory.Get<IAgenticProcessRunner>(
					new ConstructorArgument("userConnection", UserConnection)).RunProcess(request);
			} catch (SecurityException ex) {
				Log.Warn("Run process permission denied.", ex);
				return Failure(HttpStatusCode.Forbidden, ex.Message);
			} catch (Exception ex) {
				Log.Error("Run process failed.", ex);
				return Failure(HttpStatusCode.InternalServerError, "Internal server error");
			}
		}

		internal virtual void SetJsonContentType() {
#if NETSTANDARD2_0
			var context = HttpContextAccessor.GetInstance();
			if (context?.Response != null) {
				context.Response.ContentType = "application/json; charset=utf-8";
			}
#else
			WebOperationContext context = WebOperationContext.Current;
			if (context != null) {
				context.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			}
#endif
		}

		private static RunAgenticProcessResponse Failure(HttpStatusCode status, string message) {
			return new RunAgenticProcessResponse {
				Success = false,
				ErrorInfo = new ErrorInfo {
					Message = message,
					ErrorCode = ((int)status).ToString()
				}
			};
		}
	}
}

