namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Text;
	using System.Web;
	using System.Web.SessionState;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	#region Class: ToolServiceMcp

	/// <summary>
	/// In-process MCP-protocol API for the CrtMCPPublishingApp tool service
	/// (APD-1374). The single externally-exposed transport — there is no
	/// legacy REST face. JSON-RPC 2.0 over HTTP POST at
	/// <c>/0/rest/ToolServiceMcp/{code}/v1/mcp</c>.
	///
	/// Projection + execution are delegated to <see cref="IMcpToolsListService"/>
	/// / <see cref="IMcpToolsCallService"/> so the wire surface stays thin and
	/// future transports (auth proxies, streaming, multiplexing) can plug in
	/// without re-implementing exposure lookup, RBAC, or argument validation.
	///
	/// Supported methods:
	///   * <c>initialize</c>   — capability negotiation
	///   * <c>tools/list</c>   — projection of exposure records
	///   * <c>tools/call</c>   — execute an exposed tool
	///   * <c>ping</c>         — connection liveness check
	///
	/// Notifications (request with no <c>id</c>, or method prefixed
	/// <c>notifications/</c>) are accepted silently. Batch requests (JSON
	/// array at top level) are supported per JSON-RPC 2.0; the response is
	/// an array of replies excluding notifications.
	///
	/// Auth in this MVP piggy-backs on the standard Creatio session.
	/// Bearer/PAT token support is added in APD-1375 (PR 4 of Phase 2).
	/// SSE / Streamable HTTP streaming is added in PR 3 of Phase 1.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[SspServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ToolServiceMcp : BaseService, IReadOnlySessionState
	{

		#region Constants: Private

		private const string AuthenticationRequiredMessage = "Authentication is required.";
		private const string JsonContentType = "application/json";

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(ToolServiceMcp));

		#endregion

		#region Methods: Private

		private static bool IsAuthenticated(UserConnection userConnection) {
			return ToolServiceRequestGuard.IsAuthenticated(userConnection);
		}

		private static bool IsNotificationMethod(string method) {
			return method != null
				&& method.StartsWith("notifications/", StringComparison.Ordinal);
		}

		private static McpJsonRpcResponse MakeErrorResponse(object id, int code, string message,
				object data = null) {
			return new McpJsonRpcResponse {
				Id = id,
				Error = new McpJsonRpcError {
					Code = code,
					Message = message,
					Data = data
				}
			};
		}

		/// <summary>
		/// Maps a deserialized envelope (object, dictionary, or any non-object
		/// value from a batch) into an <see cref="McpJsonRpcRequest"/>. Returns
		/// <c>null</c> when the input can't be coerced into a JSON object — the
		/// dispatcher then produces a per-item <c>InvalidRequest</c> error,
		/// which matches JSON-RPC 2.0 spec for malformed batch elements
		/// (non-object items must yield a single <c>InvalidRequest</c> reply,
		/// not fail the whole batch with <c>ParseError</c>).
		/// </summary>
		internal static McpJsonRpcRequest MapRequest(object raw) {
			Dictionary<string, object> dict = raw as Dictionary<string, object>
				?? RoundTripToDictionary(raw);
			if (dict == null) {
				return null;
			}
			bool hasId = dict.ContainsKey("id");
			object id = hasId ? dict["id"] : null;
			string jsonrpc = dict.TryGetValue("jsonrpc", out object jsonrpcValue) ? jsonrpcValue as string : null;
			string method = dict.TryGetValue("method", out object methodValue) ? methodValue as string : null;
			Dictionary<string, object> parameters = null;
			if (dict.TryGetValue("params", out object paramsValue) && paramsValue != null) {
				// `params` is allowed to be an object, an array (positional),
				// or omitted per JSON-RPC 2.0. We coerce to an object-shape
				// dictionary; arrays / scalars become null and handlers that
				// require object params surface InvalidParams downstream
				// instead of a protocol-level ParseError.
				parameters = paramsValue as Dictionary<string, object>
					?? RoundTripToDictionary(paramsValue);
			}
			return new McpJsonRpcRequest {
				Jsonrpc = jsonrpc,
				Id = id,
				Method = method,
				Params = parameters,
				IsNotification = !hasId
			};
		}

		/// <summary>
		/// Safely coerces an arbitrary deserialized value into a
		/// <see cref="Dictionary{TKey, TValue}"/>. Returns <c>null</c> for
		/// <c>null</c>, arrays, and scalars; never throws. Used to handle the
		/// fact that <c>params</c> in JSON-RPC 2.0 may legitimately be a
		/// non-object value, in which case the caller treats it as missing
		/// for object-shaped parameter binding.
		/// </summary>
		internal static Dictionary<string, object> RoundTripToDictionary(object value) {
			if (value == null) {
				return null;
			}
			if (value is Dictionary<string, object> alreadyTyped) {
				return alreadyTyped;
			}
			try {
				string json = JsonConvert.SerializeObject(value);
				return JsonConvert.DeserializeObject<Dictionary<string, object>>(json,
					ToolServiceRequestGuard.LosslessArgumentSettings);
			} catch (Exception) {
				// Value is valid JSON but not an object (array / scalar).
				// Surface as null and let the dispatcher / handlers decide
				// how to react.
				return null;
			}
		}

		/// <summary>
		/// Parse the raw body into one or more <see cref="McpJsonRpcRequest"/>
		/// envelopes. Distinguishes three failure modes so each maps to the
		/// JSON-RPC 2.0 error code the spec mandates:
		///   * Syntactically invalid JSON → <c>parseError</c> set, caller
		///     emits <c>ParseError</c> (-32700) with a sanitized message —
		///     the parser exception is logged but never echoed to the wire
		///     because Newtonsoft's text can leak implementation details
		///     (e.g. internal type names, exact byte positions).
		///   * Syntactically valid JSON that isn't an object on the single
		///     path (e.g. <c>42</c>, <c>true</c>, <c>"x"</c>, <c>null</c>) →
		///     deserialized but <see cref="MapRequest"/> returns null,
		///     <see cref="DispatchSingle"/> emits <c>InvalidRequest</c>
		///     (-32600). Earlier code mis-classified these as ParseError
		///     because direct <c>Dictionary&lt;,&gt;</c> deserialization
		///     throws on non-object roots.
		///   * Per-element non-object in a batch (<c>[{...}, 42, {...}]</c>)
		///     → already handled by the batch path returning per-item
		///     InvalidRequest, not a whole-batch ParseError.
		/// </summary>
		internal static object ParseRequestBody(string body, out bool isBatch, out string parseError) {
			isBatch = false;
			parseError = null;
			if (body.IsNullOrWhiteSpace()) {
				parseError = "Empty request body.";
				return null;
			}
			string trimmed = body.TrimStart();
			if (trimmed.StartsWith("[", StringComparison.Ordinal)) {
				isBatch = true;
				try {
					var rawList = JsonConvert.DeserializeObject<List<object>>(body,
						ToolServiceRequestGuard.LosslessArgumentSettings);
					return rawList?.Select(MapRequest).ToList();
				} catch (Exception exception) {
					Log.Warn("MCP request body could not be parsed as a JSON array.", exception);
					parseError = "Invalid JSON.";
					return null;
				}
			}
			// Single-request path: deserialize loosely first so we can tell
			// "valid JSON, wrong shape" from "invalid JSON". Without this
			// split, scalar bodies like `42` or `"x"` would be misclassified
			// as ParseError because deserializing them straight into a
			// Dictionary<string, object> throws.
			object raw;
			try {
				raw = JsonConvert.DeserializeObject<object>(body,
					ToolServiceRequestGuard.LosslessArgumentSettings);
			} catch (Exception exception) {
				Log.Warn("MCP request body could not be parsed as JSON.", exception);
				parseError = "Invalid JSON.";
				return null;
			}
			// MapRequest returns null for non-object inputs (array / scalar /
			// JSON null), and DispatchSingle's null-request branch emits the
			// JSON-RPC InvalidRequest reply the spec requires.
			return MapRequest(raw);
		}

		private static string ReadRequestBody(Stream requestStream) {
#if NETSTANDARD2_0 || NET
			Stream stream = requestStream ?? Terrasoft.Web.Http.Abstractions.HttpContext.Current?.Request?.Body;
#else
			Stream stream = requestStream ?? HttpContext.Current?.Request?.InputStream;
#endif
			if (stream == null || !stream.CanRead) {
				return null;
			}
			if (stream.CanSeek) {
				stream.Seek(0, SeekOrigin.Begin);
			}
			using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true)) {
				return reader.ReadToEnd();
			}
		}

		private static IMcpMethodHandler ResolveHandler(string method, UserConnection userConnection,
				Guid serverId, string serverName, bool suspendCapable) {
			switch (method) {
				case "initialize":
					return new McpInitializeHandler(serverName);
				case "ping":
					return new McpPingHandler();
				case "tools/list":
					return new McpToolsListHandler(userConnection, serverId);
				case "tools/call":
					return new McpToolsCallHandler(userConnection, serverId, suspendCapable);
				default:
					return null;
			}
		}

		/// <summary>
		/// Reads the caller's <c>X-Suspend-Capable</c> advertisement off the inbound
		/// request (suspendable-agent-execution design §8.2). The header is stamped by
		/// the runtime only when the dispatch runs under the suspend-aware AgentWorkflow
		/// turn loop; the Test / Live-preview window and any feature-OFF dispatch leave
		/// it absent, so the gate falls closed. Truthy values per the §8.2 contract are
		/// <c>true</c> / <c>1</c> / <c>yes</c> (case-insensitive); anything else — or an
		/// absent header — is treated as not suspend-capable.
		/// </summary>
		private static bool GetSuspendCapableHeader() {
			var context = Terrasoft.Web.Http.Abstractions.HttpContext.Current;
			if (context?.Request?.Headers == null) {
				return false;
			}
			string raw = context.Request.Headers["X-Suspend-Capable"];
			if (raw.IsNullOrWhiteSpace()) {
				return false;
			}
			string normalized = raw.Trim();
			return normalized.Equals("true", StringComparison.OrdinalIgnoreCase)
				|| normalized.Equals("1", StringComparison.Ordinal)
				|| normalized.Equals("yes", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Resolves the <c>{code}</c> path segment to an online MCP server.
		/// Returns <c>null</c> and sets <paramref name="errorResponse"/> (a
		/// ready-to-return JSON-RPC error with HTTP 404) when the code is
		/// missing, unknown, or the server is offline — so unknown/offline
		/// servers leak no tool data.
		/// </summary>
		private McpServerModel ResolveOnlineServer(string code, out Stream errorResponse) {
			errorResponse = null;
			if (code.IsNullOrWhiteSpace()) {
				errorResponse = WriteResponse(
					MakeErrorResponse(null, McpJsonRpcErrorCodes.ResourceNotFound,
						"MCP server code is required."),
					HttpStatusCode.NotFound);
				return null;
			}
			var repository = ClassFactory.Get<IMcpServerRepository>(
				new ConstructorArgument("userConnection", UserConnection));
			McpServerModel server = repository.GetServerByCode(code);
			if (server == null) {
				errorResponse = WriteResponse(
					MakeErrorResponse(null, McpJsonRpcErrorCodes.ResourceNotFound,
						$"MCP server '{code}' was not found."),
					HttpStatusCode.NotFound);
				return null;
			}
			if (!server.IsOnline) {
				errorResponse = WriteResponse(
					MakeErrorResponse(null, McpJsonRpcErrorCodes.ServerOffline,
						$"MCP server '{code}' is offline."),
					HttpStatusCode.NotFound);
				return null;
			}
			return server;
		}

		/// <summary>
		/// Forces the response Content-Type onto both the WCF outgoing response
		/// and the Creatio HttpContext. The WCF assignment is the one that
		/// actually wins: when an operation returns <see cref="Stream"/>, WCF
		/// switches to raw message encoding and overwrites the ambient
		/// <c>HttpContext.Response.ContentType</c> with <c>application/octet-stream</c>
		/// after the operation returns — so without setting
		/// <see cref="WebOperationContext"/>.OutgoingResponse, MCP clients reject
		/// the reply ("Unexpected content type: application/octet-stream"). The
		/// HttpContext assignment is kept for the .NET (Core) host path where
		/// WebOperationContext is unavailable.
		/// </summary>
		private static void SetResponseHeaders(string contentType, HttpStatusCode statusCode) {
#if !(NETSTANDARD2_0 || NET)
			// Classic .NET Framework / WCF host only: returning Stream makes WCF
			// switch to raw message encoding and overwrite HttpContext.Response
			// .ContentType with application/octet-stream after the operation
			// returns, so the WCF outgoing response is the assignment that wins.
			// WebOperationContext does not exist on the .NET (Core) host, where
			// the HttpContext assignment below is sufficient.
			var outgoing = WebOperationContext.Current?.OutgoingResponse;
			if (outgoing != null) {
				outgoing.StatusCode = statusCode;
				outgoing.ContentType = contentType;
			}
#endif
			var context = Terrasoft.Web.Http.Abstractions.HttpContext.Current;
			if (context?.Response != null) {
				context.Response.StatusCode = (int)statusCode;
				context.Response.ContentType = contentType;
			}
		}

		private static Stream WriteJsonResponse(object body, HttpStatusCode statusCode = HttpStatusCode.OK) {
			SetResponseHeaders(JsonContentType, statusCode);
			string json = body == null
				? string.Empty
				: JsonConvert.SerializeObject(body);
			return new MemoryStream(Encoding.UTF8.GetBytes(json));
		}

		private static string GetAcceptHeader() {
			var context = Terrasoft.Web.Http.Abstractions.HttpContext.Current;
			if (context?.Request?.Headers == null) {
				return null;
			}
			return context.Request.Headers["Accept"];
		}

		private static Stream WriteSseResponse(object body, HttpStatusCode statusCode = HttpStatusCode.OK) {
			SetResponseHeaders(McpStreamableHttpWriter.SseContentType, statusCode);
			byte[] payload = McpStreamableHttpWriter.BuildSinglePayload(body);
			return new MemoryStream(payload);
		}

		/// <summary>
		/// Dispatches to JSON or Server-Sent Events based on the client's
		/// <c>Accept</c> header. Per MCP Streamable HTTP spec, a server MAY
		/// stream when the client says so; we pick JSON otherwise. Empty /
		/// no-content paths (HTTP 204) should call <see cref="WriteJsonResponse"/>
		/// directly to avoid emitting an SSE frame around a null body.
		/// </summary>
		private static Stream WriteResponse(object body, HttpStatusCode statusCode = HttpStatusCode.OK) {
			string accept = GetAcceptHeader();
			if (McpStreamableHttpWriter.ClientPrefersSse(accept)) {
				return WriteSseResponse(body, statusCode);
			}
			return WriteJsonResponse(body, statusCode);
		}

		private McpJsonRpcResponse DispatchSingle(McpJsonRpcRequest request, Guid serverId,
				string serverName, bool suspendCapable) {
			if (request == null) {
				return MakeErrorResponse(null, McpJsonRpcErrorCodes.InvalidRequest,
					"Request envelope is missing.");
			}
			if (request.Jsonrpc != McpJsonRpcConstants.Version) {
				return request.IsNotification
					? null
					: MakeErrorResponse(request.Id, McpJsonRpcErrorCodes.InvalidRequest,
						"Unsupported JSON-RPC version. Expected '2.0'.");
			}
			if (request.Method.IsNullOrWhiteSpace()) {
				return request.IsNotification
					? null
					: MakeErrorResponse(request.Id, McpJsonRpcErrorCodes.InvalidRequest,
						"Missing 'method'.");
			}
			if (request.IsNotification || IsNotificationMethod(request.Method)) {
				return null;
			}
			IMcpMethodHandler handler = ResolveHandler(request.Method, UserConnection, serverId,
				serverName, suspendCapable);
			if (handler == null) {
				return MakeErrorResponse(request.Id, McpJsonRpcErrorCodes.MethodNotFound,
					$"Method '{request.Method}' is not supported by this MCP server.");
			}
			try {
				McpHandlerOutcome outcome = handler.Handle(request.Params);
				if (outcome.IsError) {
					return MakeErrorResponse(request.Id, outcome.ErrorCode.Value,
						outcome.ErrorMessage, outcome.ErrorData);
				}
				return new McpJsonRpcResponse {
					Id = request.Id,
					Result = outcome.Result
				};
			} catch (Exception exception) {
				Log.Error($"Unexpected exception while handling MCP method '{request.Method}'.", exception);
				return MakeErrorResponse(request.Id, McpJsonRpcErrorCodes.InternalError,
					"Internal server error.");
			}
		}

		#endregion

		#region Methods: Public

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "{code}/v1/mcp",
			BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream Mcp(string code, Stream requestStream) {
			if (!IsAuthenticated(UserConnection)) {
				return WriteResponse(
					MakeErrorResponse(null, McpJsonRpcErrorCodes.Unauthenticated,
						AuthenticationRequiredMessage),
					HttpStatusCode.Unauthorized);
			}
			// Resolve {code} → online MCP server. Unknown / offline servers fail
			// closed (404 + JSON-RPC error) before any body parsing or tool
			// projection, so tools are partitioned per server.
			McpServerModel server = ResolveOnlineServer(code, out Stream serverError);
			if (server == null) {
				return serverError;
			}
			// Read once at the transport: an HTTP header applies to every JSON-RPC
			// request in the body (a batch shares one header set).
			bool suspendCapable = GetSuspendCapableHeader();
			string body = ReadRequestBody(requestStream);
			object parsed = ParseRequestBody(body, out bool isBatch, out string parseError);
			if (parseError != null) {
				// parseError is already a sanitized, client-safe string —
				// ParseRequestBody logs the raw parser exception internally
				// and surfaces only a generic message here so wire output
				// stays stable across Newtonsoft versions.
				return WriteResponse(
					MakeErrorResponse(null, McpJsonRpcErrorCodes.ParseError,
						"Parse error: " + parseError));
			}
			if (isBatch) {
				var requests = parsed as List<McpJsonRpcRequest> ?? new List<McpJsonRpcRequest>();
				if (requests.Count == 0) {
					return WriteResponse(
						MakeErrorResponse(null, McpJsonRpcErrorCodes.InvalidRequest,
							"Empty batch."));
				}
				var responses = new List<McpJsonRpcResponse>();
				foreach (McpJsonRpcRequest req in requests) {
					McpJsonRpcResponse response = DispatchSingle(req, server.UId, server.Title, suspendCapable);
					if (response != null) {
						responses.Add(response);
					}
				}
				return responses.Count == 0
					? WriteJsonResponse(null, HttpStatusCode.NoContent)
					: WriteResponse(responses);
			}
			var singleRequest = parsed as McpJsonRpcRequest;
			McpJsonRpcResponse singleResponse = DispatchSingle(singleRequest, server.UId, server.Title,
				suspendCapable);
			if (singleResponse == null) {
				return WriteJsonResponse(null, HttpStatusCode.NoContent);
			}
			return WriteResponse(singleResponse);
		}

		#endregion

	}

	#endregion

}

