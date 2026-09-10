namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: RuntimeToolExecutionOptionsSchema

	/// <exclude/>
	public class RuntimeToolExecutionOptionsSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public RuntimeToolExecutionOptionsSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public RuntimeToolExecutionOptionsSchema(RuntimeToolExecutionOptionsSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("e7b16a55-2aec-4f54-bb1f-dd1f03757a17");
			Name = "RuntimeToolExecutionOptions";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,143,77,106,3,49,12,133,215,25,152,59,8,178,207,1,154,210,77,8,165,155,118,72,114,1,199,125,25,12,30,121,144,100,104,24,122,247,218,158,80,232,166,27,97,189,31,62,139,221,4,157,157,7,93,32,226,52,221,108,119,72,124,11,99,22,103,33,113,223,45,125,183,201,26,120,164,243,93,13,83,241,99,132,175,166,238,94,193,144,224,247,125,87,82,91,193,88,84,58,68,167,250,68,167,204,22,38,92,82,138,199,47,248,92,27,31,115,235,181,120,96,131,176,139,228,107,254,255,248,102,105,149,95,196,32,105,134,88,64,225,12,249,26,131,95,253,185,189,233,237,200,121,130,184,107,196,179,154,148,207,191,208,9,154,163,13,78,202,201,5,252,94,47,167,133,70,216,158,180,142,239,7,2,252,185,82,218,190,170,127,197,162,253,0,42,122,242,8,57,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("e7b16a55-2aec-4f54-bb1f-dd1f03757a17"));
		}

		#endregion

	}

	#endregion

}

