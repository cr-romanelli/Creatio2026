namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwProviderConnectionRecordSchema

	/// <exclude/>
	public class KnwProviderConnectionRecordSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwProviderConnectionRecordSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwProviderConnectionRecordSchema(KnwProviderConnectionRecordSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("60b21c78-ce03-4a9e-a4d4-c6defa844e54");
			Name = "KnwProviderConnectionRecord";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4b4f6d9f-1234-44a2-bd3b-b64923d93c0c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,84,205,110,218,64,16,62,131,196,59,140,168,84,37,18,194,247,18,144,42,14,17,65,74,81,210,62,192,178,30,200,40,102,214,221,93,23,81,196,187,119,118,109,8,24,228,64,47,150,61,204,126,63,179,223,192,106,133,46,87,26,97,108,81,121,50,253,177,201,41,51,190,211,222,118,218,173,194,17,47,225,117,227,60,174,6,157,182,84,190,88,92,146,97,24,103,202,185,111,48,229,245,204,154,63,148,162,29,27,102,212,2,193,47,168,141,77,99,123,146,36,240,224,138,213,74,217,205,168,250,126,193,220,162,67,246,14,20,232,195,41,176,241,24,44,140,149,250,59,155,117,134,233,18,33,175,240,123,64,172,179,34,13,138,142,78,41,78,65,21,254,77,240,72,171,88,202,149,21,91,30,173,235,239,37,36,71,26,242,98,158,145,6,29,28,52,27,104,109,163,137,15,211,134,157,183,133,246,198,138,247,9,11,5,171,172,236,169,59,141,133,9,147,39,149,209,95,12,94,25,215,226,193,121,197,50,111,179,0,17,45,71,16,65,91,92,12,187,13,82,186,201,168,210,187,38,255,22,15,186,28,53,45,8,211,154,221,115,191,101,37,118,1,75,231,176,75,105,119,244,83,48,10,166,223,5,130,48,202,240,4,203,238,85,157,221,74,255,33,137,0,151,241,194,179,68,12,111,255,9,114,184,241,253,16,38,159,169,60,207,72,51,195,135,160,89,168,186,238,232,187,157,147,183,50,39,120,122,253,241,28,20,123,69,44,12,33,132,87,135,236,152,148,170,80,52,5,235,238,177,160,84,252,244,64,210,20,226,28,228,245,32,86,47,76,225,208,86,215,127,15,97,71,91,173,73,10,67,129,27,196,143,231,112,3,195,8,89,22,166,231,136,242,251,5,158,178,125,92,35,145,222,58,111,108,220,133,216,199,229,64,78,203,253,56,93,22,1,206,209,122,66,89,149,89,92,185,134,69,121,68,249,55,240,183,69,242,114,216,171,237,142,195,20,167,91,88,162,31,192,238,26,238,230,240,54,210,85,55,20,135,127,11,229,45,217,254,220,239,165,171,190,66,205,181,91,240,53,238,64,153,124,7,119,216,95,246,97,174,28,254,178,89,15,84,78,83,220,220,95,53,166,179,140,157,138,172,37,170,172,158,22,119,255,0,26,31,95,37,185,6,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("60b21c78-ce03-4a9e-a4d4-c6defa844e54"));
		}

		#endregion

	}

	#endregion

}

