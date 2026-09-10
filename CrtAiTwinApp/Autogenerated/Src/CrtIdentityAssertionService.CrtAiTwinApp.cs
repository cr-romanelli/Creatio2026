namespace Terrasoft.Configuration
{
	using System.Net;
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Web.SessionState;
	using Creatio.FeatureToggling;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	/// <summary>
	/// REST service responsible for identity assertion checks for the AI chat.
	///
	/// Route: <c>POST /0/rest/CrtIdentityAssertionService/EnsureUser</c>.
	/// Business logic lives in <c>IdentityAssertionHandler</c> (Files/src/cs,
	/// not visible to this schema on platform integration builds); this class
	/// only maps its result onto the HTTP contract.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CrtIdentityAssertionService : BaseService, IReadOnlySessionState
	{
		private const string StatusCodeOne = "success";
		private const string StatusMissingEmail = "missing_email";
		private const string StatusConfirmationEmailSent = "confirmation_email_sent";

		[OperationContract]
		[WebInvoke(
			Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public EnsureUserResponse EnsureUser(EnsureUserRequest request)
		{
			if (!Features.GetIsEnabled<CrtIdentityAssertionEnsureUserFlow>())
			{
				return new EnsureUserResponse
				{
					Status = StatusCodeOne
				};
			}

			var handler = ClassFactory.Get<IIdentityAssertionHandler>();
			EnsureUserResult result = handler.EnsureUser(ResolveForceResendInvite(request));

			// APD-4399: when the API-result-cache feature is on, hint the shell
			// that this outcome may be cached client-side (success — for the tab
			// lifetime, confirmation_email_sent — briefly). missing_email is
			// never hinted: the user can fix the profile at any moment.
			bool cacheable = Features.GetIsEnabled<CrtAiTwinApiResultCache>();

			switch (result.Status)
			{
				case EnsureUserStatus.MissingEmail:
					throw new WebFaultException<EnsureUserResponse>(
						new EnsureUserResponse
						{
							Status = StatusMissingEmail
						},
						HttpStatusCode.BadRequest);
				case EnsureUserStatus.ConfirmationEmailSent:
					return new EnsureUserResponse
					{
						Status = StatusConfirmationEmailSent,
						Cacheable = cacheable,
						RetryAfterSeconds = result.RetryAfterSeconds
					};
				default:
					return new EnsureUserResponse
					{
						Status = StatusCodeOne,
						Cacheable = cacheable
					};
			}
		}

		// Internal seam for the unit tests: a null request (an empty POST body,
		// which is what every pre-flag shell sends) and an unset field must both
		// resolve to false — flipping this guard would turn every implicit panel
		// mount into an invitation resend once Identity honors the flag.
		internal static bool ResolveForceResendInvite(EnsureUserRequest request)
		{
			return request != null && request.ForceResendInvite;
		}
	}

	[DataContract]
	public class EnsureUserRequest
	{
		/// <summary>
		/// Optional. When <c>true</c> (an explicit user action, e.g. the Resend
		/// button), asks Identity to re-send the pending invitation email.
		/// Absent/<c>false</c> on implicit calls (panel mounts), so background
		/// re-checks stop producing invitation emails once Identity honors the
		/// flag (stage 2). Passed through to
		/// <c>POST /api/auth/external-provisioning</c> as
		/// <c>forceResendInvite</c>; omitted from that request while false.
		/// </summary>
		[DataMember(Name = "forceResendInvite", IsRequired = false, EmitDefaultValue = false)]
		public bool ForceResendInvite { get; set; }
	}

	[DataContract]
	public class EnsureUserResponse
	{
		[DataMember(Name = "status")]
		public string Status { get; set; }

		/// <summary>
		/// APD-4399: when <c>true</c>, the client may cache this outcome
		/// (see the CrtAiTwinApiResultCache feature). Omitted from the JSON
		/// when <c>false</c>, so the pre-existing wire contract is unchanged
		/// while the feature is off.
		/// </summary>
		[DataMember(Name = "cacheable", EmitDefaultValue = false)]
		public bool Cacheable { get; set; }

		/// <summary>
		/// Seconds until Identity accepts the next invitation-email resend —
		/// a pass-through of the <c>Retry-After</c> header of Identity's 429
		/// <c>too_many_attempts</c> response, so the UI can render the exact
		/// delay. Omitted when Identity did not report a delay.
		/// </summary>
		[DataMember(Name = "retryAfterSeconds", EmitDefaultValue = false)]
		public int? RetryAfterSeconds { get; set; }
	}

	// The contract types below live in this schema (not Files/src/cs) because
	// schema sources are also compiled into Terrasoft.Configuration on platform
	// integration builds, where the package Files assembly and its CrtAiTwinApp
	// namespace do not exist — schemas may only reference types that compile
	// there. The IdentityAssertionHandler implementation stays in Files/src/cs
	// (it needs ATF.Repository / Terrasoft.OAuthIntegration) and picks these
	// types up via `using Terrasoft.Configuration;`.

	#region Interface: IIdentityAssertionHandler

	internal interface IIdentityAssertionHandler
	{
		EnsureUserResult EnsureUser(bool forceResendInvite);
	}

	#endregion

	#region Class: EnsureUserResult

	internal class EnsureUserResult
	{
		public EnsureUserStatus Status { get; set; }
		public string Token { get; set; }

		/// <summary>
		/// Seconds until Identity accepts the next invitation-email resend —
		/// a pass-through of the <c>Retry-After</c> header of Identity's
		/// 429 <c>too_many_attempts</c> response. <c>null</c> when Identity
		/// did not report a delay.
		/// </summary>
		public int? RetryAfterSeconds { get; set; }
	}

	#endregion

	#region Enum: EnsureUserStatus

	internal enum EnsureUserStatus
	{
		Success,
		MissingEmail,
		ConfirmationEmailSent
	}

	#endregion

	/// <summary>
	/// Gates whether <c>CrtIdentityAssertionService.EnsureUser</c> runs its full
	/// identity-assertion token flow.
	/// </summary>
	public class CrtIdentityAssertionEnsureUserFlow : FeatureMetadata
	{
		// IsEnabled is intentionally left unset here: the default (disabled) state
		// is carried by the AdminUnitFeatureState_CrtIdentityAssertionEnsureUserFlow
		// data record for "All employees", so it stays admin-toggleable without a
		// code deploy.
		public CrtIdentityAssertionEnsureUserFlow()
		{
			Description = "Enables the full identity-assertion token flow in CrtIdentityAssertionService.EnsureUser.";
		}
	}
}

