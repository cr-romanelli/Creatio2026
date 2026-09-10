namespace Terrasoft.Configuration.CrtIdentityManagement
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.IdentityManagementLog;
	using Terrasoft.OAuthIntegration;

	#region Class: SysIdentityProviderEventListener

	/// <summary>
	/// Event listener for SysIdentityProvider entity.
	/// Logs create, update, and delete operations to the audit log.
	/// </summary>
	[EntityEventListener(SchemaName = "SysIdentityProvider")]
	public class SysIdentityProviderEventListener : BaseEntityEventListener
	{

		#region Fields: Private

		private static readonly Regex PlainTextRegex = new Regex(
			@"^[^<>]*$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);

		#endregion

		#region Methods: Private

		private static string GetProviderName(Entity entity) {
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

		private static bool IsPlainText(string value) {
			return string.IsNullOrWhiteSpace(value) || PlainTextRegex.IsMatch(value);
		}

		private static void ValidatePlainText(UserConnection userConnection, string value, string resourceName) {
			if (IsPlainText(value)) {
				return;
			}
			var errorMessage = new LocalizableString(
				userConnection.ResourceStorage,
				"SysIdentityProviderEventListener",
				resourceName);
			throw new ValidateException(errorMessage);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			string providerName = GetProviderName(entity);
			IdentityManagementLogger.Instance.LogIdentityProviderCreated(entity.UserConnection, providerName);
		}

		/// <inheritdoc />
		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			IdentityProviderAccessor.ExpireCache();
			var entity = (Entity)sender;
			string providerName = GetProviderName(entity);
			string changedSettings = GetChangedColumnsDescription(e, entity);
			IdentityManagementLogger.Instance.LogIdentityProviderChanged(
				entity.UserConnection, providerName, changedSettings);
		}

		/// <inheritdoc />
		public override void OnDeleting(object sender, EntityBeforeEventArgs e) {
			var entity = (Entity)sender;
			var userConnection = entity.UserConnection;
			Guid providerId = entity.GetTypedColumnValue<Guid>("Id");
			var defaultProviderId = SysSettings.GetValue<Guid>(userConnection, "DefaultIdentityProvider", Guid.Empty);
			if (defaultProviderId == providerId) {
				var errorMessage = new LocalizableString(
					userConnection.ResourceStorage,
					"SysIdentityProviderEventListener",
					"LocalizableStrings.DefaultIdentityProviderCannotBeDeleted.Value");
				throw new DbOperationException(errorMessage);
			}
			base.OnDeleting(sender, e);
			IdentityProviderAccessor.ExpireCache();
			string providerName = GetProviderName(entity);
			IdentityManagementLogger.Instance.LogIdentityProviderDeleted(entity.UserConnection, providerName);
		}

		/// <inheritdoc />
		public override void OnSaving(object sender, EntityBeforeEventArgs e) {
			var entity = (Entity)sender;
			var userConnection = entity.UserConnection;
			string name = entity.GetTypedColumnValue<string>("Name");
			string description = entity.GetTypedColumnValue<string>("Description");
			string serverUrl = entity.GetTypedColumnValue<string>("ServerUrl");
			ValidatePlainText(userConnection, name,
				"LocalizableStrings.ProviderNameMustNotContainHtmlMarkup.Value");
			ValidatePlainText(userConnection, description,
				"LocalizableStrings.ProviderDescriptionMustNotContainHtmlMarkup.Value");
			if (string.IsNullOrWhiteSpace(serverUrl)) {
				return;
			}
			Uri uri;
			bool isValidServerUrl = Uri.TryCreate(serverUrl, UriKind.Absolute, out uri)
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			if (!isValidServerUrl) {
				var serverUrlErrorMessage = new LocalizableString(userConnection.ResourceStorage, "SysIdentityProviderEventListener",
					"LocalizableStrings.ServerURLMustBeAnAbsoluteHttpOrHttpsURL.Value");
				throw new ValidateException(string.Format(serverUrlErrorMessage, serverUrl));
			}
			base.OnSaving(sender, e);
		}

		#endregion

	}

	#endregion

}

