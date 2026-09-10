using System;
using System.Net;
using Common.Logging;
using Creatio.FeatureToggling;
using Newtonsoft.Json;
using Terrasoft.Common;
using Terrasoft.Core;
using Terrasoft.Core.Factories;
using Terrasoft.Core.Requests;
using Terrasoft.OAuthIntegration;

namespace CrtAIAgenticBridgeApp.Runtime
{
	/// <summary>
	/// Selects the auth mechanism for outbound AI Studio requests. Disabled by
	/// default, so the senders fall back to the static <c>AIStudioEventApiKey</c>
	/// system setting. When enabled, they authenticate via OAuth (the identity
	/// token provider) instead. Flipping the flag selects the mechanism at
	/// runtime, no redeploy.
	/// See <see cref="AIPlatformHttpSenderBase.ApplyAuthorization"/>.
	/// </summary>
	public class UseAIStudioOAuth : FeatureMetadata
	{
		public UseAIStudioOAuth() {
			IsEnabled = false;
		}
	}

	internal abstract class AIPlatformHttpSenderBase
	{
		// The config (WorkspaceConsole/net6+) compiler references the
		// Terrasoft.NewtonsoftWrapper facade, not Newtonsoft.Json directly, so the
		// Newtonsoft.Json.Serialization contract-resolver types
		// (DefaultContractResolver / CamelCaseNamingStrategy) are not available here
		// (APD-2106). camelCase wire keys therefore come from explicit
		// [DataContract]/[DataMember(Name = "...")] on the serialized DTOs rather than
		// a CamelCaseNamingStrategy resolver; every payload type this facade serializes
		// (BulkEventOccurrence*, EventOccurrenceRequest, ProcessCompletionData,
		// EntityChangeData, RunAgenticProcessResponse, TriggerCatalog*) declares its
		// camelCase member names that way. Only NullValueHandling stays here so a null
		// member is still omitted from the serialized body. Dates keep Newtonsoft's
		// default ISO-8601 handling (the AI Studio ingress contract). All senders
		// serialize HTTP bodies through JsonConvert.SerializeObject with these settings;
		// Terrasoft.Common.Json.Json.Serialize is NOT used for wire payloads because it
		// emits unquoted property names (QuoteName = false) and forces MicrosoftDateFormat
		// dates, neither of which the AI Studio JSON contract accepts.
		internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore
		};

		private readonly UserConnection _userConnection;

		protected AIPlatformHttpSenderBase(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		protected UserConnection UserConnection => _userConnection;

		protected bool UseOAuth => Features.GetIsEnabled<UseAIStudioOAuth>();

		protected virtual IIdentityTokenProvider IdentityTokenProvider =>
			Terrasoft.Configuration.IdentityTokenProviderHelper.GetInstance(Constants.OAuth.ServiceCode);

		protected bool TryCreateBaseUrl(ILog log, out string baseUrl, out string apiKey) {
			return TryCreateBaseUrl(log, out baseUrl, out apiKey, out _);
		}

		protected bool TryCreateBaseUrl(ILog log, out string baseUrl, out string apiKey,
				out string failureReason) {
			baseUrl = GetSysSettingValue(Constants.SysSettingCodes.AIStudioServiceUrl);
			if (string.IsNullOrWhiteSpace(baseUrl)) {
				failureReason = GetLocalizableString("ServiceUrlMissingMessage");
				log.Warn(failureReason);
				apiKey = null;
				return false;
			}
			if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _)) {
				failureReason = string.Format(
					GetLocalizableString("ServiceUrlInvalidMessage"), baseUrl);
				log.Warn(failureReason);
				apiKey = null;
				return false;
			}
			apiKey = GetSysSettingValue(Constants.SysSettingCodes.AIStudioEventApiKey);
			if (UseOAuth) {
				if (!IsOAuthClientReady(log)) {
					failureReason = GetLocalizableString("OAuthNotConfiguredMessage");
					log.Warn(failureReason);
					return false;
				}
			} else if (string.IsNullOrWhiteSpace(apiKey)) {
				failureReason = GetLocalizableString("ApiKeyMissingMessage");
				log.Warn(failureReason);
				return false;
			}
			failureReason = null;
			return true;
		}

		private bool IsOAuthClientReady(ILog log) {
			try {
				IIdentityTokenProvider provider = IdentityTokenProvider;
				return provider != null && provider.IsIdentityClientInitialized();
			} catch (Exception ex) {
				log.Error("Failed to resolve the AI Studio OAuth token provider for " +
					$"service code '{Constants.OAuth.ServiceCode}'.", ex);
				return false;
			}
		}

		protected void ApplyAuthorization(HttpRequestConfig config, string apiKey) {
			if (UseOAuth) {
				// Called as a static method (not extension syntax) so this schema does
				// not need a broad `using Terrasoft.Configuration;` import, which would
				// collide with the package's own Constants type.
				Terrasoft.Configuration.HttpRequestClientExt.WithOAuth<UseAIStudioOAuth>(
					config,
					IdentityTokenProvider,
					GetSysSettingValue(Constants.SysSettingCodes.AIStudioOAuthScope));
				return;
			}
			config.Headers.Add("Authorization", $"Bearer {apiKey}");
		}

		protected string GetLocalizableString(string localizableStringName) {
			return GetLocalizableString(nameof(AIPlatformHttpSenderBase), localizableStringName);
		}

		protected virtual string GetLocalizableString(string schemaName, string localizableStringName) {
			string lsv = "LocalizableStrings." + localizableStringName + ".Value";
			var ls = new LocalizableString(
				_userConnection.Workspace.ResourceStorage, schemaName, lsv);
			return ls.ToString();
		}

		protected IHttpRequestClient GetHttpClient() {
			return ClassFactory.Get<IHttpRequestClient>();
		}

		protected string GetSysSettingValue(string settingCode) {
			return Terrasoft.Core.Configuration.SysSettings.GetValue(
				_userConnection, settingCode, string.Empty);
		}

		// APD-1464 S6/R7: read the bulkSendPeriod SystemSetting (integer
		// seconds) governing how often the bridge flushes batched events.
		// Falls back to the 5s default when the setting is absent or
		// non-positive so a misconfiguration never disables flushing.
		// Static so the single owner of the flush cadence (OutboundEventBatcher,
		// which is not an AIPlatformHttpSenderBase) shares this one implementation
		// instead of carrying its own copy.
		internal static TimeSpan GetBulkSendPeriod(UserConnection userConnection) {
			int defaultSeconds = Constants.Defaults.BulkSendPeriodSeconds;
			int seconds = Terrasoft.Core.Configuration.SysSettings.GetValue(
				userConnection,
				Constants.SysSettingCodes.AIStudioBulkSendPeriod,
				defaultSeconds);
			if (seconds <= 0) {
				seconds = defaultSeconds;
			}
			return TimeSpan.FromSeconds(seconds);
		}

		protected static HttpRequestConfig CreateJsonRequest(HttpRequestMethod method, string url) {
			return new HttpRequestConfig {
				Url = new Uri(url),
				Method = method,
				RequestTimeout = Constants.Http.RequestTimeoutMs
			};
		}

		protected static bool IsSuccessStatusCode(HttpStatusCode statusCode) {
			return statusCode == HttpStatusCode.OK
				|| statusCode == HttpStatusCode.Created
				|| statusCode == HttpStatusCode.Accepted
				|| statusCode == HttpStatusCode.NoContent;
		}
	}
}

