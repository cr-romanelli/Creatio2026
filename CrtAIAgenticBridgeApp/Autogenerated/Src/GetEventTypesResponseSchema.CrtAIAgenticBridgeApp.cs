namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: GetEventTypesResponseSchema

	/// <exclude/>
	public class GetEventTypesResponseSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public GetEventTypesResponseSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public GetEventTypesResponseSchema(GetEventTypesResponseSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0006-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "GetEventTypesResponse";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0006-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,145,81,75,195,48,20,133,159,87,232,127,8,123,210,135,246,15,12,31,102,87,70,31,20,89,39,62,140,61,164,233,165,6,218,164,36,55,131,58,252,239,222,116,115,58,141,58,161,4,202,61,249,114,207,57,206,74,213,176,114,176,8,221,44,142,220,167,223,116,229,20,202,14,210,18,140,228,173,124,225,40,181,34,81,28,41,222,129,237,185,0,150,25,156,23,243,6,72,41,110,141,172,27,152,247,125,154,43,52,195,131,150,10,109,250,4,21,1,118,82,128,77,23,168,227,104,31,71,147,205,130,35,207,52,201,184,192,171,123,194,177,27,54,109,0,19,216,17,43,193,161,7,155,24,122,68,43,11,211,235,45,221,233,93,213,74,193,68,203,173,101,75,192,220,43,215,94,184,58,234,72,228,225,7,250,29,116,21,152,19,27,141,108,26,48,153,174,143,184,119,158,165,9,121,94,127,204,217,158,209,38,51,102,253,241,234,253,134,145,146,66,178,231,176,211,78,228,116,179,101,133,87,124,197,77,14,200,112,4,62,72,28,18,43,158,161,227,137,208,173,235,84,192,126,62,202,202,81,149,141,162,49,218,159,237,187,162,62,223,116,233,100,205,30,139,250,82,179,130,247,190,254,96,118,217,97,246,47,163,99,215,1,179,127,182,254,205,249,69,245,135,186,10,101,248,91,105,244,189,1,192,251,85,197,47,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0006-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

