namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Class: SectionColumnDataItem

	[DataContract]
	public class SectionColumnDataItem
	{

		#region Properties: Public

		[DataMember(Name = "value")]
		public Guid Value { get; set; }

		[DataMember(Name = "displayValue")]
		public string DisplayValue { get; set; }

		[DataMember(Name = "isEntityReference")]
		public bool IsEntityReference { get; set; }

		#endregion

	}

	#endregion

	#region Class: PaginationOptions

	[DataContract]
	public class PaginationOptions
	{

		#region Properties: Public

		[DataMember(Name = "pageSize")]
		public int PageSize { get; set; }

		[DataMember(Name = "columnsCursorId")]
		public string ColumnsCursorId { get; set; }

		[DataMember(Name = "referencesCursorId")]
		public string ReferencesCursorId { get; set; }

		[DataMember(Name = "direction")]
		public string Direction { get; set; } = "First";

		#endregion

	}

	#endregion

	#region Class: SearchFilterOptions

	[DataContract]
	public class SearchFilterOptions
	{

		#region Properties: Public

		[DataMember(Name = "columnsSearchText")]
		public string ColumnsSearchText { get; set; }

		[DataMember(Name = "referencesSearchText")]
		public string ReferencesSearchText { get; set; }

		#endregion

	}

	#endregion

	#region Class: PaginationMetadata

	[DataContract]
	public class PaginationMetadata
	{

		#region Properties: Public

		[DataMember(Name = "hasMore")]
		public bool HasMore { get; set; }

		[DataMember(Name = "lastCursorId")]
		public string LastCursorId { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionRelatedColumnsSelectorResponse

	[DataContract]
	public class SectionRelatedColumnsSelectorResponse
	{

		#region Properties: Public

		[DataMember(Name = "references")]
		public IEnumerable<SectionLinkedItemDto> References { get; set; } =
			Enumerable.Empty<SectionLinkedItemDto>();

		[DataMember(Name = "columns")]
		public IEnumerable<SectionColumnDataItem> Columns { get; set; } =
			Enumerable.Empty<SectionColumnDataItem>();

		[DataMember(Name = "columnsPagination")]
		public PaginationMetadata ColumnsPagination { get; set; }

		[DataMember(Name = "referencesPagination")]
		public PaginationMetadata ReferencesPagination { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionLinkedItemDto

	[DataContract]
	public class SectionLinkedItemDto
	{

		#region Properties: Public

		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "caption")]
		public string Caption { get; set; }

		[DataMember(Name = "terminalSchemaUId")]
		public Guid TerminalSchemaUId { get; set; }

		[DataMember(Name = "terminalSchemaName")]
		public string TerminalSchemaName { get; set; }

		[DataMember(Name = "pathSegments")]
		public IEnumerable<SectionLinkedItemSegmentDto> Segments { get; set; } =
			Enumerable.Empty<SectionLinkedItemSegmentDto>();

		#endregion

	}

	#endregion

	#region Class: SectionLinkedItemSegmentDto

	[DataContract]
	public class SectionLinkedItemSegmentDto
	{

		#region Properties: Public

		[DataMember(Name = "relationType")]
		public string RelationType { get; set; }

		[DataMember(Name = "caption")]
		public string Caption { get; set; }

		[DataMember(Name = "sourceSchemaUId")]
		public Guid SourceSchemaUId { get; set; }

		[DataMember(Name = "sourceSchemaName")]
		public string SourceSchemaName { get; set; }

		[DataMember(Name = "targetSchemaUId")]
		public Guid TargetSchemaUId { get; set; }

		[DataMember(Name = "targetSchemaName")]
		public string TargetSchemaName { get; set; }

		[DataMember(Name = "columnUId")]
		public Guid? ColumnUId { get; set; }

		[DataMember(Name = "columnName")]
		public string ColumnName { get; set; }

		#endregion

	}

	#endregion

	#region Class: ResolveSectionRelatedColumnsSelectorDataRequest

	[DataContract]
	public class ResolveSectionRelatedColumnsSelectorDataRequest
	{

		#region Properties: Public

		[DataMember(Name = "rootSchemaUId")]
		public Guid RootSchemaUId { get; set; }

		[DataMember(Name = "currentSchemaUId")]
		public Guid CurrentSchemaUId { get; set; }

		[DataMember(Name = "currentPathSegments")]
		public List<PathSegmentReference> CurrentPathSegments { get; set; } = new List<PathSegmentReference>();

		[DataMember(Name = "paginationOptions")]
		public PaginationOptions PaginationOptions { get; set; }

		[DataMember(Name = "searchFilter")]
		public SearchFilterOptions SearchFilter { get; set; }

		[DataMember(Name = "loadScope")]
		public string LoadScope { get; set; } = "Both";

		#endregion

	}

	#endregion

	#region Interface: IKnwRelatedColumnsSelectorService

	internal interface IKnwRelatedColumnsSelectorService
	{

		#region Methods: Public

		IEnumerable<SectionColumnDataItem> GetSectionColumnDataItems(EntitySchema schema);

		SectionRelatedColumnsSelectorResponse Resolve(ResolveSectionRelatedColumnsSelectorDataRequest request);

		#endregion

	}

	#endregion

	#region Class: KnwRelatedColumnsSelectorService

	[DefaultBinding(typeof(IKnwRelatedColumnsSelectorService))]
	internal class KnwRelatedColumnsSelectorService : IKnwRelatedColumnsSelectorService
	{

		#region Class: SelectorColumnsPageResult

		private sealed class SelectorColumnsPageResult
		{

			#region Properties: Public

			public IEnumerable<SectionColumnDataItem> Items { get; set; } = Enumerable.Empty<SectionColumnDataItem>();

			public bool HasMore { get; set; }

			public string LastCursorId { get; set; }

			#endregion

		}

		#endregion

		#region Constants: Private

		private const string RelatedColumnsSelectorPageSizeSettingCode = "RelatedColumnsSelectorPageSize";
		private const int DefaultRelatedColumnsSelectorPageSize = 20;

		#endregion

		#region Fields: Private

		private readonly UserConnection _userConnection;
		private readonly IKnwSectionLinkedItemsService _sectionLinkedItemsService;

		#endregion

		#region Constructors: Public

		public KnwRelatedColumnsSelectorService(UserConnection userConnection) {
			_userConnection = userConnection ?? throw new ArgumentNullException(nameof(userConnection));
			_sectionLinkedItemsService = ClassFactory.Get<IKnwSectionLinkedItemsService>(
				new ConstructorArgument("userConnection", _userConnection));
		}

		#endregion

		#region Methods: Private

		private SectionRelatedColumnsSelectorResponse CreateEmptySelectorResponse() {
			return new SectionRelatedColumnsSelectorResponse {
				ColumnsPagination = new PaginationMetadata { HasMore = false },
				ReferencesPagination = new PaginationMetadata { HasMore = false }
			};
		}

		private int GetRelatedColumnsSelectorPageSize() {
			try {
				int pageSizeValue = SysSettings.GetValue(_userConnection,
					RelatedColumnsSelectorPageSizeSettingCode, DefaultRelatedColumnsSelectorPageSize);
				return pageSizeValue > 0 ? pageSizeValue : DefaultRelatedColumnsSelectorPageSize;
			} catch (Exception) {
				return DefaultRelatedColumnsSelectorPageSize;
			}
		}

		private static string BuildPathKeyFromSegments(IEnumerable<PathSegmentReference> pathSegments) {
			if (pathSegments == null) {
				return string.Empty;
			}
			return string.Join("/", pathSegments
				.Where(segment => segment != null)
				.Select(segment => {
					string relationType = segment.RelationType?.ToLowerInvariant() ?? "unknown";
					return $"{relationType}:{segment.SourceSchemaUId}:{segment.ColumnUId}:{segment.TargetSchemaUId}";
				}));
		}

		private static bool IsSelectableReferenceColumn(EntitySchemaColumn column) {
			return column != null
				&& !column.IsVirtual
				&& column.UsageType != EntitySchemaColumnUsageType.None
				&& column.ReferenceSchemaUId != Guid.Empty;
		}

		private static IEnumerable<EntitySchemaColumn> GetSelectableSectionColumns(EntitySchema schema) {
			return schema?.Columns
				.Cast<EntitySchemaColumn>()
				.Where(column => !column.IsVirtual && column.UsageType != EntitySchemaColumnUsageType.None)
				?? Enumerable.Empty<EntitySchemaColumn>();
		}

		private static SectionColumnDataItem GetSectionColumnDataItem(EntitySchemaColumn column) {
			if (column == null) {
				return null;
			}
			return new SectionColumnDataItem {
				Value = column.UId,
				DisplayValue = column.Caption?.Value ?? column.Name,
				IsEntityReference = IsSelectableReferenceColumn(column)
			};
		}

		private static bool MatchesColumnSearch(string searchLower, SectionColumnDataItem item) {
			if (string.IsNullOrWhiteSpace(searchLower)) {
				return true;
			}
			return item?.DisplayValue != null && item.DisplayValue.ToLowerInvariant().Contains(searchLower);
		}

		private SelectorColumnsPageResult GetSectionColumnsPage(EntitySchema currentSchema, string searchText,
				string cursorId, int pageSize) {
			if (currentSchema == null || pageSize <= 0) {
				return new SelectorColumnsPageResult();
			}

			string searchLower = searchText?.Trim().ToLowerInvariant();
			bool hasCursor = !string.IsNullOrWhiteSpace(cursorId);
			string cursorValue = cursorId?.Trim();
			bool cursorMatched = !hasCursor;

			var pageItems = new List<SectionColumnDataItem>(pageSize + 1);
			var firstPageCandidates = hasCursor ? new List<SectionColumnDataItem>(pageSize + 1) : null;

			IEnumerable<EntitySchemaColumn> orderedColumns = GetSelectableSectionColumns(currentSchema)
				.OrderBy(column => column.UId);
			foreach (EntitySchemaColumn column in orderedColumns) {
				SectionColumnDataItem item = GetSectionColumnDataItem(column);
				if (item == null || item.IsEntityReference) {
					continue;
				}
				if (!MatchesColumnSearch(searchLower, item)) {
					continue;
				}

				if (!cursorMatched) {
					string itemCursor = item.Value.ToString("D");
					if (string.Equals(itemCursor, cursorValue, StringComparison.OrdinalIgnoreCase)) {
						cursorMatched = true;
						continue;
					}
					if (firstPageCandidates != null && firstPageCandidates.Count < pageSize + 1) {
						firstPageCandidates.Add(item);
					}
					continue;
				}

				pageItems.Add(item);
				if (pageItems.Count >= pageSize + 1) {
					break;
				}
			}

			if (hasCursor && !cursorMatched) {
				pageItems = firstPageCandidates ?? new List<SectionColumnDataItem>();
			}

			bool hasMore = pageItems.Count > pageSize;
			if (hasMore) {
				pageItems.RemoveAt(pageItems.Count - 1);
			}
			string lastCursorId = pageItems.Count > 0
				? pageItems[pageItems.Count - 1].Value.ToString("D")
				: null;
			return new SelectorColumnsPageResult {
				Items = pageItems,
				HasMore = hasMore,
				LastCursorId = lastCursorId
			};
		}

		private static void ResolveLoadScope(string loadScope, out bool loadColumns, out bool loadReferences) {
			if (string.Equals(loadScope, "Columns", StringComparison.OrdinalIgnoreCase)) {
				loadColumns = true;
				loadReferences = false;
				return;
			}
			if (string.Equals(loadScope, "References", StringComparison.OrdinalIgnoreCase)) {
				loadColumns = false;
				loadReferences = true;
				return;
			}
			loadColumns = true;
			loadReferences = true;
		}

		#endregion

		#region Methods: Public

		public IEnumerable<SectionColumnDataItem> GetSectionColumnDataItems(EntitySchema schema) {
			return GetSelectableSectionColumns(schema)
				.Select(GetSectionColumnDataItem)
				.Where(item => item != null);
		}

		public SectionRelatedColumnsSelectorResponse Resolve(ResolveSectionRelatedColumnsSelectorDataRequest request) {
			if (request == null) {
				return CreateEmptySelectorResponse();
			}
			if (request.RootSchemaUId == Guid.Empty || request.CurrentSchemaUId == Guid.Empty) {
				return CreateEmptySelectorResponse();
			}

			EntitySchema rootSchema = _userConnection.EntitySchemaManager.FindInstanceByUId(request.RootSchemaUId);
			EntitySchema currentSchema = _userConnection.EntitySchemaManager.FindInstanceByUId(request.CurrentSchemaUId);
			if (rootSchema == null || currentSchema == null) {
				return CreateEmptySelectorResponse();
			}

			string currentPathKey = BuildPathKeyFromSegments(request.CurrentPathSegments);
			EntitySchema resolvedSchema = _sectionLinkedItemsService.ResolveCurrentSchema(rootSchema, currentPathKey);
			if (resolvedSchema == null || resolvedSchema.UId != currentSchema.UId) {
				return CreateEmptySelectorResponse();
			}

			PaginationOptions paginationOptions = request.PaginationOptions ?? new PaginationOptions();
			int pageSize = paginationOptions.PageSize > 0
				? paginationOptions.PageSize
				: GetRelatedColumnsSelectorPageSize();
			bool isNextPageDirection = string.Equals(paginationOptions.Direction, "Next",
				StringComparison.OrdinalIgnoreCase);

			string columnsCursorId = isNextPageDirection ? paginationOptions.ColumnsCursorId : null;
			string referencesCursorId = isNextPageDirection ? paginationOptions.ReferencesCursorId : null;
			SearchFilterOptions searchFilter = request.SearchFilter ?? new SearchFilterOptions();
			ResolveLoadScope(request.LoadScope, out bool loadColumns, out bool loadReferences);

			IEnumerable<SectionLinkedItemDto> pagedReferences = Enumerable.Empty<SectionLinkedItemDto>();
			bool referencesHasMore = false;
			string referencesLastCursor = null;
			if (loadReferences) {
				SelectorLinkedItemsPageResult referencesPage = _sectionLinkedItemsService.GetSelectorLinkedItemsPage(
					rootSchema, currentPathKey, searchFilter.ReferencesSearchText, referencesCursorId, pageSize);
				pagedReferences = referencesPage.Items;
				referencesHasMore = referencesPage.HasMore;
				referencesLastCursor = referencesPage.LastCursorId;
			}

			IEnumerable<SectionColumnDataItem> pagedColumns = Enumerable.Empty<SectionColumnDataItem>();
			bool columnsHasMore = false;
			string columnsLastCursor = null;
			if (loadColumns && rootSchema.UId != currentSchema.UId) {
				SelectorColumnsPageResult columnsPage = GetSectionColumnsPage(currentSchema,
					searchFilter.ColumnsSearchText, columnsCursorId, pageSize);
				pagedColumns = columnsPage.Items;
				columnsHasMore = columnsPage.HasMore;
				columnsLastCursor = columnsPage.LastCursorId;
			}

			return new SectionRelatedColumnsSelectorResponse {
				References = pagedReferences,
				Columns = pagedColumns,
				ColumnsPagination = new PaginationMetadata {
					HasMore = columnsHasMore,
					LastCursorId = columnsLastCursor
				},
				ReferencesPagination = new PaginationMetadata {
					HasMore = referencesHasMore,
					LastCursorId = referencesLastCursor
				}
			};
		}

		#endregion

	}

	#endregion

}

