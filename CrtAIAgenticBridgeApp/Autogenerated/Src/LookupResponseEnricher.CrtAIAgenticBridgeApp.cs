namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Logging;
	using Creatio.FeatureToggling;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;

	#region Class: EnableAgenticLookupDisplayValueEnrichment

	/// <summary>
	/// Toggles lookup display-value resolution. Enabled by default, so display values are resolved out
	/// of the box; disabling this feature skips the (batched) entity reads and emits the lookup with its
	/// identifier only. It does NOT change the structural shape — a lookup is always wrapped as
	/// <c>{ value, displayValue? }</c>, never a bare identifier — so the structured output never drifts
	/// between the two states. Read by both the bridge (value resolution) and the MCP publishing app
	/// (advertised output schema) so the two never disagree on shape.
	/// </summary>
	/// <inheritdoc cref="FeatureMetadata"/>
	public class EnableAgenticLookupDisplayValueEnrichment : FeatureMetadata
	{

		#region Constructors: Public

		public EnableAgenticLookupDisplayValueEnrichment() {
			IsEnabled = true;
		}

		#endregion

	}

	#endregion

	#region Class: LookupResponseEnricher

	/// <inheritdoc cref="ILookupResponseEnricher"/>
	[DefaultBinding(typeof(ILookupResponseEnricher))]
	internal class LookupResponseEnricher : ILookupResponseEnricher
	{

		#region Constants: Private

		// EntitySchemaQuery IN-clause batching, mirrors CopilotParametrizedActionResponseProvider.
		private const int EntityRecordsChunkSize = 900;
		private const string ValueKey = "value";
		private const string DisplayValueKey = "displayValue";

		#endregion

		#region Class: PendingLookup

		// One emitted { value, displayValue } box awaiting its display value. The box is a reference
		// type, so it is filled in place after the batched read resolves the display column.
		private sealed class PendingLookup
		{
			public Guid EntitySchemaUId { get; set; }
			public Guid RecordId { get; set; }
			public Dictionary<string, object> Box { get; set; }
		}

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");
		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		public LookupResponseEnricher(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Private

		private static bool IsEnrichmentEnabled() {
			return Features.GetIsEnabled<EnableAgenticLookupDisplayValueEnrichment>();
		}

		private static IReadOnlyDictionary<string, ProcessSchemaParameter> MapParametersByName(
				IEnumerable<ProcessSchemaParameter> parameters) {
			var map = new Dictionary<string, ProcessSchemaParameter>();
			if (parameters == null) {
				return map;
			}
			foreach (ProcessSchemaParameter parameter in parameters) {
				if (parameter.Name.IsNotNullOrWhiteSpace()) {
					map[parameter.Name] = parameter;
				}
			}
			return map;
		}

		private static bool HasItemProperties(ProcessSchemaParameter parameter) {
			return parameter.ItemProperties != null && parameter.ItemProperties.Count > 0;
		}

		private static ProcessSchemaParameter GetFirstItemProperty(ProcessSchemaParameter parameter) {
			if (!HasItemProperties(parameter)) {
				return null;
			}
			foreach (ProcessSchemaParameter itemProperty in parameter.ItemProperties) {
				return itemProperty;
			}
			return null;
		}

		// Dispatches one value by its declared parameter type, collecting lookup boxes to fill later.
		// Mirrors CopilotParametrizedActionResponseProvider.ProcessParameters.
		private object ConvertValue(ProcessSchemaParameter parameter, object value, List<PendingLookup> pending,
				bool resolveDisplayValues) {
			Guid dataValueTypeUId = parameter?.DataValueType?.UId ?? Guid.Empty;
			if (dataValueTypeUId == DataValueType.LookupDataValueTypeUId) {
				return ConvertLookup(parameter, value, pending, resolveDisplayValues);
			}
			if (dataValueTypeUId == DataValueType.CompositeObjectDataValueTypeUId) {
				return ConvertCompositeObject(parameter, value as ICompositeObject, pending, resolveDisplayValues);
			}
			if (dataValueTypeUId == DataValueType.CompositeObjectListDataValueTypeUId) {
				return ConvertCompositeObjectList(parameter, value, pending, resolveDisplayValues);
			}
			if (dataValueTypeUId == DataValueType.ObjectListDataValueTypeUId) {
				return ConvertObjectList(parameter, value, pending, resolveDisplayValues);
			}
			return value;
		}

		// A lookup is ALWAYS wrapped as an object so the structured shape never drifts: when enrichment
		// is enabled the box gains a resolved displayValue ({ value, displayValue }); when disabled it
		// carries the identifier only ({ value }). An empty id collapses value to null.
		private object ConvertLookup(ProcessSchemaParameter parameter, object value, List<PendingLookup> pending,
				bool resolveDisplayValues) {
			var box = new Dictionary<string, object> {
				[ValueKey] = value
			};
			if (!(value is Guid recordId)) {
				return box;
			}
			if (recordId.IsEmpty()) {
				box[ValueKey] = null;
				if (resolveDisplayValues) {
					box[DisplayValueKey] = string.Empty;
				}
				return box;
			}
			if (resolveDisplayValues) {
				box[DisplayValueKey] = string.Empty;
				if (parameter.ReferenceSchemaUId.IsNotEmpty()) {
					pending.Add(new PendingLookup {
						EntitySchemaUId = parameter.ReferenceSchemaUId,
						RecordId = recordId,
						Box = box
					});
				}
			}
			return box;
		}

		private object ConvertCompositeObject(ProcessSchemaParameter parameter, ICompositeObject compositeObject,
				List<PendingLookup> pending, bool resolveDisplayValues) {
			if (compositeObject == null || !HasItemProperties(parameter)) {
				return compositeObject;
			}
			return BuildCompositeObject(parameter, compositeObject, pending, resolveDisplayValues);
		}

		private object ConvertCompositeObjectList(ProcessSchemaParameter parameter, object value,
				List<PendingLookup> pending, bool resolveDisplayValues) {
			if (!(value is ICompositeObjectList<ICompositeObject> compositeObjectList) ||
					!HasItemProperties(parameter)) {
				return value;
			}
			var items = new List<object>();
			foreach (ICompositeObject compositeObject in compositeObjectList) {
				if (compositeObject == null) {
					continue;
				}
				items.Add(BuildCompositeObject(parameter, compositeObject, pending, resolveDisplayValues));
			}
			return items;
		}

		private object ConvertObjectList(ProcessSchemaParameter parameter, object value, List<PendingLookup> pending,
				bool resolveDisplayValues) {
			if (!(value is IObjectList objectList)) {
				return value;
			}
			// A scalar object list has a single declared item property describing every element.
			ProcessSchemaParameter elementParameter = GetFirstItemProperty(parameter);
			var items = new List<object>();
			foreach (object element in objectList) {
				items.Add(ConvertValue(elementParameter, element, pending, resolveDisplayValues));
			}
			return items;
		}

		private Dictionary<string, object> BuildCompositeObject(ProcessSchemaParameter parameter,
				ICompositeObject compositeObject, List<PendingLookup> pending, bool resolveDisplayValues) {
			var result = new Dictionary<string, object>();
			foreach (ProcessSchemaParameter itemProperty in parameter.ItemProperties) {
				if (itemProperty.Name.IsNullOrWhiteSpace()) {
					continue;
				}
				object columnValue = compositeObject.TryGetValue(itemProperty.Name, out object found) ? found : null;
				result[itemProperty.Name] = ConvertValue(itemProperty, columnValue, pending, resolveDisplayValues);
			}
			return result;
		}

		// Batched display-value read, mirrors CopilotParametrizedActionResponseProvider.LoadLookupDisplayValues:
		// one EntitySchemaQuery per referenced entity, record ids chunked to keep the IN clause bounded.
		private void ResolveDisplayValues(List<PendingLookup> pending) {
			if (pending.Count == 0) {
				return;
			}
			foreach (IGrouping<Guid, PendingLookup> entityGroup in pending.GroupBy(item => item.EntitySchemaUId)) {
				EntitySchema entitySchema;
				try {
					entitySchema = _userConnection.EntitySchemaManager.GetInstanceByUId(entityGroup.Key);
				} catch (Exception exception) {
					Log.Warn($"Failed to resolve entity schema '{entityGroup.Key}' for lookup enrichment.", exception);
					continue;
				}
				if (entitySchema?.PrimaryDisplayColumn == null) {
					continue;
				}
				LoadEntityDisplayValues(entitySchema, entityGroup.ToList());
			}
		}

		private void LoadEntityDisplayValues(EntitySchema entitySchema, List<PendingLookup> entityLookups) {
			string primaryColumnName = entitySchema.PrimaryColumn.Name;
			string displayColumnName = entitySchema.PrimaryDisplayColumn.Name;
			var boxesByRecordId = new Dictionary<Guid, List<Dictionary<string, object>>>();
			foreach (PendingLookup lookup in entityLookups) {
				if (!boxesByRecordId.TryGetValue(lookup.RecordId, out List<Dictionary<string, object>> boxes)) {
					boxes = new List<Dictionary<string, object>>();
					boxesByRecordId[lookup.RecordId] = boxes;
				}
				boxes.Add(lookup.Box);
			}
			foreach (IEnumerable<Guid> chunk in boxesByRecordId.Keys.SplitOnChunks(EntityRecordsChunkSize)) {
				var recordIds = chunk.ToList();
				if (recordIds.Count == 0) {
					continue;
				}
				var esq = new EntitySchemaQuery(entitySchema);
				esq.PrimaryQueryColumn.IsAlwaysSelect = true;
				esq.AddColumn(displayColumnName);
				// Pass an actual object[] of boxed ids: CreateFilterWithParameters takes params object[],
				// so a bare IEnumerable would be captured as ONE parameter (PK = <the enumerable>) and the
				// IN filter would never form. Materializing to an array makes each id a separate parameter,
				// which builds the intended IN clause.
				IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(
					FilterComparisonType.Equal, primaryColumnName, recordIds.Cast<object>().ToArray());
				esq.Filters.Add(filter);
				EntityCollection entities = esq.GetEntityCollection(_userConnection);
				foreach (Entity entity in entities) {
					Guid recordId = entity.GetTypedColumnValue<Guid>(primaryColumnName);
					if (!boxesByRecordId.TryGetValue(recordId, out List<Dictionary<string, object>> boxes)) {
						continue;
					}
					string displayValue = entity.GetTypedColumnValue<string>(displayColumnName) ?? string.Empty;
					boxes.ForEach(box => box[DisplayValueKey] = displayValue);
				}
			}
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public IDictionary<string, object> Enrich(IDictionary<string, object> resultValues,
				IEnumerable<ProcessSchemaParameter> parameters) {
			if (resultValues == null || resultValues.Count == 0 || parameters == null) {
				return resultValues;
			}
			// Wrapping is unconditional (no structural drift); the feature toggle only controls the
			// batched entity reads that resolve the display value.
			bool resolveDisplayValues = IsEnrichmentEnabled();
			IReadOnlyDictionary<string, ProcessSchemaParameter> parametersByName =
				MapParametersByName(parameters);
			var pending = new List<PendingLookup>();
			var enriched = new Dictionary<string, object>(resultValues.Count);
			try {
				foreach (KeyValuePair<string, object> pair in resultValues) {
					ProcessSchemaParameter parameter = parametersByName.TryGetValue(pair.Key,
						out ProcessSchemaParameter found) ? found : null;
					enriched[pair.Key] = parameter == null
						? pair.Value
						: ConvertValue(parameter, pair.Value, pending, resolveDisplayValues);
				}
			} catch (Exception exception) {
				// Building the wrapped shape failed (not expected). This is the only path that can
				// reintroduce a bare identifier — fall back to the raw result rather than emit a
				// half-built payload, so a tool result is never failed over enrichment.
				Log.Warn("Lookup wrapping failed; returning raw result values.", exception);
				return resultValues;
			}
			if (resolveDisplayValues) {
				try {
					ResolveDisplayValues(pending);
				} catch (Exception exception) {
					// Display-value resolution is best-effort: keep the wrapped { value } shape (no
					// drift), just without the resolved labels, rather than reverting to bare ids.
					Log.Warn("Lookup display-value resolution failed; returning wrapped values " +
						"without labels.", exception);
				}
			}
			return enriched;
		}

		#endregion

	}

	#endregion

}

