namespace Terrasoft.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	#region Class: ToolServiceRequestGuard

	internal static class ToolServiceRequestGuard
	{

		#region Fields: Public

		/// <summary>
		/// Settings for deserializing the loosely-typed JSON-RPC envelope and MCP
		/// tool <c>arguments</c> into <see cref="Dictionary{TKey, TValue}"/> /
		/// <see cref="object"/> graphs.
		///
		/// <c>DateParseHandling.None</c> is mandatory: Newtonsoft's default
		/// (<c>DateParseHandling.DateTime</c>) silently rewrites any string that
		/// merely looks like a date (e.g. <c>"2026-06-19T14:00:00"</c>) into a
		/// boxed <see cref="DateTime"/>. JSON has no date type — at the transport
		/// layer every value is a string, number, bool, object or array, and it is
		/// the tool's input schema (<c>"type": "string"</c>, the shape
		/// <see cref="RuntimeToolFacade"/> advertises for DateTime parameters) that
		/// decides how a value is interpreted. With the default handling a
		/// string-typed parameter arrives as a <see cref="DateTime"/> and
		/// <see cref="TryValidateValue"/> rejects it with "must be a string".
		/// Preserving values verbatim keeps validation and the downstream BP
		/// contract (which re-parses by the declared parameter type) in sync.
		///
		/// Requires the real Newtonsoft.Json assembly — the Terrasoft Newtonsoft
		/// wrapper exposes no <c>DateParseHandling</c> knob (see the package csproj
		/// reference note).
		/// </summary>
		public static readonly JsonSerializerSettings LosslessArgumentSettings =
			new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };

		#endregion

		#region Methods: Private

		private static string BuildError(string path, string message) {
			return $"Invalid arguments: {path} {message}.";
		}

		private static IDictionary<string, object> ConvertToDictionary(object value) {
			if (value is IDictionary<string, object> dictionary) {
				return dictionary;
			}
			if (!(value is IDictionary legacyDictionary)) {
				return null;
			}
			var converted = new Dictionary<string, object>(StringComparer.Ordinal);
			foreach (DictionaryEntry entry in legacyDictionary) {
				if (!(entry.Key is string key)) {
					return null;
				}
				converted[key] = entry.Value;
			}
			return converted;
		}

		private static bool IsInteger(object value) {
			if (value == null) {
				return false;
			}
			switch (Type.GetTypeCode(value.GetType())) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return true;
				default:
					return false;
			}
		}

		private static bool IsNumber(object value) {
			if (value == null) {
				return false;
			}
			switch (Type.GetTypeCode(value.GetType())) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		private static bool ContainsUnsafeStringContent(string value) {
			if (value == null) {
				return false;
			}
			if (value.IndexOf('\0') >= 0) {
				return true;
			}
			for (int index = 0; index < value.Length; index++) {
				char character = value[index];
				if (char.IsControl(character) && character != '\r' && character != '\n' && character != '\t') {
					return true;
				}
			}
			string normalized = value.ToLowerInvariant();
			return normalized.Contains("../")
				|| normalized.Contains("..\\")
				|| normalized.Contains("<script")
				|| normalized.Contains("javascript:");
		}

		private static bool TryValidateSafeContent(object value, string path, out string errorMessage) {
			if (value == null) {
				errorMessage = null;
				return true;
			}
			if (value is string stringValue) {
				if (ContainsUnsafeStringContent(stringValue)) {
					errorMessage = BuildError(path, "contains unsafe content");
					return false;
				}
				errorMessage = null;
				return true;
			}
			IDictionary<string, object> dictionary = ConvertToDictionary(value);
			if (dictionary != null) {
				foreach (KeyValuePair<string, object> item in dictionary) {
					if (!TryValidateSafeContent(item.Value, $"{path}.{item.Key}", out errorMessage)) {
						return false;
					}
				}
				errorMessage = null;
				return true;
			}
			IList<object> items = ConvertToList(value);
			if (items != null) {
				for (int index = 0; index < items.Count; index++) {
					if (!TryValidateSafeContent(items[index], $"{path}[{index}]", out errorMessage)) {
						return false;
					}
				}
			}
			errorMessage = null;
			return true;
		}

		private static IList<object> ConvertToList(object value) {
			if (value == null || value is string || !(value is IEnumerable enumerable)) {
				return null;
			}
			var items = new List<object>();
			foreach (object item in enumerable) {
				items.Add(item);
			}
			return items;
		}

		private static string NormalizeTypeName(string type) {
			return (type ?? string.Empty).Trim().ToLowerInvariant();
		}

		private static bool TryValidateDictionary(IDictionary<string, object> arguments,
				IDictionary<string, McpToolProperty> properties, IEnumerable<string> requiredNames,
				string pathPrefix, out string errorMessage) {
			var declaredProperties = properties ?? new Dictionary<string, McpToolProperty>(StringComparer.Ordinal);
			foreach (string requiredName in requiredNames ?? Enumerable.Empty<string>()) {
				if (requiredName == null) {
					continue;
				}
				object requiredValue;
				if (!arguments.TryGetValue(requiredName, out requiredValue) || requiredValue == null) {
					errorMessage = BuildError($"{pathPrefix}.{requiredName}", "is required");
					return false;
				}
			}
			foreach (KeyValuePair<string, object> argument in arguments) {
				McpToolProperty propertySchema;
				if (!declaredProperties.TryGetValue(argument.Key, out propertySchema)) {
					errorMessage = BuildError($"{pathPrefix}.{argument.Key}", "is not declared in the tool schema");
					return false;
				}
				if (!TryValidateValue(argument.Value, propertySchema, $"{pathPrefix}.{argument.Key}", out errorMessage)) {
					return false;
				}
			}
			errorMessage = null;
			return true;
		}

		private static bool TryValidateValue(object value, McpToolProperty schema, string path,
				out string errorMessage) {
			if (value == null || schema == null) {
				errorMessage = null;
				return true;
			}
			switch (NormalizeTypeName(schema.Type)) {
				case "array":
					IList<object> items = ConvertToList(value);
					if (items == null) {
						errorMessage = BuildError(path, "must be an array");
						return false;
					}
					for (int index = 0; index < items.Count; index++) {
						if (!TryValidateValue(items[index], schema.Items, $"{path}[{index}]", out errorMessage)) {
							return false;
						}
					}
					errorMessage = null;
					return true;
				case "boolean":
					if (value is bool) {
						errorMessage = null;
						return true;
					}
					errorMessage = BuildError(path, "must be a boolean");
					return false;
				case "integer":
					if (IsInteger(value)) {
						errorMessage = null;
						return true;
					}
					errorMessage = BuildError(path, "must be an integer");
					return false;
				case "number":
					if (IsNumber(value)) {
						errorMessage = null;
						return true;
					}
					errorMessage = BuildError(path, "must be a number");
					return false;
				case "object":
					IDictionary<string, object> dictionary = ConvertToDictionary(value);
					if (dictionary == null) {
						errorMessage = BuildError(path, "must be an object");
						return false;
					}
					if (schema.Properties == null && schema.Required == null) {
						errorMessage = null;
						return true;
					}
					return TryValidateDictionary(dictionary, schema.Properties, schema.Required, path, out errorMessage);
				case "string":
					if (value is string) {
						errorMessage = null;
						return true;
					}
					errorMessage = BuildError(path, "must be a string");
					return false;
				default:
					errorMessage = null;
					return true;
			}
		}

		#endregion

		#region Methods: Public

		/// <summary>
		/// Resolves and authorizes the supplied tools via <see cref="IMcpToolRuntime"/>,
		/// which dispatches per source type (business process vs source-code action). The
		/// returned map is keyed on <c>McpTool.Id</c> (the uniform identity both kinds
		/// advertise as <c>RuntimeToolDefinition.ToolUId</c>).
		/// </summary>
		public static IReadOnlyDictionary<Guid, RuntimeToolDefinition> GetAuthorizedTools(
				IMcpToolRuntime runtime, IReadOnlyCollection<McpToolModel> tools) {
			return BuildAuthorizedToolMap(runtime.GetCandidateTools(tools));
		}

		private static IReadOnlyDictionary<Guid, RuntimeToolDefinition> BuildAuthorizedToolMap(
				IReadOnlyCollection<RuntimeToolDefinition> candidates) {
			return candidates
				.Where(tool => tool != null && tool.CanExecute && tool.IsEnabled)
				.GroupBy(tool => tool.ToolUId)
				.ToDictionary(group => group.Key, group => group.First());
		}

		public static bool TryResolveAuthorizedTool(IReadOnlyDictionary<Guid, RuntimeToolDefinition> tools,
				Guid toolUId, out RuntimeToolDefinition toolDefinition) {
			return tools.TryGetValue(toolUId, out toolDefinition) &&
				toolDefinition != null &&
				toolDefinition.CanExecute &&
				toolDefinition.IsEnabled;
		}

		public static bool TryValidateArguments(McpToolInputSchema inputSchema,
				IDictionary<string, object> arguments, out string errorMessage) {
			var safeArguments = arguments ?? new Dictionary<string, object>(StringComparer.Ordinal);
			if (inputSchema != null && !TryValidateDictionary(
				safeArguments,
				inputSchema.Properties,
				inputSchema.Required,
				"arguments",
				out errorMessage)) {
				return false;
			}
			// Always scan for unsafe content, even when no schema is supplied — a
			// null schema must never become a bypass for control-char / traversal /
			// script payloads (defense-in-depth for future callers).
			return TryValidateSafeContent(safeArguments, "arguments", out errorMessage);
		}

		/// <summary>
		/// Builds a normalized <see cref="McpToolInputSchema"/> from stored JSON
		/// (or an empty object schema when absent/blank), guaranteeing non-null
		/// <c>Properties</c> and a default <c>"object"</c> type so downstream
		/// JSON-Schema validators don't choke. Single-sourced for the list/call
		/// services.
		/// </summary>
		public static McpToolInputSchema BuildInputSchema(string schemaJson) {
			McpToolInputSchema schema = string.IsNullOrWhiteSpace(schemaJson)
				? new McpToolInputSchema()
				: JsonConvert.DeserializeObject<McpToolInputSchema>(schemaJson) ?? new McpToolInputSchema();
			if (schema.Properties == null) {
				schema.Properties = new Dictionary<string, McpToolProperty>();
			}
			if (string.IsNullOrWhiteSpace(schema.Type)) {
				schema.Type = "object";
			}
			return schema;
		}

		/// <summary>
		/// Normalizes a raw MCP <c>arguments</c> value into a fully-materialized
		/// <see cref="Dictionary{TKey, TValue}"/> graph of plain CLR types
		/// (<see cref="string"/>, <see cref="long"/>, <see cref="double"/>,
		/// <see cref="bool"/>, nested dictionaries and lists).
		///
		/// Deserializing the JSON-RPC envelope into <c>Dictionary&lt;string, object&gt;</c>
		/// leaves NESTED objects/arrays as <see cref="JObject"/> / <see cref="JArray"/>
		/// (and their scalars as <see cref="JValue"/>), which neither
		/// <see cref="TryValidateValue"/> (it tests <c>value is string</c> etc.) nor the
		/// downstream BP serialization handle. Object / object-list tool parameters carry
		/// exactly such nested values, so the graph is round-tripped and recursively
		/// converted to plain CLR types here — once, at the edge.
		///
		/// <see cref="LosslessArgumentSettings"/> keeps date-looking strings as strings
		/// (see its remarks). Anything that isn't a JSON object (array / scalar at the
		/// top level) yields an empty argument set.
		/// </summary>
		public static IDictionary<string, object> NormalizeArguments(object rawArguments) {
			if (rawArguments == null) {
				return new Dictionary<string, object>(StringComparer.Ordinal);
			}
			try {
				string roundtrip = JsonConvert.SerializeObject(rawArguments);
				JToken token = JsonConvert.DeserializeObject<JToken>(roundtrip, LosslessArgumentSettings);
				return MaterializeToken(token) as IDictionary<string, object>
					?? new Dictionary<string, object>(StringComparer.Ordinal);
			} catch (Exception) {
				return new Dictionary<string, object>(StringComparer.Ordinal);
			}
		}

		private static object MaterializeToken(JToken token) {
			if (token == null) {
				return null;
			}
			switch (token.Type) {
				case JTokenType.Object:
					var map = new Dictionary<string, object>(StringComparer.Ordinal);
					foreach (JProperty property in ((JObject)token).Properties()) {
						map[property.Name] = MaterializeToken(property.Value);
					}
					return map;
				case JTokenType.Array:
					var list = new List<object>();
					foreach (JToken item in (JArray)token) {
						list.Add(MaterializeToken(item));
					}
					return list;
				case JTokenType.Integer:
				case JTokenType.Float:
				case JTokenType.Boolean:
				case JTokenType.String:
					// Newtonsoft boxes these as long / double / bool / string — exactly
					// the CLR shapes IsInteger / IsNumber / the string checks expect.
					return ((JValue)token).Value;
				case JTokenType.Null:
				case JTokenType.Undefined:
					return null;
				default:
					// Guid, Uri, TimeSpan, etc. — JSON has no such types at the transport
					// layer; surface the string form so the declared parameter type (e.g.
					// a uuid-format string) drives interpretation downstream.
					return token.ToString();
			}
		}

		/// <summary>
		/// Single-sourced session-auth predicate shared by the MCP transport,
		/// discovery, and management endpoints: a real, non-empty current user.
		/// </summary>
		public static bool IsAuthenticated(Terrasoft.Core.UserConnection userConnection) {
			return userConnection?.CurrentUser != null && userConnection.CurrentUser.Id != Guid.Empty;
		}

		#endregion

	}

	#endregion

}

