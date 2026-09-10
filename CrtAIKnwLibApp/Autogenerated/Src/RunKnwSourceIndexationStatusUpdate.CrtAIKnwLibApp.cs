namespace Terrasoft.Core.Process
{

	using Creatio.Copilot;
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

	#region Class: RunKnwSourceIndexationStatusUpdateMethodsWrapper

	/// <exclude/>
	public class RunKnwSourceIndexationStatusUpdateMethodsWrapper : ProcessModel
	{

		public RunKnwSourceIndexationStatusUpdateMethodsWrapper(Process process)
			: base(process) {
			AddScriptTaskMethod("ScriptTask1Execute", ScriptTask1Execute);
		}

		#region Methods: Private

		private bool ScriptTask1Execute(ProcessExecutingContext context) {
			var processor = new KnwSrcIndexationStatusProcessor();
			processor.Process(UserConnection);
			return true;
		}

		#endregion

	}

	#endregion

}

