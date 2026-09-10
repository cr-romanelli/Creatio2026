namespace Creatio.Copilot
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Security.Cryptography;
	using System.Threading;
	using System.Threading.Tasks;
	using Terrasoft.Common;
	using Terrasoft.Configuration;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;
	using Terrasoft.OAuthIntegration;
	using SysSettings = Terrasoft.Core.Configuration.SysSettings;

	#region Struct: KnwStartSessionResponse

	[Serializable]
	[DataContract]
	public struct KnwStartSessionResponse
	{

		#region Properties: Public

		[DataMember(Name = "sessionId")]
		public Guid SessionId { get; set; }

		[DataMember(Name = "indexId")]
		public Guid IndexId { get; set; }

		#endregion

	}

	#endregion

	#region Struct: KnwSessionResponse

	[Serializable]
	[DataContract]
	public struct KnwSessionResponse
	{

		#region Properties: Public

		[DataMember(Name = "indexId")]
		public Guid IndexId { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "chunksAcked")]
		public int ChunksAcked { get; set; }

		[DataMember(Name = "tokenUsage")]
		public int TokenUsage { get; set; }

		#endregion

	}

	#endregion

	#region Struct: KnwIndexStateResponse

	[Serializable]
	[DataContract]
	public struct KnwIndexStateResponse
	{

		#region Properties: Public

		[DataMember(Name = "progress")]
		public double? Progress { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwKnowledgeSourceServingIndexResponse

	[Serializable]
	[DataContract]
	public sealed class KnwKnowledgeSourceServingIndexResponse
	{

		#region Properties: Public

		[DataMember(Name = "indexId")]
		public Guid IndexId { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "createdAt")]
		public DateTimeOffset? CreatedAt { get; set; }

		[DataMember(Name = "updatedAt")]
		public DateTimeOffset? UpdatedAt { get; set; }

		[DataMember(Name = "activeUnitCount")]
		public int? ActiveUnitCount { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwKnowledgeSourceSessionSnapshotResponse

	[Serializable]
	[DataContract]
	public class KnwKnowledgeSourceSessionSnapshotResponse
	{

		#region Properties: Public

		[DataMember(Name = "sessionId")]
		public Guid SessionId { get; set; }

		[DataMember(Name = "sessionStatus")]
		public string SessionStatus { get; set; }

		[DataMember(Name = "sessionOperation")]
		public string SessionOperation { get; set; }

		[DataMember(Name = "sessionCreatedAt")]
		public DateTimeOffset? SessionCreatedAt { get; set; }

		[DataMember(Name = "sessionCompletedAt")]
		public DateTimeOffset? SessionCompletedAt { get; set; }

		[DataMember(Name = "indexId")]
		public Guid IndexId { get; set; }

		[DataMember(Name = "indexStatus")]
		public string IndexStatus { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwKnowledgeSourceWorkResponse

	[Serializable]
	[DataContract]
	public sealed class KnwKnowledgeSourceWorkResponse : KnwKnowledgeSourceSessionSnapshotResponse
	{

		#region Properties: Public

		[DataMember(Name = "mode")]
		public string Mode { get; set; }

		[DataMember(Name = "totalUnits")]
		public int? TotalUnits { get; set; }

		[DataMember(Name = "ingestedUnits")]
		public int? IngestedUnits { get; set; }

		[DataMember(Name = "extractedUnits")]
		public int? ExtractedUnits { get; set; }

		[DataMember(Name = "chunkedUnits")]
		public int? ChunkedUnits { get; set; }

		[DataMember(Name = "embeddedUnits")]
		public int? EmbeddedUnits { get; set; }

		[DataMember(Name = "upsertedInactiveUnits")]
		public int? UpsertedInactiveUnits { get; set; }

		[DataMember(Name = "upsertedActiveUnits")]
		public int? UpsertedActiveUnits { get; set; }

		[DataMember(Name = "failedUnits")]
		public int? FailedUnits { get; set; }

		[DataMember(Name = "deletedUnits")]
		public int? DeletedUnits { get; set; }

		[DataMember(Name = "completedUnits")]
		public int? CompletedUnits { get; set; }

		[DataMember(Name = "progress")]
		public double? Progress { get; set; }

		#endregion

	}

	#endregion

	#region Struct: KnwKnowledgeSourceIndexingStatusResponse

	[Serializable]
	[DataContract]
	public struct KnwKnowledgeSourceIndexingStatusResponse
	{

		#region Properties: Public

		[DataMember(Name = "knowledgeSourceId")]
		public Guid KnowledgeSourceId { get; set; }

		[DataMember(Name = "summaryStatus")]
		public string SummaryStatus { get; set; }

		[DataMember(Name = "servingIndex")]
		public KnwKnowledgeSourceServingIndexResponse ServingIndex { get; set; }

		[DataMember(Name = "latestSession")]
		public KnwKnowledgeSourceSessionSnapshotResponse LatestSession { get; set; }

		[DataMember(Name = "work")]
		public KnwKnowledgeSourceWorkResponse Work { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwServiceProblemException

	/// <summary>
	/// Represents a structured API problem response returned by the service.
	/// </summary>
	internal sealed class KnwServiceProblemException : InvalidOperationException
	{

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwServiceProblemException"/> class.
		/// </summary>
		/// <param name="statusCode">HTTP status code.</param>
		/// <param name="code">Machine-readable code.</param>
		/// <param name="detail">Problem details message.</param>
		/// <param name="paramsJson">Serialized problem payload returned by the API.</param>
		/// <param name="message">Fallback exception message.</param>
		/// <param name="innerException">Inner exception.</param>
		public KnwServiceProblemException(int? statusCode, string code, string detail, string paramsJson,
				string message, Exception innerException = null)
			: base(message, innerException) {
			StatusCode = statusCode;
			Code = code;
			Detail = detail;
			ParamsJson = paramsJson;
		}

		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets HTTP status code.
		/// </summary>
		public int? StatusCode { get; }

		/// <summary>
		/// Gets machine-readable problem code.
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// Gets problem detail message.
		/// </summary>
		public string Detail { get; }

		/// <summary>
		/// Gets serialized problem payload JSON.
		/// </summary>
		public string ParamsJson { get; }

		#endregion

	}

	#endregion

	#region Class: KnwServiceClient

	/// <summary>
	/// Implementation of <see cref="IKnwServiceClient"/> for interacting with the knowledge base service.
	/// </summary>
	[DefaultBinding(typeof(IKnwServiceClient))]
	internal class KnwServiceClient : IKnwServiceClient
	{

		#region Class: KnwGetKnwItemResponse

		[Serializable]
		[DataContract]
		private class KnwGetKnwItemResponse
		{

			#region Properties: Public

			[DataMember(Name = "results")]
			public List<KnwBaseItemResponse> Results { get; set; } = new List<KnwBaseItemResponse>();

			#endregion

		}

		#endregion

		#region Class: CreateSessionResponse

		[Serializable]
		[DataContract]
		private class CreateSessionResponse
		{

			#region Properties: Public

			[DataMember(Name = "sessionId")]
			public Guid SessionId { get; set; }

			[DataMember(Name = "indexId")]
			public Guid IndexId { get; set; }

			#endregion

		}

		#endregion

		//TODO Discussed with Krzysztof to remove data wrapper from microservice as it is redundant
		#region Class: CreateSessionResponseWrapper

		[Serializable]
		[DataContract]
		private class ServiceResponseWrapper
		{
			#region Properties: Public

			[DataMember(Name = "data")]
			public CreateSessionResponse Data { get; set; }

			#endregion
		}

		#endregion

		#region Class: ChunkUploadResponse

		[Serializable]
		[DataContract]
		private class ChunkUploadResponse
		{

			#region Properties: Public

			[DataMember(Name = "acknowledged")]
			public bool Acknowledged { get; set; }

			#endregion

		}

		#endregion

		#region Class: SessionStatusResponse

		[Serializable]
		[DataContract]
		private class SessionStatusResponse
		{

			#region Properties: Public

			[DataMember(Name = "sessionId")]
			public Guid SessionId { get; set; }

			[DataMember(Name = "status")]
			public string Status { get; set; }

			#endregion

		}

		#endregion

		#region Class: ErrorResponse

		[Serializable]
		[DataContract]
		private class ErrorResponse
		{

			#region Properties: Public

			[DataMember(Name = "error")]
			public string Error { get; set; }

			#endregion

		}

		#endregion

		#region Class: ApiRoutes

		private static class ApiRoutes
		{

			#region Fields: Public

			public static readonly string Retrieve = AppendVersion("/retrieve");
			public static readonly string Sessions = AppendVersion("/sessions");
			public static readonly string KnowledgeSourceFiles = AppendVersion("/knowledgesources/{0}/files");
			public static readonly string KnowledgeSourceInvalidate = AppendVersion("/KnowledgeSources/{0}/invalidate");
			public static readonly string Configurations = AppendVersion("/configurations");
			public static readonly string ConfigurationsResolve = AppendVersion("/configurations/resolve");
			public static readonly string HealthReady = AppendVersion("/health/ready");

			#endregion

			#region Methods: Public

			public static string ConfigurationById(Guid id) => AppendVersion($"/configurations/{id}");

			#endregion

			#region Methods: Private

			private static string AppendVersion(string suffix) => ApiPrefix + DefaultVersionPrefix + suffix;

			#endregion

			#region Methods: Public

			public static string SessionChunks(Guid sessionId) {
				return AppendVersion($"/sessions/{sessionId}/chunks");
			}

			public static string SessionFinalize(Guid sessionId) {
				return AppendVersion($"/sessions/{sessionId}/finalize");
			}

			public static string GetSession(Guid sessionId) {
				return AppendVersion($"/sessions/{sessionId}");
			}

			public static string SessionCancel(Guid sessionId) {
				return AppendVersion($"/sessions/{sessionId}/cancel");
			}

			public static string IndexState(Guid indexId) {
				return AppendVersion($"/indexing/{indexId}/state");
			}

			public static string IndexCancel(Guid indexId) {
				return AppendVersion($"/indexing/{indexId}/cancel");
			}

			public static string KnowledgeSourceIndexingStatus(Guid knowledgeSourceId) {
				return AppendVersion($"/knowledgesources/{knowledgeSourceId}/indexing-status");
			}

			#endregion

		}

		#endregion

		#region Constants: Private

		private const string CorrelationIdHeader = "X-Correlation-ID";
		private const string DefaultVersionPrefix = "v1";
		private const string ApiPrefix = "api/";
		private const string KnwSettingUrlName = "KnowledgeServiceUrl";
		private const string KnowledgeServiceUploadChunkSizeName = "KnowledgeServiceUploadChunkSize";
		private const string InitialSessionType = "Initial";
		private const string IncrementalSessionType = "Incremental";
		private const string OAuthScope = "use_enrichment";
		private const int DefaultTimeoutSeconds = 30;
		private const int DefaultChunkSizeBytes = 4194304;

		#endregion

		#region Fields: Private

		private readonly UserConnection _userConnection;

		private readonly IHttpRequestClient _httpRequestClient;

		private readonly IIdentityServiceWrapper _identityServiceWrapper;
		private IKnwServiceProblemHelper _serviceProblemHelper;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwServiceClient"/> class.
		/// </summary>
		/// <param name="userConnection">The user connection context.</param>
		/// <param name="httpRequestClient">The HTTP request client for service communication.</param>
		public KnwServiceClient(UserConnection userConnection, IHttpRequestClient httpRequestClient) {
			_userConnection = userConnection;
			_httpRequestClient = httpRequestClient;
			_identityServiceWrapper = IdentityServiceWrapperHelper.GetInstance();
		}

		#endregion

		#region Properties: Private

		// KnowledgeServiceUrl is read as-is. It is configured at install time (defaulting to the demo URL when it cannot
		// be resolved from the enrichment URL) and may be changed by an administrator. The runtime intentionally does
		// NOT resolve or fall back at call time: silently substituting a different URL would point existing, already
		// indexed knowledge sources at the wrong knowledge service. If the setting is empty the request fails.
		private string KnwServiceUrlSetting => SysSettings.GetValue(_userConnection, KnwSettingUrlName, string.Empty);

		private int ChunkSizeBytes => SysSettings.GetValue(_userConnection, KnowledgeServiceUploadChunkSizeName, DefaultChunkSizeBytes);

		private Uri ServiceUrl {
			get {
				string baseValue = KnwServiceUrlSetting;
				if (string.IsNullOrWhiteSpace(baseValue)) {
					return null;
				}
				baseValue = baseValue.Trim();
				baseValue = NormalizeBaseUrl(baseValue);
				return new Uri(baseValue);
			}
		}

		private int RequestTimeout => DefaultTimeoutSeconds;

		private IKnwServiceProblemHelper ServiceProblemHelper =>
			_serviceProblemHelper ?? (_serviceProblemHelper = ClassFactory.Get<IKnwServiceProblemHelper>());

		#endregion

		#region Methods: Private

		private static string CalculateHash(byte[] bytes) {
			string hash;
			using (SHA256 sha256 = SHA256.Create()) {
				byte[] hashBytes = sha256.ComputeHash(bytes);
				hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
			}
			return hash;
		}

		private bool UploadChunk(Guid sessionId, string itemId, string displayName, int sequence, byte[] chunkData,
					CancellationToken cancellationToken) {
			var body = new {
				itemId = itemId,
				itemName = displayName,
				sequence = sequence,
				bytes = Convert.ToBase64String(chunkData),
				size = chunkData.LongLength,
				hash = CalculateHash(chunkData)
			};
			string route = ApiRoutes.SessionChunks(sessionId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.POST, body: body,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(request);
			EnsureSuccess(response, $"Upload chunk {sequence} for item '{displayName}' (ID: {itemId})", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				return false;
			}
			var dto = JsonConvert.DeserializeObject<ChunkUploadResponse>(response.Content);
			return dto?.Acknowledged ?? false;
		}

		private bool UploadChunkWithRetry(Guid sessionId, string itemId, string displayName, int sequence,
					byte[] chunkData, CancellationToken cancellationToken, int maxRetries = 2) {
			int attempt = 0;
			Exception lastException = null;
			while (attempt <= maxRetries) {
				try {
					cancellationToken.ThrowIfCancellationRequested();
					bool acknowledged = UploadChunk(sessionId, itemId, displayName, sequence, chunkData,
						cancellationToken);
					if (acknowledged) {
						return true;
					}
					lastException = new InvalidOperationException(
						$"Chunk {sequence} not acknowledged by service on attempt {attempt + 1}");
				} catch (Exception ex) {
					if (ServiceProblemHelper.IsTerminalUploadFailure(ex)) {
						KnwServiceProblemException problemException = ServiceProblemHelper.FindProblemException(ex);
						throw problemException ?? ex;
					}
					lastException = ex;
				}
				attempt++;
				if (attempt <= maxRetries) {
					cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
				}
			}
			throw new InvalidOperationException(
				$"Failed to upload chunk {sequence} after {maxRetries + 1} attempts", lastException);
		}

		private static string NormalizeBaseUrl(string raw) {
			if (!Uri.TryCreate(raw, UriKind.Absolute, out Uri parsedUrl)) {
				throw new InvalidOperationException(
					"Knowledge service base URL must be an absolute URL with explicit http/https protocol.");
			}
			if (!string.Equals(parsedUrl.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(parsedUrl.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidOperationException(
					"Knowledge service base URL must use http or https protocol.");
			}
			return raw;
		}

		private static ConfigurationScopeResponse DeserializeConfigurationSingle(string content, string context) {
			if (string.IsNullOrWhiteSpace(content)) {
				throw new InvalidOperationException($"{context} response content is empty.");
			}
			ConfigurationScopeResponse dto = JsonConvert.DeserializeObject<ConfigurationScopeResponse>(content);
			if (dto == null) {
				throw new InvalidOperationException($"{context} response content cannot be deserialized.");
			}
			return dto;
		}

		private static KnwStartSessionResponse DeserializeCreateSessionResponse(string content) {
			string trimmed = content?.TrimStart() ?? string.Empty;
			if (trimmed.StartsWith("<", StringComparison.Ordinal)) {
				throw new InvalidOperationException("Create indexing session returned non-JSON content that looks " +
					"like HTML. Check KnowledgeServiceUrl and API endpoint settings.");
			}
			try {
				ServiceResponseWrapper wrapped = JsonConvert.DeserializeObject<ServiceResponseWrapper>(content);
				if (wrapped?.Data != null && wrapped.Data.SessionId != Guid.Empty) {
					return new KnwStartSessionResponse {
						SessionId = wrapped.Data.SessionId,
						IndexId = wrapped.Data.IndexId
					};
				}
				CreateSessionResponse direct = JsonConvert.DeserializeObject<CreateSessionResponse>(content);
				if (direct != null && direct.SessionId != Guid.Empty) {
					return new KnwStartSessionResponse {
						SessionId = direct.SessionId,
						IndexId = direct.IndexId
					};
				}
			} catch (Exception e) {
				throw new InvalidOperationException("Failed to deserialize create indexing session response. " +
					"Ensure KnowledgeServiceUrl points to the Knowledge Service API and returns JSON.", e);
			}
			throw new InvalidOperationException("Invalid create session response. Expected either wrapped " +
				"'data.sessionId/indexId' or direct 'sessionId/indexId' JSON fields.");
		}

		private static void EnsureSuccess(IHttpResponse response, string context, int timeoutSeconds) {
			if (response == null) {
				throw new InvalidOperationException($"{context} response is null.");
			}
			if (response.IsTimedOut) {
				throw new TimeoutException($"{context} request timed out after {timeoutSeconds} seconds");
			}
			if (!response.IsSuccessStatusCode || response.Exception != null) {
				KnwServiceProblemException problemException = TryCreateProblemException(response, context);
				if (problemException != null) {
					throw problemException;
				}
				string serverError = null;
				if (!string.IsNullOrWhiteSpace(response.Content)) {
					try {
						var errorDto = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
						if (!string.IsNullOrWhiteSpace(errorDto?.Error)) {
							serverError = errorDto.Error;
						}
					} catch {
						serverError = null;
					}
				}
				string msg = serverError ?? response.Exception?.Message ?? response.ReasonPhrase;
				throw new InvalidOperationException($"{context} request failed: {msg}", response.Exception);
			}
		}

		private static KnwServiceProblemException TryCreateProblemException(IHttpResponse response, string context) {
			if (response == null || string.IsNullOrWhiteSpace(response.Content)) {
				return null;
			}
			try {
				var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
				if (payload == null || !payload.TryGetValue("code", out object codeObj)) {
					return null;
				}
				string code = codeObj?.ToString();
				if (string.IsNullOrWhiteSpace(code)) {
					return null;
				}
				int? statusCode = response.StatusCode == 0 ? (int?)null : (int)response.StatusCode;
				string detail = null;
				if (payload.TryGetValue("detail", out object detailObj)) {
					detail = detailObj?.ToString();
				}
				if (string.IsNullOrWhiteSpace(detail)) {
					detail = response.ReasonPhrase;
				}
				string message = $"{context} request failed: {code}";
				return new KnwServiceProblemException(statusCode, code, detail, response.Content, message,
					response.Exception);
			} catch {
				return null;
			}
		}

		private HttpRequestConfig CreateRequest(string relativePath, HttpRequestMethod method, object body = null,
				Dictionary<string, string> queryParams = null, CancellationToken cancellationToken = default) {
			Uri serviceUrl = ServiceUrl;
			if (serviceUrl == null) {
				throw new InvalidOperationException("Knowledge service base URL is not configured.");
			}
			var url = new Uri(serviceUrl, relativePath);
			var correlationId = Guid.NewGuid().ToString();
			var request = new HttpRequestConfig {
				Url = url,
				Method = method,
				RequestTimeout = RequestTimeout,
				Body = body,
				CancellationToken = cancellationToken
			}.WithOAuth<KnwFeatures.UseOAuth>(_identityServiceWrapper, OAuthScope);
			if (queryParams != null) {
				foreach (KeyValuePair<string, string> kv in queryParams) {
					request.AddQueryParam(kv.Key, kv.Value);
				}
			}
			request.Headers.Add(CorrelationIdHeader, correlationId);
			return request;
		}

		private HttpRequestConfig CreateRetrieveRequest(Guid knwSourceId, Guid sessionId, string query, int skip,
				int take, IEnumerable<string> keywords, CancellationToken ct) {
			var body = new {
				sessionId = sessionId.ToString(),
				query,
				knowledgeSourceIds = new[] {
					knwSourceId
				},
				skip,
				take,
				keywords = keywords?.Any() == true ? keywords : null
			};
			return CreateRequest(ApiRoutes.Retrieve, HttpRequestMethod.POST, body: body, cancellationToken: ct);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public IEnumerable<KnwBaseItemResponse> GetKnowledgeBaseItemsByIds(KnwSearchQuery request,
				CancellationToken cancellationToken) {
			HttpRequestConfig retrieveRequest = CreateRetrieveRequest(request.KnwSourceId, request.SessionId,
				request.Query, request.Skip, request.Take, request.Keywords, cancellationToken);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(retrieveRequest);
			if (response == null) {
				return Enumerable.Empty<KnwBaseItemResponse>();
			}
			if (response.IsTimedOut) {
				throw new TimeoutException($"RAG retrieval request timed out after {RequestTimeout} seconds");
			}
			if (!response.IsSuccessStatusCode || response.Exception != null) {
				string msg = response.Exception?.Message ?? response.ReasonPhrase;
				throw new InvalidOperationException("RAG retrieval request failed: " + msg, response.Exception);
			}
			if (string.IsNullOrWhiteSpace(response.Content)) {
				return Enumerable.Empty<KnwBaseItemResponse>();
			}
			try {
				KnwGetKnwItemResponse serviceResponse = JsonConvert.DeserializeObject<KnwGetKnwItemResponse>(
					response.Content);
				if (serviceResponse?.Results == null || serviceResponse.Results.Count == 0) {
					return Enumerable.Empty<KnwBaseItemResponse>();
				}
				return serviceResponse.Results;
			} catch (Exception e) {
				throw new InvalidOperationException("Failed to deserialize RAG retrieval response", e);
			}
		}

		/// <inheritdoc />
		public KnwStartSessionResponse CreateIndexingSession(Guid knowledgeSourceId, KnwIndexingSessionTypeEnum sessionType,
				CancellationToken cancellationToken) {
			string sessionTypeValue;
			switch (sessionType) {
				case KnwIndexingSessionTypeEnum.Initial:
					sessionTypeValue = InitialSessionType;
					break;
				case KnwIndexingSessionTypeEnum.Incremental:
					sessionTypeValue = IncrementalSessionType;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(sessionType), sessionType,
						"Unsupported indexing session type");
			}
			var body = new {
				knowledgeSourceId = knowledgeSourceId,
				chunkSize = DefaultChunkSizeBytes,
				sessionType = sessionTypeValue
			};
			HttpRequestConfig request = CreateRequest(ApiRoutes.Sessions, HttpRequestMethod.POST, body: body,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(request);
			EnsureSuccess(response, "Create indexing session", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Create indexing session response content is empty.");
			}
			return DeserializeCreateSessionResponse(response.Content);
		}

		/// <inheritdoc />
		public bool UploadItem(Guid sessionId, KnwItem item, CancellationToken cancellationToken) {
			if (sessionId.IsEmpty()) {
				throw new ArgumentNullOrEmptyException(nameof(sessionId));
			}
			if (item == null) {
				throw new ArgumentNullOrEmptyException(nameof(item));
			}
			if (string.IsNullOrWhiteSpace(item.Id)) {
				throw new ArgumentException("KnwItem.Id must be provided", nameof(item));
			}
			if (item.ContentStreamFactory == null) {
				return false;
			}
			using (Stream stream = item.ContentStreamFactory(cancellationToken)) {
				if (stream == null) {
					return false;
				}
				int chunkSize = ChunkSizeBytes;
				byte[] buffer = new byte[chunkSize];
				int sequence = 0;
				int bytesRead;
				while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
					cancellationToken.ThrowIfCancellationRequested();
					byte[] chunkData;
					if (bytesRead < buffer.Length) {
						chunkData = new byte[bytesRead];
						Array.Copy(buffer, 0, chunkData, 0, bytesRead);
					} else {
						chunkData = buffer;
					}
					bool acknowledged = UploadChunkWithRetry(sessionId, item.Id, item.DisplayName, sequence, chunkData, cancellationToken);
					if (!acknowledged) {
						return false;

					}
					sequence++;
				}
				if (sequence == 0) {
					return false;
				}
				return true;
			}
		}

		/// <inheritdoc />
		public string FinalizeSession(Guid sessionId, CancellationToken cancellationToken) {
			string route = ApiRoutes.SessionFinalize(sessionId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.POST,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Finalize session", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Finalize session response content is empty.");
			}
			var dto = JsonConvert.DeserializeObject<SessionStatusResponse>(response.Content);
			if (dto == null || string.IsNullOrWhiteSpace(dto.Status)) {
				throw new InvalidOperationException("Invalid finalize session response.");
			}
			return dto.Status;
		}

		/// <inheritdoc />
		public string CancelSession(Guid sessionId, CancellationToken cancellationToken) {
			string route = ApiRoutes.SessionCancel(sessionId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.POST,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Cancel session", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Cancel session response content is empty.");
			}
			var dto = JsonConvert.DeserializeObject<SessionStatusResponse>(response.Content);
			if (dto == null || dto.SessionId == Guid.Empty) {
				throw new InvalidOperationException("Invalid cancel session response.");
			}
			return dto.Status;
		}

		/// <inheritdoc />
		public KnwIndexStateResponse CancelIndexingProcess(Guid indexId, CancellationToken cancellationToken) {
			string route = ApiRoutes.IndexCancel(indexId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.POST,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);

			// Handle 204 NoContent success response
			if (response.StatusCode == System.Net.HttpStatusCode.NoContent) {
				return new KnwIndexStateResponse {
					Status = "Cancelled",
					Progress = null
				};
			}

			EnsureSuccess(response, "Cancel indexing", RequestTimeout);

			// Handle other success responses with content
			if (!string.IsNullOrWhiteSpace(response.Content)) {
				var dto = JsonConvert.DeserializeObject<KnwIndexStateResponse>(response.Content);
				return dto;
			}

			return new KnwIndexStateResponse {
				Status = "Cancelled",
				Progress = null
			};
		}

		/// <inheritdoc />
		public KnwSessionResponse GetSession(Guid sessionId, CancellationToken cancellationToken) {
			string route = ApiRoutes.GetSession(sessionId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.GET,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Get session", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Get session response content is empty.");
			}
			KnwSessionResponse dto = JsonConvert.DeserializeObject<KnwSessionResponse>(response.Content);
			return dto;
		}

		/// <inheritdoc />
		public void DeleteFiles(Guid knowledgeSourceId, List<string> fileIds, CancellationToken cancellationToken) {
			if (knowledgeSourceId.IsEmpty()) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			if (fileIds == null) {
				throw new ArgumentNullException(nameof(fileIds), "File IDs list cannot be null");
			}

			string route = string.Format(ApiRoutes.KnowledgeSourceFiles, knowledgeSourceId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.DELETE, body: fileIds,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(request);
			if (response == null) {
				throw new InvalidOperationException("Delete files request received no response");
			}
			if (response.IsTimedOut) {
				throw new TimeoutException($"Delete files request timed out after {RequestTimeout} seconds");
			}
			if (!response.IsSuccessStatusCode) {
				string msg = response.Exception?.Message ?? response.ReasonPhrase;
				throw new InvalidOperationException($"Delete files request failed: {msg}", response.Exception);
			}
		}

		/// <inheritdoc />
		public void InvalidateKnowledgeSource(Guid knowledgeSourceId, CancellationToken cancellationToken) {
			if (knowledgeSourceId.IsEmpty()) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			string route = string.Format(ApiRoutes.KnowledgeSourceInvalidate, knowledgeSourceId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.POST,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			if (response?.StatusCode == System.Net.HttpStatusCode.NoContent) {
				return;
			}
			EnsureSuccess(response, "Invalidate knowledge source", RequestTimeout);
		}

		/// <inheritdoc />
		public KnwIndexStateResponse GetIndexState(Guid indexId, CancellationToken cancellationToken) {
			if (indexId.IsEmpty()) {
				throw new ArgumentNullOrEmptyException(nameof(indexId));
			}
			string route = ApiRoutes.IndexState(indexId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.GET,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Get index state", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Get index state response content is empty.");
			}
			KnwIndexStateResponse dto = JsonConvert.DeserializeObject<KnwIndexStateResponse>(response.Content);
			return dto;
		}

		/// <inheritdoc />
		public KnwKnowledgeSourceIndexingStatusResponse GetKnowledgeSourceIndexingStatus(Guid knowledgeSourceId,
				CancellationToken cancellationToken) {
			if (knowledgeSourceId.IsEmpty()) {
				throw new ArgumentNullOrEmptyException(nameof(knowledgeSourceId));
			}
			string route = ApiRoutes.KnowledgeSourceIndexingStatus(knowledgeSourceId);
			HttpRequestConfig request = CreateRequest(route, HttpRequestMethod.GET,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Get knowledge source indexing status", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				throw new InvalidOperationException("Get knowledge source indexing status response content is empty.");
			}
			KnwKnowledgeSourceIndexingStatusResponse dto =
				JsonConvert.DeserializeObject<KnwKnowledgeSourceIndexingStatusResponse>(response.Content);
			return dto;
		}

		/// <inheritdoc />
		public Task<ConfigurationScopeResponse> GetResolvedConfigurationAsync(Guid knowledgeSourceId,
				CancellationToken ct = default) {
			var queryParams = new Dictionary<string, string> {
				["knowledgeSourceId"] = knowledgeSourceId.ToString()
			};
			HttpRequestConfig request = CreateRequest(ApiRoutes.ConfigurationsResolve, HttpRequestMethod.GET,
				queryParams: queryParams, cancellationToken: ct);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Resolve configuration", RequestTimeout);
			ConfigurationScopeResponse dto = DeserializeConfigurationSingle(response.Content, "Resolve configuration");
			return Task.FromResult(dto);
		}

		/// <inheritdoc />
		public Task<ConfigurationScopeResponse> CreateConfigurationAsync(ConfigurationScopeRequest request,
				CancellationToken ct = default) {
			if (request == null) {
				throw new ArgumentNullException(nameof(request));
			}
			var body = new Dictionary<string, object> {
				["knowledgeSourceId"] = request.KnowledgeSourceId
			};
			if (request.RetrieveSettings != null) {
				body["retrieveSettings"] = request.RetrieveSettings;
			}
			if (request.RankingSettings != null) {
				body["rankingSettings"] = request.RankingSettings;
			}
			HttpRequestConfig httpRequest = CreateRequest(ApiRoutes.Configurations, HttpRequestMethod.POST,
				body: body, cancellationToken: ct);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(httpRequest);
			EnsureSuccess(response, "Create configuration", RequestTimeout);
			ConfigurationScopeResponse dto = DeserializeConfigurationSingle(response.Content, "Create configuration");
			return Task.FromResult(dto);
		}

		/// <inheritdoc />
		public Task<ConfigurationScopeResponse> UpdateConfigurationAsync(Guid id, ConfigurationScopeRequest request,
				CancellationToken ct = default) {
			if (request == null) {
				throw new ArgumentNullException(nameof(request));
			}
			var body = new Dictionary<string, object>();
			if (request.RetrieveSettings != null) {
				body["retrieveSettings"] = request.RetrieveSettings;
			}
			if (request.RankingSettings != null) {
				body["rankingSettings"] = request.RankingSettings;
			}
			HttpRequestConfig httpRequest = CreateRequest(ApiRoutes.ConfigurationById(id), HttpRequestMethod.PUT,
				body: body, cancellationToken: ct);
			IHttpResponse response = _httpRequestClient.SendWithJsonBody(httpRequest);
			EnsureSuccess(response, "Update configuration", RequestTimeout);
			ConfigurationScopeResponse dto = DeserializeConfigurationSingle(response.Content, "Update configuration");
			return Task.FromResult(dto);
		}

		/// <inheritdoc />
		public Task DeleteConfigurationAsync(Guid id, CancellationToken ct = default) {
			HttpRequestConfig httpRequest = CreateRequest(ApiRoutes.ConfigurationById(id), HttpRequestMethod.DELETE,
				cancellationToken: ct);
			IHttpResponse response = _httpRequestClient.Send(httpRequest);
			EnsureSuccess(response, "Delete configuration", RequestTimeout);
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public Task<IEnumerable<ConfigurationScopeResponse>> ListConfigurationsAsync(
				Guid? knowledgeSourceId = null, CancellationToken ct = default) {
			Dictionary<string, string> queryParams = null;
			if (knowledgeSourceId.HasValue) {
				queryParams = new Dictionary<string, string> {
					["knowledgeSourceId"] = knowledgeSourceId.Value.ToString()
				};
			}
			HttpRequestConfig httpRequest = CreateRequest(ApiRoutes.Configurations, HttpRequestMethod.GET,
				queryParams: queryParams, cancellationToken: ct);
			IHttpResponse response = _httpRequestClient.Send(httpRequest);
			EnsureSuccess(response, "List configurations", RequestTimeout);
			if (string.IsNullOrWhiteSpace(response.Content)) {
				return Task.FromResult<IEnumerable<ConfigurationScopeResponse>>(Array.Empty<ConfigurationScopeResponse>());
			}
			List<ConfigurationScopeResponse> dto =
				JsonConvert.DeserializeObject<List<ConfigurationScopeResponse>>(response.Content)
				?? new List<ConfigurationScopeResponse>();
			return Task.FromResult<IEnumerable<ConfigurationScopeResponse>>(dto);
		}

		/// <inheritdoc />
		public void CheckServiceHealth(CancellationToken cancellationToken) {
			HttpRequestConfig request = CreateRequest(ApiRoutes.HealthReady, HttpRequestMethod.GET,
				cancellationToken: cancellationToken);
			IHttpResponse response = _httpRequestClient.Send(request);
			EnsureSuccess(response, "Knowledge service health check", RequestTimeout);
		}

		#endregion

	}

	#endregion

}

