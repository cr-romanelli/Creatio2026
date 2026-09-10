namespace Creatio.Copilot
{
	using System;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.FeatureToggling;

	#region Class: KnwFeatureActivationScriptExecutor

	/// <summary>
	/// Executes feature activation scripts for enabling specific features in the system.
	/// </summary>
	public class KnwFeatureActivationScriptExecutor : IInstallScriptExecutor
	{
		
		#region Fields: Private

		internal const string AccountEnrichmentServiceUrlSettingCode = "AccountEnrichmentServiceUrl";
		internal const string KnowledgeServiceUrlSettingCode = "KnowledgeServiceUrl";
		internal const string StageEnrichmentServiceUrl = KnwLibUtils.StageEnrichmentServiceUrl;
		internal const string StageKnowledgeServiceUrl = KnwLibUtils.StageKnowledgeServiceUrl;
		internal const string PreprodKnowledgeServiceUrl = KnwLibUtils.PreprodKnowledgeServiceUrl;
		internal const string DemoKnowledgeServiceUrl = KnwLibUtils.DemoKnowledgeServiceUrl;

		// "All employees" role - the SysAdminUnit a system setting's global default value is stored against.
		private static readonly Guid AllEmployeesRoleId = new Guid("a29a3ba5-4b0d-de11-9a51-005056c00008");

		#endregion

		#region Methods: Private

		private void ConfigureKnowledgeServiceUrl(UserConnection userConnection) {
			string currentKnowledgeServiceUrl = GetSysSettingValue(userConnection, KnowledgeServiceUrlSettingCode);
			if (!string.IsNullOrWhiteSpace(currentKnowledgeServiceUrl)) {
				KnwLibUtils.Logger.Info($"System setting '{KnowledgeServiceUrlSettingCode}' already has value. Skip auto-configuration.");
				return;
			}
			string enrichmentServiceUrl = GetSysSettingValue(userConnection, AccountEnrichmentServiceUrlSettingCode);
			string resolvedKnowledgeServiceUrl = KnwLibUtils.ResolveKnowledgeServiceUrl(enrichmentServiceUrl);
			if (string.IsNullOrWhiteSpace(resolvedKnowledgeServiceUrl)) {
				// The enrichment URL is empty or not a parseable URL, so a value cannot be derived. KnowledgeServiceUrl
				// must not be left empty after installation, so fall back to the demo knowledge service URL. An
				// administrator can change it afterwards in System Settings.
				resolvedKnowledgeServiceUrl = DemoKnowledgeServiceUrl;
				KnwLibUtils.Logger.Info(
					$"System setting '{AccountEnrichmentServiceUrlSettingCode}' is empty or could not be parsed. " +
					$"Defaulting '{KnowledgeServiceUrlSettingCode}' to '{resolvedKnowledgeServiceUrl}'.");
				SetSysSettingValue(userConnection, KnowledgeServiceUrlSettingCode, resolvedKnowledgeServiceUrl);
				return;
			}
			SetSysSettingValue(userConnection, KnowledgeServiceUrlSettingCode, resolvedKnowledgeServiceUrl);
			KnwLibUtils.Logger.Info(
				$"System setting '{KnowledgeServiceUrlSettingCode}' was set to '{resolvedKnowledgeServiceUrl}' based on '{AccountEnrichmentServiceUrlSettingCode}'.");
		}

		private void RemoveUserSpecificKnowledgeServiceUrlValues(UserConnection userConnection) {
			// AfterInstall runs on update too. An older version persisted KnowledgeServiceUrl via
			// SysSettings.SetValue, creating per-user values that shadow the global default and prevent
			// administrators from updating the setting (ENG-92088). Uninstall clears these, but an in-place
			// update does not - so remove any per-user values here so the global default is authoritative.
			// No-op on a clean install (no per-user values exist).
			int removedCount = RemoveUserSpecificSysSettingValues(userConnection, KnowledgeServiceUrlSettingCode);
			if (removedCount > 0) {
				KnwLibUtils.Logger.Info(
					$"Removed {removedCount} per-user value(s) of '{KnowledgeServiceUrlSettingCode}' so the global default value is authoritative.");
			}
		}

		protected virtual int RemoveUserSpecificSysSettingValues(UserConnection userConnection, string code) {
			Select settingIdSubSelect = new Select(userConnection)
				.Column("Id")
				.From("SysSettings")
				.Where("Code").IsEqual(Column.Parameter(code)) as Select;
			return new Delete(userConnection)
				.From("SysSettingsValue")
				.Where("SysSettingsId").In(settingIdSubSelect)
				.And("SysAdminUnitId").IsNotEqual(Column.Parameter(AllEmployeesRoleId))
				.Execute();
		}

		protected virtual string GetSysSettingValue(UserConnection userConnection, string code) {
			return SysSettings.GetValue(userConnection, code, string.Empty);
		}

		protected virtual void SetSysSettingValue(UserConnection userConnection, string code, string value) {
			// Use SetDefValue (not SetValue) so the configuration is written as the system setting's global
			// default value. SetValue would create a value bound to the installing user's SysAdminUnit, which
			// shadows the global value and prevents administrators from updating it in System Settings (the
			// per-user shadowing that caused ENG-92088).
			SysSettings.SetDefValue(userConnection, code, value);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public void Execute(UserConnection userConnection) {
			var featuresToEnable = new[] {
				"GenAIFeatures.EnableKnwRetrieval"
			};
			IDbFeatureStateChanger dbFeatureStateChanger = ClassFactory.Get<IDbFeatureStateChanger>(
				new ConstructorArgument("userConnection", userConnection));
			foreach (string featureCode in featuresToEnable) {
				try {
					dbFeatureStateChanger.ChangeFeatureState(featureCode, Guid.Empty, true);
					KnwLibUtils.Logger.Info($"Successfully enabled the default value for the '{featureCode}' feature.");
				} catch (Exception e) {
					KnwLibUtils.Logger.Error($"Error enabling feature '{featureCode}'.", e);
				}
			}
			try {
				RemoveUserSpecificKnowledgeServiceUrlValues(userConnection);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error(
					$"Error removing per-user values of '{KnowledgeServiceUrlSettingCode}' during install.",
					e);
			}
			try {
				ConfigureKnowledgeServiceUrl(userConnection);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error(
					$"Error configuring system setting '{KnowledgeServiceUrlSettingCode}' during install.",
					e);
			}
		}

		#endregion

	}

	#endregion

}

