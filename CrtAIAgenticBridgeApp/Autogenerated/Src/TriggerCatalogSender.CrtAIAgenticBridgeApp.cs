namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Runtime.Serialization;
	using Common.Logging;
	using CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto;
	using CrtAIAgenticBridgeApp.TriggerDiscovery;
	using Newtonsoft.Json;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;

	public interface ITriggerCatalogSender
	{
		TriggerPushSyncResponseDto SyncAll();
		void CreateTrigger(string triggerCode);
		void UpdateTrigger(string triggerCode);
		void DeleteTrigger(string triggerCode);
	}

	[DefaultBinding(typeof(ITriggerCatalogSender))]
	internal class TriggerCatalogSender : AIPlatformHttpSenderBase, ITriggerCatalogSender
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		public TriggerCatalogSender(UserConnection userConnection) : base(userConnection) {
		}

		public TriggerPushSyncResponseDto SyncAll() {
			TriggerCatalogPushRequest[] items = GetProvider().GetAllTriggers();
			var payload = new TriggerCatalogSyncPayload {
				Triggers = items
			};
			return SendSyncRequest(Constants.ApiRoutes.TriggerCatalogSync, payload);
		}

		public void CreateTrigger(string triggerCode) {
			TriggerCatalogPushRequest payload = GetProvider().GetTrigger(triggerCode);
			if (payload == null) {
				Log.Warn($"Trigger '{triggerCode}' was not found in the local catalog. " +
					"Skipping create push.");
				return;
			}
			SendJson(HttpRequestMethod.POST, Constants.ApiRoutes.TriggerCatalog, payload);
		}

		public void UpdateTrigger(string triggerCode) {
			TriggerCatalogPushRequest payload = GetProvider().GetTrigger(triggerCode);
			if (payload == null) {
				Log.Warn($"Trigger '{triggerCode}' was not found in the local catalog. " +
					"Skipping update push.");
				return;
			}
			SendJson(HttpRequestMethod.PUT, Constants.ApiRoutes.TriggerCatalogItem(triggerCode),
				payload);
		}

		public void DeleteTrigger(string triggerCode) {
			SendJson(HttpRequestMethod.DELETE,
				Constants.ApiRoutes.TriggerCatalogItem(triggerCode), null);
		}

		private ITriggerMetadataProvider GetProvider() {
			return ClassFactory.Get<ITriggerMetadataProvider>(
				new ConstructorArgument("userConnection", UserConnection));
		}

		private TriggerPushSyncResponseDto SendSyncRequest(string route, object payload) {
			if (!TryCreateBaseUrl(Log, out string baseUrl, out string apiKey,
					out string configFailureReason)) {
				throw new InvalidOperationException(configFailureReason);
			}
			string url = baseUrl.TrimEnd('/') + route;
			HttpRequestConfig config = CreateJsonRequest(HttpRequestMethod.POST, url);
			string jsonBody = JsonConvert.SerializeObject(payload, JsonSettings);
			IHttpResponse response;
			try {
				ApplyAuthorization(config, apiKey);
				response = GetHttpClient().SendWithJsonBody(config, jsonBody);
			} catch (Exception ex) {
				Log.Error($"HTTP request to AI Studio failed for URL '{url}'.", ex);
				string template = GetLocalizableString(
					nameof(TriggerCatalogSender), "SyncTransportFailureMessage");
				throw new InvalidOperationException(
					string.Format(template, url, ex.Message), ex);
			}
			if (!IsSuccessStatusCode(response.StatusCode)) {
				string remoteDetail = ExtractRemoteErrorDetail(response);
				Log.Warn($"AI Studio returned HTTP {(int)response.StatusCode} " +
					$"for trigger catalog send to '{url}'. {remoteDetail}");
				throw new InvalidOperationException(BuildSyncRejectedMessage(response));
			}
			return DeserializeSyncResponse(response);
		}

		private string BuildSyncRejectedMessage(IHttpResponse response) {
			string errorMessage = ExtractRemoteErrorMessage(response);
			if (string.IsNullOrWhiteSpace(errorMessage)) {
				return GetLocalizableString(nameof(TriggerCatalogSender), "SyncRejectedMessage");
			}
			// Only the prefix is localizable; the remote error message is surfaced as-is
			// (length-capped by ExtractRemoteErrorMessage), not localized.
			string template = GetLocalizableString(
				nameof(TriggerCatalogSender), "SyncRejectedWithDetailMessage");
			return string.Format(template, errorMessage);
		}

		private const int MaxRemoteMessageLength = 500;

		private static string ExtractRemoteErrorDetail(IHttpResponse response) {
			string content = response?.Content;
			if (string.IsNullOrWhiteSpace(content)) {
				return null;
			}
			return Truncate(content.Trim());
		}

		private static string Truncate(string value) {
			return value.Length > MaxRemoteMessageLength
				? value.Substring(0, MaxRemoteMessageLength) + "..."
				: value;
		}

		private static string ExtractRemoteErrorMessage(IHttpResponse response) {
			string content = response?.Content;
			if (string.IsNullOrWhiteSpace(content)) {
				return null;
			}
			try {
				// Expected shape: { "error": { "code": "...", "message": "..." } }.
				// Parsing into a DTO (instead of Newtonsoft.Json.Linq JObject) keeps this
				// configuration-compiled code from depending on the Newtonsoft.Json.Linq
				// namespace, which is not available in every WorkspaceConsole build (APD-2106).
				// Deserialized without JsonSettings because the Terrasoft.NewtonsoftWrapper
				// facade (net6+ config build) has no DeserializeObject(string, JsonSerializerSettings)
				// overload; the default resolver matches the camelCase wire keys to the DTO
				// members case-insensitively, and NullValueHandling only affects serialization.
				// A non-object root, a string/array "error", or a non-scalar "message" all
				// fail deserialization into the DTO and surface as an exception -> null,
				// matching the previous JObject/JValue behavior.
				RemoteErrorEnvelope envelope =
					JsonConvert.DeserializeObject<RemoteErrorEnvelope>(content);
				string message = envelope?.Error?.Message;
				// Cap the surfaced message so a verbose remote response cannot bloat the
				// user-facing error, mirroring the log-detail cap in ExtractRemoteErrorDetail.
				return string.IsNullOrWhiteSpace(message) ? null : Truncate(message.Trim());
			} catch (Exception ex) when (!(ex is OutOfMemoryException)) {
				// JsonException is not exposed by the NewtonsoftWrapper facade, so the
				// parse-failure -> null contract catches the base Exception type instead.
				// A non-recoverable OutOfMemoryException is excluded by the filter so it
				// propagates rather than being silently downgraded to "no remote message".
				return null;
			}
		}

		private TriggerPushSyncResponseDto DeserializeSyncResponse(IHttpResponse response) {
			if (string.IsNullOrWhiteSpace(response?.Content)) {
				return response?.GetResult<TriggerPushSyncResponseDto>();
			}
			TriggerPushSyncEnvelopeDto envelope =
				JsonConvert.DeserializeObject<TriggerPushSyncEnvelopeDto>(response.Content);
			if (envelope?.Data != null) {
				return envelope.Data;
			}
			return JsonConvert.DeserializeObject<TriggerPushSyncResponseDto>(response.Content);
		}

		private void SendJson(HttpRequestMethod method, string route, object payload) {
			if (!TryCreateBaseUrl(Log, out string baseUrl, out string apiKey)) {
				return;
			}
			string url = baseUrl.TrimEnd('/') + route;
			try {
				HttpRequestConfig config = CreateJsonRequest(method, url);
				ApplyAuthorization(config, apiKey);
				IHttpResponse response;
				if (payload == null) {
					response = GetHttpClient().Send(config);
				} else {
					string jsonBody = JsonConvert.SerializeObject(payload, JsonSettings);
					response = GetHttpClient().SendWithJsonBody(config, jsonBody);
				}
				if (!IsSuccessStatusCode(response.StatusCode)) {
					Log.Warn($"AI Studio returned HTTP {(int)response.StatusCode} " +
						$"for trigger catalog send to '{url}'.");
				}
			} catch (Exception ex) {
				Log.Error($"HTTP request to AI Studio failed for URL '{url}'.", ex);
			}
		}
	}

	// Wire keys are declared with [DataMember(Name = "...")] rather than a
	// CamelCaseNamingStrategy resolver: the Newtonsoft.Json.Serialization resolver
	// types are unavailable on the Terrasoft.NewtonsoftWrapper facade the config build
	// references (APD-2106). Newtonsoft honors [DataContract]/[DataMember] on serialize,
	// so JsonConvert.SerializeObject keeps the payload camelCase without a resolver, and
	// EmitDefaultValue = false keeps a null/absent member out of the body (the
	// null-omission the JsonSettings NullValueHandling.Ignore setting provides).
	[DataContract]
	internal class TriggerCatalogSyncPayload
	{
		[DataMember(Name = "triggers", EmitDefaultValue = false)]
		public TriggerCatalogPushRequest[] Triggers { get; set; }
	}

	// Minimal contract for extracting the remote error.message from a rejected sync
	// response without taking a dependency on the Newtonsoft.Json.Linq namespace (APD-2106).
	// The PascalCase members map to the lowercase wire keys ("error", "message") via
	// Newtonsoft's default case-insensitive property matching rather than [JsonProperty],
	// because the JsonPropertyAttribute type is not exposed by the Terrasoft.NewtonsoftWrapper
	// facade the configuration compiler references (same WorkspaceConsole limitation as the
	// Newtonsoft.Json.Linq gap above).
	internal class RemoteErrorEnvelope
	{
		public RemoteError Error { get; set; }
	}

	internal class RemoteError
	{
		public string Message { get; set; }
	}

	[DataContract]
	public class TriggerCatalogPushRequest
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "sourceSystem")]
		public string SourceSystem { get; set; } = Constants.SourceSystems.Creatio;

		[DataMember(Name = "fields", EmitDefaultValue = false)]
		public TriggerFieldDto[] Fields { get; set; }

		[DataMember(Name = "eventTypes", EmitDefaultValue = false)]
		public TriggerCatalogPushEventTypeRequest[] EventTypes { get; set; }
	}

	[DataContract]
	public class TriggerCatalogPushEventTypeRequest
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "mode")]
		public string Mode { get; set; }
	}

	public sealed class TriggerPushSyncResponseDto
	{
		public DateTime SyncedAt { get; set; }
		public int TriggersUpserted { get; set; }
		public int EventTypesUpserted { get; set; }
		public int TriggersRemoved { get; set; }
		public int EventTypesRemoved { get; set; }
	}

	internal sealed class TriggerPushSyncEnvelopeDto
	{
		public TriggerPushSyncResponseDto Data { get; set; }
	}
}

