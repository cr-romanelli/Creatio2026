namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Text;
	using System.Web.SessionState;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	#region Class: McpDiscoveryApi

	/// <summary>
	/// MCP server discovery endpoint. Returns a JSON descriptor with the
	/// per-tenant MCP endpoint URL, advertised capabilities, transport
	/// options, and current auth method.
	///
	/// Useful for:
	///   * admins verifying the composable app is installed and reachable
	///   * agents that resolve the full connection profile from one URL
	///   * the future <c>creatio-ai-control-plane</c> consumer per APD-48,
	///     which populates the per-tenant tool catalog from this descriptor
	///
	/// Authenticated for parity with the rest of CrtMCPPublishingApp; the
	/// connection URL is published in product docs / admin UI, so clients
	/// don't need anonymous discovery to know they should connect here.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[SspServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class McpDiscoveryApi : BaseService, IReadOnlySessionState
	{

		#region Constants: Private

		private const string AuthenticationRequiredMessage = "Authentication is required.";
		private const string JsonContentType = "application/json";
		private const string RpcServiceBase = "/0/rest/ToolServiceMcp";

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(McpDiscoveryApi));

		#endregion

		#region Methods: Private

		private static bool IsAuthenticated(UserConnection userConnection) {
			return ToolServiceRequestGuard.IsAuthenticated(userConnection);
		}

		private static Stream WriteJson(object body, HttpStatusCode statusCode = HttpStatusCode.OK) {
#if !(NETSTANDARD2_0 || NET)
			// Classic .NET Framework / WCF host only: returning Stream makes WCF
			// raw message encoding overwrite HttpContext.Response.ContentType with
			// application/octet-stream after the operation returns, so the WCF
			// outgoing response is the assignment that wins. WebOperationContext
			// does not exist on the .NET (Core) host, where the HttpContext
			// assignment below is sufficient.
			var outgoing = WebOperationContext.Current?.OutgoingResponse;
			if (outgoing != null) {
				outgoing.StatusCode = statusCode;
				outgoing.ContentType = JsonContentType;
			}
#endif
			var ctx = Terrasoft.Web.Http.Abstractions.HttpContext.Current;
			if (ctx?.Response != null) {
				ctx.Response.StatusCode = (int)statusCode;
				ctx.Response.ContentType = JsonContentType;
			}
			string json = body == null ? string.Empty : JsonConvert.SerializeObject(body);
			return new MemoryStream(Encoding.UTF8.GetBytes(json));
		}

		#endregion

		#region Methods: Public

		[OperationContract]
		[WebInvoke(Method = "GET", UriTemplate = "WellKnown/{code}",
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream WellKnown(string code) {
			if (!IsAuthenticated(UserConnection)) {
				return WriteJson(new Dictionary<string, object> {
					["error"] = AuthenticationRequiredMessage
				}, HttpStatusCode.Unauthorized);
			}
			try {
				var repository = ClassFactory.Get<IMcpServerRepository>(
					new ConstructorArgument("userConnection", UserConnection));
				McpServerModel server = string.IsNullOrWhiteSpace(code) ? null : repository.GetServerByCode(code);
				// Unknown / offline servers are not described — same fail-closed
				// posture as the runtime route, so discovery can't be used to
				// probe for servers that won't accept traffic.
				if (server == null || !server.IsOnline) {
					return WriteJson(new Dictionary<string, object> {
						["error"] = $"MCP server '{code}' was not found or is offline."
					}, HttpStatusCode.NotFound);
				}
				var descriptor = new Dictionary<string, object> {
					["name"] = server.Title,
					["code"] = server.Code,
					["description"] = server.Description,
					["version"] = McpServerInfo.Version,
					["protocolVersion"] = McpServerInfo.ProtocolVersion,
					["endpoints"] = new Dictionary<string, object> {
						["rpc"] = RpcServiceBase + "/" + server.Code + "/v1/mcp"
					},
					["capabilities"] = new Dictionary<string, object> {
						["tools"] = new Dictionary<string, object> {
							["listChanged"] = false
						}
					},
					["transport"] = new Dictionary<string, object> {
						["protocol"] = "json-rpc-2.0",
						["streamableHttp"] = true,
						["sse"] = "via Accept: text/event-stream"
					},
					["auth"] = new Dictionary<string, object> {
						["method"] = "session",
						["note"] = "Bearer token auth coming in APD-1375"
					}
				};
				return WriteJson(descriptor);
			} catch (Exception exception) {
				Log.Error("Unexpected exception while serving McpDiscoveryApi.WellKnown.", exception);
				return WriteJson(new Dictionary<string, object> {
					["error"] = "Internal server error."
				}, HttpStatusCode.InternalServerError);
			}
		}

		#endregion

	}

	#endregion

}

