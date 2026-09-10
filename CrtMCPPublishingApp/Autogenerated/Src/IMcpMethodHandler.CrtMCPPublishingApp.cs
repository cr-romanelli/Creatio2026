namespace Terrasoft.Configuration
{
	using System.Collections.Generic;

	#region Class: McpHandlerOutcome

	/// <summary>
	/// Result of dispatching a JSON-RPC method to its handler. Either a
	/// success payload (serialized into <c>result</c>) or an error. Handlers
	/// must not throw for expected business/validation flow — they return
	/// <see cref="Failure(int, string, object)"/> instead, which the
	/// dispatcher maps to a JSON-RPC error.
	/// </summary>
	internal class McpHandlerOutcome
	{

		#region Properties: Public

		public object Result { get; private set; }

		public int? ErrorCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public object ErrorData { get; private set; }

		public bool IsError { get { return ErrorCode.HasValue; } }

		#endregion

		#region Methods: Public

		public static McpHandlerOutcome Success(object result) {
			return new McpHandlerOutcome { Result = result };
		}

		public static McpHandlerOutcome Failure(int code, string message, object data = null) {
			return new McpHandlerOutcome {
				ErrorCode = code,
				ErrorMessage = message,
				ErrorData = data
			};
		}

		#endregion

	}

	#endregion

	#region Interface: IMcpMethodHandler

	/// <summary>
	/// Per-method handler invoked by <see cref="ToolServiceMcp"/> after the
	/// JSON-RPC envelope is parsed and the caller is authenticated.
	/// Handlers are stateless except for any dependencies wired to a
	/// resolved <c>UserConnection</c> at construction time.
	/// </summary>
	internal interface IMcpMethodHandler
	{

		#region Methods: Public

		McpHandlerOutcome Handle(Dictionary<string, object> parameters);

		#endregion

	}

	#endregion

}

