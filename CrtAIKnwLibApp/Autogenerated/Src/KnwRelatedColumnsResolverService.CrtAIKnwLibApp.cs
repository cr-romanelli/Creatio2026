namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Class: ResolveRelatedColumnsRequest

	[DataContract]
	public class ResolveRelatedColumnsRequest
	{

		#region Properties: Public

		[DataMember(Name = "references")]
		public List<RelatedColumnReference> References { get; set; }

		#endregion

	}

	#endregion

	#region Class: ResolveRelatedColumnsResponse

	[DataContract]
	public class ResolveRelatedColumnsResponse
	{

		#region Properties: Public

		[DataMember(Name = "relatedColumns")]
		public List<SectionRelatedColumnConfigDto> RelatedColumns { get; set; }

		#endregion

	}

	#endregion

	#region Class: SectionRelatedColumnConfigDto

	[DataContract]
	public class SectionRelatedColumnConfigDto
	{

		#region Properties: Public

		[DataMember(Name = "pathKey")]
		public string PathKey { get; set; }

		[DataMember(Name = "pathSegments")]
		public List<SectionLinkedItemSegmentDto> PathSegments { get; set; }

		[DataMember(Name = "terminalSchemaUId")]
		public Guid TerminalSchemaUId { get; set; }

		[DataMember(Name = "terminalSchemaName")]
		public string TerminalSchemaName { get; set; }

		[DataMember(Name = "terminalColumnUId")]
		public Guid TerminalColumnUId { get; set; }

		[DataMember(Name = "terminalColumnName")]
		public string TerminalColumnName { get; set; }

		#endregion

	}

	#endregion

	#region Class: PathSegmentReference

	[DataContract]
	public class PathSegmentReference
	{

		#region Properties: Public

		[DataMember(Name = "relationType")]
		public string RelationType { get; set; }

		[DataMember(Name = "sourceSchemaUId")]
		public Guid SourceSchemaUId { get; set; }

		[DataMember(Name = "targetSchemaUId")]
		public Guid TargetSchemaUId { get; set; }

		[DataMember(Name = "columnUId")]
		public Guid ColumnUId { get; set; }

		#endregion

	}

	#endregion

	#region Class: RelatedColumnReference

	[DataContract]
	public class RelatedColumnReference
	{

		#region Properties: Public

		[DataMember(Name = "pathSegments")]
		public List<PathSegmentReference> PathSegments { get; set; }

		[DataMember(Name = "terminalSchemaUId")]
		public Guid TerminalSchemaUId { get; set; }

		[DataMember(Name = "terminalColumnUId")]
		public Guid TerminalColumnUId { get; set; }

		#endregion

	}

	#endregion

	#region Interface: IKnwRelatedColumnsResolverService

	internal interface IKnwRelatedColumnsResolverService
	{

		#region Methods: Public

		SectionRelatedColumnConfigDto Resolve(RelatedColumnReference reference);

		IEnumerable<SectionRelatedColumnConfigDto> ResolveMany(IEnumerable<RelatedColumnReference> references);

		#endregion

	}

	#endregion

	#region Class: KnwRelatedColumnsResolverService

	[DefaultBinding(typeof(IKnwRelatedColumnsResolverService))]
	internal class KnwRelatedColumnsResolverService : IKnwRelatedColumnsResolverService
	{

		#region Fields: Private

		private readonly EntitySchemaManager _schemaManager;

		#endregion

		#region Constructors: Public

		public KnwRelatedColumnsResolverService(UserConnection userConnection) {
			if (userConnection == null) {
				throw new ArgumentNullException(nameof(userConnection));
			}
			_schemaManager = userConnection.EntitySchemaManager;
		}

		#endregion

		#region Methods: Private

		private static string BuildPathKey(List<SectionLinkedItemSegmentDto> segments) {
			IEnumerable<string> keys = (segments ?? new List<SectionLinkedItemSegmentDto>())
				.Where(segment => segment != null)
				.Select(segment => {
					string relationType = segment.RelationType?.ToLowerInvariant() ?? "unknown";
					return $"{relationType}:{segment.SourceSchemaUId}:{segment.ColumnUId}:{segment.TargetSchemaUId}";
				});
			return string.Join("/", keys);
		}

		private SectionLinkedItemSegmentDto ResolveSegment(PathSegmentReference segmentRef) {
			if (segmentRef == null || string.IsNullOrWhiteSpace(segmentRef.RelationType)) {
				return null;
			}

			EntitySchema sourceSchema = _schemaManager.FindInstanceByUId(segmentRef.SourceSchemaUId);
			EntitySchema targetSchema = _schemaManager.FindInstanceByUId(segmentRef.TargetSchemaUId);
			if (sourceSchema == null || targetSchema == null) {
				return null;
			}

			bool isChildRelation = string.Equals(segmentRef.RelationType, "Child", StringComparison.OrdinalIgnoreCase);
			bool isLookupRelation = string.Equals(segmentRef.RelationType, "Lookup", StringComparison.OrdinalIgnoreCase);
			if (!isChildRelation && !isLookupRelation) {
				return null;
			}

			EntitySchemaColumn column = isChildRelation
				? targetSchema.Columns.FindByUId(segmentRef.ColumnUId)
				: sourceSchema.Columns.FindByUId(segmentRef.ColumnUId);
			if (column == null) {
				return null;
			}

			string caption = isChildRelation
				? (targetSchema.Caption?.Value ?? targetSchema.Name)
				: (column.Caption?.Value ?? column.Name);

			return new SectionLinkedItemSegmentDto {
				RelationType = isChildRelation ? "Child" : "Lookup",
				SourceSchemaUId = segmentRef.SourceSchemaUId,
				SourceSchemaName = sourceSchema.Name,
				TargetSchemaUId = segmentRef.TargetSchemaUId,
				TargetSchemaName = targetSchema.Name,
				ColumnUId = segmentRef.ColumnUId,
				ColumnName = column.Name,
				Caption = caption
			};
		}

		#endregion

		#region Methods: Public

		public SectionRelatedColumnConfigDto Resolve(RelatedColumnReference reference) {
			if (reference?.PathSegments == null) {
				return null;
			}

			var resolvedSegments = new List<SectionLinkedItemSegmentDto>();
			foreach (PathSegmentReference segmentRef in reference.PathSegments) {
				SectionLinkedItemSegmentDto resolved = ResolveSegment(segmentRef);
				if (resolved == null) {
					return null;
				}
				resolvedSegments.Add(resolved);
			}

			EntitySchema terminalSchema = _schemaManager.FindInstanceByUId(reference.TerminalSchemaUId);
			if (terminalSchema == null) {
				return null;
			}
			EntitySchemaColumn terminalColumn = terminalSchema.Columns.FindByUId(reference.TerminalColumnUId);
			if (terminalColumn == null) {
				return null;
			}

			return new SectionRelatedColumnConfigDto {
				PathKey = BuildPathKey(resolvedSegments),
				PathSegments = resolvedSegments,
				TerminalSchemaUId = reference.TerminalSchemaUId,
				TerminalSchemaName = terminalSchema.Caption?.Value ?? terminalSchema.Name,
				TerminalColumnUId = reference.TerminalColumnUId,
				TerminalColumnName = terminalColumn.Caption?.Value ?? terminalColumn.Name
			};
		}

		public IEnumerable<SectionRelatedColumnConfigDto> ResolveMany(IEnumerable<RelatedColumnReference> references) {
			return references?
				.Select(reference => {
					try {
						return Resolve(reference);
					} catch (Exception ex) {
						KnwLibUtils.Logger.Warn(
							$"[{nameof(KnwRelatedColumnsResolverService)}] Failed to resolve related column reference. " +
							$"Skipping malformed item.", ex);
						return null;
					}
				})
				.Where(item => item != null)
				.ToList()
				?? Enumerable.Empty<SectionRelatedColumnConfigDto>();
		}

		#endregion

	}

	#endregion

}

