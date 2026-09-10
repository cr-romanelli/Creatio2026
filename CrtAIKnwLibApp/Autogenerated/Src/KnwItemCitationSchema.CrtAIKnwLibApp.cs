namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwItemCitationSchema

	/// <exclude/>
	public class KnwItemCitationSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwItemCitationSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwItemCitationSchema(KnwItemCitationSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("1125d723-8378-444a-8ecc-7fadb30c045e");
			Name = "KnwItemCitation";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4b4f6d9f-1234-44a2-bd3b-b64923d93c0c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,146,205,74,3,49,16,199,207,45,244,29,6,122,182,123,111,139,32,43,74,81,164,40,62,64,220,204,174,67,119,39,49,147,181,148,226,187,155,100,119,75,109,61,88,79,73,230,243,255,155,9,171,6,197,170,2,33,119,168,60,153,89,110,44,213,198,79,198,251,201,120,50,30,77,29,86,100,24,242,90,137,204,225,129,183,43,143,77,78,62,6,115,10,201,178,12,150,210,54,141,114,187,235,254,253,140,214,161,32,123,129,162,143,5,226,210,184,166,187,135,27,40,216,176,217,214,168,43,4,10,69,103,125,238,171,160,6,111,192,58,243,73,26,65,76,235,130,192,154,120,35,160,88,131,38,177,181,218,1,71,241,169,212,205,234,170,66,70,167,124,72,13,141,173,97,65,25,10,46,179,35,117,182,125,171,169,128,34,242,156,227,140,58,234,3,246,218,25,139,206,19,6,246,117,202,236,252,167,204,201,112,143,1,55,168,145,120,250,119,132,150,233,163,13,112,58,12,130,74,66,7,166,76,142,115,240,115,161,131,82,241,142,184,130,168,115,165,97,15,21,250,69,108,177,128,175,75,180,12,83,52,69,66,61,244,76,211,142,51,28,246,36,113,246,189,228,221,81,234,159,84,190,164,208,199,190,201,255,213,30,175,120,24,218,137,140,187,16,95,82,29,55,19,143,20,186,72,36,130,69,2,153,135,175,80,24,167,147,235,2,249,183,93,243,167,216,251,23,130,41,178,238,190,71,122,119,214,159,198,96,251,6,132,222,0,144,90,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("1125d723-8378-444a-8ecc-7fadb30c045e"));
		}

		#endregion

	}

	#endregion

}

