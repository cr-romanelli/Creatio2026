namespace Terrasoft.Configuration.CrtIdentityManagement
{
	using System.Linq;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.IdentityManagementLog;
	using Terrasoft.OAuthIntegration;

	#region Class: SysExternalServiceEventListener

	/// <summary>
	/// Event listener for SysExternalService entity.
	/// Logs create, update, and delete operations to the audit log.
	/// </summary>
	[EntityEventListener(SchemaName = "SysExternalService")]
	public class SysExternalServiceEventListener : BaseEntityEventListener
	{

		#region Methods: Private

		private static string GetServiceName(Entity entity) {
			return entity.GetTypedColumnValue<string>("Name");
		}

		private static string GetChangedColumnsDescription(EntityAfterEventArgs e, Entity entity) {
			var modifiedColumns = e.ModifiedColumnValues;
			if (modifiedColumns == null || modifiedColumns.Count == 0) {
				return null;
			}
			var changes = modifiedColumns
				.Where(col => col.Name != "ModifiedOn" && col.Name != "ModifiedById")
				.Select(col => {
					var oldValue = col.OldValue?.ToString() ?? "empty";
					var newValue = entity.GetColumnValue(col.Name)?.ToString() ?? "empty";
					return $"{col.Name}: '{oldValue}' -> '{newValue}'";
				});
			return string.Join("; ", changes);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			string serviceName = GetServiceName(entity);
			IdentityManagementLogger.Instance.LogExternalServiceCreated(entity.UserConnection, serviceName);
		}

		/// <inheritdoc />
		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			string serviceName = GetServiceName(entity);
			string changedSettings = GetChangedColumnsDescription(e, entity);
			IdentityManagementLogger.Instance.LogExternalServiceChanged(
				entity.UserConnection, serviceName, changedSettings);
		}

		/// <inheritdoc />
		public override void OnDeleting(object sender, EntityBeforeEventArgs e) {
			base.OnDeleting(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			string serviceName = GetServiceName(entity);
			IdentityManagementLogger.Instance.LogExternalServiceDeleted(entity.UserConnection, serviceName);
		}

		#endregion

	}

	#endregion

}

