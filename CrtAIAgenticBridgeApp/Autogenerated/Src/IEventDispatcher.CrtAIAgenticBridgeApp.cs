namespace CrtAIAgenticBridgeApp.Runtime
{
	internal interface IEventDispatcher
	{
		void Dispatch(EntityChangeDispatchArgs args);
	}
}

