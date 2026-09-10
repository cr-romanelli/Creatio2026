 namespace Terrasoft.Configuration.CrtIdentityManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Entities;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;

	#region Class: IdentityProviderManagementService

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[DefaultServiceRoute]
	public class IdentityProviderManagementService : BaseService
	{

		#region Constants: Private

		private const string OperationCode = "CanUseIdentityProvidersManagement";
		private const string ProviderSchemaName = "SysIdentityProvider";
		private const string ServiceSchemaName = "SysExternalService";
		private const string MappingSchemaName = "SysIdPToExtSvcMapping";
		private const string DefaultProviderSysSettingCode = "DefaultIdentityProvider";

		#endregion

		#region Methods: Private

		private static void ValidateCode(string serviceCode) {
			if (serviceCode.IsNullOrWhiteSpace()) {
				throw new ArgumentException("Service code is required.");
			}
		}

		private static void ValidateProviderFields(IdentityProviderSaveRequest request) {
			if (request == null) {
				throw new ArgumentException("Request body is required.");
			}
			if (request.Name.IsNullOrWhiteSpace()) {
				throw new ArgumentException("Provider name is required.");
			}
			if (request.ServerUrl.IsNullOrWhiteSpace()) {
				throw new ArgumentException("Server URL is required.");
			}
			if (!Uri.TryCreate(request.ServerUrl, UriKind.Absolute, out Uri serverUri) ||
					(serverUri.Scheme != Uri.UriSchemeHttp && serverUri.Scheme != Uri.UriSchemeHttps)) {
				throw new ArgumentException("Server URL must be an absolute http or https URL.");
			}
			if (request.ClientId.IsNullOrWhiteSpace()) {
				throw new ArgumentException("Client ID is required.");
			}
		}

		private Guid ParseId(string id, string fieldName) {
			if (id.IsNullOrWhiteSpace()) {
				return Guid.Empty;
			}
			if (!Guid.TryParse(id, out Guid parsedId)) {
				throw new ArgumentException($"{fieldName} must be a valid GUID.");
			}
			return parsedId;
		}

		private void CheckAccess() {
			UserConnection.DBSecurityEngine.CheckCanExecuteOperation(OperationCode);
		}

		private T Execute<T>(Func<T> action) where T : IdentityProviderManagementResponse, new() {
			try {
				CheckAccess();
				return action();
			} catch (Exception exception) {
				return new T {
					Success = false,
					ErrorInfo = new IdentityProviderManagementErrorInfo {
						ErrorCode = "IdentityProviderManagementError",
						Message = exception.Message
					}
				};
			}
		}

		private EntitySchema GetSchema(string schemaName) {
			return UserConnection.EntitySchemaManager.GetInstanceByName(schemaName);
		}

		private Entity CreateEntity(string schemaName) {
			return GetSchema(schemaName).CreateEntity(UserConnection);
		}

		private string GetLookupValueColumnName(EntitySchema schema, string columnName) {
			return schema.Columns.GetByName(columnName).ColumnValueName;
		}

		private Entity FetchEntity(string schemaName, Guid id) {
			if (id == Guid.Empty) {
				throw new ArgumentException("Record ID is required.");
			}
			Entity entity = CreateEntity(schemaName);
			if (!entity.FetchFromDB(id)) {
				throw new InvalidOperationException($"Record '{id}' was not found in '{schemaName}'.");
			}
			return entity;
		}

		private Guid FindSingleIdByColumn(string schemaName, string columnName, string value) {
			if (value.IsNullOrWhiteSpace()) {
				return Guid.Empty;
			}
			EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, schemaName);
			query.AddAllSchemaColumns();
			query.Filters.Add(query.CreateFilterWithParameters(FilterComparisonType.Equal, columnName, value));
			EntityCollection entities = query.GetEntityCollection(UserConnection);
			if (entities.Count > 1) {
				throw new InvalidOperationException(
					$"More than one '{schemaName}' record was found by '{columnName}' = '{value}'. Use ID instead.");
			}
			return entities.Count == 0 ? Guid.Empty : entities[0].GetTypedColumnValue<Guid>("Id");
		}

		private Guid ResolveProviderId(string id, string name) {
			Guid providerId = ParseId(id, "Provider ID");
			if (providerId != Guid.Empty) {
				FetchEntity(ProviderSchemaName, providerId);
				return providerId;
			}
			if (name.IsNullOrWhiteSpace()) {
				throw new ArgumentException("Provider ID or provider name is required.");
			}
			providerId = FindSingleIdByColumn(ProviderSchemaName, "Name", name);
			if (providerId == Guid.Empty) {
				throw new InvalidOperationException($"Identity provider '{name}' was not found.");
			}
			return providerId;
		}

		private Guid GetDefaultProviderId() {
			object value;
			try {
				value = SysSettings.GetDefValue(UserConnection, DefaultProviderSysSettingCode);
			} catch {
				return Guid.Empty;
			}
			if (value == null) {
				return Guid.Empty;
			}
			if (value is Guid guidValue) {
				return guidValue;
			}
			return Guid.TryParse(value.ToString(), out Guid parsedValue) ? parsedValue : Guid.Empty;
		}

		private IdentityProviderDto BuildProviderDto(Entity provider, Guid defaultProviderId) {
			Guid providerId = provider.GetTypedColumnValue<Guid>("Id");
			return new IdentityProviderDto {
				Id = providerId.ToString(),
				Name = provider.GetTypedColumnValue<string>("Name"),
				Description = provider.GetTypedColumnValue<string>("Description"),
				ServerUrl = provider.GetTypedColumnValue<string>("ServerUrl"),
				ClientId = provider.GetTypedColumnValue<string>("ClientId"),
				IsDefault = providerId == defaultProviderId
			};
		}

		private IdentityProviderDto GetProviderDto(Guid providerId) {
			EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, ProviderSchemaName);
			query.AddAllSchemaColumns();
			query.Filters.Add(query.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", providerId));
			EntityCollection providers = query.GetEntityCollection(UserConnection);
			if (providers.Count == 0) {
				throw new InvalidOperationException($"Identity provider '{providerId}' was not found.");
			}
			return BuildProviderDto(providers[0], GetDefaultProviderId());
		}

		private bool HasBindings(Guid providerId) {
			EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, MappingSchemaName);
			query.AddColumn("Id");
			query.Filters.Add(query.CreateFilterWithParameters(FilterComparisonType.Equal, "IdentityProvider", providerId));
			return query.GetEntityCollection(UserConnection).Count > 0;
		}

		private Guid FindServiceIdByCode(string serviceCode) {
			ValidateCode(serviceCode);
			return FindSingleIdByColumn(ServiceSchemaName, "Code", serviceCode);
		}

		private Guid GetOrCreateServiceId(string serviceCode, bool createIfMissing) {
			Guid serviceId = FindServiceIdByCode(serviceCode);
			if (serviceId != Guid.Empty) {
				return serviceId;
			}
			if (!createIfMissing) {
				throw new InvalidOperationException($"External service '{serviceCode}' was not found.");
			}
			Entity service = CreateEntity(ServiceSchemaName);
			service.SetDefColumnValues();
			service.SetColumnValue("Name", serviceCode);
			service.SetColumnValue("Code", serviceCode);
			service.Save();
			return service.GetTypedColumnValue<Guid>("Id");
		}

		private EntityCollection GetMappingsByService(Guid serviceId) {
			EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, MappingSchemaName);
			query.AddAllSchemaColumns();
			//query.AddColumn("IdentityProvider.Id").Name = "IdentityProviderId";
			//query.AddColumn("IdentityProvider.Name").Name = "IdentityProviderName";
			query.Filters.Add(query.CreateFilterWithParameters(FilterComparisonType.Equal, "Service", serviceId));
			return query.GetEntityCollection(UserConnection);
		}

		private ExternalServiceDto BuildServiceDto(Entity service) {
			Guid serviceId = service.GetTypedColumnValue<Guid>("Id");
			EntityCollection mappings = GetMappingsByService(serviceId);
			Entity mapping = mappings.Count == 0 ? null : mappings[0];
			return new ExternalServiceDto {
				Id = serviceId.ToString(),
				Name = service.GetTypedColumnValue<string>("Name"),
				Code = service.GetTypedColumnValue<string>("Code"),
				BoundProviderId = mapping == null
					? null
					: mapping.GetTypedColumnValue<Guid>("IdentityProviderId").ToString(),
				BoundProviderName = mapping == null
					? null
					: mapping.GetTypedColumnValue<string>("IdentityProviderName")
			};
		}

		private BindingDto BuildBindingDto(Guid serviceId, string serviceCode, Guid providerId) {
			return new BindingDto {
				ServiceId = serviceId.ToString(),
				ServiceCode = serviceCode,
				ProviderId = providerId.ToString()
			};
		}

		#endregion

		#region Methods: Public

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "GetProviders", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public ProvidersResponse GetProviders() {
			return Execute(() => {
				Guid defaultProviderId = GetDefaultProviderId();
				EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, ProviderSchemaName);
				query.AddAllSchemaColumns();
				EntityCollection providers = query.GetEntityCollection(UserConnection);
				return new ProvidersResponse {
					Providers = providers.Select(provider => BuildProviderDto(provider, defaultProviderId)).ToList()
				};
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "SaveProvider", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public ProviderResponse SaveProvider(IdentityProviderSaveRequest request) {
			return Execute(() => {
				ValidateProviderFields(request);
				Guid providerId = ParseId(request.Id, "Provider ID");
				if (providerId == Guid.Empty) {
					providerId = FindSingleIdByColumn(ProviderSchemaName, "Name", request.Name);
				}
				Entity provider = providerId == Guid.Empty
					? CreateEntity(ProviderSchemaName)
					: FetchEntity(ProviderSchemaName, providerId);
				if (providerId == Guid.Empty) {
					provider.SetDefColumnValues();
				}
				provider.SetColumnValue("Name", request.Name);
				provider.SetColumnValue("Description", request.Description);
				provider.SetColumnValue("ServerUrl", request.ServerUrl);
				provider.SetColumnValue("ClientId", request.ClientId);
				if (!request.ClientSecret.IsNullOrWhiteSpace()) {
					provider.SetColumnValue("ClientSecret", request.ClientSecret);
				}
				provider.Save();
				return new ProviderResponse {
					Provider = GetProviderDto(provider.GetTypedColumnValue<Guid>("Id"))
				};
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "SetProviderCredentials", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public IdentityProviderManagementResponse SetProviderCredentials(ProviderCredentialsRequest request) {
			return Execute(() => {
				if (request == null || request.ClientSecret.IsNullOrWhiteSpace()) {
					throw new ArgumentException("Client secret is required.");
				}
				Guid providerId = ResolveProviderId(request.Id, request.Name);
				Entity provider = FetchEntity(ProviderSchemaName, providerId);
				provider.SetColumnValue("ClientSecret", request.ClientSecret);
				provider.Save();
				return new IdentityProviderManagementResponse();
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "DeleteProvider", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public IdentityProviderManagementResponse DeleteProvider(ProviderSelectorRequest request) {
			return Execute(() => {
				if (request == null) {
					throw new ArgumentException("Request body is required.");
				}
				Guid providerId = ResolveProviderId(request.Id, request.Name);
				if (providerId == GetDefaultProviderId()) {
					throw new InvalidOperationException("Default identity provider cannot be deleted.");
				}
				if (HasBindings(providerId)) {
					throw new InvalidOperationException("Identity provider has service bindings and cannot be deleted.");
				}
				FetchEntity(ProviderSchemaName, providerId).Delete();
				return new IdentityProviderManagementResponse();
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "SetDefaultProvider", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public IdentityProviderManagementResponse SetDefaultProvider(ProviderSelectorRequest request) {
			return Execute(() => {
				if (request == null) {
					throw new ArgumentException("Request body is required.");
				}
				Guid providerId = ResolveProviderId(request.Id, request.Name);
				SysSettings.SetDefValue(UserConnection, DefaultProviderSysSettingCode, providerId);
				return new IdentityProviderManagementResponse();
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "GetServices", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public ServicesResponse GetServices() {
			return Execute(() => {
				EntitySchemaQuery query = new EntitySchemaQuery(UserConnection.EntitySchemaManager, ServiceSchemaName);
				query.AddAllSchemaColumns();
				EntityCollection services = query.GetEntityCollection(UserConnection);
				return new ServicesResponse {
					Services = services.Select(BuildServiceDto).ToList()
				};
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "BindProviderToService", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public BindingResponse BindProviderToService(BindProviderRequest request) {
			return Execute(() => {
				if (request == null) {
					throw new ArgumentException("Request body is required.");
				}
				ValidateCode(request.ServiceCode);
				Guid providerId = ResolveProviderId(request.ProviderId, request.ProviderName);
				Guid serviceId = GetOrCreateServiceId(request.ServiceCode, request.CreateServiceIfMissing);
				EntityCollection mappings = GetMappingsByService(serviceId);
				EntitySchema mappingSchema = GetSchema(MappingSchemaName);
				Entity mapping;
				if (mappings.Count == 0) {
					mapping = mappingSchema.CreateEntity(UserConnection);
					mapping.SetDefColumnValues();
					mapping.SetColumnValue(GetLookupValueColumnName(mappingSchema, "Service"), serviceId);
				} else {
					mapping = FetchEntity(MappingSchemaName, mappings[0].GetTypedColumnValue<Guid>("Id"));
					for (int i = 1; i < mappings.Count; i++) {
						FetchEntity(MappingSchemaName, mappings[i].GetTypedColumnValue<Guid>("Id")).Delete();
					}
				}
				mapping.SetColumnValue(GetLookupValueColumnName(mappingSchema, "IdentityProvider"), providerId);
				mapping.Save();
				return new BindingResponse {
					Binding = BuildBindingDto(serviceId, request.ServiceCode, providerId)
				};
			});
		}

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "UnbindProviderFromService", BodyStyle = WebMessageBodyStyle.Bare,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public IdentityProviderManagementResponse UnbindProviderFromService(ServiceCodeRequest request) {
			return Execute(() => {
				if (request == null) {
					throw new ArgumentException("Request body is required.");
				}
				Guid serviceId = FindServiceIdByCode(request.ServiceCode);
				if (serviceId == Guid.Empty) {
					return new IdentityProviderManagementResponse();
				}
				EntityCollection mappings = GetMappingsByService(serviceId);
				foreach (Entity mapping in mappings) {
					FetchEntity(MappingSchemaName, mapping.GetTypedColumnValue<Guid>("Id")).Delete();
				}
				return new IdentityProviderManagementResponse();
			});
		}

		#endregion

	}

	#endregion

	#region DTO

	[DataContract]
	public class IdentityProviderManagementResponse
	{

		[DataMember(Name = "success")]
		public bool Success { get; set; } = true;

		[DataMember(Name = "errorInfo", EmitDefaultValue = false)]
		public IdentityProviderManagementErrorInfo ErrorInfo { get; set; }

	}

	[DataContract]
	public class IdentityProviderManagementErrorInfo
	{

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "errorCode")]
		public string ErrorCode { get; set; }

	}

	[DataContract]
	public class ProvidersResponse : IdentityProviderManagementResponse
	{

		[DataMember(Name = "providers")]
		public List<IdentityProviderDto> Providers { get; set; } = new List<IdentityProviderDto>();

	}

	[DataContract]
	public class ProviderResponse : IdentityProviderManagementResponse
	{

		[DataMember(Name = "provider", EmitDefaultValue = false)]
		public IdentityProviderDto Provider { get; set; }

	}

	[DataContract]
	public class ServicesResponse : IdentityProviderManagementResponse
	{

		[DataMember(Name = "services")]
		public List<ExternalServiceDto> Services { get; set; } = new List<ExternalServiceDto>();

	}

	[DataContract]
	public class BindingResponse : IdentityProviderManagementResponse
	{

		[DataMember(Name = "binding", EmitDefaultValue = false)]
		public BindingDto Binding { get; set; }

	}

	[DataContract]
	public class IdentityProviderDto
	{

		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "serverUrl")]
		public string ServerUrl { get; set; }

		[DataMember(Name = "clientId")]
		public string ClientId { get; set; }

		[DataMember(Name = "isDefault")]
		public bool IsDefault { get; set; }

	}

	[DataContract]
	public class ExternalServiceDto
	{

		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "boundProviderId", EmitDefaultValue = false)]
		public string BoundProviderId { get; set; }

		[DataMember(Name = "boundProviderName", EmitDefaultValue = false)]
		public string BoundProviderName { get; set; }

	}

	[DataContract]
	public class BindingDto
	{

		[DataMember(Name = "serviceId")]
		public string ServiceId { get; set; }

		[DataMember(Name = "serviceCode")]
		public string ServiceCode { get; set; }

		[DataMember(Name = "providerId")]
		public string ProviderId { get; set; }

	}

	[DataContract]
	public class IdentityProviderSaveRequest
	{

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "serverUrl")]
		public string ServerUrl { get; set; }

		[DataMember(Name = "clientId")]
		public string ClientId { get; set; }

		[DataMember(Name = "clientSecret", EmitDefaultValue = false)]
		public string ClientSecret { get; set; }

	}

	[DataContract]
	public class ProviderSelectorRequest
	{

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		[DataMember(Name = "name", EmitDefaultValue = false)]
		public string Name { get; set; }

	}

	[DataContract]
	public class ProviderCredentialsRequest : ProviderSelectorRequest
	{

		[DataMember(Name = "clientSecret")]
		public string ClientSecret { get; set; }

	}

	[DataContract]
	public class BindProviderRequest
	{

		[DataMember(Name = "providerId", EmitDefaultValue = false)]
		public string ProviderId { get; set; }

		[DataMember(Name = "providerName", EmitDefaultValue = false)]
		public string ProviderName { get; set; }

		[DataMember(Name = "serviceCode")]
		public string ServiceCode { get; set; }

		[DataMember(Name = "createServiceIfMissing")]
		public bool CreateServiceIfMissing { get; set; }

	}

	[DataContract]
	public class ServiceCodeRequest
	{

		[DataMember(Name = "serviceCode")]
		public string ServiceCode { get; set; }

	}

	#endregion

}

