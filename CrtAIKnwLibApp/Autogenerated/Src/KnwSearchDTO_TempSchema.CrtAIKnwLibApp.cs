namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSearchDTO_TempSchema

	/// <exclude/>
	public class KnwSearchDTO_TempSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSearchDTO_TempSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSearchDTO_TempSchema(KnwSearchDTO_TempSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("d9bc7ee8-6b79-4480-ab99-934982ad705a");
			Name = "KnwSearchDTO_Temp";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,146,77,107,2,49,16,134,207,21,252,15,3,94,90,40,187,247,174,45,20,21,145,82,42,213,123,137,113,92,211,102,147,48,201,34,139,248,223,59,201,174,162,22,132,158,194,124,63,239,76,140,168,208,59,33,17,70,132,34,40,155,141,172,83,218,134,126,111,223,239,221,213,94,153,18,22,141,15,88,21,87,54,103,106,141,146,107,140,207,166,104,144,148,228,28,206,26,16,150,236,133,145,22,222,63,193,155,217,45,80,144,220,142,151,31,41,158,231,57,12,125,93,85,130,154,151,206,30,139,32,32,144,48,126,131,4,118,245,205,157,129,208,17,122,52,33,78,21,224,83,23,246,250,90,7,144,214,4,161,76,12,253,24,187,211,184,46,17,20,131,249,236,56,35,63,27,226,234,149,86,18,100,100,186,64,250,90,98,229,56,97,159,216,78,240,115,178,14,41,40,100,5,243,84,219,198,175,225,147,99,138,193,131,37,38,228,55,108,145,225,142,187,1,187,57,227,235,36,68,204,168,195,241,238,48,241,254,5,62,18,207,38,166,174,144,196,74,227,240,196,61,227,6,159,93,125,18,240,18,53,69,175,135,61,148,24,138,200,82,192,1,158,225,149,72,52,217,164,114,161,185,213,224,254,161,248,143,66,36,98,147,127,143,23,172,139,151,106,165,18,1,215,176,83,97,155,50,46,206,245,8,106,3,194,52,183,181,250,64,241,158,147,216,251,189,107,125,45,167,205,105,245,116,196,3,52,235,246,108,201,62,180,191,240,194,201,190,95,171,179,235,228,236,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("d9bc7ee8-6b79-4480-ab99-934982ad705a"));
		}

		#endregion

	}

	#endregion

}

