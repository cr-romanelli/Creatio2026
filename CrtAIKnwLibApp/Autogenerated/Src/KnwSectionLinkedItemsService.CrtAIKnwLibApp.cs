namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Class: SelectorLinkedItemsPageResult

	internal sealed class SelectorLinkedItemsPageResult
	{

		#region Properties: Public

		public IEnumerable<SectionLinkedItemDto> Items { get; set; } = Enumerable.Empty<SectionLinkedItemDto>();

		public bool HasMore { get; set; }

		public string LastCursorId { get; set; }

		#endregion

	}

	#endregion

	#region Interface: IKnwSectionLinkedItemsService

	internal interface IKnwSectionLinkedItemsService
	{

		#region Methods: Public

		SelectorLinkedItemsPageResult GetSelectorLinkedItemsPage(EntitySchema rootSchema, string parentItemKey,
			string searchText, string cursorId, int pageSize);

		EntitySchema ResolveCurrentSchema(EntitySchema rootSchema, string parentItemKey);

		#endregion

	}

	#endregion

	#region Class: KnwSectionLinkedItemsService

	[DefaultBinding(typeof(IKnwSectionLinkedItemsService))]
	internal class KnwSectionLinkedItemsService : IKnwSectionLinkedItemsService
	{

		#region Class: LinkedItemContext

		private sealed class LinkedItemContext
		{

			#region Properties: Public

			public EntitySchema CurrentSchema { get; set; }

			public List<SectionLinkedItemSegmentDto> Segments { get; set; } =
				new List<SectionLinkedItemSegmentDto>();

			public HashSet<Guid> VisitedSchemaUIds { get; set; } =
				new HashSet<Guid>();

			#endregion

		}

		#endregion

		#region Constants: Private

		private const string SectionTraversalDepthSettingCode = "KnwSrcMaxRelationHopDepth";
		private const int DefaultSectionTraversalDepth = 2;

		#endregion

		#region Fields: Private

		private readonly UserConnection _userConnection;
		private readonly EntitySchemaManager _entitySchemaManager;

		#endregion

		#region Constructors: Public

		public KnwSectionLinkedItemsService(UserConnection userConnection) {
			_userConnection = userConnection ?? throw new ArgumentNullException(nameof(userConnection));
			_entitySchemaManager = userConnection.EntitySchemaManager;
		}

		#endregion

		#region Methods: Private

		private static string GetSchemaCaption(EntitySchema schema) {
			return schema?.Caption?.Value.IsNotNullOrWhiteSpace() == true
				? schema.Caption.Value
				: schema?.Name ?? string.Empty;
		}

		private static bool IsSelectableReferenceColumn(EntitySchemaColumn column) {
			return column != null
				&& !column.IsVirtual
				&& column.UsageType != EntitySchemaColumnUsageType.None
				&& column.ReferenceSchemaUId != Guid.Empty
				&& column.IsLookupType;
		}

		private static string GetLinkedItemCaption(SectionLinkedItemSegmentDto segment) {
			return segment?.Caption.IsNotNullOrWhiteSpace() == true
				? segment.Caption
				: segment?.TargetSchemaName ?? string.Empty;
		}

		private static string BuildLinkedItemKey(IEnumerable<SectionLinkedItemSegmentDto> segments) {
			if (segments == null || !segments.Any()) {
				return string.Empty;
			}
			var segmentKeys = segments.Select(segment => {
				string relationType = segment.RelationType?.ToLowerInvariant() ?? "unknown";
				return $"{relationType}:{segment.SourceSchemaUId}:{segment.ColumnUId}:{segment.TargetSchemaUId}";
			});
			return string.Join("/", segmentKeys);
		}

		private static string GetReferenceCursorId(SectionLinkedItemDto item) {
			return item?.Key;
		}

		private static int GetLinkedItemDepth(LinkedItemContext context) {
			return context?.Segments?.Count ?? 0;
		}

		private IEnumerable<EntitySchemaColumn> GetSelectableSectionColumns(EntitySchema schema) {
			if (schema == null) {
				return Enumerable.Empty<EntitySchemaColumn>();
			}
			return schema.Columns
				.Where(column => !column.IsVirtual && column.UsageType != EntitySchemaColumnUsageType.None);
		}

		private IEnumerable<SectionLinkedItemSegmentDto> GetLookupLinkedItemSegments(EntitySchema schema) {
			if (schema == null) {
				return Enumerable.Empty<SectionLinkedItemSegmentDto>();
			}

			return GetSelectableSectionColumns(schema)
				.Where(IsSelectableReferenceColumn)
				.Select(column => CreateLookupSegment(schema, column))
				.Where(segment => segment != null);
		}

		private SectionLinkedItemSegmentDto CreateLookupSegment(EntitySchema sourceSchema, EntitySchemaColumn column) {
			EntitySchema targetSchema = _entitySchemaManager.FindInstanceByUId(column.ReferenceSchemaUId);
			if (targetSchema == null) {
				return null;
			}

			string caption = column.Caption?.Value;
			if (string.IsNullOrWhiteSpace(caption)) {
				caption = GetSchemaCaption(targetSchema);
			}

			return new SectionLinkedItemSegmentDto {
				RelationType = "Lookup",
				Caption = caption,
				SourceSchemaUId = sourceSchema.UId,
				SourceSchemaName = sourceSchema.Name,
				TargetSchemaUId = targetSchema.UId,
				TargetSchemaName = targetSchema.Name,
				ColumnUId = column.UId,
				ColumnName = column.Name
			};
		}

		private IEnumerable<SectionLinkedItemSegmentDto> GetChildLinkedItemSegments(EntitySchema schema) {
			if (schema == null) {
				yield break;
			}

			var dedup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var usageTypes = new[] {
				EntitySchemaColumnUsageType.General,
				EntitySchemaColumnUsageType.Advanced
			};

			foreach (var usageType in usageTypes) {
				var references = GetSchemaOppositeReferences(schema.UId, usageType);
				foreach (EntitySchemaOppositeReferenceInfo reference in references) {
					SectionLinkedItemSegmentDto segment = CreateChildSegment(schema, reference);
					if (segment == null) {
						continue;
					}
					string key = BuildLinkedItemKey(new[] { segment });
					if (dedup.Add(key)) {
						yield return segment;
					}
				}
			}
		}

		private SectionLinkedItemSegmentDto CreateChildSegment(EntitySchema sourceSchema,
				EntitySchemaOppositeReferenceInfo reference) {
			EntitySchema targetSchema = _entitySchemaManager.FindInstanceByUId(reference.SchemaUId);
			if (targetSchema == null) {
				return null;
			}

			EntitySchemaColumn targetColumn = targetSchema.Columns.FindByUId(reference.ColumnUId);
			if (!IsSelectableReferenceColumn(targetColumn)) {
				return null;
			}

			return new SectionLinkedItemSegmentDto {
				RelationType = "Child",
				Caption = GetSchemaCaption(targetSchema),
				SourceSchemaUId = sourceSchema.UId,
				SourceSchemaName = sourceSchema.Name,
				TargetSchemaUId = targetSchema.UId,
				TargetSchemaName = targetSchema.Name,
				ColumnUId = targetColumn.UId,
				ColumnName = targetColumn.Name
			};
		}

		private IEnumerable<EntitySchemaOppositeReferenceInfo> GetSchemaOppositeReferences(
				Guid schemaUId, EntitySchemaColumnUsageType usageType) {
			EntitySchema schema = _entitySchemaManager?.FindInstanceByUId(schemaUId);
			if (schema == null) {
				return Enumerable.Empty<EntitySchemaOppositeReferenceInfo>();
			}

			try {
				return _entitySchemaManager.GetSchemaOppositeReferences(schemaUId, usageType, _userConnection);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error(
					$"[{nameof(KnwSectionLinkedItemsService)}] Failed to load opposite references for schema '{schema.Name}' " +
					$"and usage type '{usageType}'.", e);
				return Enumerable.Empty<EntitySchemaOppositeReferenceInfo>();
			}
		}

		private IEnumerable<SectionLinkedItemSegmentDto> EnumerateNextLinkedItemSegments(EntitySchema schema) {
			if (schema == null) {
				yield break;
			}

			var dedup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (SectionLinkedItemSegmentDto lookupSegment in GetLookupLinkedItemSegments(schema)) {
				string key = BuildLinkedItemKey(new[] { lookupSegment });
				if (dedup.Add(key)) {
					yield return lookupSegment;
				}
			}
			foreach (SectionLinkedItemSegmentDto childSegment in GetChildLinkedItemSegments(schema)) {
				string key = BuildLinkedItemKey(new[] { childSegment });
				if (dedup.Add(key)) {
					yield return childSegment;
				}
			}
		}

		private SectionLinkedItemSegmentDto FindNextSegmentByKey(EntitySchema schema, string segmentKey) {
			return EnumerateNextLinkedItemSegments(schema)
				.FirstOrDefault(segment => string.Equals(
					BuildLinkedItemKey(new[] { segment }),
					segmentKey,
					StringComparison.OrdinalIgnoreCase));
		}

		private static bool MatchesSearch(string searchLower, SectionLinkedItemDto item) {
			if (string.IsNullOrWhiteSpace(searchLower)) {
				return true;
			}
			return item?.Caption != null && item.Caption.ToLowerInvariant().Contains(searchLower);
		}

		private SectionLinkedItemDto CreateLinkedItemDto(LinkedItemContext context, SectionLinkedItemSegmentDto segment) {
			IEnumerable<SectionLinkedItemSegmentDto> pathSegments = context.Segments.Concat(new[] { segment });
			return new SectionLinkedItemDto {
				Key = BuildLinkedItemKey(pathSegments),
				Caption = GetLinkedItemCaption(segment),
				TerminalSchemaUId = segment.TargetSchemaUId,
				TerminalSchemaName = segment.TargetSchemaName,
				Segments = pathSegments.ToList()
			};
		}

		private LinkedItemContext ResolveLinkedItemContext(EntitySchema rootSchema, string parentItemKey,
				int traversalDepth) {
			if (rootSchema == null || traversalDepth <= 0) {
				return null;
			}

			var context = new LinkedItemContext {
				CurrentSchema = rootSchema,
				VisitedSchemaUIds = new HashSet<Guid> { rootSchema.UId }
			};

			if (string.IsNullOrWhiteSpace(parentItemKey)) {
				return context;
			}

			string[] segmentKeys = parentItemKey.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (segmentKeys.Length == 0 || segmentKeys.Length >= traversalDepth + 1) {
				return null;
			}

			foreach (string segmentKey in segmentKeys) {
				SectionLinkedItemSegmentDto resolvedSegment = FindNextSegmentByKey(context.CurrentSchema, segmentKey);

				if (resolvedSegment == null || context.VisitedSchemaUIds.Contains(resolvedSegment.TargetSchemaUId)) {
					return null;
				}

				EntitySchema targetSchema = _entitySchemaManager.FindInstanceByUId(resolvedSegment.TargetSchemaUId);
				if (targetSchema == null) {
					return null;
				}

				context.Segments.Add(resolvedSegment);
				context.VisitedSchemaUIds.Add(resolvedSegment.TargetSchemaUId);
				context.CurrentSchema = targetSchema;
			}

			return context;
		}

		private int GetSectionTraversalDepth() {
			try {
				int depth = SysSettings.GetValue(_userConnection, SectionTraversalDepthSettingCode,
					DefaultSectionTraversalDepth);
				return depth > 0 ? depth : DefaultSectionTraversalDepth;
			} catch (Exception ex) {
				KnwLibUtils.Logger.Warn(
					$"[{nameof(KnwSectionLinkedItemsService)}] Failed to read '{SectionTraversalDepthSettingCode}' system setting. " +
					$"Using default value: {DefaultSectionTraversalDepth}.", ex);
				return DefaultSectionTraversalDepth;
			}
		}

		#endregion

		#region Methods: Public

		public EntitySchema ResolveCurrentSchema(EntitySchema rootSchema, string parentItemKey) {
			if (rootSchema == null) {
				return null;
			}

			int traversalDepth = GetSectionTraversalDepth();
			return ResolveLinkedItemContext(rootSchema, parentItemKey, traversalDepth)?.CurrentSchema;
		}

		public SelectorLinkedItemsPageResult GetSelectorLinkedItemsPage(EntitySchema rootSchema, string parentItemKey,
				string searchText, string cursorId, int pageSize) {
			if (rootSchema == null || pageSize <= 0) {
				return new SelectorLinkedItemsPageResult();
			}

			int traversalDepth = GetSectionTraversalDepth();
			LinkedItemContext context = ResolveLinkedItemContext(rootSchema, parentItemKey, traversalDepth);
			if (context == null) {
				return new SelectorLinkedItemsPageResult();
			}

			int parentDepth = GetLinkedItemDepth(context);
			int remainingDepth = traversalDepth - parentDepth;
			if (remainingDepth <= 0) {
				return new SelectorLinkedItemsPageResult();
			}

			string searchLower = searchText?.Trim().ToLowerInvariant();
			bool hasCursor = !string.IsNullOrWhiteSpace(cursorId);
			string cursorValue = cursorId?.Trim();
			bool cursorMatched = !hasCursor;

			var pageItems = new List<SectionLinkedItemDto>(pageSize + 1);
			var firstPageCandidates = hasCursor ? new List<SectionLinkedItemDto>(pageSize + 1) : null;

			foreach (SectionLinkedItemSegmentDto segment in EnumerateNextLinkedItemSegments(context.CurrentSchema)) {
				if (segment == null || context.VisitedSchemaUIds.Contains(segment.TargetSchemaUId)) {
					continue;
				}

				SectionLinkedItemDto item = CreateLinkedItemDto(context, segment);
				if (!MatchesSearch(searchLower, item)) {
					continue;
				}

				string itemCursorId = GetReferenceCursorId(item);
				if (!cursorMatched) {
					if (string.Equals(itemCursorId, cursorValue, StringComparison.OrdinalIgnoreCase)) {
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
				pageItems = firstPageCandidates ?? new List<SectionLinkedItemDto>();
			}

			bool hasMore = pageItems.Count > pageSize;
			if (hasMore) {
				pageItems.RemoveAt(pageItems.Count - 1);
			}
			string lastCursor = pageItems.Any() ? GetReferenceCursorId(pageItems.Last()) : null;
			return new SelectorLinkedItemsPageResult {
				Items = pageItems,
				HasMore = hasMore,
				LastCursorId = lastCursor
			};
		}

		#endregion

	}

	#endregion

}

