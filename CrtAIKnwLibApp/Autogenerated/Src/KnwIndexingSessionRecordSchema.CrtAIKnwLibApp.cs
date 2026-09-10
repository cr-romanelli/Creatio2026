namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwIndexingSessionRecordSchema

	/// <exclude/>
	public class KnwIndexingSessionRecordSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwIndexingSessionRecordSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwIndexingSessionRecordSchema(KnwIndexingSessionRecordSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("54666ef2-9009-4739-8b3d-d7d1ddbda60f");
			Name = "KnwIndexingSessionRecord";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("fc5a15eb-1c52-4ea9-828b-8fee971a8bbd");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,181,148,77,111,19,49,16,134,207,32,241,31,70,234,5,164,106,115,39,8,14,11,170,42,132,168,178,225,132,56,56,246,36,49,89,219,203,140,77,26,16,255,157,177,179,27,66,104,171,208,180,167,93,143,231,227,153,119,108,123,229,144,59,165,17,106,66,21,109,168,234,208,217,54,68,248,249,236,233,147,196,214,47,160,217,112,68,55,62,88,87,147,228,163,117,88,53,72,86,181,246,71,142,246,226,37,126,103,132,11,89,64,221,42,230,151,240,222,175,47,189,193,107,9,110,144,89,118,38,168,3,153,226,59,26,141,224,21,39,231,20,109,94,247,235,9,118,132,140,62,50,40,96,175,58,94,10,81,152,203,106,229,195,186,69,179,64,224,144,72,184,109,159,25,120,155,250,92,44,186,77,38,155,172,36,224,168,34,130,242,6,86,184,129,76,44,22,215,113,53,212,30,237,21,255,188,107,102,214,226,151,108,120,171,162,170,131,143,164,116,204,134,46,205,90,171,165,152,18,10,208,185,193,91,251,203,26,74,200,78,142,43,10,29,82,180,40,154,92,149,60,219,253,67,9,138,225,2,5,62,144,84,146,111,92,34,36,111,191,37,233,215,136,46,118,110,145,178,32,121,227,80,1,160,82,189,218,165,222,239,112,104,224,34,89,3,151,130,8,11,140,227,92,101,12,191,30,4,71,20,9,218,138,232,230,159,97,29,129,36,90,54,197,247,20,54,157,136,4,171,159,253,45,50,221,13,179,63,83,201,146,248,157,79,14,154,146,240,222,92,159,166,53,152,225,56,230,163,8,235,37,250,178,53,204,78,144,73,164,187,27,78,14,37,78,37,252,77,6,202,238,31,253,163,64,137,89,129,28,125,207,115,153,239,90,49,232,224,186,22,255,3,112,218,71,211,163,65,14,202,221,11,175,30,2,78,129,35,116,33,254,1,217,187,18,115,10,174,184,224,117,68,242,170,21,39,250,110,143,186,9,253,83,114,202,61,232,193,202,201,127,40,172,114,43,78,129,58,114,148,242,178,70,112,193,100,98,3,207,245,18,245,74,126,102,155,226,249,53,204,94,28,59,227,15,125,146,155,71,124,134,222,108,223,231,178,222,90,255,54,138,237,55,97,45,200,208,40,7,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("54666ef2-9009-4739-8b3d-d7d1ddbda60f"));
		}

		#endregion

	}

	#endregion

}

