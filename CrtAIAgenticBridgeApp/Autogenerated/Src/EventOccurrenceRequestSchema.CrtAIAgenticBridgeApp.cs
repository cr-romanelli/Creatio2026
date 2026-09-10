namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: EventOccurrenceRequestSchema

	/// <exclude/>
	public class EventOccurrenceRequestSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public EventOccurrenceRequestSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public EventOccurrenceRequestSchema(EventOccurrenceRequestSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1004-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "EventOccurrenceRequest";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1004-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,144,193,106,2,49,16,134,207,187,176,239,48,120,106,47,190,128,181,176,221,122,240,208,10,186,55,233,33,102,135,16,216,77,210,201,164,160,226,187,59,113,177,104,233,65,72,6,50,252,255,55,127,38,69,235,12,108,246,145,113,152,85,101,186,121,78,215,201,177,29,112,186,65,178,170,183,7,197,214,59,17,85,165,83,3,198,160,52,66,67,92,47,107,131,162,212,111,100,59,131,117,8,87,103,85,30,171,178,216,190,43,86,141,119,76,74,243,151,52,66,218,245,86,131,238,85,140,176,248,17,239,74,235,68,132,78,227,26,191,19,70,126,105,179,233,85,196,153,80,108,151,198,121,194,220,251,192,97,135,148,49,87,78,100,202,161,91,178,198,32,53,190,67,56,130,65,158,65,204,229,148,3,23,99,138,209,252,244,41,241,97,14,19,204,179,219,125,192,108,154,60,255,3,93,220,42,30,197,250,241,55,93,205,247,76,145,98,43,107,129,213,175,224,81,100,39,173,123,216,101,65,112,41,127,24,133,92,57,103,2,193,151,68,218,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1004-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

