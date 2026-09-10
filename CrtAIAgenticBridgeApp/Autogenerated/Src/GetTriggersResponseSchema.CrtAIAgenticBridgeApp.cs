namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: GetTriggersResponseSchema

	/// <exclude/>
	public class GetTriggersResponseSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public GetTriggersResponseSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public GetTriggersResponseSchema(GetTriggersResponseSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0004-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "GetTriggersResponse";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0004-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,61,143,193,138,194,64,12,134,207,22,250,14,193,211,122,112,94,64,60,116,85,22,15,46,162,130,7,241,144,142,97,8,180,51,101,146,10,42,251,238,155,98,119,33,201,233,251,191,36,189,112,12,112,124,136,82,235,14,125,84,110,201,29,41,51,54,252,68,229,20,23,101,81,22,17,91,146,14,61,193,42,107,181,173,2,25,233,63,51,223,2,85,93,231,54,81,243,99,159,56,170,184,51,213,38,184,179,39,113,107,77,101,241,42,139,201,101,141,138,171,100,24,122,253,248,54,29,44,97,26,72,231,154,57,4,202,50,207,182,33,69,161,233,236,106,129,174,175,27,246,224,27,20,129,47,210,211,136,29,70,202,144,193,251,22,239,168,173,41,255,107,217,158,145,183,230,207,51,198,119,164,120,179,128,221,117,185,194,118,224,224,5,118,198,2,100,24,63,22,177,182,250,5,110,178,99,139,25,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0004-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

