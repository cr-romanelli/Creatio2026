namespace Terrasoft.Core.Process.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Creatio.Segmentation;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Process;

	#region Class: SegmentActualizeUserTask

	/// <exclude/>
	public partial class SegmentActualizeUserTask
	{

		#region Methods: Protected

		protected override bool InternalExecute(ProcessExecutingContext context) {
			try {
				var jobManager = ClassFactory.Get<ISegmentActualizationJobManager>();
				jobManager.ScheduleActualization(SegmentId, ExcludeCurrentMembersBeforeActualization);
				Success = true;
			} catch (Exception ex) {
				Success = false;
				ErrorMessage = ex.ToString();
			}
			return true;
		}

		#endregion

		#region Methods: Public

		public override bool CompleteExecuting(params object[] parameters) {
			return base.CompleteExecuting(parameters);
		}

		public override void CancelExecuting(params object[] parameters) {
			base.CancelExecuting(parameters);
		}

		public override string GetExecutionData() {
			return string.Empty;
		}

		public override ProcessElementNotification GetNotificationData() {
			return base.GetNotificationData();
		}

		#endregion

	}

	#endregion

}

