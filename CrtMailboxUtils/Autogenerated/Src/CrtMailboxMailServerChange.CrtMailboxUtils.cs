namespace Terrasoft.Core.Process
{

	using IntegrationApi.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Drawing;
	using System.Globalization;
	using System.Text;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;
	using Terrasoft.Core.Process.Configuration;

	#region Class: CrtMailboxMailServerChangeMethodsWrapper

	/// <exclude/>
	public class CrtMailboxMailServerChangeMethodsWrapper : ProcessModel
	{

		public CrtMailboxMailServerChangeMethodsWrapper(Process process)
			: base(process) {
			AddScriptTaskMethod("ScriptTask1Execute", ScriptTask1Execute);
		}

		#region Methods: Private

		private bool ScriptTask1Execute(ProcessExecutingContext context) {
			var uc = Get<UserConnection>("UserConnection");
			var managerFactory = ClassFactory.Get<IListenerManagerFactory>();
			var manager =  managerFactory.GetExchangeListenerManager(uc);
			var mailboxId = Get<Guid>("Mailbox");
			var senderEmailAddress = Get<string>("SenderEmailAddress");
			var sysAdminUnitId = Get<Guid>("OldOwnerId");
			manager.StopListener(mailboxId);
			var appSchedulerWraper = ClassFactory.Get<IAppSchedulerWraper>();
			appSchedulerWraper.RemoveJob($"{senderEmailAddress}_SyncExchangeActivitiesProcess_{sysAdminUnitId}", "Exchange");
			return true;
		}

		#endregion

	}

	#endregion

}

