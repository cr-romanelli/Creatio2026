namespace Terrasoft.Core.Process.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Process;

	#region Class: SegmentActualizeUserTask

	[DesignModeProperty(Name = "Success", Group = "", ValuesProvider = "ProcessSchemaParameterValueProvider", Editor="xtype=processschemaparametervalueedit;dataProvider=processschemaparametervalueprovider", ResourceManager = "b24b5549cecd4b4b8d03b0dc27779249", CaptionResourceItem = "Parameters.Success.Caption", DescriptionResourceItem = "Parameters.Success.Caption", UseSolutionStorage = true)]
	[DesignModeProperty(Name = "ErrorMessage", Group = "", ValuesProvider = "ProcessSchemaParameterValueProvider", Editor="xtype=processschemaparametervalueedit;dataProvider=processschemaparametervalueprovider", ResourceManager = "b24b5549cecd4b4b8d03b0dc27779249", CaptionResourceItem = "Parameters.ErrorMessage.Caption", DescriptionResourceItem = "Parameters.ErrorMessage.Caption", UseSolutionStorage = true)]
	[DesignModeProperty(Name = "SegmentId", Group = "", ValuesProvider = "ProcessSchemaParameterValueProvider", Editor="xtype=processschemaparametervalueedit;dataProvider=processschemaparametervalueprovider", ResourceManager = "b24b5549cecd4b4b8d03b0dc27779249", CaptionResourceItem = "Parameters.SegmentId.Caption", DescriptionResourceItem = "Parameters.SegmentId.Caption", UseSolutionStorage = true)]
	[DesignModeProperty(Name = "ExcludeCurrentMembersBeforeActualization", Group = "", ValuesProvider = "ProcessSchemaParameterValueProvider", Editor="xtype=processschemaparametervalueedit;dataProvider=processschemaparametervalueprovider", ResourceManager = "b24b5549cecd4b4b8d03b0dc27779249", CaptionResourceItem = "Parameters.ExcludeCurrentMembersBeforeActualization.Caption", DescriptionResourceItem = "Parameters.ExcludeCurrentMembersBeforeActualization.Caption", UseSolutionStorage = true)]
	/// <exclude/>
	public partial class SegmentActualizeUserTask : ProcessUserTask
	{

		#region Constructors: Public

		public SegmentActualizeUserTask(UserConnection userConnection)
			: base(userConnection) {
			SchemaUId = new Guid("b24b5549-cecd-4b4b-8d03-b0dc27779249");
		}

		#endregion

		#region Properties: Public

		public virtual bool Success {
			get;
			set;
		}

		public virtual string ErrorMessage {
			get;
			set;
		}

		public virtual Guid SegmentId {
			get;
			set;
		}

		public virtual bool ExcludeCurrentMembersBeforeActualization {
			get;
			set;
		}

		#endregion

		#region Methods: Public

		public override void WritePropertiesData(DataWriter writer) {
			writer.WriteStartObject(Name);
			base.WritePropertiesData(writer);
			if (Status == Core.Process.ProcessStatus.Inactive) {
				writer.WriteFinishObject();
				return;
			}
			if (UseFlowEngineMode) {
				if (!HasMapping("Success")) {
					writer.WriteValue("Success", Success, false);
				}
			}
			if (UseFlowEngineMode) {
				if (!HasMapping("ErrorMessage")) {
					writer.WriteValue("ErrorMessage", ErrorMessage, null);
				}
			}
			if (UseFlowEngineMode) {
				if (!HasMapping("SegmentId")) {
					writer.WriteValue("SegmentId", SegmentId, Guid.Empty);
				}
			}
			if (UseFlowEngineMode) {
				if (!HasMapping("ExcludeCurrentMembersBeforeActualization")) {
					writer.WriteValue("ExcludeCurrentMembersBeforeActualization", ExcludeCurrentMembersBeforeActualization, false);
				}
			}
			writer.WriteFinishObject();
		}

		#endregion

		#region Methods: Protected

		protected override void ApplyPropertiesDataValues(DataReader reader) {
			base.ApplyPropertiesDataValues(reader);
			switch (reader.CurrentName) {
				case "Success":
					if (!UseFlowEngineMode) {
						break;
					}
					Success = reader.GetBoolValue();
				break;
				case "ErrorMessage":
					if (!UseFlowEngineMode) {
						break;
					}
					ErrorMessage = reader.GetStringValue();
				break;
				case "SegmentId":
					if (!UseFlowEngineMode) {
						break;
					}
					SegmentId = reader.GetGuidValue();
				break;
				case "ExcludeCurrentMembersBeforeActualization":
					if (!UseFlowEngineMode) {
						break;
					}
					ExcludeCurrentMembersBeforeActualization = reader.GetBoolValue();
				break;
			}
		}

		#endregion

	}

	#endregion

}

