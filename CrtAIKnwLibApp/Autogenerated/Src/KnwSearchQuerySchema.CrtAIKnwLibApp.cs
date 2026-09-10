namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSearchQuerySchema

	/// <exclude/>
	public class KnwSearchQuerySchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSearchQuerySchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSearchQuerySchema(KnwSearchQuerySchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("46cd36b5-57e4-4ffb-98d7-b0708283d464");
			Name = "KnwSearchQuery";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d5e01d64-57e6-47d0-82bc-33ac8cbaf486");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,165,148,205,78,227,48,16,199,207,69,226,29,70,226,188,233,114,221,2,151,170,170,16,23,216,178,15,224,198,147,98,213,177,179,51,182,186,81,181,239,206,216,9,208,6,84,41,244,20,205,231,255,231,120,198,78,213,200,141,42,17,230,132,42,24,95,204,125,99,172,15,151,23,251,203,139,73,100,227,54,176,106,57,96,61,27,216,146,105,45,150,82,227,184,88,162,67,50,165,228,72,214,21,225,70,188,48,183,138,249,23,60,184,221,10,21,149,47,79,17,169,205,25,211,233,20,110,56,214,181,162,246,174,183,127,99,67,200,232,2,131,2,206,5,240,55,85,64,229,9,194,11,194,214,249,157,69,189,65,88,43,198,226,173,207,244,160,145,113,1,201,41,11,101,210,254,36,61,217,103,249,119,194,71,242,13,82,48,40,152,143,113,109,77,217,197,135,124,217,177,68,65,19,20,78,223,99,30,246,145,228,31,26,45,248,166,50,72,197,123,151,67,186,73,147,53,96,25,141,206,108,185,236,94,195,30,54,24,102,169,243,12,254,143,65,96,100,78,7,25,165,188,234,138,206,211,253,184,159,211,138,28,40,205,76,190,128,99,61,184,237,131,197,162,110,66,59,27,163,239,98,189,70,2,95,129,145,81,20,159,7,222,154,6,140,59,196,147,121,138,54,240,105,64,25,25,88,165,218,33,220,207,115,137,130,218,34,84,228,235,111,49,61,167,234,33,211,245,56,40,107,56,36,164,45,182,59,79,58,83,85,198,202,142,140,194,185,95,200,233,144,212,218,226,77,119,101,119,240,240,214,242,91,51,228,155,244,110,200,158,162,211,63,34,11,79,233,101,117,255,133,188,236,74,107,211,199,181,223,57,145,68,85,247,224,105,94,78,194,202,94,45,156,254,35,61,231,125,203,129,249,5,240,149,96,116,79,66,182,59,239,177,83,124,175,93,128,126,84,45,5,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("46cd36b5-57e4-4ffb-98d7-b0708283d464"));
		}

		#endregion

	}

	#endregion

}

