namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using System.Collections.Generic;
	using Common.Logging;
	using Creatio.FeatureToggling;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Tasks;

	#region Class: EnableAIPlatformOutboundTriggerDelivery

	/// <inheritdoc cref="FeatureMetadata"/>
	public class EnableAIPlatformOutboundTriggerDelivery : FeatureMetadata
	{

		#region Constructors: Public

		public EnableAIPlatformOutboundTriggerDelivery() {
			IsEnabled = true;
		}

		#endregion

	}

	#endregion

	#region Class: EntityChangeEventListener

	[EntityEventListener(IsGlobal = true)]
	internal class EntityChangeEventListener : BaseEntityEventListener
	{

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		#endregion

		#region Methods: Public

		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			HandleEntityChange((Entity)sender, Constants.ChangeTypes.Inserted, e);
		}

		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			HandleEntityChange((Entity)sender, Constants.ChangeTypes.Updated, e);
		}

		public override void OnDeleted(object sender, EntityAfterEventArgs e) {
			base.OnDeleted(sender, e);
			HandleEntityChange((Entity)sender, Constants.ChangeTypes.Deleted, e);
		}

		#endregion

		#region Methods: Internal

		internal virtual void HandleEntityChange(Entity entity, string changeType,
				EntityAfterEventArgs e) {
			try {
				var isFeatureEnabled = Features.GetIsEnabled<EnableAIPlatformOutboundTriggerDelivery>();
				if (entity.SchemaName == Constants.EntityNames.AIPlatformEntityEvents) {
					var invalidatingProvider = ClassFactory.Get<IEntityChangeEventTypeProvider>(
						new ConstructorArgument("userConnection", entity.UserConnection));
					invalidatingProvider.InvalidateCache();
					if (!isFeatureEnabled) {
						return;
					}
					try {
						EnqueueTriggerCatalogUpdate();
					} catch (Exception ex) {
						Log.Error("Failed to enqueue trigger catalog update after AIPlatformEntityEvents change.", ex);
					}
					return;
				}
				if (!isFeatureEnabled) {
					return;
				}
				UserConnection userConnection = entity.UserConnection;
				if (userConnection.EntitySchemaManager.FindInstanceByName(
						Constants.EntityNames.AIPlatformEntityEvents) == null) {
					return;
				}
				string entitySchemaName = entity.SchemaName;
				Guid entitySchemaUId = entity.Schema.UId;
				var provider = ClassFactory.Get<IEntityChangeEventTypeProvider>(
					new ConstructorArgument("userConnection", userConnection));
				EventTypeEntry[] entries = provider.GetEventTypeEntries(entitySchemaUId,
					changeType);
				if (entries == null || entries.Length == 0) {
					return;
				}
				ChangedFieldValue[] changedFields = null;
				if (changeType == Constants.ChangeTypes.Updated) {
					changedFields = BuildChangedFields(entity);
				}
				var args = new EntityChangeDispatchArgs {
					EntitySchemaName = entitySchemaName,
					EntitySchemaUId = entitySchemaUId,
					EntityId = entity.PrimaryColumnValue,
					ChangeType = changeType,
					OccurredAt = DateTime.UtcNow,
					ChangedFields = changedFields
				};
				try {
					EnqueueBackgroundTask(args);
				} catch (Exception ex) {
					Log.Error("Failed to enqueue entity change async operation " +
						$"for {entitySchemaName}/{entity.PrimaryColumnValue}", ex);
				}
			} catch (ItemNotFoundException ex) {
				Log.Warn($"EntityChangeEventListener.HandleEntityChange skipped for " +
					$"{entity?.SchemaName}/{entity?.PrimaryColumnValue} ({changeType}): " +
					"a referenced schema is no longer registered (typical during package uninstall).", ex);
			}
		}

		internal virtual void EnqueueBackgroundTask(EntityChangeDispatchArgs args) {
			Task.StartNewWithUserConnection<DispatchEntityChangeAsyncOperation,
				EntityChangeDispatchArgs>(args);
		}

		internal virtual void EnqueueTriggerCatalogUpdate() {
			TriggerCatalogDispatchAsyncOperation.Schedule(
				TriggerCatalogDispatchArgs.CreateUpdateTrigger(
					Constants.TriggerCodes.CreatioEntityChange));
		}

		internal static ChangedFieldValue[] BuildChangedFields(Entity entity) {
			var result = new List<ChangedFieldValue>();
			IEnumerable<EntityColumnValue> changedColumns = entity.GetChangedColumnValues();
			foreach (EntityColumnValue column in changedColumns) {
				string columnName = column.Name;
				if (string.IsNullOrEmpty(columnName)) {
					continue;
				}
				EntitySchemaColumn schemaColumn = entity.Schema.Columns.FindByColumnValueName(columnName)
					?? entity.Schema.Columns.FindByName(columnName);
				if (schemaColumn == null) {
					Log.Warn($"Column '{columnName}' not found in schema '{entity.Schema.Name}'; " +
						"ColumnUId will be Guid.Empty and will not match any watched-column filter.");
				}
				result.Add(new ChangedFieldValue {
					Name = columnName,
					ColumnUId = schemaColumn?.UId ?? Guid.Empty
				});
			}
			return result.ToArray();
		}

		#endregion

	}

	#endregion

}

