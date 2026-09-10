namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: RunAgenticProcessRequestSchema

	/// <exclude/>
	public class RunAgenticProcessRequestSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public RunAgenticProcessRequestSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public RunAgenticProcessRequestSchema(RunAgenticProcessRequestSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("ef6a56d2-e61b-4e70-9ed3-4239bee2e53a");
			Name = "RunAgenticProcessRequest";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("a1cfbc7f-e847-4cb1-8f54-a86ec756c98c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,146,207,78,2,49,16,198,207,108,178,239,208,112,210,196,236,11,160,38,8,28,56,160,27,48,122,32,28,134,50,172,53,221,237,58,51,107,130,196,119,119,186,2,162,225,128,73,219,244,207,247,253,230,107,211,134,93,85,152,217,134,5,203,108,16,188,71,43,46,84,156,61,44,95,117,58,9,43,244,189,52,105,142,101,211,166,18,87,98,54,67,114,224,221,7,68,199,65,244,136,68,192,97,45,138,163,86,244,238,44,182,160,65,168,132,192,138,106,211,164,130,18,185,6,139,102,64,210,31,247,11,84,170,189,35,183,42,176,95,215,217,72,181,155,60,184,74,56,123,198,229,142,195,105,178,77,147,206,124,8,2,123,220,66,55,234,102,233,157,53,214,3,179,209,128,59,92,78,65,61,60,197,183,6,89,84,23,205,223,238,9,150,75,164,139,123,141,97,110,76,151,237,11,150,16,87,221,203,8,220,19,89,168,189,250,225,216,108,77,129,210,51,28,135,207,120,147,211,64,66,110,188,228,64,186,22,164,184,205,221,43,51,42,157,12,113,13,122,246,4,190,137,210,53,120,198,19,69,231,11,51,61,1,57,55,64,189,119,181,117,206,174,253,243,9,174,35,169,21,230,224,232,214,228,191,129,231,230,176,129,8,125,251,73,198,171,255,189,128,134,57,242,254,45,216,209,174,237,11,188,137,155,137,195,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("ef6a56d2-e61b-4e70-9ed3-4239bee2e53a"));
		}

		#endregion

	}

	#endregion

}

