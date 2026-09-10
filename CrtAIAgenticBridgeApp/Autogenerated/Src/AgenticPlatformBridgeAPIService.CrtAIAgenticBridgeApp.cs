namespace CrtAIAgenticBridgeApp.EntryPoints.WebServices
{
	using System;
	using System.Net;
	using System.Runtime.Serialization;
	using System.Security;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Web.SessionState;
	using Common.Logging;
	using CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto;
	using CrtAIAgenticBridgeApp.Runtime;
	using CrtAIAgenticBridgeApp.TriggerDiscovery;
	using Terrasoft.Common;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.ServiceModelContract;
	using Terrasoft.Web.Common;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AgenticPlatformBridgeAPIService : BaseService, IReadOnlySessionState
	{
		private const string CanManageSolutionOperationCode = "CanManageSolution";
		private const int MaxIdentifierLength = 128;
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");
		private ITriggerMetadataProvider _triggerMetadataProvider;

		private ITriggerMetadataProvider TriggerMetadataProvider =>
			_triggerMetadataProvider ?? (_triggerMetadataProvider = ClassFactory.Get<ITriggerMetadataProvider>(
				new ConstructorArgument("userConnection", UserConnection)));

		[OperationContract]
		[WebInvoke(Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public TriggerPushResponse PushTriggers() {
			try {
				UserConnection.DBSecurityEngine.CheckCanExecuteOperation(CanManageSolutionOperationCode);
				TriggerPushSyncResponseDto syncResult = ClassFactory.Get<ITriggerCatalogSender>(
						new ConstructorArgument("userConnection", UserConnection))
					.SyncAll();
				if (syncResult == null) {
					return new TriggerPushResponse {
						Success = false,
						ErrorInfo = new ErrorInfo {
							Message = GetLocalizableString("SyncStatsMissingMessage")
						}
					};
				}
				return new TriggerPushResponse {
					Success = true,
					SyncedAt = syncResult.SyncedAt,
					TriggersUpserted = syncResult.TriggersUpserted,
					EventTypesUpserted = syncResult.EventTypesUpserted,
					TriggersRemoved = syncResult.TriggersRemoved,
					EventTypesRemoved = syncResult.EventTypesRemoved
				};
			} catch (SecurityException e) {
				Log.Error("Push triggers permission check failed.", e);
				return new TriggerPushResponse {
					Success = false,
					ErrorInfo = new ErrorInfo {
						Message = e.Message
					}
				};
			} catch (Exception ex) {
				Log.Error("Push triggers failed.", ex);
				return new TriggerPushResponse {
					Success = false,
					ErrorInfo = new ErrorInfo {
						Message = ex.Message
					}
				};
			}
		}

		// Read-only; intentionally ungated. This endpoint is called by the Freedom UI form page
		// for any authenticated user configuring event types. No sensitive mutation occurs.
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public TriggerCatalogPushRequest[] GetTriggers() {
			return TriggerMetadataProvider.GetAllTriggers();
		}

		// Read-only; intentionally ungated. Called by the Freedom UI form page to populate
		// the event-type selector for any authenticated user.
		[OperationContract]
		[WebInvoke(Method = "GET", UriTemplate = "GetEventTypes?triggerCode={triggerCode}",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public GetEventTypesResponse GetEventTypes(string triggerCode) {
			if (string.IsNullOrWhiteSpace(triggerCode) || triggerCode.Length > MaxIdentifierLength) {
				SetStatusCode(400);
				return null;
			}
			GetEventTypesResponse response = TriggerMetadataProvider.GetEventTypes(triggerCode);
			if (response == null) {
				SetStatusCode(404);
				return null;
			}
			return response;
		}

		// Read-only; intentionally ungated. Exposes column UId/caption for the Freedom UI
		// column picker. Column metadata is not sensitive and is already visible in Creatio
		// schema manager to all authenticated users.
		[OperationContract]
		[WebInvoke(Method = "GET", UriTemplate = "GetEntitySchemaColumns?schemaName={schemaName}",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public GetEntitySchemaColumnsResponse GetEntitySchemaColumns(string schemaName) {
			if (string.IsNullOrWhiteSpace(schemaName) || schemaName.Length > MaxIdentifierLength) {
				SetStatusCode(400);
				return null;
			}
			GetEntitySchemaColumnsResponse response = TriggerMetadataProvider.GetEntitySchemaColumns(schemaName);
			if (response == null) {
				SetStatusCode(404);
				return null;
			}
			return response;
		}

		internal virtual void SetStatusCode(int statusCode) {
#if NETSTANDARD2_0
			HttpContextAccessor.GetInstance().Response.StatusCode = statusCode;
#else
			WebOperationContext.Current.OutgoingResponse.StatusCode = (HttpStatusCode)statusCode;
#endif
		}

		internal virtual string GetLocalizableString(string localizableStringName) {
			string lsv = "LocalizableStrings." + localizableStringName + ".Value";
			var ls = new LocalizableString(UserConnection.Workspace.ResourceStorage,
				nameof(AgenticPlatformBridgeAPIService), lsv);
			return ls.ToString();
		}
	}

	[DataContract]
	public class TriggerPushResponse : BaseResponse
	{
		[DataMember(Name = "syncedAt", EmitDefaultValue = false)]
		public DateTime SyncedAt { get; set; }

		[DataMember(Name = "triggersUpserted", EmitDefaultValue = false)]
		public int TriggersUpserted { get; set; }

		[DataMember(Name = "eventTypesUpserted", EmitDefaultValue = false)]
		public int EventTypesUpserted { get; set; }

		[DataMember(Name = "triggersRemoved", EmitDefaultValue = false)]
		public int TriggersRemoved { get; set; }

		[DataMember(Name = "eventTypesRemoved", EmitDefaultValue = false)]
		public int EventTypesRemoved { get; set; }
	}
}

