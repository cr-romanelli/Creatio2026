namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Threading;
	using System.Web.SessionState;
	using Terrasoft.Configuration;
	using Terrasoft.Configuration.Workplace;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.ServiceModelContract;
	using Terrasoft.Web.Common;
	using Terrasoft.Web.Common.ServiceRouting;
	using WorkplaceApi;

	#region Class: KnwSourceIndexingRequest

	/// <summary>
	/// Request object for starting the indexing of a knowledge source.
	/// </summary>
	[DataContract]
	public class KnwSourceIndexingRequest
	{

		#region Properties: Public

		/// <summary>
		/// The unique identifier of the knowledge source to be indexed.
		/// </summary>
		[DataMember(Name = "knwSourceId")]
		public Guid KnwSourceId { get; set; }

		#endregion

	}

	#endregion

	#region Class: DeleteFilesRequest

	/// <summary>
	/// Request body for deleting files (knowledge source ID comes from URL path).
	/// </summary>
	[DataContract]
	public class DeleteFilesRequest
	{

		#region Properties: Public

		/// <summary>
		/// The list of file identifiers to be deleted (as strings from JSON).
		/// </summary>
		[DataMember(Name = "fileIds")]
		public List<string> FileIds { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionDataResponse

	/// <summary>
	/// Response object containing data about a section.
	/// </summary>
	[DataContract]
	public class SectionDataResponse
	{

		#region Properties: Public

		[DataMember(Name = "sections")]
		public IEnumerable<LookupValue> Sections { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionColumnsResponse

	[DataContract]
	public class SectionColumnsResponse
	{

		#region Properties: Public

		[DataMember(Name = "columns")]
		public IEnumerable<SectionColumnDataItem> Columns { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionDynamicFoldersResponse

	[DataContract]
	public class SectionDynamicFoldersResponse
	{

		#region Properties: Public

		[DataMember(Name = "folders")]
		public IEnumerable<LookupValue> Folders { get; set; }

		#endregion

	}

	#endregion

	#region Class: EndUserContextDto

	/// <summary>
	/// Request end-user context passed by external callers.
	/// </summary>
	[DataContract]
	public class EndUserContextDto
	{

		#region Properties: Public

		[DataMember(Name = "userId")]
		public string UserId { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		#endregion

	}

	#endregion

	#region Class: SearchKnowledgeRequest

	/// <summary>
	/// Request body for knowledge search.
	/// </summary>
	[DataContract]
	public class SearchKnowledgeRequest
	{

		#region Properties: Public

		[DataMember(Name = "knwSourceIds")]
		public Guid[] KnwSourceIds { get; set; }

		[DataMember(Name = "query")]
		public string Query { get; set; }

		[DataMember(Name = "keywords")]
		public string[] Keywords { get; set; }

		[DataMember(Name = "sessionId")]
		public Guid? SessionId { get; set; }

		[DataMember(Name = "take")]
		public int Take { get; set; } = 10;

		[DataMember(Name = "skip")]
		public int Skip { get; set; }

		[DataMember(Name = "endUserContext")]
		public EndUserContextDto EndUserContext { get; set; }

		#endregion

	}

	#endregion

	#region Class: SearchCitationDto

	/// <summary>
	/// Search result citation.
	/// </summary>
	[DataContract]
	public class SearchCitationDto
	{

		#region Properties: Public

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "location")]
		public string Location { get; set; }

		#endregion

	}

	#endregion

	#region Class: SearchKnowledgeResultItem

	/// <summary>
	/// Successful search result item.
	/// </summary>
	[DataContract]
	public class SearchKnowledgeResultItem
	{

		#region Properties: Public

		[DataMember(Name = "knwSourceId")]
		public Guid KnwSourceId { get; set; }

		[DataMember(Name = "itemId")]
		public Guid? ItemId { get; set; }

		[DataMember(Name = "text")]
		public string Text { get; set; }

		[DataMember(Name = "score")]
		public double Score { get; set; }

		[DataMember(Name = "citation")]
		public SearchCitationDto Citation { get; set; }

		#endregion

	}

	#endregion

	#region Class: SearchKnowledgeErrorItem

	/// <summary>
	/// Source-level search error.
	/// </summary>
	[DataContract]
	public class SearchKnowledgeErrorItem
	{

		#region Properties: Public

		[DataMember(Name = "knwSourceId")]
		public Guid KnwSourceId { get; set; }

		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		#endregion

	}

	#endregion

	#region Class: SearchKnowledgeResponse

	/// <summary>
	/// Search response envelope.
	/// </summary>
	[DataContract]
	public class SearchKnowledgeResponse
	{

		#region Properties: Public

		[DataMember(Name = "sessionId")]
		public Guid SessionId { get; set; }

		[DataMember(Name = "results")]
		public IEnumerable<SearchKnowledgeResultItem> Results { get; set; } =
			Enumerable.Empty<SearchKnowledgeResultItem>();

		[DataMember(Name = "errors")]
		public IEnumerable<SearchKnowledgeErrorItem> Errors { get; set; } =
			Enumerable.Empty<SearchKnowledgeErrorItem>();

		#endregion

	}

	#endregion

	#region Class: KnowledgeSourceItem

	/// <summary>
	/// Available knowledge source item.
	/// </summary>
	[DataContract]
	public class KnowledgeSourceItem
	{

		#region Properties: Public

		[DataMember(Name = "id")]
		public Guid Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		#endregion

	}

	#endregion

	#region Class: GetAvailableKnowledgeSourcesResponse

	/// <summary>
	/// Response with search-ready knowledge sources.
	/// </summary>
	[DataContract]
	public class GetAvailableKnowledgeSourcesResponse
	{

		#region Properties: Public

		[DataMember(Name = "sources")]
		public IEnumerable<KnowledgeSourceItem> Sources { get; set; } = Enumerable.Empty<KnowledgeSourceItem>();

		#endregion

	}

	#endregion

	#region Class: KnwServiceHealthResponse

	/// <summary>
	/// Response object for the knowledge management service health check.
	/// </summary>
	[DataContract]
	public class KnwServiceHealthResponse
	{

		#region Properties: Public

		/// <summary>
		/// Health status reported by the knowledge management service.
		/// </summary>
		[DataMember(Name = "status")]
		public string Status { get; set; }

		#endregion

	}

	#endregion

	#region Class: KnwLibApiService

	/// <summary>
	/// Service for managing knowledge library operations, including starting the indexing of knowledge sources.
	/// </summary>
	[ServiceContract]
	[DefaultServiceRoute]
	[SspServiceRoute]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class KnwLibApiService : BaseService, IReadOnlySessionState
	{

		#region Class: DynamicFolderLookupQuery

		internal sealed class DynamicFolderLookupQuery
		{
			public EntitySchemaQuery Esq { get; set; }

			public string DisplayColumnPath { get; set; }

			public string FilterDataColumnPath { get; set; }
		}

		#endregion

		#region Constants: Private

		private const string FolderSchemaSuffix = "Folder";
		private const string DynamicFolderTypeIdString = "65CA0946-0084-4874-B117-C13199AF3B95";

		#endregion

		#region Constants: Internal

		internal const string FolderTreeSchemaName = "FolderTree";
		internal const string EntitySchemaNameColumnPath = "EntitySchemaName";
		internal const string FilterDataColumnPath = "FilterData";
		internal const string FolderTypeIdColumnPath = "FolderTypeId";
		internal const string IsDeletedColumnPath = "IsDeleted";
		internal const string NameColumnPath = "Name";
		internal const string SearchDataColumnPath = "SearchData";

		#endregion

		#region Fields: Private

		private static readonly Guid DynamicFolderTypeId = new Guid(DynamicFolderTypeIdString);

		#endregion

		#region Properties: Private

		private IKnwIndexingRunner _indexingRunner;
		private IKnwIndexingRunner IndexingRunner =>
			_indexingRunner ?? (_indexingRunner = ClassFactory.Get<IKnwIndexingRunner>());

		private IWorkplaceManager _workplaceManager;
		private IWorkplaceManager WorkplaceManager => _workplaceManager
			?? (_workplaceManager = ClassFactory.Get<IWorkplaceManager>(new ConstructorArgument("uc", UserConnection)));

		private IKnwServiceClient _knwServiceClient;
		private IKnwServiceClient KnwServiceClient =>
			_knwServiceClient ?? (_knwServiceClient = ClassFactory.Get<IKnwServiceClient>(
				new ConstructorArgument("userConnection", UserConnection)));

		private IKnwSearchClient_Temp _knwSearchClient;
		private IKnwSearchClient_Temp KnwSearchClient =>
			_knwSearchClient ?? (_knwSearchClient = ClassFactory.Get<IKnwSearchClient_Temp>(
				new ConstructorArgument("userConnection", UserConnection)));

		private IKnwSourceManager _knwSourceManager;
		private IKnwSourceManager KnwSourceManager =>
			_knwSourceManager ?? (_knwSourceManager = ClassFactory.Get<IKnwSourceManager>(
				new ConstructorArgument("userConnection", UserConnection)));

		private IKnwRelatedColumnsSelectorService _knwRelatedColumnsSelectorService;
		private IKnwRelatedColumnsSelectorService KnwRelatedColumnsSelectorService =>
			_knwRelatedColumnsSelectorService ?? (_knwRelatedColumnsSelectorService =
				ClassFactory.Get<IKnwRelatedColumnsSelectorService>(
					new ConstructorArgument("userConnection", UserConnection)));

		private IKnwRelatedColumnsResolverService _knwRelatedColumnsResolverService;
		private IKnwRelatedColumnsResolverService KnwRelatedColumnsResolverService =>
			_knwRelatedColumnsResolverService ?? (_knwRelatedColumnsResolverService =
				ClassFactory.Get<IKnwRelatedColumnsResolverService>(
					new ConstructorArgument("userConnection", UserConnection)));

		#endregion

		#region Methods: Private

		private static Guid ParseKnwSourceIdOrThrow(string knwSourceId) {
			if (!Guid.TryParse(knwSourceId, out Guid sourceId)) {
				throw new WebFaultException<string>(
					"Invalid knowledge source ID format.",
					HttpStatusCode.BadRequest);
			}
			return sourceId;
		}

		private static bool ExceptionContains(Exception exception, string marker) {
			for (Exception current = exception; current != null; current = current.InnerException) {
				if (!string.IsNullOrWhiteSpace(current.Message) &&
						current.Message.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0) {
					return true;
				}
			}
			return false;
		}

		private static bool IsConflictException(Exception exception) {
			return ExceptionContains(exception, "409") || ExceptionContains(exception, "Conflict");
		}

		private static bool IsNotFoundException(Exception exception) {
			return ExceptionContains(exception, "404") || ExceptionContains(exception, "Not Found");
		}

		private List<ConfigurationScopeResponse> LoadConfigurations(Guid sourceId) {
			return KnwServiceClient.ListConfigurationsAsync(sourceId, CancellationToken.None)
				.GetAwaiter().GetResult()
				?.ToList() ?? new List<ConfigurationScopeResponse>();
		}

		private ConfigurationScopeResponse UpdateConfiguration(Guid configId, ConfigurationScopeRequest request) {
			return KnwServiceClient.UpdateConfigurationAsync(configId, request, CancellationToken.None)
				.GetAwaiter().GetResult();
		}

		private ConfigurationScopeResponse CreateConfiguration(ConfigurationScopeRequest request) {
			return KnwServiceClient.CreateConfigurationAsync(request, CancellationToken.None)
				.GetAwaiter().GetResult();
		}

		private ConfigurationScopeResponse CreateWithConflictRetry(Guid sourceId,
				ConfigurationScopeRequest configRequest) {
			try {
				return CreateConfiguration(configRequest);
			} catch (Exception createException) {
				KnwLibUtils.Logger.Error($"Failed to create configuration for source {sourceId}.", createException);
				if (!IsConflictException(createException)) {
					throw new WebFaultException<string>(
						"Failed to create configuration for this knowledge source.",
						HttpStatusCode.BadRequest);
				}

				List<ConfigurationScopeResponse> retryConfigs = LoadConfigurations(sourceId);
				if (retryConfigs.Count != 1) {
					throw new WebFaultException<string>(
						"Configuration conflict detected while saving settings. Please retry.",
						HttpStatusCode.Conflict);
				}

				try {
					return UpdateConfiguration(retryConfigs[0].Id, configRequest);
				} catch (Exception retryUpdateException) {
					KnwLibUtils.Logger.Error(
						$"Failed to update configuration {retryConfigs[0].Id} after create conflict for source {sourceId}.",
						retryUpdateException);
					throw new WebFaultException<string>(
						"Configuration conflict detected while saving settings. Please retry.",
						HttpStatusCode.Conflict);
				}
			}
		}

		private static void ValidateSearchRequestOrThrow(SearchKnowledgeRequest request) {
			if (request == null) {
				throw new WebFaultException<string>("Search request body is required.", HttpStatusCode.BadRequest);
			}
			if (request.KnwSourceIds == null || request.KnwSourceIds.Length == 0) {
				throw new WebFaultException<string>(
					"At least one knowledge source ID is required.",
					HttpStatusCode.BadRequest);
			}
			if (request.KnwSourceIds.Any(id => id == Guid.Empty)) {
				throw new WebFaultException<string>(
					"Knowledge source IDs must not be empty.",
					HttpStatusCode.BadRequest);
			}
			if (string.IsNullOrWhiteSpace(request.Query)) {
				throw new WebFaultException<string>(
					"Search query is required.",
					HttpStatusCode.BadRequest);
			}
			if (request.Skip < 0) {
				throw new WebFaultException<string>(
					"Skip must be greater than or equal to 0.",
					HttpStatusCode.BadRequest);
			}
			if (request.Take <= 0) {
				throw new WebFaultException<string>(
					"Take must be greater than 0.",
					HttpStatusCode.BadRequest);
			}
		}

		private static KnwEndUserContext MapEndUserContext(EndUserContextDto dto) {
			if (dto == null) {
				return null;
			}
			return new KnwEndUserContext {
				UserId = dto.UserId ?? string.Empty,
				DisplayName = dto.DisplayName ?? string.Empty
			};
		}

		private static SearchKnowledgeResultItem MapSearchResult(KnwSearchItemResponse_Temp result) {
			return new SearchKnowledgeResultItem {
				KnwSourceId = result.KnwSourceId,
				ItemId = result.ItemDetail?.ItemId,
				Text = result.Response,
				Score = result.Score,
				Citation = new SearchCitationDto {
					Title = result.ItemDetail?.Title ?? string.Empty,
					Location = result.ItemDetail?.Location ?? string.Empty
				}
			};
		}

		private static SearchKnowledgeErrorItem MapSearchError(KnwSearchItemResponse_Temp result) {
			return new SearchKnowledgeErrorItem {
				KnwSourceId = result.KnwSourceId,
				Code = string.IsNullOrWhiteSpace(result.ErrorCode) ? "unknown" : result.ErrorCode,
				Message = result.Error ?? string.Empty
			};
		}

		private static SearchKnowledgeErrorItem MapTopLevelSearchError(string errorMessage) {
			return new SearchKnowledgeErrorItem {
				KnwSourceId = Guid.Empty,
				Code = "search_failed",
				Message = errorMessage ?? string.Empty
			};
		}

		#endregion

		#region Methods: Public

		private static EntitySchemaQuery CreateEsq(EntitySchema schema) {
			return new EntitySchemaQuery(schema) {
				IgnoreDisplayValues = true,
				UnmaskColumnValues = true,
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
		}

		private static string GetDisplayColumnPath(EntitySchema folderSchema) {
			if (folderSchema == null) {
				return null;
			}
			if (folderSchema.Columns.FindByName(NameColumnPath) != null) {
				return NameColumnPath;
			}
			return folderSchema.PrimaryDisplayColumn?.Name;
		}

		private static DynamicFolderLookupQuery CreateDynamicFolderLookupQuery(EntitySchema schema,
				string displayColumnPath) {
			if (string.IsNullOrWhiteSpace(displayColumnPath)) {
				return null;
			}
			EntitySchemaQuery esq = CreateEsq(schema);
			esq.AddColumn(displayColumnPath);
			return new DynamicFolderLookupQuery {
				Esq = esq,
				DisplayColumnPath = displayColumnPath
			};
		}

		internal static DynamicFolderLookupQuery CreateDynamicFoldersFromEntityFolderSchemaQuery(
				EntitySchema folderSchema) {
			DynamicFolderLookupQuery query = CreateDynamicFolderLookupQuery(folderSchema,
				GetDisplayColumnPath(folderSchema));
			if (query == null) {
				return null;
			}
			EntitySchemaQuery esq = query.Esq;
			if (folderSchema.Columns.FindByName(FolderTypeIdColumnPath) != null) {
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal,
					FolderTypeIdColumnPath, DynamicFolderTypeId));
			} else {
				string filterDataColumnPath = GetFolderFilterDataColumnPath(folderSchema);
				if (string.IsNullOrWhiteSpace(filterDataColumnPath)) {
					return null;
				}
				query.FilterDataColumnPath = filterDataColumnPath;
				esq.AddColumn(filterDataColumnPath);
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNotNull,
					filterDataColumnPath));
			}
			AddIsDeletedFilter(esq, folderSchema);
			return query;
		}

		internal static DynamicFolderLookupQuery CreateDynamicFoldersFromFolderTreeQuery(EntitySchema folderTreeSchema,
				string sectionSchemaName) {
			DynamicFolderLookupQuery query = CreateDynamicFolderLookupQuery(folderTreeSchema, NameColumnPath);
			if (query == null) {
				return null;
			}
			EntitySchemaQuery esq = query.Esq;
			query.FilterDataColumnPath = FilterDataColumnPath;
			esq.AddColumn(query.FilterDataColumnPath);
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal,
				EntitySchemaNameColumnPath, sectionSchemaName));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.IsNotNull,
				query.FilterDataColumnPath));
			AddIsDeletedFilter(esq, folderTreeSchema);
			return query;
		}

		private static void AddIsDeletedFilter(EntitySchemaQuery esq, EntitySchema schema) {
			if (schema.Columns.FindByName(IsDeletedColumnPath) == null) {
				return;
			}
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal,
				IsDeletedColumnPath, false));
		}

		private static string GetFolderFilterDataColumnPath(EntitySchema folderSchema) {
			if (folderSchema.Columns.FindByName(SearchDataColumnPath) != null) {
				return SearchDataColumnPath;
			}
			if (folderSchema.Columns.FindByName(FilterDataColumnPath) != null) {
				return FilterDataColumnPath;
			}
			return null;
		}

		private IEnumerable<LookupValue> ExecuteDynamicFolderLookupQuery(DynamicFolderLookupQuery query) {
			if (query == null) {
				return Enumerable.Empty<LookupValue>();
			}
			IEnumerable<Entity> entities = query.Esq.GetEntityCollection(UserConnection);
			if (!string.IsNullOrWhiteSpace(query.FilterDataColumnPath)) {
				entities = entities.Where(entity => HasFilterData(entity, query.FilterDataColumnPath));
			}
			return entities
				.Select(entity => new LookupValue {
					Value = entity.PrimaryColumnValue,
					DisplayValue = entity.GetTypedColumnValue<string>(query.DisplayColumnPath)
				})
				.OrderBy(folder => folder.DisplayValue)
				.ToList();
		}

		private IEnumerable<LookupValue> GetDynamicFoldersFromEntityFolderSchema(EntitySchema folderSchema) {
			DynamicFolderLookupQuery query = CreateDynamicFoldersFromEntityFolderSchemaQuery(folderSchema);
			return ExecuteDynamicFolderLookupQuery(query);
		}

		private IEnumerable<LookupValue> GetDynamicFoldersFromFolderTree(string sectionSchemaName) {
			EntitySchema folderTreeSchema = UserConnection.EntitySchemaManager.GetInstanceByName(FolderTreeSchemaName);
			DynamicFolderLookupQuery query = CreateDynamicFoldersFromFolderTreeQuery(folderTreeSchema,
				sectionSchemaName);
			return ExecuteDynamicFolderLookupQuery(query);
		}

		private static bool HasFilterData(Entity folderEntity, string filterDataColumnName) {
			if (folderEntity == null || string.IsNullOrWhiteSpace(filterDataColumnName)) {
				return false;
			}
			byte[] filterDataBytes = folderEntity.GetBytesValue(filterDataColumnName);
			if (filterDataBytes != null && filterDataBytes.Length > 0) {
				return true;
			}
			string filterDataText = folderEntity.GetTypedColumnValue<string>(filterDataColumnName);
			return !string.IsNullOrWhiteSpace(filterDataText);
		}

		#endregion

		#region Methods: Public

		/// <summary>
		/// Starts the indexing process for the specified knowledge source by delegating to the indexing runner.
		/// </summary>
		/// <param name="request">The request containing the knowledge source identifier.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public void StartIndexing(KnwSourceIndexingRequest request) {
			IndexingRunner.StartIndexing(request.KnwSourceId, UserConnection);
		}

		/// <summary>
		/// Cancels the indexing process for the specified knowledge source by delegating to the indexing runner.
		/// </summary>
		/// <param name="request">The request containing the knowledge source identifier.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public void CancelIndexing(KnwSourceIndexingRequest request) {
			IndexingRunner.CancelIndexing(request.KnwSourceId, UserConnection);
		}

		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public SectionDataResponse GetSectionsData() {
			WorkplacesInfoDto workplacesInfo = WorkplaceManager.GetAvailableWorkplacesInfo(new AvailableWorkplacesInfoRequest {
				UserId = UserConnection.CurrentUser.Id,
				ApplicationClientTypeId = BaseConsts.BrowserClientTypeId
			});
			var sectionDataItems = workplacesInfo.Workplaces
				.SelectMany(w => w.Sections)
				.GroupBy(s => s.EntityUId)
				.Select(g => g.First())
				.Select(s => new LookupValue {
					Value = s.EntityUId,
					DisplayValue = s.Caption,
					PrimaryColorValue = s.IconBackground
				})
				.OrderBy(item => item.DisplayValue)
				.ToList();
			return new SectionDataResponse {
				Sections = sectionDataItems
			};
		}

		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public SectionColumnsResponse GetSectionColumnsData(Guid sectionEntitySchemaUId) {
			EntitySchema sectionSchema = UserConnection.EntitySchemaManager.GetInstanceByUId(sectionEntitySchemaUId);
			return new SectionColumnsResponse {
				Columns = KnwRelatedColumnsSelectorService.GetSectionColumnDataItems(sectionSchema)
			};
		}

		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public SectionDynamicFoldersResponse GetSectionDynamicFoldersData(Guid sectionEntitySchemaUId) {
			if (sectionEntitySchemaUId == Guid.Empty) {
				return new SectionDynamicFoldersResponse {
					Folders = Enumerable.Empty<LookupValue>()
				};
			}
			EntitySchema sectionSchema = UserConnection.EntitySchemaManager.GetInstanceByUId(sectionEntitySchemaUId);
			if (sectionSchema == null) {
				return new SectionDynamicFoldersResponse {
					Folders = Enumerable.Empty<LookupValue>()
				};
			}
			string folderSchemaName = sectionSchema.Name + FolderSchemaSuffix;
			var folderSchemaItem = UserConnection.EntitySchemaManager.FindItemByName(folderSchemaName);
			IEnumerable<LookupValue> folders = Enumerable.Empty<LookupValue>();
			if (folderSchemaItem != null) {
				folders = GetDynamicFoldersFromEntityFolderSchema(UserConnection.EntitySchemaManager
					.GetInstanceByName(folderSchemaName));
			}
			IList<LookupValue> resolvedFolders = folders?.ToList() ?? new List<LookupValue>();
			if (!resolvedFolders.Any()) {
				resolvedFolders = GetDynamicFoldersFromFolderTree(sectionSchema.Name).ToList();
			}
			return new SectionDynamicFoldersResponse {
				Folders = resolvedFolders
			};
		}

		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public SectionRelatedColumnsSelectorResponse ResolveSectionRelatedColumnsSelectorData(
				ResolveSectionRelatedColumnsSelectorDataRequest request) {
			return KnwRelatedColumnsSelectorService.Resolve(request);
		}

		/// <summary>
		/// Resolves compact RelatedColumn references to full display DTOs with display names.
		/// Used by frontend when loading configuration from database.
		/// </summary>
		/// <param name="request">Request containing compact references.</param>
		/// <returns>Response with resolved RelatedColumns including display names.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public ResolveRelatedColumnsResponse ResolveRelatedColumns(ResolveRelatedColumnsRequest request) {
			return new ResolveRelatedColumnsResponse {
				RelatedColumns = KnwRelatedColumnsResolverService.ResolveMany(request?.References).ToList()
			};
		}

		/// <summary>
		/// Searches knowledge across the specified search-ready sources. 
		/// </summary>
		/// <param name="request">Search request.</param>
		/// <returns>Search response with results and per-source errors.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST",
			UriTemplate = "SearchKnowledge",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public SearchKnowledgeResponse SearchKnowledge(SearchKnowledgeRequest request) {
			ValidateSearchRequestOrThrow(request);
			Guid sessionId = request.SessionId.GetValueOrDefault();
			if (sessionId == Guid.Empty) {
				sessionId = Guid.NewGuid();
			}
			KnwLibUtils.Logger.Debug(
				$"[{nameof(KnwLibApiService)}] SearchKnowledge mapped request. " +
				$"SessionId={sessionId}, DistinctSourceCount={request.KnwSourceIds.Distinct().Count()}");
			var searchRequest = new KnwSearchRequest_Temp {
				SessionId = sessionId,
				Skip = request.Skip,
				Take = request.Take,
				EndUserContext = MapEndUserContext(request.EndUserContext),
				KnwSourceQueries = request.KnwSourceIds
					.Distinct()
					.ToDictionary(
						id => id,
						id => (IEnumerable<KnwSearchRequestQuery_Temp>)new[] {
							new KnwSearchRequestQuery_Temp {
								Query = request.Query,
								Keywords = request.Keywords
							}
						})
			};
			KnwSearchDTO_Temp searchResult = KnwSearchClient.Search(searchRequest);
			IEnumerable<KnwSearchItemResponse_Temp> searchItems =
				searchResult?.KnwItems ?? Enumerable.Empty<KnwSearchItemResponse_Temp>();
			IList<SearchKnowledgeErrorItem> errors = searchItems
				.Where(item => !string.IsNullOrWhiteSpace(item.Error))
				.Select(MapSearchError)
				.ToList();
			if (!string.IsNullOrWhiteSpace(searchResult?.ErrorMessage)) {
				errors.Add(MapTopLevelSearchError(searchResult.ErrorMessage));
			}
			IList<SearchKnowledgeResultItem> results = searchItems
				.Where(item => string.IsNullOrWhiteSpace(item.Error))
				.Select(MapSearchResult)
				.ToList();
			KnwLibUtils.Logger.Info(
				$"[{nameof(KnwLibApiService)}] SearchKnowledge final response prepared. " +
				$"SessionId={sessionId}, ResultCount={results.Count}, ErrorCount={errors.Count}");
			return new SearchKnowledgeResponse {
				SessionId = sessionId,
				Results = results,
				Errors = errors
			};
		}

		/// <summary>
		/// Returns user-visible search-ready knowledge sources.
		/// </summary>
		/// <returns>Available knowledge sources.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET",
			UriTemplate = "GetAvailableKnowledgeSources",
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public GetAvailableKnowledgeSourcesResponse GetAvailableKnowledgeSources() {
			IReadOnlyCollection<KnwSourceRecord> availableSources = KnwSourceManager.GetAvailableSources();
			KnwLibUtils.Logger.Info(
				$"[{nameof(KnwLibApiService)}] GetAvailableKnowledgeSources completed. " +
				$"CurrentUserId={UserConnection.CurrentUser.Id}, CurrentUserName={UserConnection.CurrentUser.Name}, " +
				$"SourceCount={availableSources.Count}, SourceIds=[{string.Join(", ", availableSources.Select(source => source.Id.ToString()))}]");
			return new GetAvailableKnowledgeSourcesResponse {
				Sources = availableSources.Select(source => new KnowledgeSourceItem {
					Id = source.Id,
					Name = source.Name,
					Description = source.Description,
					Status = source.Status.ToString()
				}).ToList()
			};
		}

		/// <summary>
		/// Deletes files from the specified knowledge source.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source (from URL path).</param>
		/// <param name="request">The request body containing file IDs to delete.</param>
		[OperationContract]
		[WebInvoke(Method = "DELETE",
			UriTemplate = "DeleteFilesFromKnwSource/{knwSourceId}",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public void DeleteFilesFromKnwSource(string knwSourceId, DeleteFilesRequest request) {
			Guid sourceId = ParseKnwSourceIdOrThrow(knwSourceId);
			KnwServiceClient.DeleteFiles(sourceId, request.FileIds, CancellationToken.None);
		}

		/// <summary>
		/// Retrieves the resolved configuration for a specific knowledge source.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		/// <returns>The resolved configuration with metadata showing the source of each setting group.</returns>
		[OperationContract]
		[WebGet(UriTemplate = "GetKnwSourceConfiguration?knwSourceId={knwSourceId}",
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public ConfigurationScopeResponse GetKnwSourceConfiguration(string knwSourceId) {
			Guid sourceId = ParseKnwSourceIdOrThrow(knwSourceId);
			return KnwServiceClient.GetResolvedConfigurationAsync(sourceId, CancellationToken.None).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Saves (creates or updates) configuration for a specific knowledge source.
		/// </summary>
		/// <param name="request">The configuration request containing the knowledge source ID and settings.</param>
		/// <returns>The saved configuration response.</returns>
		/// <remarks>
		/// This method first checks if a configuration already exists for the knowledge source.
		/// If it exists, the configuration is updated; otherwise, a new configuration is created.
		/// </remarks>
		[OperationContract]
		[WebInvoke(Method = "POST",
			UriTemplate = "SaveKnwSourceConfiguration",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public ConfigurationScopeResponse SaveKnwSourceConfiguration(KnwSourceConfigRequest request) {
			try {
				if (request.ScoreThreshold.HasValue &&
					(request.ScoreThreshold.Value < 0f || request.ScoreThreshold.Value > 1f)) {
					throw new WebFaultException<string>(
						"ScoreThreshold must be between 0 and 1.",
						HttpStatusCode.BadRequest);
				}

				var configRequest = new ConfigurationScopeRequest {
					KnowledgeSourceId = request.KnwSourceId,
					RetrieveSettings = new RetrieveSettingsOverride {
						ScoreThreshold = request.ScoreThreshold,
						KeywordBoostingEnabled = request.KeywordBoostingEnabled
					}
				};

				List<ConfigurationScopeResponse> existingConfigs = LoadConfigurations(request.KnwSourceId);
				if (existingConfigs.Count > 1) {
					throw new WebFaultException<string>(
						"Multiple configurations found for this knowledge source. Resolve duplicates before saving.",
						HttpStatusCode.Conflict);
				}

				if (existingConfigs.Count == 1) {
					ConfigurationScopeResponse existingConfig = existingConfigs[0];
					try {
						return UpdateConfiguration(existingConfig.Id, configRequest);
					} catch (Exception updateException) {
						KnwLibUtils.Logger.Error(
							$"Failed to update configuration {existingConfig.Id} for source {request.KnwSourceId}.",
							updateException);
						if (!IsNotFoundException(updateException)) {
							throw new WebFaultException<string>(
								"Failed to update existing configuration for this knowledge source.",
								HttpStatusCode.BadRequest);
						}
						// 404: configuration was deleted between LoadConfigurations and UpdateConfiguration.
						// Fall through to CreateWithConflictRetry below.
					}
				}
				return CreateWithConflictRetry(request.KnwSourceId, configRequest);
			} catch (FaultException) {
				throw;
			} catch (Exception ex) {
				KnwLibUtils.Logger.Error("Failed to save knowledge source configuration.", ex);
				throw new WebFaultException<string>(
					"Failed to save knowledge source configuration.",
					HttpStatusCode.BadRequest);
			}
		}

		/// <summary>
		/// Deletes the configuration for a specific knowledge source, reverting to client-level defaults.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		[OperationContract]
		[WebInvoke(Method = "DELETE",
			UriTemplate = "DeleteKnwSourceConfiguration?knwSourceId={knwSourceId}",
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public void DeleteKnwSourceConfiguration(string knwSourceId) {
			try {
				Guid sourceId = ParseKnwSourceIdOrThrow(knwSourceId);

				// Find the configuration for this knowledge source
				var existingConfigs = KnwServiceClient.ListConfigurationsAsync(sourceId, CancellationToken.None)
					.GetAwaiter().GetResult();
				var existingConfig = existingConfigs.FirstOrDefault();

				if (existingConfig != null) {
					KnwServiceClient.DeleteConfigurationAsync(existingConfig.Id, CancellationToken.None)
						.GetAwaiter().GetResult();
				}
			} catch (FaultException) {
				throw;
			} catch (Exception ex) {
				KnwLibUtils.Logger.Error("Failed to delete knowledge source configuration.", ex);
				throw new WebFaultException<string>(
					"Failed to delete knowledge source configuration.",
					HttpStatusCode.BadRequest);
			}
		}

		/// <summary>
		/// Checks the health of the knowledge management microservice.
		/// Returns a healthy status when the microservice is reachable and ready.
		/// </summary>
		/// <returns>Health status response.</returns>
		[OperationContract]
		[WebInvoke(Method = "GET",
			UriTemplate = "CheckHealth",
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		public KnwServiceHealthResponse CheckHealth() {
			try {
				KnwServiceClient.CheckServiceHealth(CancellationToken.None);
				return new KnwServiceHealthResponse { Status = "Healthy" };
			} catch (FaultException) {
				throw;
			} catch (Exception ex) {
				KnwLibUtils.Logger.Error("Knowledge service health check failed.", ex);
				throw new WebFaultException<string>(ex.Message, HttpStatusCode.ServiceUnavailable);
			}
		}

		#endregion

	}

	#endregion

}

