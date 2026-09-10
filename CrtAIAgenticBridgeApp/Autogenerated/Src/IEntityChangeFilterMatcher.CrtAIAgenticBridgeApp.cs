using System;

namespace CrtAIAgenticBridgeApp.Runtime
{
	public interface IEntityChangeFilterMatcher
	{
		bool IsMatch(Guid entitySchemaUId, Guid recordId, string serializedFilters);
	}
}

