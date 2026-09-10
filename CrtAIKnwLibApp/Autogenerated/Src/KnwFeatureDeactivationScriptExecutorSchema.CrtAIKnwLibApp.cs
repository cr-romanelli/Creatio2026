namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwFeatureDeactivationScriptExecutorSchema

	/// <exclude/>
	public class KnwFeatureDeactivationScriptExecutorSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwFeatureDeactivationScriptExecutorSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwFeatureDeactivationScriptExecutorSchema(KnwFeatureDeactivationScriptExecutorSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("19bdb9eb-660d-419f-9d6e-da61fa66fd66");
			Name = "KnwFeatureDeactivationScriptExecutor";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("53a07838-beb2-4c00-80aa-44a4ef826a1b");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,83,77,107,227,48,16,61,187,208,255,48,120,23,234,66,113,238,253,130,226,102,67,216,46,44,77,122,90,246,160,200,99,71,32,75,70,31,105,77,200,127,223,145,101,167,73,72,97,47,182,52,122,154,247,222,204,72,177,6,109,203,56,66,97,144,57,161,243,66,183,66,106,119,121,177,189,188,72,188,21,170,134,69,103,29,54,119,251,253,18,141,97,86,87,142,192,6,191,138,231,63,24,119,218,8,180,95,35,136,210,27,92,234,186,150,116,76,56,66,126,51,88,11,173,160,144,204,218,91,248,169,222,7,216,51,82,66,177,9,42,213,130,27,209,186,233,7,114,79,28,253,189,201,100,2,247,214,55,13,51,221,227,176,143,0,180,192,192,246,55,192,105,48,104,145,22,107,132,18,43,230,165,3,235,152,67,208,21,216,22,185,168,4,150,80,69,78,11,66,69,40,115,108,197,44,230,35,211,228,128,170,245,43,41,56,240,160,248,191,4,195,45,204,231,138,104,165,60,117,146,108,123,55,251,50,252,66,183,214,37,21,226,119,79,18,15,123,5,66,173,209,8,87,106,62,9,34,70,21,27,45,202,209,120,246,102,209,20,90,41,228,65,4,248,163,237,53,132,30,39,201,134,153,189,223,165,158,42,182,146,8,15,160,240,253,207,223,1,146,164,51,84,79,243,193,152,205,35,136,172,190,162,163,22,111,152,76,123,220,238,174,255,205,159,87,3,116,17,74,91,172,153,170,209,64,121,54,250,16,91,29,199,165,203,103,232,238,207,223,127,204,162,22,18,6,228,194,58,227,195,149,39,83,251,6,149,203,210,99,123,233,205,169,223,235,168,174,162,217,99,124,13,25,101,8,83,57,120,47,116,137,161,221,167,165,24,203,148,56,211,141,203,228,172,149,252,53,140,214,225,65,118,144,251,6,102,94,148,249,180,105,93,55,40,73,18,42,225,139,88,189,57,33,109,254,66,239,128,146,204,85,165,179,239,233,144,6,174,182,7,57,118,87,251,161,165,154,123,28,134,217,122,206,209,218,202,75,217,229,233,152,124,7,156,185,224,115,250,193,177,237,7,224,211,204,57,230,169,49,218,4,106,38,36,189,129,253,91,169,206,75,201,169,194,184,39,139,253,15,223,221,48,193,168,202,56,196,253,62,70,143,131,20,251,7,9,15,203,247,130,4,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("19bdb9eb-660d-419f-9d6e-da61fa66fd66"));
		}

		#endregion

	}

	#endregion

}

