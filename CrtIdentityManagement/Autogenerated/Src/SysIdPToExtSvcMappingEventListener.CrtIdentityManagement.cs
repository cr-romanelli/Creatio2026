namespace Terrasoft.Configuration.CrtIdentityManagement
{
	using System;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.IdentityManagementLog;
	using Terrasoft.OAuthIntegration;

	#region Class: SysIdPToExtSvcMappingEventListener

	/// <summary>
	/// Event listener for SysIdPToExtSvcMapping entity.
	/// Logs IdP binding changes (create, update, delete) to the audit log.
	/// </summary>
	[EntityEventListener(SchemaName = "SysIdPToExtSvcMapping")]
	public class SysIdPToExtSvcMappingEventListener : BaseEntityEventListener
	{

		#region Constants: Private

		private const string EmptyBindingValue = "not set";

		#endregion

		#region Methods: Private

		private static Guid GetIdentityProviderId(Entity entity) {
			return entity.GetTypedColumnValue<Guid>("IdentityProviderId");
		}

		private static Guid GetExternalServiceId(Entity entity) {
			return entity.GetTypedColumnValue<Guid>("ServiceId");
		}

		private static string GetIdentityProviderNameById(UserConnection userConnection, Guid identityProviderId) {
			if (identityProviderId == Guid.Empty) {
				return EmptyBindingValue;
			}
			var select = new Select(userConnection)
				.Top(1)
				.Column("Name")
				.From("SysIdentityProvider")
				.Where("Id").IsEqual(Column.Parameter(identityProviderId)) as Select;
			return select.ExecuteScalar<string>() ?? identityProviderId.ToString();
		}

		private static string GetExternalServiceNameById(UserConnection userConnection, Guid serviceId) {
			if (serviceId == Guid.Empty) {
				return EmptyBindingValue;
			}
			var select = new Select(userConnection)
				.Top(1)
				.Column("Name")
				.From("SysExternalService")
				.Where("Id").IsEqual(Column.Parameter(serviceId)) as Select;
			return select.ExecuteScalar<string>() ?? serviceId.ToString();
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			Guid serviceId = GetExternalServiceId(entity);
			Guid newProviderId = GetIdentityProviderId(entity);
			string serviceName = GetExternalServiceNameById(entity.UserConnection, serviceId);
			string newProviderName = GetIdentityProviderNameById(entity.UserConnection, newProviderId);
			IdentityManagementLogger.Instance.LogIdpBindingChanged(
				entity.UserConnection, serviceName, EmptyBindingValue, newProviderName);
		}

		/// <inheritdoc />
		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			var providerColumn = e.ModifiedColumnValues?.FindByName("IdentityProviderId");
			if (providerColumn == null) {
				return;
			}
			Guid serviceId = GetExternalServiceId(entity);
			Guid oldProviderId = providerColumn.OldValue is Guid value ? value : Guid.Empty;
			Guid newProviderId = GetIdentityProviderId(entity);
			string serviceName = GetExternalServiceNameById(entity.UserConnection, serviceId);
			string oldProviderName = GetIdentityProviderNameById(entity.UserConnection, oldProviderId);
			string newProviderName = GetIdentityProviderNameById(entity.UserConnection, newProviderId);
			IdentityManagementLogger.Instance.LogIdpBindingChanged(
				entity.UserConnection, serviceName, oldProviderName, newProviderName);
		}

		/// <inheritdoc />
		public override void OnDeleting(object sender, EntityBeforeEventArgs e) {
			base.OnDeleting(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			Guid serviceId = GetExternalServiceId(entity);
			Guid oldProviderId = GetIdentityProviderId(entity);
			string serviceName = GetExternalServiceNameById(entity.UserConnection, serviceId);
			string oldProviderName = GetIdentityProviderNameById(entity.UserConnection, oldProviderId);
			IdentityManagementLogger.Instance.LogIdpBindingChanged(
				entity.UserConnection, serviceName, oldProviderName, EmptyBindingValue);
		}

		#endregion

	}

	#endregion

}

