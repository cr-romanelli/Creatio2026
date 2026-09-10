namespace Creatio.Copilot
{
	using System;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.FeatureToggling;

	#region Class: KnwFeatureDeactivationScriptExecutor

	/// <summary>
	/// Executes a script to reset the default state of specified features in the database.
	/// </summary>
	public class KnwFeatureDeactivationScriptExecutor : IInstallScriptExecutor
	{

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
					dbFeatureStateChanger.ResetFeatureState(featureCode, Guid.Empty);
					KnwLibUtils.Logger.Info($"Feature '{featureCode}' default value reset successfully.");
				} catch (Exception e) {
					KnwLibUtils.Logger.Error($"Failed to reset feature '{featureCode}'.", e);
				}
			}
		}

		#endregion

	}

	#endregion

}

