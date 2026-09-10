namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;

	#region Interface: IEntityChangeEventTypeProvider

	public interface IEntityChangeEventTypeProvider
	{

		#region Methods: Public

		EventTypeEntry[] GetEventTypeEntries(Guid entitySchemaUId, string changeType);

		void InvalidateCache();

		#endregion

	}

	#endregion

}

