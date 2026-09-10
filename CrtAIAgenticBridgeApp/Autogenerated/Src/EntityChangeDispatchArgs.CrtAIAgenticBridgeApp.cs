using System;

namespace CrtAIAgenticBridgeApp.Runtime
{
	internal class EntityChangeDispatchArgs
	{
		public string EntitySchemaName { get; set; }

		public Guid EntitySchemaUId { get; set; }

		public Guid EntityId { get; set; }

		public string ChangeType { get; set; }

		public DateTime OccurredAt { get; set; }

		public ChangedFieldValue[] ChangedFields { get; set; }
	}
}

