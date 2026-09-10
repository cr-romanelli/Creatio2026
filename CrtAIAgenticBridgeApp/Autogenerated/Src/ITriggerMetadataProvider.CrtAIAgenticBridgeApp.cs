using CrtAIAgenticBridgeApp.Runtime;
using CrtAIAgenticBridgeApp.EntryPoints.WebServices.Dto;

namespace CrtAIAgenticBridgeApp.TriggerDiscovery
{
	public interface ITriggerMetadataProvider
	{
		TriggerCatalogPushRequest[] GetAllTriggers();

		TriggerCatalogPushRequest GetTrigger(string triggerCode);

		GetEventTypesResponse GetEventTypes(string triggerCode);

		GetEntitySchemaColumnsResponse GetEntitySchemaColumns(string schemaName);
	}
}

