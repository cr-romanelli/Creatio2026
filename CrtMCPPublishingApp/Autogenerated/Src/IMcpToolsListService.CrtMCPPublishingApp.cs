namespace Terrasoft.Configuration
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	#region Class: McpToolDescriptor

	/// <summary>
	/// MCP <c>tools/list</c> tool descriptor. Serializes to the wire shape
	/// <c>{ name, description, inputSchema, outputSchema?, annotations? }</c>
	/// expected by MCP clients (AI Studio, Claude Desktop, ChatGPT Connectors).
	///
	/// Named <c>McpToolDescriptor</c> rather than <c>McpTool</c> to avoid a
	/// type-name collision with the <c>McpTool</c> entity generated into the
	/// same <c>Terrasoft.Configuration</c> assembly (that collision raised
	/// CS0436 across the tools/list service, handler, and this interface).
	///
	/// Optional fields use <c>EmitDefaultValue = false</c> so a tool with no
	/// output schema / annotations doesn't pick up <c>"outputSchema": null</c>
	/// on round-trip — strict JSON-Schema validators (LiteLLM, OpenAI
	/// function-calling) reject null sub-fields.
	/// </summary>
	[DataContract]
	public class McpToolDescriptor
	{

		#region Properties: Public

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "inputSchema")]
		public McpToolInputSchema InputSchema { get; set; }

		[DataMember(Name = "outputSchema", EmitDefaultValue = false)]
		public object OutputSchema { get; set; }

		[DataMember(Name = "annotations", EmitDefaultValue = false)]
		public object Annotations { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpToolInputSchema

	/// <summary>
	/// JSON-Schema shape exposed by MCP <c>tools/list</c>. Must serialize to a
	/// strictly valid OpenAI function-calling schema — LiteLLM and similar
	/// gateways reject any field that serializes as <c>null</c>:
	///   * <c>properties</c> defaults to an empty dictionary in the constructor
	///     so it never round-trips to <c>"properties": null</c>.
	///   * <c>required</c> uses <c>EmitDefaultValue = false</c> so a null/empty
	///     list is omitted entirely rather than emitted as <c>"required": null</c>.
	///   * <c>additionalProperties</c> is preserved verbatim when present in
	///     the source schema (RuntimeToolFacade emits <c>false</c> for
	///     strict argument validation).
	/// </summary>
	[DataContract]
	public class McpToolInputSchema
	{

		#region Constructors: Public

		public McpToolInputSchema() {
			Properties = new Dictionary<string, McpToolProperty>();
		}

		#endregion

		#region Properties: Public

		[DataMember(Name = "type")]
		public string Type { get; set; } = "object";

		[DataMember(Name = "properties")]
		public Dictionary<string, McpToolProperty> Properties { get; set; }

		[DataMember(Name = "required", EmitDefaultValue = false)]
		public List<string> Required { get; set; }

		[DataMember(Name = "additionalProperties", EmitDefaultValue = false)]
		public object AdditionalProperties { get; set; }

		#endregion

	}

	#endregion

	#region Class: McpToolProperty

	/// <summary>
	/// Per-property descriptor inside <see cref="McpToolInputSchema"/>.
	/// All optional fields use <c>EmitDefaultValue = false</c> so a
	/// scalar property (e.g. <c>{"type":"string"}</c>) doesn't pick up
	/// <c>"items": null</c>, <c>"properties": null</c>, <c>"required": null</c>
	/// on round-trip — same OpenAI function-calling concern as the parent
	/// schema. <c>format</c> is preserved (RuntimeToolFacade emits it
	/// for <see cref="System.Guid"/> as <c>uuid</c> and
	/// <see cref="System.DateTime"/> as <c>date-time</c>).
	/// </summary>
	[DataContract]
	public class McpToolProperty
	{

		#region Properties: Public

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "items", EmitDefaultValue = false)]
		public McpToolProperty Items { get; set; }

		[DataMember(Name = "properties", EmitDefaultValue = false)]
		public Dictionary<string, McpToolProperty> Properties { get; set; }

		[DataMember(Name = "required", EmitDefaultValue = false)]
		public List<string> Required { get; set; }

		[DataMember(Name = "type", EmitDefaultValue = false)]
		public string Type { get; set; }

		[DataMember(Name = "format", EmitDefaultValue = false)]
		public string Format { get; set; }

		#endregion

	}

	#endregion

	#region Interface: IMcpToolsListService

	/// <summary>
	/// Transport-agnostic projection of tool records into the MCP-shape
	/// tool list. Consumed by the in-process MCP transport (APD-1374), which
	/// returns the result as MCP <c>tools/list</c> wire format.
	///
	/// Owning the projection in a single service keeps tool + RBAC + schema
	/// derivation rules consistent and lets future transports (auth proxies,
	/// SSE streams) reuse it without duplication.
	/// </summary>
	internal interface IMcpToolsListService
	{

		#region Methods: Public

		IReadOnlyList<McpToolDescriptor> GetTools();

		#endregion

	}

	#endregion

}

