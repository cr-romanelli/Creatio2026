namespace CrtAiTwinApp
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using Creatio.FeatureToggling;
	using global::Common.Logging;
	using Terrasoft.Common.Threading;
	// Contract types (IIdentityAssertionHandler, EnsureUserResult,
	// EnsureUserStatus) live in the CrtIdentityAssertionService schema so they
	// also compile on platform integration builds — see the note there.
	using Terrasoft.Configuration;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Store;
	using Terrasoft.OAuthIntegration;
	using Terrasoft.OAuthIntegration.DTO;
	using CoreSysSettings = Terrasoft.Core.Configuration.SysSettings;

	#region Class: IdentityAssertionHandler

	/// <summary>
	/// Business logic behind <see cref="Terrasoft.Configuration.CrtIdentityAssertionService"/>:
	/// validates that the current user has an email, obtains a current-user identity
	/// assertion JWT, provisions the user in Identity via
	/// <c>POST /api/auth/external-provisioning</c> and, once the user is active, links
	/// the federated identity (sub, iss) to the native Identity account via
	/// <c>POST /api/auth/external-linking</c> (APD-4360).
	/// </summary>
	[DefaultBinding(typeof(IIdentityAssertionHandler))]
	// internal (not public) to match IIdentityAssertionHandler, which lives in
	// the CrtIdentityAssertionService schema with internal accessibility — a
	// public class exposing internal types in its signatures is CS0050. The
	// ClassFactory binding still resolves through the public constructor.
	internal class IdentityAssertionHandler : IIdentityAssertionHandler
	{

		#region Constants: Private

		private const string IdentityApiScope = "identity_platform_api";
		private const string ExternalProvisioningResource = "/api/auth/external-provisioning";
		private const string ExternalLinkingResource = "/api/auth/external-linking";
		private const string IdentityServerUrlSettingCode = "IdentityServerUrl";
		private const string DefaultIdentityProviderSettingCode = "DefaultIdentityProvider";
		private const string IdentityProvidersManagementFeature = "IdentityProvidersManagement";

		// External-service code of the AI Studio registration in the Identity
		// Management registry (seeded by CrtAIAgenticBridgeApp); the registry maps
		// it to the Identity connection that owns the client credentials.
		private const string IdentityServiceCode = "CreatioAIStudio";

		// Wire-stable status values of POST /api/auth/external-provisioning
		// (see Identity docs/EXTERNAL_PROVISIONING_USER_GUIDE.md).
		private const string ProvisioningStatusActive = "active";
		private const string ProvisioningStatusInvited = "invited";
		private const string ProvisioningStatusReinvited = "reinvited";

		// Wire-stable status values of POST /api/auth/external-linking
		// (see Identity docs/EXTERNAL_LINKING_GUIDE.md). Both mean the federated
		// identity is linked to the current native user; already_linked is the
		// idempotent repeat outcome.
		private const string LinkingStatusLinked = "linked";
		private const string LinkingStatusAlreadyLinked = "already_linked";

		// Session marker: set once the user completed provisioning + linking, so
		// repeated chat openings within one Creatio session skip both Identity
		// calls (they are idempotent, so a lost marker only costs a cheap re-run).
		// The session boundary also bounds staleness: Creatio gets no signal when
		// Identity deactivates a user, and the next login re-checks (the actual
		// enforcement — token exchange at Identity — rejects a deactivated user
		// immediately regardless). The value is the UTC completion moment, not a
		// flag, so a hard TTL cap can be layered on later without a format change.
		// Deliberately NOT gated by CrtAiTwinApiResultCache: this replaces the
		// always-on in-memory dedupe that predates that feature (APD-4360),
		// narrowing its scope from app-domain to session — a traffic dedupe, not
		// a rollout-gated cache.
		private const string ProvisionedSessionKey = "CrtAiTwinApp.EnsureUser.ProvisionedAtUtc";

		// Defensive clamp for the Retry-After passthrough: a malformed or huge
		// header value must not overflow the int cast downstream nor lock the
		// client-side confirmation cache for hours.
		private const int MaxRetryAfterSeconds = 3600;

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(typeof(IdentityAssertionHandler));

		private static readonly HttpClient IdentityHttpClient = new HttpClient {
			Timeout = TimeSpan.FromSeconds(30)
		};

		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		public IdentityAssertionHandler(UserConnection userConnection)
		{
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Public

		public virtual EnsureUserResult EnsureUser(bool forceResendInvite)
		{
			string email = GetCurrentUserEmail();
			if (string.IsNullOrEmpty(email))
			{
				return new EnsureUserResult
				{
					Status = EnsureUserStatus.MissingEmail
				};
			}

			if (_userConnection.SessionData.GetValue(ProvisionedSessionKey, DateTime.MinValue)
				!= DateTime.MinValue)
			{
				return new EnsureUserResult
				{
					Status = EnsureUserStatus.Success
				};
			}

			string token = GetCurrentUserAssertionToken();
			string accessToken = GetIdentityAccessToken();
			string identityServerUrl = GetIdentityServerUrl();

			(bool isActive, int? retryAfterSeconds) =
				ProvisionExternalUser(identityServerUrl, accessToken, token, forceResendInvite);
			if (!isActive)
			{
				return new EnsureUserResult
				{
					Status = EnsureUserStatus.ConfirmationEmailSent,
					RetryAfterSeconds = retryAfterSeconds
				};
			}

			// APD-4363: bind the federated identity (sub, iss) to the native
			// Identity account, so Token Exchange resolves this user natively.
			LinkExternalUser(identityServerUrl, accessToken, token);

			_userConnection.SessionData[ProvisionedSessionKey] = DateTime.UtcNow;
			return new EnsureUserResult
			{
				Status = EnsureUserStatus.Success,
				Token = token
			};
		}

		#endregion

		#region Methods: Private

		private string GetCurrentUserEmail()
		{
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager, "SysAdminUnit")
			{
				UseAdminRights = false,
				IgnoreDisplayValues = true
			};
			esq.PrimaryQueryColumn.IsVisible = false;
			var emailColumn = esq.AddColumn("Email");
			Entity entity = esq.GetEntity(_userConnection, _userConnection.CurrentUser.Id);
			return entity == null ? null : entity.GetTypedColumnValue<string>(emailColumn.Name);
		}

		// ICurrentUserAssertionIssuer is registered only in the ASP.NET Core service
		// collection. The core composition root bridges it into ClassFactory
		// (ClassFactory.Bind(CoreApiContainer.Resolve<ICurrentUserAssertionIssuer>) in
		// ClassFactoryInitializer.InitializeCommonBindings), which is what lets this
		// Configuration/package code resolve it through ClassFactory.
		private static string GetCurrentUserAssertionToken()
		{
			var assertionIssuer = ClassFactory.Get<ICurrentUserAssertionIssuer>();
			CurrentUserAssertionResult result = AsyncPump.Run(() => assertionIssuer.IssueAsync());
			if (result == null || !result.IsSuccess)
			{
				string message = string.Format(
					"IdentityAssertionHandler: current-user identity assertion could not be issued. " +
					"ErrorCode: {0}. ErrorMessage: {1}.",
					result?.ErrorCode,
					result?.ErrorMessage);
				Log.Error(message);
				throw new InvalidOperationException(message);
			}

			return result.Data.Assertion;
		}

		// Same dual-path as the core GlobalSearch OAuth handler: with the
		// IdentityProvidersManagement feature on, the client credentials come from
		// the Identity Management registry — the connection mapped to the
		// CreatioAIStudio external service (or the default connection when no
		// mapping exists). With the feature off, the legacy path resolves
		// IIdentityServiceWrapper the way CrtCoreBase's IdentityServiceWrapperHelper
		// does, using the IdentityServerUrl/IdentityServerClientId/
		// IdentityServerClientSecret system settings. Both providers cache their
		// tokens internally.
		private static IIdentityTokenProvider GetIdentityTokenProvider()
		{
			if (Features.GetIsEnabled(IdentityProvidersManagementFeature))
			{
				var accessor = ClassFactory.Get<IIdentityProviderAccessor>();
				return accessor.GetTokenProvider(new IdentityProviderResolutionRequest
				{
					ServiceCode = IdentityServiceCode
				});
			}
			return GlobalAppSettings.FeatureUseSeparateSettingsForOAuth20
				? ClassFactory.Get<IIdentityServiceWrapper>("ExternalAccess")
				: ClassFactory.Get<IIdentityServiceWrapper>();
		}

		private static string GetIdentityAccessToken()
		{
			IIdentityTokenProvider identityService = GetIdentityTokenProvider();
			if (!identityService.IsIdentityClientInitialized())
			{
				const string message =
					"IdentityAssertionHandler: the Identity client is not configured. " +
					"Check the Identity Management connection mapped to the CreatioAIStudio service " +
					"(IdentityProvidersManagement feature) or the IdentityServerUrl/IdentityServerClientId/" +
					"IdentityServerClientSecret system settings.";
				Log.Error(message);
				throw new InvalidOperationException(message);
			}

			string accessToken = identityService.GetAccessToken(IdentityApiScope);
			if (string.IsNullOrWhiteSpace(accessToken))
			{
				const string message =
					"IdentityAssertionHandler: could not obtain a client-credentials access token " +
					"for the Identity user-provisioning flow.";
				Log.Error(message);
				throw new InvalidOperationException(message);
			}

			return accessToken;
		}

		// The registry's own token machinery exposes no ServerUrl
		// (IIdentityProviderAccessor only hands out token providers), so the
		// /api/auth/* base URL is read from the same registry rows the core
		// resolver uses: the connection mapped to CreatioAIStudio, then the
		// DefaultIdentityProvider connection. An unresolved registry falls back to
		// the legacy IdentityServerUrl system setting.
		private string GetIdentityServerUrl()
		{
			if (Features.GetIsEnabled(IdentityProvidersManagementFeature))
			{
				string registryUrl = GetRegistryIdentityServerUrl();
				if (!string.IsNullOrWhiteSpace(registryUrl))
				{
					return registryUrl.TrimEnd('/');
				}
				Log.WarnFormat(
					"IdentityAssertionHandler: the Identity Management registry has no server URL for " +
					"the '{0}' service; falling back to the {1} system setting.",
					IdentityServiceCode,
					IdentityServerUrlSettingCode);
			}

			string identityServerUrl =
				CoreSysSettings.GetValue(_userConnection, IdentityServerUrlSettingCode, string.Empty);
			if (string.IsNullOrWhiteSpace(identityServerUrl))
			{
				const string message =
					"IdentityAssertionHandler: the IdentityServerUrl system setting is empty.";
				Log.Error(message);
				throw new InvalidOperationException(message);
			}

			return identityServerUrl.TrimEnd('/');
		}

		private string GetRegistryIdentityServerUrl()
		{
			Guid providerId = GetBoundIdentityProviderId();
			if (providerId == Guid.Empty)
			{
				providerId = CoreSysSettings.GetValue(
					_userConnection, DefaultIdentityProviderSettingCode, Guid.Empty);
			}
			return providerId == Guid.Empty ? null : GetIdentityProviderServerUrl(providerId);
		}

		private Guid GetBoundIdentityProviderId()
		{
			var select = new Select(_userConnection)
					.Column("mapping", "IdentityProviderId")
				.From("SysIdPToExtSvcMapping").As("mapping")
				.InnerJoin("SysExternalService").As("svc")
					.On("mapping", "ServiceId").IsEqual("svc", "Id")
				.Where("svc", "Code").IsEqual(Column.Parameter(IdentityServiceCode)) as Select;
			return select.ExecuteScalar<Guid>();
		}

		private string GetIdentityProviderServerUrl(Guid providerId)
		{
			var select = new Select(_userConnection)
					.Column("ServerUrl")
				.From("SysIdentityProvider")
				.Where("Id").IsEqual(Column.Parameter(providerId)) as Select;
			return select.ExecuteScalar<string>();
		}

		/// <summary>
		/// Calls the Identity external-provisioning endpoint for the user identified by
		/// <paramref name="externalToken"/>. Returns <c>true</c> when the user already
		/// has an active Identity membership, <c>false</c> when an invitation email was
		/// created/resent (or is still outstanding) and the user must confirm it first.
		/// Any other outcome is a configuration/trust error and throws.
		/// </summary>
		private static (bool IsActive, int? RetryAfterSeconds) ProvisionExternalUser(
			string identityServerUrl, string accessToken, string externalToken, bool forceResendInvite)
		{
			var result = PostExternalTokenApi(
				identityServerUrl + ExternalProvisioningResource, accessToken, externalToken, forceResendInvite);

			// 429 too_many_attempts: an invitation is outstanding and its email was
			// (re)sent recently — same user-facing outcome as invited/reinvited, so
			// it must not surface as an error. Identity reports the remaining wait
			// in the Retry-After header; pass it through so the client can render
			// the exact delay instead of guessing.
			if (result.StatusCode == 429)
			{
				return (false, result.RetryAfterSeconds);
			}

			ThrowIfNotOk(ExternalProvisioningResource, result.StatusCode, result.Body);

			switch (ReadStatus(result.Body))
			{
				case ProvisioningStatusActive:
					return (true, null);
				case ProvisioningStatusInvited:
				case ProvisioningStatusReinvited:
					return (false, null);
				default:
					throw UnrecognizedStatus(ExternalProvisioningResource, result.Body);
			}
		}

		/// <summary>
		/// Calls the Identity external-linking endpoint to bind the federated identity
		/// (sub, iss) carried by <paramref name="externalToken"/> to the already-active
		/// native Identity user. Both <c>linked</c> and <c>already_linked</c> are
		/// success (the endpoint is idempotent). Everything else — including
		/// <c>native_user_not_found</c> and <c>federated_identity_already_linked</c>
		/// (the identity is bound to a different native user) — throws, because no
		/// user-facing recovery exists for those states yet.
		/// </summary>
		private static void LinkExternalUser(string identityServerUrl, string accessToken, string externalToken)
		{
			var result = PostExternalTokenApi(
				identityServerUrl + ExternalLinkingResource, accessToken, externalToken,
				forceResendInvite: false);
			ThrowIfNotOk(ExternalLinkingResource, result.StatusCode, result.Body);

			switch (ReadStatus(result.Body))
			{
				case LinkingStatusLinked:
				case LinkingStatusAlreadyLinked:
					return;
				default:
					throw UnrecognizedStatus(ExternalLinkingResource, result.Body);
			}
		}

		// Internal seam for the unit tests: the wire shape of the Identity
		// request is the load-bearing Stage-1 invariant (forceResendInvite is
		// omitted while false, so implicit mounts keep the pre-flag body), and
		// the static HttpClient offers no interception point to observe it
		// through the public EnsureUser path.
		internal static string BuildExternalTokenRequestBody(string externalToken, bool forceResendInvite)
		{
			return JsonSerializer.Serialize(new ExternalTokenRequest
			{
				ExternalToken = externalToken,
				ForceResendInvite = forceResendInvite
			});
		}

		private static (int StatusCode, string Body, int? RetryAfterSeconds) PostExternalTokenApi(
			string url, string accessToken, string externalToken, bool forceResendInvite)
		{
			using (var request = new HttpRequestMessage(HttpMethod.Post, url))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				string requestBody = BuildExternalTokenRequestBody(externalToken, forceResendInvite);
				request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

				using (HttpResponseMessage response = AsyncPump.Run(() => IdentityHttpClient.SendAsync(request)))
				{
					string responseBody = AsyncPump.Run(() => response.Content.ReadAsStringAsync());
					return ((int)response.StatusCode, responseBody, ReadRetryAfterSeconds(response));
				}
			}
		}

		private static int? ReadRetryAfterSeconds(HttpResponseMessage response)
		{
			var retryAfter = response.Headers.RetryAfter;
			if (retryAfter == null)
			{
				return null;
			}
			double seconds;
			if (retryAfter.Delta.HasValue)
			{
				seconds = retryAfter.Delta.Value.TotalSeconds;
			}
			else if (retryAfter.Date.HasValue)
			{
				seconds = (retryAfter.Date.Value - DateTimeOffset.UtcNow).TotalSeconds;
			}
			else
			{
				return null;
			}
			return (int)Math.Ceiling(Math.Min(Math.Max(seconds, 0), MaxRetryAfterSeconds));
		}

		private static void ThrowIfNotOk(string resource, int statusCode, string body)
		{
			if (statusCode >= 200 && statusCode < 300)
			{
				return;
			}
			string message = string.Format(
				"IdentityAssertionHandler: {0} returned HTTP {1}. Response: {2}.",
				resource,
				statusCode,
				Truncate(body, 500));
			Log.Error(message);
			throw new InvalidOperationException(message);
		}

		private static string ReadStatus(string body)
		{
			try
			{
				ExternalTokenApiResponse response = JsonSerializer.Deserialize<ExternalTokenApiResponse>(body);
				return response?.Status;
			}
			catch (JsonException)
			{
				return null;
			}
		}

		private static InvalidOperationException UnrecognizedStatus(string resource, string body)
		{
			string message = string.Format(
				"IdentityAssertionHandler: {0} returned an unrecognized status: {1}.",
				resource,
				Truncate(body, 500));
			Log.Error(message);
			return new InvalidOperationException(message);
		}

		private static string Truncate(string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
			{
				return value;
			}
			return value.Substring(0, maxLength) + "...";
		}

		#endregion

		#region Classes: Private

		private class ExternalTokenRequest
		{
			[JsonPropertyName("externalToken")]
			public string ExternalToken { get; set; }

			// Stage-1 passthrough of the UI's explicit-resend flag (Identity
			// honors it in stage 2; until then it ignores the unknown field).
			// Omitted while false so implicit-mount requests keep today's exact
			// wire shape.
			[JsonPropertyName("forceResendInvite")]
			[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
			public bool ForceResendInvite { get; set; }
		}

		private class ExternalTokenApiResponse
		{
			[JsonPropertyName("status")]
			public string Status { get; set; }
		}

		#endregion

	}

	#endregion

}
