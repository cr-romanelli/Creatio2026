using System;
using System.Collections.Generic;

namespace CrtAIAgenticBridgeApp.Runtime
{
	public class EventTypeEntry
	{
		public string Code { get; set; }

		public string EntitySchemaFilters { get; set; }

		public Guid ChangeTypeId { get; set; }

		/// <summary>
		/// Pre-parsed set of watched column UIDs. Null means wildcard (any column).
		/// Non-null means filter: dispatch only when at least one changed field UID is in this set.
		/// <see cref="EntityChangeEventTypeProvider"/> normalises a stored empty JSON array to null.
		/// Populated at cache-fill time by <see cref="EntityChangeEventTypeProvider"/>.
		/// </summary>
		public HashSet<Guid> WatchedColumnIds { get; set; }
	}
}

