namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System.Collections.Generic;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;

	#region Class: ActualizeCanManageMCPServersContentGranteeScriptExecutor

	/// <summary>
	/// Post-install actualizer for the <c>CanManageMCPServersContent</c> system
	/// operation. The package data binding seeds only the System administrators
	/// grantee (the one role whose <c>SysAdminUnit</c> record is bound in a base
	/// dependency, so the binding wizard allows it). This script idempotently
	/// adds the Administrator and Developer roles, which can't be shipped as a
	/// grantee data binding because their <c>SysAdminUnit</c> records aren't bound
	/// in any dependency. Mirrors the platform's
	/// <c>ActualizeCanManageDebugOperationGrantee</c> pattern: it runs as an
	/// AfterInstall script and is safe to re-run (existence-checked).
	/// </summary>
	public class ActualizeCanManageMCPServersContentGranteeScriptExecutor : IInstallScriptExecutor
	{

		#region Fields: Private

		private static readonly ILog Log =
			LogManager.GetLogger(nameof(ActualizeCanManageMCPServersContentGranteeScriptExecutor));
		private readonly string _operationId = "798174c5-f68c-4a27-b9d9-5589b65fc1a9";
		private readonly string _administratorAdminUnitId = "6c83b1d3-1e2e-4a3c-a0bf-bbfb3eb7a460";
		private readonly string _developerAdminUnitId = "ef35155c-c7db-485d-a9b1-b6a1f7d23412";

		#endregion

		#region Methods: Private

		private void AddOperationGrantee(UserConnection userConnection, string adminUnitId, int position) {
			var adminUnitEntity = userConnection.EntitySchemaManager.GetEntityByName("SysAdminUnit", userConnection);
			var adminUnitCondition = new Dictionary<string, object> {
				{ "Id", adminUnitId }
			};
			if (!adminUnitEntity.FetchFromDB(adminUnitCondition)) {
				Log.Warn($"ActualizeCanManageMCPServersContentGrantee: SysAdminUnit '{adminUnitId}' not found; " +
					$"skipped granting operation '{_operationId}'. The role may be absent on this environment.");
				return;
			}
			var granteeEntity = userConnection.EntitySchemaManager.GetEntityByName("SysAdminOperationGrantee", userConnection);
			var condition = new Dictionary<string, object> {
				{ "SysAdminOperation", _operationId },
				{ "SysAdminUnit", adminUnitId }
			};
			if (granteeEntity.FetchFromDB(condition)) {
				return;
			}
			granteeEntity.SetDefColumnValues();
			granteeEntity.SetColumnValue("SysAdminOperationId", _operationId);
			granteeEntity.SetColumnValue("SysAdminUnitId", adminUnitId);
			granteeEntity.SetColumnValue("CanExecute", true);
			granteeEntity.SetColumnValue("Position", position);
			granteeEntity.Save();
		}

		#endregion

		#region Methods: Public

		public void Execute(UserConnection userConnection) {
			AddOperationGrantee(userConnection, _administratorAdminUnitId, 1);
			AddOperationGrantee(userConnection, _developerAdminUnitId, 2);
		}

		#endregion

	}

	#endregion

}

