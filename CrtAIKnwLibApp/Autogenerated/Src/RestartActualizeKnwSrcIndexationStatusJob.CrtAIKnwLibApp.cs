namespace Terrasoft.Core.Process
{

	using Creatio.Copilot;
	using Factories;
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
	using Terrasoft.Core.Process;
	using Terrasoft.Core.Process.Configuration;

	#region Class: RestartActualizeKnwSrcIndexationStatusJobMethodsWrapper

	/// <exclude/>
	public class RestartActualizeKnwSrcIndexationStatusJobMethodsWrapper : ProcessModel
	{

		public RestartActualizeKnwSrcIndexationStatusJobMethodsWrapper(Process process)
			: base(process) {
			AddScriptTaskMethod("RestartQuartzJobSTExecute", RestartQuartzJobSTExecute);
		}

		#region Methods: Private

		private bool RestartQuartzJobSTExecute(ProcessExecutingContext context) {
			
			var appSchedulerWrapper = ClassFactory.Get<IAppSchedulerWraper>();
			var indexationStatusJob = ClassFactory.Get<IKnwSrcIndexationStatusJobActualizer>();
			indexationStatusJob.Register(context.UserConnection, appSchedulerWrapper);
			return true;
		}

		#endregion

	}

	#endregion

}

