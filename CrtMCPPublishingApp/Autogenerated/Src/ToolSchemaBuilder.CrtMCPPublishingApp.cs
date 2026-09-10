namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Common;
	using Terrasoft.Core;

	#region Interface: IToolParameter

	/// <summary>
	/// Source-agnostic view of a tool parameter, so the JSON-schema mapping can be
	/// single-sourced across both tool kinds. A business-process
	/// <c>ProcessSchemaParameter</c> and a Copilot <c>SourceCodeActionParameter</c>
	/// each adapt to this; <see cref="ToolSchemaBuilder"/> then never sees either
	/// concrete type. The two carry the same underlying type system — a
	/// <see cref="Terrasoft.Core.DataValueType"/> (scalar, composite object, object
	/// list, lookup) plus nested item properties — which is what makes the shared
	/// builder sound.
	/// </summary>
	internal interface IToolParameter
	{

		#region Properties: Public

		string Name { get; }

		// Already resolved (parameter description, falling back to caption).
		string Description { get; }

		bool IsRequired { get; }

		// Direction split is the adapter's concern: In/Var are inputs, Out/Var (and
		// IsResult, for processes) are outputs.
		bool IsInput { get; }

		bool IsOutput { get; }

		Terrasoft.Core.DataValueType DataValueType { get; }

		// Nested parameters for composite (object / object-list) types; empty otherwise.
		IReadOnlyList<IToolParameter> ItemProperties { get; }

		#endregion

	}

	#endregion

	#region Class: ToolSchemaBuilder

	/// <summary>
	/// Single source of truth for the tool parameter -> MCP JSON Schema mapping,
	/// shared by the business-process and source-code-action facades (and the admin
	/// preview via <c>ProcessRuntimeToolFacade</c>'s thin delegations). Keyed off the
	/// concrete <see cref="Terrasoft.Core.DataValueType"/>:
	///   * scalars                       -> their JSON type (<see cref="TryGetJsonSchemaType"/>)
	///   * CompositeObjectDataValueType  -> object (shape from ItemProperties)
	///   * ObjectListDataValueType       -> array of scalars (element type left open)
	///   * CompositeObjectListDataValueType -> array of objects (shape from ItemProperties)
	///   * lookup (output)               -> { value, displayValue } object
	///
	/// Input is strict — an unsupported parameter throws (the tool is excluded);
	/// output is advisory — an unsupported parameter degrades to "string". Only the
	/// input schema carries <c>required</c> and <c>additionalProperties:false</c>.
	/// </summary>
	internal static class ToolSchemaBuilder
	{

		#region Methods: Private

		// True when the parameter is a lookup (its value is a record id referencing another entity).
		private static bool IsLookupParameter(IToolParameter parameter) {
			return parameter?.DataValueType?.UId == Terrasoft.Core.DataValueType.LookupDataValueTypeUId;
		}

		// The shape the bridge's LookupResponseEnricher emits for an output lookup: always a { value }
		// object — value carries the record id, or null for an empty/unset lookup — with a displayValue
		// label added when display-value enrichment is enabled. value is always present (required);
		// displayValue is advertised but optional. value's type allows null to match the empty-lookup
		// case. additionalProperties is locked because the enricher emits exactly these two keys.
		private static Dictionary<string, object> BuildLookupOutputSchema() {
			var properties = new Dictionary<string, object> {
				["value"] = new Dictionary<string, object> {
					["type"] = new List<string> { "string", "null" },
					["format"] = "uuid"
				},
				["displayValue"] = new Dictionary<string, object> { ["type"] = "string" }
			};
			return new Dictionary<string, object> {
				["type"] = "object",
				["properties"] = properties,
				["additionalProperties"] = false,
				["required"] = new List<string> { "value" }
			};
		}

		// Returns true and fills propertySchema when the parameter is one of the explicitly
		// supported object / object-list types; false otherwise. Keyed off the concrete
		// DataValueType class — NOT the CLR ValueType interface — to stay precise (e.g.
		// FileDataValueType must stay unsupported). Nested item properties never throw —
		// they are advisory shape hints (throwOnUnsupported: false).
		private static bool TryBuildComplexSchema(IToolParameter parameter,
				out Dictionary<string, object> propertySchema, bool lookupAsObject = false) {
			propertySchema = null;
			Terrasoft.Core.DataValueType dataValueType = parameter.DataValueType;
			if (dataValueType is CompositeObjectDataValueType) {
				propertySchema = new Dictionary<string, object> { ["type"] = "object" };
				AddItemProperties(parameter, propertySchema, lookupAsObject);
				return true;
			}
			if (dataValueType is ObjectListDataValueType) {
				// Scalar list: element type is inferred at parse time, so "items" is left open.
				propertySchema = new Dictionary<string, object> { ["type"] = "array" };
				return true;
			}
			if (dataValueType is CompositeObjectListDataValueType) {
				var items = new Dictionary<string, object> { ["type"] = "object" };
				AddItemProperties(parameter, items, lookupAsObject);
				propertySchema = new Dictionary<string, object> {
					["type"] = "array",
					["items"] = items
				};
				return true;
			}
			return false;
		}

		// Describes a composite object's shape from the parameter's nested ItemProperties
		// (themselves IToolParameter, so the build recurses to support nested object lists).
		// No declared item properties -> the object is left open.
		private static void AddItemProperties(IToolParameter parameter,
				Dictionary<string, object> target, bool lookupAsObject = false) {
			IReadOnlyList<IToolParameter> itemProperties = parameter.ItemProperties;
			if (itemProperties == null || itemProperties.Count == 0) {
				return;
			}
			var properties = new Dictionary<string, object>();
			var required = new List<string>();
			foreach (IToolParameter nested in itemProperties) {
				if (nested == null || nested.Name.IsNullOrWhiteSpace()) {
					continue;
				}
				properties[nested.Name] =
					BuildParameterPropertySchema(nested, throwOnUnsupported: false, lookupAsObject: lookupAsObject);
				if (nested.IsRequired) {
					required.Add(nested.Name);
				}
			}
			if (properties.Count == 0) {
				return;
			}
			target["properties"] = properties;
			// Lock the object to its declared shape: ToolServiceRequestGuard rejects undeclared
			// keys whenever "properties" is present, so the advertised schema must match.
			target["additionalProperties"] = false;
			if (required.Count > 0) {
				target["required"] = required;
			}
		}

		private static string GetJsonSchemaFormat(IToolParameter parameter) {
			Type valueType = parameter.DataValueType != null ? parameter.DataValueType.ValueType : null;
			if (valueType == typeof(Guid)) {
				return "uuid";
			}
			if (valueType == typeof(DateTime)) {
				return "date-time";
			}
			return null;
		}

		#endregion

		#region Methods: Public

		/// <summary>
		/// Assembles the JSON-schema object for one direction from a parameter set.
		/// </summary>
		// lookupAsObject: render output lookups as the { value, displayValue } object the
		// agentic bridge's LookupResponseEnricher emits. Defaults to !isInput (BP behavior:
		// enriched on output, bare uuid on input). Source-code actions have NO enricher, so
		// they pass false for output too — a bare uuid string — and must construct/return the
		// enriched shape themselves (e.g. via the McpTool.OutputSchema override) if they want it.
		public static Dictionary<string, object> BuildSchemaObject(
				IEnumerable<IToolParameter> parameters, bool isInput, bool? lookupAsObject = null) {
			bool renderLookupAsObject = lookupAsObject ?? !isInput;
			var properties = new Dictionary<string, object>();
			var required = new List<string>();
			foreach (IToolParameter parameter in parameters ?? Enumerable.Empty<IToolParameter>()) {
				if (parameter == null || parameter.Name.IsNullOrWhiteSpace()) {
					continue;
				}
				if (isInput ? !parameter.IsInput : !parameter.IsOutput) {
					continue;
				}
				properties[parameter.Name] =
					BuildParameterPropertySchema(parameter, throwOnUnsupported: isInput, lookupAsObject: renderLookupAsObject);
				if (isInput && parameter.IsRequired) {
					required.Add(parameter.Name);
				}
			}
			var schemaObject = new Dictionary<string, object> {
				["type"] = "object",
				["properties"] = properties
			};
			if (isInput) {
				schemaObject["additionalProperties"] = false;
			}
			if (required.Count > 0) {
				schemaObject["required"] = required;
			}
			return schemaObject;
		}

		/// <summary>
		/// Builds the JSON-schema fragment for a single parameter. <paramref name="lookupAsObject"/>
		/// renders a lookup as the { value, displayValue } object (output schema); when false a
		/// lookup stays a bare uuid string (input schema). <paramref name="throwOnUnsupported"/>
		/// excludes a tool whose input carries an unrepresentable type; output degrades to "string".
		/// </summary>
		public static Dictionary<string, object> BuildParameterPropertySchema(IToolParameter parameter,
				bool throwOnUnsupported, bool lookupAsObject = false) {
			parameter.CheckArgumentNull(nameof(parameter));
			Type valueType = parameter.DataValueType != null ? parameter.DataValueType.ValueType : null;
			Dictionary<string, object> propertySchema;
			if (lookupAsObject && IsLookupParameter(parameter)) {
				propertySchema = BuildLookupOutputSchema();
			} else if (TryBuildComplexSchema(parameter, out propertySchema, lookupAsObject)) {
				// complex (array/object) — already populated
			} else if (TryGetJsonSchemaType(valueType, out string jsonType)) {
				propertySchema = new Dictionary<string, object> { ["type"] = jsonType };
				string format = GetJsonSchemaFormat(parameter);
				if (format.IsNotNullOrWhiteSpace()) {
					propertySchema["format"] = format;
				}
			} else if (throwOnUnsupported) {
				string typeName = parameter.DataValueType != null
					? (parameter.DataValueType.Name ??
						(parameter.DataValueType.ValueType != null ? parameter.DataValueType.ValueType.Name : "unknown"))
					: "unknown";
				throw new NotSupportedException(
					$"Parameter '{parameter.Name}' has unsupported data value type '{typeName}'.");
			} else {
				propertySchema = new Dictionary<string, object> { ["type"] = "string" };
			}
			if (parameter.Description.IsNotNullOrWhiteSpace()) {
				propertySchema["description"] = parameter.Description;
			}
			return propertySchema;
		}

		/// <summary>
		/// True when a parameter's type can be represented in an MCP schema — the supported
		/// scalar set plus the object / object-list types parsed from JSON.
		/// </summary>
		public static bool IsSupportedParameterType(IToolParameter parameter) {
			Terrasoft.Core.DataValueType dataValueType = parameter != null ? parameter.DataValueType : null;
			if (dataValueType == null) {
				return false;
			}
			return dataValueType is CompositeObjectDataValueType ||
				dataValueType is ObjectListDataValueType ||
				dataValueType is CompositeObjectListDataValueType ||
				TryGetJsonSchemaType(dataValueType.ValueType, out _);
		}

		/// <summary>
		/// Maps a CLR <see cref="Type"/> to a JSON-schema scalar type, returning false for
		/// types outside the supported set (text, integer, number, boolean, date/time, uuid).
		/// </summary>
		public static bool TryGetJsonSchemaType(Type valueType, out string jsonType) {
			if (valueType == typeof(string) || valueType == typeof(Guid) || valueType == typeof(DateTime)) {
				jsonType = "string";
				return true;
			}
			if (valueType == typeof(bool)) {
				jsonType = "boolean";
				return true;
			}
			if (valueType == typeof(short) || valueType == typeof(int) || valueType == typeof(long)) {
				jsonType = "integer";
				return true;
			}
			if (valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(float)) {
				jsonType = "number";
				return true;
			}
			jsonType = null;
			return false;
		}

		#endregion

	}

	#endregion

}

