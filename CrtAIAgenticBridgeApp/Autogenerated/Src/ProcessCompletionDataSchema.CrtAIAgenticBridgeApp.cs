namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: ProcessCompletionDataSchema

	/// <exclude/>
	public class ProcessCompletionDataSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public ProcessCompletionDataSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public ProcessCompletionDataSchema(ProcessCompletionDataSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("57994029-c85e-45fb-ab11-73424ab73c22");
			Name = "ProcessCompletionData";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("a1cfbc7f-e847-4cb1-8f54-a86ec756c98c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,147,193,106,195,48,12,134,207,11,228,29,76,79,27,148,190,64,183,65,151,118,165,135,142,210,194,46,99,7,213,81,51,15,59,14,146,124,232,202,222,125,118,75,71,8,57,164,16,7,100,253,250,254,223,33,14,108,234,74,237,142,44,232,166,121,22,90,229,164,240,214,162,22,227,107,158,44,177,70,50,186,43,217,134,90,140,195,201,46,54,193,154,31,72,234,40,202,179,26,28,114,3,26,85,65,50,91,205,42,140,74,253,66,166,172,112,214,52,215,201,60,59,229,217,221,199,28,4,10,95,11,129,150,207,184,209,132,189,53,90,105,11,204,106,67,94,35,115,225,93,99,49,25,36,117,20,165,201,203,232,26,221,30,233,254,45,122,170,39,53,210,158,8,237,57,203,170,28,61,36,224,149,200,66,41,126,209,86,168,147,170,80,166,138,211,235,55,133,239,199,54,151,28,93,228,50,152,242,154,113,56,140,245,23,58,72,85,111,192,221,127,123,48,80,64,2,247,195,206,173,161,32,36,242,180,142,135,129,10,71,99,181,112,70,230,120,128,96,229,29,108,72,146,3,88,198,62,163,69,107,116,168,221,1,140,197,114,97,209,197,63,228,70,191,215,246,236,45,31,139,144,35,127,3,20,107,65,226,161,190,171,185,57,95,8,160,227,227,37,195,88,249,253,119,188,37,207,106,219,97,118,179,220,197,21,159,63,56,48,168,232,114,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("57994029-c85e-45fb-ab11-73424ab73c22"));
		}

		#endregion

	}

	#endregion

}

