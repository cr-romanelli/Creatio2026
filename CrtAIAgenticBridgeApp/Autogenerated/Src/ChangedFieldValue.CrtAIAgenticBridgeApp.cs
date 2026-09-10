using System;

namespace CrtAIAgenticBridgeApp.Runtime
{
	// Internal representation of a changed entity column used by the runtime
	// to match watched-column filters. Not serialized into the outbound
	// trigger payload — payload exposes only the resolved field names per
	// matched event type (see EventDispatcher).
	public class ChangedFieldValue
	{
		public string Name { get; set; }

		public Guid ColumnUId { get; set; }
	}
}

