namespace CrtAIAgenticBridgeApp.Runtime
{
	public interface IEventSender
	{
		void Send<TData>(EventOccurrenceRequest<TData> request);
	}
}

