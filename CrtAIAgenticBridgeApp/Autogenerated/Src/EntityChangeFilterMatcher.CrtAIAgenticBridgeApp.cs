using System;
using Common.Logging;
using Terrasoft.Common.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Core.Factories;
using Terrasoft.Nui.ServiceModel.Extensions;

namespace CrtAIAgenticBridgeApp.Runtime
{
	[DefaultBinding(typeof(IEntityChangeFilterMatcher))]
	internal class EntityChangeFilterMatcher : IEntityChangeFilterMatcher
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private readonly UserConnection _userConnection;

		public EntityChangeFilterMatcher(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		public bool IsMatch(Guid entitySchemaUId, Guid recordId, string serializedFilters) {
			if (string.IsNullOrEmpty(serializedFilters)) {
				return true;
			}
			try {
				var filters = Json.Deserialize<Terrasoft.Nui.ServiceModel.DataContract.Filters>(
					serializedFilters);
				if (filters == null) {
					return true;
				}
				IEntitySchemaQueryFilterItem esqFilter =
					filters.BuildEsqFilter(entitySchemaUId, _userConnection);
				if (esqFilter == null) {
					return true;
				}
				Terrasoft.Core.Entities.EntitySchema entitySchema =
					_userConnection.EntitySchemaManager.GetInstanceByUId(entitySchemaUId);
				var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager,
					entitySchema.Name);
				esq.PrimaryQueryColumn.IsVisible = true;
				esq.Filters.Add(esqFilter);
				esq.Filters.Add(esq.CreateFilterWithParameters(
					FilterComparisonType.Equal,
					entitySchema.PrimaryColumn.Name,
					recordId));
				Terrasoft.Core.Entities.EntityCollection result =
					esq.GetEntityCollection(_userConnection);
				return result.Count > 0;
			} catch (Exception ex) {
				Log.Error($"Failed to evaluate entity change filter for " +
					$"schema {entitySchemaUId}, record {recordId}. " +
					"Allowing event to proceed.", ex);
				return true;
			}
		}
	}
}

