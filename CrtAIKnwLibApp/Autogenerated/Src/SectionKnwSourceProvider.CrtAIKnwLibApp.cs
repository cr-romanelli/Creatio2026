namespace Creatio.Copilot
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using Terrasoft.Common;
	using Terrasoft.Configuration;
	using Terrasoft.Configuration.PageableSelectHelper;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Nui.ServiceModel.DataContract;
	using Terrasoft.Nui.ServiceModel.Extensions;
	using EntityCollection = Terrasoft.Core.Entities.EntityCollection;
	using EntitySchema = Terrasoft.Core.Entities.EntitySchema;
	using EntitySchemaColumn = Terrasoft.Core.Entities.EntitySchemaColumn;

	#region Class: SectionKnwSourceProvider

	/// <summary>
	/// Implementation of <see cref="IKnwBatchProvider"/> for section-based knowledge sources.
	/// Indexes entity records from configured Creatio sections.
	/// </summary>
	[DefaultBinding(typeof(IKnwProvider), Name = "SectionKnowledgeSourceProvider")]
	public class SectionKnwSourceProvider : IKnwBatchProvider
	{

		#region Constants: Private

		private const string SectionItemType = "Section";
		private const int MaxSectionNameLength = 240;
		private const string DefaultDisplayColumnName = "Name";
		private const string SectionRelativeRecordTemplate =
			"Shell/Navigation/Navigation.aspx?recordId={0}&schemaName={1}";
		private const string IdColumnName = "Id";
		private const string ModifiedOnColumnName = "ModifiedOn";
		private const string RecordIdMetadataKey = "RecordId";
		private const string EntitySchemaNameMetadataKey = "EntitySchemaName";
		private const string DynamicFolderScopeType = "DynamicFolder";
		private static readonly Guid DynamicFolderTypeId = new Guid("65CA0946-0084-4874-B117-C13199AF3B95");

		#endregion

		#region Fields: Private

		private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars().Append(' ').ToArray();
		private readonly IBaseUriResolver _baseUriResolver;
		private UserConnection _userConnection;
		private Guid _knwSourceId;
		private Guid _entitySchemaUId;
		private List<string> _columnSpecs;
		private Filters _filterJson;
		private string _scopeType;
		private Guid _dynamicFolderId;
		private readonly IPageableSelectHelper_Temp _pageableSelectHelper;
		private IKnwRelatedColumnsResolverService _relatedColumnsResolverService;

		#endregion

		#region Class: SectionProviderConfig

		private sealed class SectionProviderConfig
		{

			#region Properties: Public

			public Guid RootSchemaUId { get; set; }

			public List<string> Columns { get; set; }

			public Filters Filters { get; set; }

			public string ScopeType { get; set; }

			public Guid? DynamicFolderId { get; set; }

			/// <summary>
			/// Related columns stored as compact references (UIds only).
			/// Resolved to full DTOs at runtime during initialization.
			/// </summary>
			public List<RelatedColumnReference> RelatedColumns { get; set; }

			#endregion

		}

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="SectionKnwSourceProvider"/> type.
		/// </summary>
		/// <param name="baseUriResolver">Base Uri Resolver.</param>
		public SectionKnwSourceProvider(IBaseUriResolver baseUriResolver, IPageableSelectHelper_Temp pageableSelectHelper) {
			_baseUriResolver = baseUriResolver;
			_pageableSelectHelper = pageableSelectHelper;
		}

		#endregion

		#region Methods: Private

		private static string GetDisplayColumnName(EntitySchema schema) {
			return schema.PrimaryDisplayColumn?.Name ?? DefaultDisplayColumnName;
		}

		private static void EnsureDisplayColumnInQuery(EntitySchemaQuery esq, EntitySchema schema) {
			string displayColumnName = GetDisplayColumnName(schema);
			EntitySchemaColumn displayColumn = schema.Columns.FindByName(displayColumnName);
			if (displayColumn != null && esq.Columns.FindByName(displayColumn.Name) == null) {
				esq.AddColumn(displayColumn.Name);
			}
		}

		private static void ApplyModifiedSinceFilter(EntitySchemaQuery esq, DateTime? modifiedSince) {
			if (!modifiedSince.HasValue) {
				return;
			}
			IEntitySchemaQueryFilterItem modifiedOnFilter = esq.CreateFilterWithParameters(
				FilterComparisonType.Greater, ModifiedOnColumnName, modifiedSince.Value);
			esq.Filters.Add(modifiedOnFilter);
		}

		private static List<string> NormalizeItemIds(ISet<string> itemIds) {
			if (itemIds == null || itemIds.Count == 0) {
				return new List<string>();
			}
			return itemIds
				.Select(x => {
					if (!Guid.TryParse(x, out Guid parsed) || parsed == Guid.Empty) {
						return null;
					}
					return parsed.ToString();
				})
				.Where(x => !x.IsNullOrWhiteSpace())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		private static void ApplyItemIdsFilter(EntitySchemaQuery esq, ICollection<string> itemIds) {
			if (itemIds == null || itemIds.Count == 0) {
				return;
			}
			IEntitySchemaQueryFilterItem idsFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id",
				itemIds.ToList());
			esq.Filters.Add(idsFilter);
		}

		private static string FormatDisplayName(string displayName, Guid fallbackId) {
			if (displayName.IsNullOrWhiteSpace()) {
				displayName = fallbackId.ToString("N");
			}
			foreach (char c in _invalidFileNameChars) {
				displayName = displayName.Replace(c,
					'_');
			}
			displayName = displayName.Trim('.',
				'_');
			displayName = Regex.Replace(displayName,
				@"_{2,}",
				"_");
			if (displayName.Length > MaxSectionNameLength) {
				displayName = displayName.Substring(0,
					MaxSectionNameLength).TrimEnd('_', '-');
			}
			if (displayName.IsNullOrWhiteSpace()) {
				displayName = fallbackId.ToString("N");
			}
			displayName += ".json";
			return displayName;
		}

		private string SerializeEntityToJson(Entity entity) {
			if (entity == null) {
				return "{}";
			}
			var jsonDict = new Dictionary<string, object>();
			foreach (EntitySchemaColumn column in entity.Schema.Columns) {
				try {
					if (column.DataValueType.IsBinary) {
						continue;
					}
					object value = entity.GetColumnValue(column.Name);
					if (value != null) {
						if (value is Guid guid) {
							jsonDict[column.Name] = guid.ToString();
						} else if (value is DateTime dateTime) {
							jsonDict[column.Name] = dateTime.ToString("o");
						} else {
							jsonDict[column.Name] = value;
						}
					}
				} catch (Exception e) {
					WriteLog($"Failed to serialize column '{column.Name}' for entity", e, entity.PrimaryColumnValue);
				}
			}
			return JsonConvert.SerializeObject(jsonDict);
		}

		private List<string> BuildEffectiveColumnSpecs(EntitySchema rootSchema, IEnumerable<string> rootColumnSpecs,
				IEnumerable<SectionRelatedColumnConfigDto> relatedColumns) {
			var effectiveColumnSpecs = new List<string>();
			var seenSpecs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string columnSpec in rootColumnSpecs ?? Enumerable.Empty<string>()) {
				string resolvedColumnPath = ResolveRootColumnPath(rootSchema, columnSpec);
				AddEffectiveColumnSpec(effectiveColumnSpecs, seenSpecs, resolvedColumnPath);
			}
			int relatedCount = 0;
			int derivedCount = 0;
			foreach (SectionRelatedColumnConfigDto relatedColumn in relatedColumns ??
					Enumerable.Empty<SectionRelatedColumnConfigDto>()) {
				relatedCount++;
				string derivedSpec = BuildRelatedColumnSpec(rootSchema, relatedColumn);
				if (AddEffectiveColumnSpec(effectiveColumnSpecs, seenSpecs, derivedSpec)) {
					derivedCount++;
				}
			}
			WriteLog(
				$"Parsed config column specs. RootColumns={rootColumnSpecs?.Count() ?? 0}, " +
				$"RelatedColumns={relatedCount}, DerivedRelatedColumns={derivedCount}, " +
				$"EffectiveColumns={effectiveColumnSpecs.Count}");
			return effectiveColumnSpecs;
		}

		private static bool AddEffectiveColumnSpec(ICollection<string> target, ISet<string> seenSpecs,
				string columnSpec) {
			if (columnSpec.IsNullOrWhiteSpace()) {
				return false;
			}
			string normalizedSpec = columnSpec.Trim();
			if (!seenSpecs.Add(normalizedSpec)) {
				return false;
			}
			target.Add(normalizedSpec);
			return true;
		}

		private string ResolveRootColumnPath(EntitySchema rootSchema, string columnSpec) {
			if (rootSchema == null || columnSpec.IsNullOrWhiteSpace()) {
				return null;
			}
			try {
				string columnPath = rootSchema.FindSchemaColumnPathByMetaPath(columnSpec);
				if (!columnPath.IsNullOrWhiteSpace()) {
					return columnPath;
				}
				WriteLog($"Column '{columnSpec}' not found in schema '{rootSchema.Name}'");
			} catch (Exception e) {
				WriteLog($"Failed to resolve configured column '{columnSpec}'", e);
			}
			return null;
		}

		private EntitySchemaColumn FindColumn(EntitySchema schema, Guid? columnUId, string columnName) {
			if (schema == null) {
				return null;
			}
			if (columnUId.HasValue && !columnUId.Value.IsEmpty()) {
				EntitySchemaColumn columnByUId = schema.Columns.FindByUId(columnUId.Value);
				if (columnByUId != null) {
					return columnByUId;
				}
			}
			return columnName.IsNullOrWhiteSpace()
				? null
				: schema.Columns.FindByName(columnName);
		}

		private string BuildRelatedColumnSpec(EntitySchema rootSchema, SectionRelatedColumnConfigDto relatedColumn) {
			if (relatedColumn == null) {
				return null;
			}
			if (rootSchema == null) {
				WriteLog($"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because root schema is missing.");
				return null;
			}
			if (relatedColumn.TerminalColumnUId.IsEmpty()) {
				WriteLog(
					$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because terminal column is empty.");
				return null;
			}
			if (relatedColumn.PathSegments == null || !relatedColumn.PathSegments.Any()) {
				WriteLog(
					$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because path segments are missing.");
				return null;
			}
			var pathParts = new List<string>();
			EntitySchema currentSchema = rootSchema;
			foreach (SectionLinkedItemSegmentDto segment in relatedColumn.PathSegments) {
				if (segment == null) {
					WriteLog(
						$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because a path segment is null.");
					return null;
				}
				if (string.Equals(segment.RelationType, "Lookup", StringComparison.OrdinalIgnoreCase)) {
					EntitySchemaColumn lookupColumn = FindColumn(currentSchema, segment.ColumnUId,
						segment.ColumnName);
					if (lookupColumn == null) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because lookup column " +
							$"'{segment.ColumnUId?.ToString() ?? segment.ColumnName ?? "<empty>"}' was not found.");
						return null;
					}
					if (lookupColumn.ReferenceSchema == null && lookupColumn.ReferenceSchemaUId.IsEmpty()) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because lookup column " +
							$"'{lookupColumn.Name}' is not a reference.");
						return null;
					}
					EntitySchema targetSchema = lookupColumn.ReferenceSchema ??
						_userConnection?.EntitySchemaManager?.FindInstanceByUId(lookupColumn.ReferenceSchemaUId);
					if (targetSchema == null) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because target schema for " +
							$"lookup '{lookupColumn.Name}' was not found.");
						return null;
					}
					pathParts.Add(lookupColumn.Name);
					currentSchema = targetSchema;
					continue;
				}
				if (string.Equals(segment.RelationType, "Child", StringComparison.OrdinalIgnoreCase)) {
					EntitySchema targetSchema = _userConnection?.EntitySchemaManager?.FindInstanceByUId(segment.TargetSchemaUId);
					if (targetSchema == null) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because child schema " +
							$"'{segment.TargetSchemaUId}' was not found.");
						return null;
					}
					EntitySchemaColumn childReferenceColumn = FindColumn(targetSchema, segment.ColumnUId,
						segment.ColumnName);
					if (childReferenceColumn == null) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because child reference " +
							$"column '{segment.ColumnUId?.ToString() ?? segment.ColumnName ?? "<empty>"}' was not found.");
						return null;
					}
					if (childReferenceColumn.ReferenceSchemaUId != currentSchema.UId) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because child reference " +
							$"column '{childReferenceColumn.Name}' does not point to parent schema '{currentSchema.Name}'.");
						return null;
					}
					EntitySchemaColumn parentPrimaryColumn = currentSchema.PrimaryColumn;
					if (parentPrimaryColumn == null) {
						WriteLog(
							$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because parent schema " +
							$"'{currentSchema.Name}' has no primary column.");
						return null;
					}
					pathParts.Add($"[{targetSchema.Name}:{childReferenceColumn.Name}:{parentPrimaryColumn.Name}]");
					currentSchema = targetSchema;
					continue;
				}
				WriteLog(
					$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because relation type " +
					$"'{segment.RelationType ?? "<empty>"}' is not supported.");
				return null;
			}
			EntitySchemaColumn terminalColumn = FindColumn(currentSchema, relatedColumn.TerminalColumnUId,
				relatedColumn.TerminalColumnName);
			if (terminalColumn == null) {
				WriteLog(
					$"Skipping related column '{relatedColumn.PathKey ?? "<empty>"}' because terminal column " +
					$"'{relatedColumn.TerminalColumnUId}' was not found.");
				return null;
			}
			pathParts.Add(terminalColumn.Name);
			return string.Join(".", pathParts);
		}

		private void WriteLog(string context, Exception exception, Guid? recordId = null) {
			string sourcePart = _knwSourceId.IsNotEmpty() ? $"SourceId={_knwSourceId}" : "SourceId=<empty>";
			string recordPart = recordId.HasValue && recordId.Value.IsNotEmpty() ?
				$", RecordId={recordId.Value}" : string.Empty;
			KnwLibUtils.Logger.Error($"[{nameof(SectionKnwSourceProvider)}] {context}. {sourcePart}{recordPart}",
				exception);
		}

		private void WriteLog(string message) {
			string sourcePart = _knwSourceId.IsNotEmpty() ? $"SourceId={_knwSourceId}" : "SourceId=<empty>";
			KnwLibUtils.Logger.Info($"[{nameof(SectionKnwSourceProvider)}] {message}. {sourcePart}");
		}

		private EntitySchemaQuery GetEsq(DateTime? modifiedSince = null, ICollection<string> itemIds = null) {
			EntitySchema schema = GetEntitySchema();
			if (schema == null) {
				return null;
			}
			var esq = new EntitySchemaQuery(schema) {
				PrimaryQueryColumn = { IsAlwaysSelect = true },
				IgnoreDisplayValues = true,
			};
			AddColumnsToQuery(esq, schema);
			AddBasicColumnsToQuery(esq, schema);
			if (!ApplyCustomFilters(esq)) {
				return null;
			}
			ApplyModifiedSinceFilter(esq, modifiedSince);
			ApplyItemIdsFilter(esq, itemIds);
			return esq;
		}

		private void AddBasicColumnsToQuery(EntitySchemaQuery esq, EntitySchema schema) {
			EnsureDisplayColumnInQuery(esq, schema);
			EntitySchemaColumn modifiedColumn = schema.Columns.FindByName(ModifiedOnColumnName);
			if (modifiedColumn != null && esq.Columns.FindByName(modifiedColumn.Name) == null) {
				esq.AddColumn(ModifiedOnColumnName);
			}
		}

		private EntitySchema GetEntitySchema() {
			if (_entitySchemaUId.IsEmpty()) {
				return null;
			}
			return _userConnection.EntitySchemaManager.GetInstanceByUId(_entitySchemaUId);
		}

		private void AddColumnsToQuery(EntitySchemaQuery esq, EntitySchema schema) {
			if (_columnSpecs == null || !_columnSpecs.Any()) {
				return;
			}
			string primaryColumnName = esq.RootSchema.PrimaryColumn?.Name;
			foreach (string columnSpec in _columnSpecs) {
				if (string.IsNullOrWhiteSpace(columnSpec)) {
					continue;
				}
				if (!string.IsNullOrEmpty(primaryColumnName) &&
						columnSpec.Equals(primaryColumnName, StringComparison.OrdinalIgnoreCase)) {
					continue;
				}
				try {
					if (columnSpec != esq.PrimaryQueryColumn.Path) {
						esq.AddColumn(columnSpec);
					}
				} catch (Exception e) {
					WriteLog($"Failed to add column '{columnSpec}' to query", e);
				}
			}
		}

		private bool ApplyCustomFilters(EntitySchemaQuery esq) {
			if (string.Equals(_scopeType, DynamicFolderScopeType, StringComparison.OrdinalIgnoreCase)) {
				return ApplyDynamicFolderFilter(esq);
			}
			if (_filterJson == null) {
				return true;
			}
			IEntitySchemaQueryFilterItem filters = _filterJson.BuildEsqFilter(esq, _userConnection);
			esq.Filters.Add(filters);
			return true;
		}

		private bool ApplyDynamicFolderFilter(EntitySchemaQuery esq) {
			if (_dynamicFolderId.IsEmpty()) {
				WriteLog("Dynamic folder scope selected but DynamicFolderId is empty.");
				return false;
			}
			byte[] filterData = GetDynamicFolderFilterData();
			if (filterData == null || filterData.Length == 0) {
				WriteLog($"Dynamic folder '{_dynamicFolderId}' does not exist or has no filter data.");
				return false;
			}
			try {
				IEntitySchemaQueryFilterItem folderFilters = CommonUtilities.ConvertClientFilterDataToEsqFilters(
					_userConnection, filterData, _entitySchemaUId);
				if (folderFilters == null) {
					WriteLog($"Failed to build dynamic folder filter for '{_dynamicFolderId}'.");
					return false;
				}
				esq.Filters.Add(folderFilters);
				return true;
			} catch (Exception e) {
				WriteLog($"Failed to apply dynamic folder filter for '{_dynamicFolderId}'", e);
				return false;
			}
		}

		private byte[] GetDynamicFolderFilterData() {
			EntitySchema sectionSchema = GetEntitySchema();
			if (sectionSchema == null) {
				return null;
			}
			string folderSchemaName = sectionSchema.Name + "Folder";
			var folderSchemaItem = _userConnection.EntitySchemaManager.FindItemByName(folderSchemaName);
			if (folderSchemaItem != null) {
				byte[] entityFolderFilterData = GetEntityFolderSearchData(folderSchemaName);
				if (entityFolderFilterData != null && entityFolderFilterData.Length > 0) {
					return entityFolderFilterData;
				}
			}
			return GetFolderTreeFilterData(sectionSchema.Name);
		}

		private static byte[] GetFilterDataBytes(Entity folderEntity, string filterDataColumnName) {
			byte[] filterData = folderEntity.GetBytesValue(filterDataColumnName);
			if (filterData != null && filterData.Length > 0) {
				return filterData;
			}
			string filterDataText = folderEntity.GetTypedColumnValue<string>(filterDataColumnName);
			return string.IsNullOrWhiteSpace(filterDataText)
				? null
				: Encoding.UTF8.GetBytes(filterDataText);
		}

		private byte[] GetEntityFolderSearchData(string folderSchemaName) {
			try {
				EntitySchema folderSchema = _userConnection.EntitySchemaManager.GetInstanceByName(folderSchemaName);
				string searchDataColumnPath = null;
				if (folderSchema.Columns.FindByName("SearchData") != null) {
					searchDataColumnPath = "SearchData";
				} else if (folderSchema.Columns.FindByName("FilterData") != null) {
					searchDataColumnPath = "FilterData";
				}
				if (string.IsNullOrWhiteSpace(searchDataColumnPath)) {
					return null;
				}
				var esq = new EntitySchemaQuery(folderSchema) {
					PrimaryQueryColumn = {
						IsAlwaysSelect = true
					},
					IgnoreDisplayValues = true,
					UnmaskColumnValues = true,
					RowCount = 1
				};
				string searchDataColumnName = esq.AddColumn(searchDataColumnPath).Name;
				bool hasFolderTypeIdColumn = folderSchema.Columns.FindByName("FolderTypeId") != null;
				string folderTypeIdColumnName = null;
				if (hasFolderTypeIdColumn) {
					folderTypeIdColumnName = esq.AddColumn("FolderTypeId").Name;
					esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "FolderTypeId",
						DynamicFolderTypeId));
				}
				string isDeletedColumnName = null;
				if (folderSchema.Columns.FindByName("IsDeleted") != null) {
					isDeletedColumnName = esq.AddColumn("IsDeleted").Name;
				}
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, IdColumnName,
					_dynamicFolderId));
				Entity folderEntity = esq.GetEntityCollection(_userConnection).FirstOrDefault();
				if (folderEntity == null) {
					return null;
				}
				if (hasFolderTypeIdColumn && folderEntity.GetTypedColumnValue<Guid>(folderTypeIdColumnName) != DynamicFolderTypeId) {
					return null;
				}
				if (isDeletedColumnName != null && folderEntity.GetTypedColumnValue<bool>(isDeletedColumnName)) {
					return null;
				}
				return GetFilterDataBytes(folderEntity, searchDataColumnName);
			} catch (Exception e) {
				WriteLog($"Failed to resolve entity folder search data for '{_dynamicFolderId}'", e);
				return null;
			}
		}

		private byte[] GetFolderTreeFilterData(string sectionSchemaName) {
			try {
				EntitySchema folderTreeSchema = _userConnection.EntitySchemaManager.GetInstanceByName("FolderTree");
				var esq = new EntitySchemaQuery(folderTreeSchema) {
					PrimaryQueryColumn = {
						IsAlwaysSelect = true
					},
					IgnoreDisplayValues = true,
					UnmaskColumnValues = true,
					RowCount = 1
				};
				string filterDataColumnName = esq.AddColumn("FilterData").Name;
				string isDeletedColumnName = null;
				if (folderTreeSchema.Columns.FindByName("IsDeleted") != null) {
					isDeletedColumnName = esq.AddColumn("IsDeleted").Name;
				}
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, IdColumnName,
					_dynamicFolderId));
				esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal,
					"EntitySchemaName", sectionSchemaName));
				Entity folderEntity = esq.GetEntityCollection(_userConnection).FirstOrDefault();
				if (folderEntity == null) {
					return null;
				}
				if (isDeletedColumnName != null && folderEntity.GetTypedColumnValue<bool>(isDeletedColumnName)) {
					return null;
				}
				return GetFilterDataBytes(folderEntity, filterDataColumnName);
			} catch (Exception e) {
				WriteLog($"Failed to resolve FolderTree filter data for '{_dynamicFolderId}'", e);
				return null;
			}
		}

		private Func<CancellationToken, Stream> CreateContentStreamFactory(string jsonContent) {
			return cancellationToken => {
				if (cancellationToken.IsCancellationRequested) {
					return Stream.Null;
				}
				try {
					byte[] bytes = Encoding.UTF8.GetBytes(jsonContent);
					var stream = new MemoryStream(bytes);
					return stream;
				} catch (Exception e) {
					WriteLog("Failed to create content stream", e);
					return Stream.Null;
				}
			};
		}

		private KnwItem GetKnwItem(Entity entity) {
			Guid id = entity.PrimaryColumnValue;
			string displayColumnName = GetDisplayColumnName(entity.Schema);
			string displayName = entity.GetTypedColumnValue<string>(displayColumnName);
			displayName = FormatDisplayName(displayName, id);
			DateTime modifiedOn = entity.GetTypedColumnValue<DateTime>(ModifiedOnColumnName);
			if (modifiedOn == DateTime.MinValue) {
				modifiedOn = DateTime.UtcNow;
			}
			string jsonContent = SerializeEntityToJson(entity);
			int sizeBytes = Encoding.UTF8.GetByteCount(jsonContent);
			return new KnwItem {
				Id = id.ToString("D"),
				KnwSourceId = _knwSourceId,
				ItemType = SectionItemType,
				DisplayName = displayName,
				SizeBytes = sizeBytes,
				ModifiedOn = modifiedOn,
				Hash = string.Empty,
				Metadata = new Dictionary<string, string> {
					{ RecordIdMetadataKey, id.ToString() },
					{ EntitySchemaNameMetadataKey, entity.Schema.Name }
				},
				ContentStreamFactory = CreateContentStreamFactory(jsonContent)
			};
		}

		private string GetUrl(string formattedItemId, EntitySchema schema) {
			string relativeUrl = string.Format(SectionRelativeRecordTemplate,
				formattedItemId,
				schema.Name);
			return new Uri(_baseUriResolver.ResolveBaseUri(), relativeUrl).ToString();
		}

		private int GetBatchSize() {
			const int defaultBatchSize = 250;
			try {
				var batchSizeValue = SysSettings.GetValue(_userConnection, "KnowledgeIndexingBatchSize", defaultBatchSize);
				if (batchSizeValue <= 0) {
					WriteLog($"Invalid batch size value: {batchSizeValue}. Using default: {defaultBatchSize}");
					return defaultBatchSize;
				}
				return batchSizeValue;
			} catch (Exception e) {
				WriteLog($"Failed to retrieve batch size from settings. Using default: {defaultBatchSize}", e);
				return defaultBatchSize;
			}
		}

		private QueryColumnExpression GetSelectIdColumnExpression(Select select) {
			QueryColumnExpression idExpression = select.Columns.FindByAlias(IdColumnName);
			if (idExpression == null) {
				string message = $"Select expression doesn't contain {IdColumnName} column.";
				throw new InvalidOperationException(message);
			}
			idExpression.Alias = IdColumnName;
			return idExpression;
		}

		private Guid? TryGetGuidFromDataRow(DataRow row, string columnName) {
			try {
				if (row.Table.Columns.Contains(columnName)) {
					object value = row[columnName];
					if (value != null && value != DBNull.Value && Guid.TryParse(value.ToString(), out Guid guid)) {
						return guid;
					}
				}
			} catch {
				WriteLog($"Failed to parse Guid from column '{columnName}' in DataRow");
			}
			return null;
		}

		private string SerializeDataRowToJson(DataRow row) {
			if (row == null) {
				return "{}";
			}
			var jsonDict = new Dictionary<string, object>();
			foreach (DataColumn column in row.Table.Columns) {
				try {
					object value = row[column];
					if (value != null && value != DBNull.Value) {
						if (value is Guid guid) {
							jsonDict[column.ColumnName] = guid.ToString();
						} else if (value is DateTime dateTime) {
							jsonDict[column.ColumnName] = dateTime.ToString("o");
						} else {
							jsonDict[column.ColumnName] = value;
						}
					}
				} catch (Exception e) {
					WriteLog($"Failed to serialize column '{column.ColumnName}'", e);
				}
			}
			return JsonConvert.SerializeObject(jsonDict);
		}

		private KnwItem BuildKnwItemFromDataRow(DataRow row, EntitySchema schema) {
			if (row == null) {
				throw new ArgumentNullException(nameof(row));
			}
			object idValue = row[IdColumnName];
			if (idValue == null || idValue == DBNull.Value) {
				throw new InvalidOperationException("Record Id is missing");
			}
			Guid id = new Guid(idValue.ToString());
			string displayColumnName = GetDisplayColumnName(schema);
			string displayName = string.Empty;
			if (row.Table.Columns.Contains(displayColumnName)) {
				object displayValue = row[displayColumnName];
				if (displayValue != null && displayValue != DBNull.Value) {
					displayName = displayValue.ToString();
				}
			}
			displayName = FormatDisplayName(displayName, id);
			DateTime modifiedOn = DateTime.UtcNow;
			if (row.Table.Columns.Contains(ModifiedOnColumnName)) {
				object modifiedValue = row[ModifiedOnColumnName];
				if (modifiedValue != null && modifiedValue != DBNull.Value && modifiedValue is DateTime dt) {
					modifiedOn = dt;
				}
			}
			string jsonContent = SerializeDataRowToJson(row);
			int sizeBytes = Encoding.UTF8.GetByteCount(jsonContent);
			return new KnwItem {
				Id = id.ToString("D"),
				KnwSourceId = _knwSourceId,
				ItemType = SectionItemType,
				DisplayName = displayName,
				SizeBytes = sizeBytes,
				ModifiedOn = modifiedOn,
				Hash = string.Empty,
				Metadata = new Dictionary<string, string> {
					{ RecordIdMetadataKey, id.ToString() },
					{ EntitySchemaNameMetadataKey, schema.Name }
				},
				ContentStreamFactory = CreateContentStreamFactory(jsonContent)
			};
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public IEnumerable<KnwItem> Discover(KnwProviderDiscoveryOptions options,
				CancellationToken ct = default) {
			if (ct.IsCancellationRequested || _userConnection == null || _knwSourceId.IsEmpty()) {
				return Enumerable.Empty<KnwItem>();
			}
			var discoveryOptions = options ?? new KnwProviderDiscoveryOptions();
			List<string> normalizedItemIds = NormalizeItemIds(discoveryOptions.ItemIds);
			if (discoveryOptions.ItemIds != null && normalizedItemIds.Count == 0) {
				return Enumerable.Empty<KnwItem>();
			}
			if (_entitySchemaUId.IsEmpty()) {
				WriteLog("RootSchemaUId is not configured", new InvalidOperationException("RootSchemaUId is required"));
				return Enumerable.Empty<KnwItem>();
			}
			EntitySchemaQuery esq = GetEsq(discoveryOptions.ModifiedSince, normalizedItemIds);
			if (esq == null) {
				return Enumerable.Empty<KnwItem>();
			}
			EntityCollection entities;
			try {
				entities = esq.GetEntityCollection(_userConnection);
			} catch (Exception e) {
				WriteLog($"Failed to execute ESQ for entity schema '{_entitySchemaUId}'", e);
				entities = new EntityCollection(_userConnection, _entitySchemaUId);
			}
			WriteLog($"Retrieved {entities.Count} entities from {_entitySchemaUId} schema");
			var result = new List<KnwItem>(entities.Count);
			foreach (Entity entity in entities) {
				if (ct.IsCancellationRequested) {
					break;
				}
				try {
					KnwItem item = GetKnwItem(entity);
					result.Add(item);
				} catch (Exception e) {
					WriteLog($"Failed to create KnwItem for entity", e, entity.PrimaryColumnValue);
				}
			}
			return result;
		}

		/// <inheritdoc />
		public IEnumerable<IReadOnlyList<KnwItem>> DiscoverInBatches(KnwProviderDiscoveryOptions options, CancellationToken ct = default) {
			if (ct.IsCancellationRequested || _userConnection == null || _knwSourceId.IsEmpty()) {
				yield break;
			}

			var discoveryOptions = options ?? new KnwProviderDiscoveryOptions();
			List<string> normalizedItemIds = NormalizeItemIds(discoveryOptions.ItemIds);
			if (discoveryOptions.ItemIds != null && normalizedItemIds.Count == 0) {
				yield break;
			}

			EntitySchemaQuery baseEsq = GetEsq(discoveryOptions.ModifiedSince, normalizedItemIds);
			if (baseEsq == null) {
				yield break;
			}

			EntitySchema schema = GetEntitySchema();
			if (schema == null) {
				yield break;
			}

			int batchSize = GetBatchSize();
			int batchNumber = 0;
			int totalItems = 0;

			WriteLog($"Starting batch discovery with batchSize={batchSize}");

			Select select = baseEsq.GetSelectQuery(_userConnection);

			var readOptions = new PageableSelectReadOptions_Temp {
				BaseSelect = select,
				PageSize = batchSize,
				IdColumnAlias = IdColumnName,
				GetIdExpression = () => GetSelectIdColumnExpression(select),
				ConfigureNextOptions = null,
				GetLoopGuardKey = row => row[IdColumnName]?.ToString(),
				QueryKind = QueryKind.Limited,
				CancellationToken = ct
			};

			foreach (DataTable dataTable in _pageableSelectHelper.ReadPages(readOptions)) {

				if (ct.IsCancellationRequested) {
					WriteLog($"Batch discovery cancelled. Total batches={batchNumber}, Total items={totalItems}");
					yield break;
				}

				batchNumber++;
				int recordsCount = dataTable.Rows.Count;

				WriteLog($"Fetching batch {batchNumber}");

				var batch = new List<KnwItem>(recordsCount);
				foreach (DataRow row in dataTable.Rows) {
					if (ct.IsCancellationRequested) {
						WriteLog($"Batch discovery cancelled. Total batches={batchNumber}, Total items={totalItems}");
						yield break;
					}

					try {
						KnwItem item = BuildKnwItemFromDataRow(row, schema);
						batch.Add(item);
					} catch (Exception e) {
						Guid? recordId = TryGetGuidFromDataRow(row, IdColumnName);
						WriteLog("Failed to create KnwItem for record", e, recordId);
					}
				}

				totalItems += batch.Count;
				WriteLog($"Batch {batchNumber} complete: fetched={recordsCount} items");

				if (batch.Count > 0) {
					yield return batch.AsReadOnly();
				}
			}

			WriteLog($"Batch discovery completed. Total batches={batchNumber}, Total items={totalItems}");
		}

		/// <inheritdoc />
		public void Initialize(KnwProviderInitializationOptions initializationOptions) {
			_userConnection = initializationOptions.UserConnection;
			_relatedColumnsResolverService = ClassFactory.Get<IKnwRelatedColumnsResolverService>(
				new ConstructorArgument("userConnection", _userConnection));
			_knwSourceId = initializationOptions.KnwSource?.Id ?? Guid.Empty;
			_scopeType = string.Empty;
			_dynamicFolderId = Guid.Empty;
			string knwSourceConfig = initializationOptions.KnwSource?.KnwSourceConfig;
			if (!knwSourceConfig.IsNullOrWhiteSpace()) {
				try {
					var config = JsonConvert.DeserializeObject<SectionProviderConfig>(knwSourceConfig);
					_entitySchemaUId = config?.RootSchemaUId ?? Guid.Empty;
					EntitySchema rootSchema = _userConnection?.EntitySchemaManager?.GetInstanceByUId(_entitySchemaUId);

					// Resolve storage references to runtime DTOs
					var references = config?.RelatedColumns ?? Enumerable.Empty<RelatedColumnReference>();
					var totalReferences = references.Count();
					var resolvedColumns = references
						.Select(reference => _relatedColumnsResolverService.Resolve(reference))
						.Where(dto => dto != null)
						.ToList();

					int skippedCount = totalReferences - resolvedColumns.Count;
					if (skippedCount > 0) {
						WriteLog($"Skipped {skippedCount} related column(s) due to invalid schema/column references (schemas may have been deleted)");
					}

					WriteLog($"Successfully resolved {resolvedColumns.Count} related column(s) from storage");

					_columnSpecs = BuildEffectiveColumnSpecs(rootSchema, config?.Columns, resolvedColumns);
					_filterJson = config?.Filters;
					_scopeType = config?.ScopeType ?? string.Empty;
					_dynamicFolderId = config?.DynamicFolderId ?? Guid.Empty;
					WriteLog($"Parsed config: Schema={_entitySchemaUId}, ScopeType={_scopeType}, DynamicFolderId={_dynamicFolderId}");
				} catch (Exception e) {
					WriteLog("Failed to parse KnwSourceConfig JSON", e);
					_entitySchemaUId = Guid.Empty;
					_columnSpecs = new List<string>();
					_filterJson = null;
					_scopeType = string.Empty;
					_dynamicFolderId = Guid.Empty;
				}
			} else {
				_entitySchemaUId = Guid.Empty;
				_columnSpecs = new List<string>();
				_filterJson = null;
				_scopeType = string.Empty;
				_dynamicFolderId = Guid.Empty;
			}
		}

		/// <inheritdoc />
		public Dictionary<string, KnwItemCitation> GetItemsCitations(ISet<string> itemIds) {
			var result = new Dictionary<string, KnwItemCitation>();
			if (itemIds == null || _userConnection == null || _knwSourceId.IsEmpty() || _entitySchemaUId.IsEmpty()) {
				return result;
			}
			var itemIdList = itemIds.ToList();
			if (itemIdList.Count == 0) {
				return result;
			}
			var guidList = new List<string>();
			foreach (string itemId in itemIdList) {
				if (Guid.TryParse(itemId, out Guid guid) && guid != Guid.Empty) {
					guidList.Add(guid.ToString());
				}
			}
			if (guidList.Count == 0) {
				return result;
			}
			EntitySchema schema = GetEntitySchema();
			if (schema == null) {
				return result;
			}
			try {
				var esq = new EntitySchemaQuery(schema) {
					PrimaryQueryColumn = { IsAlwaysSelect = true }
				};
				string displayColumnName = GetDisplayColumnName(schema);
				EnsureDisplayColumnInQuery(esq, schema);
				IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(
					FilterComparisonType.Equal, IdColumnName, guidList);
				esq.Filters.Add(filter);
				EntityCollection entities = esq.GetEntityCollection(_userConnection);
				foreach (Entity entity in entities) {
					Guid id = entity.PrimaryColumnValue;
					string name = entity.GetTypedColumnValue<string>(displayColumnName);
					if (name.IsNullOrWhiteSpace()) {
						name = id.ToString("N");
					}
					string formattedItemId = id.ToString();
					result[formattedItemId] = new KnwItemCitation {
						ItemId = formattedItemId,
						SourceLocation = GetUrl(formattedItemId, schema),
						SourceDisplayName = name
					};
				}
			} catch (Exception e) {
				WriteLog("Failed to retrieve citations for section items", e);
			}
			return result;
		}

		/// <inheritdoc />
		public ISet<string> GetUserAllowedItems(ISet<string> itemIds) {
			var allowedItems = new HashSet<string>();
			if (_userConnection == null) {
				throw new InvalidOperationException("UserConnection is not initialized");
			}
			if (_entitySchemaUId.IsEmpty()) {
				WriteLog("RootSchemaUId is not configured", new InvalidOperationException("RootSchemaUId is required"));
				return allowedItems;
			}
			if (itemIds.IsNullOrEmpty()) {
				return allowedItems;
			}
			EntitySchema schema = GetEntitySchema();
			if (schema == null) {
				return allowedItems;
			}
			foreach (string itemId in itemIds) {
				if (!Guid.TryParse(itemId, out Guid recordId) || recordId == Guid.Empty) {
					continue;
				}
				try {
					SchemaRecordRightLevels rightLevel = _userConnection.DBSecurityEngine
						.GetEntitySchemaRecordRightLevel(schema.Name, recordId);
					if (rightLevel.HasFlag(SchemaRecordRightLevels.CanRead)) {
						allowedItems.Add(itemId);
					}
				} catch (Exception e) {
					WriteLog($"Failed to check access rights for item '{itemId}'", e, recordId);
				}
			}
			return allowedItems;
		}

		#endregion

	}

	#endregion

}

