namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSearchItemResponse_TempSchema

	/// <exclude/>
	public class KnwSearchItemResponse_TempSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSearchItemResponse_TempSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSearchItemResponse_TempSchema(KnwSearchItemResponse_TempSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("3097c718-930c-44c3-b160-b371be20a427");
			Name = "KnwSearchItemResponse_Temp";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,181,148,75,79,2,49,16,199,207,146,240,29,38,225,162,7,193,179,248,56,32,33,68,15,4,184,155,210,14,208,216,109,215,62,36,132,248,221,109,203,46,44,187,38,236,70,60,206,179,191,105,255,29,73,18,52,41,161,8,3,141,196,114,213,29,168,148,11,101,219,173,93,187,117,229,12,151,43,152,109,141,197,164,223,110,121,79,71,227,138,43,9,3,65,140,185,135,87,185,25,251,216,11,90,194,69,76,232,245,122,240,96,92,146,16,189,125,202,236,41,166,26,13,74,107,192,174,17,88,204,54,160,150,64,36,112,95,15,92,198,200,135,84,27,129,108,133,96,144,104,186,6,95,150,42,105,176,155,119,238,21,90,167,110,33,56,5,26,80,78,73,222,231,152,164,62,99,23,145,14,208,19,173,82,212,150,163,39,159,196,226,125,188,204,28,29,35,244,184,74,123,146,12,219,73,254,233,16,56,243,131,240,37,71,29,6,8,129,48,65,247,208,167,72,152,35,142,28,103,207,16,248,198,12,118,176,66,219,15,125,251,240,221,4,192,114,43,176,254,161,198,234,240,122,243,88,117,114,40,60,102,193,238,48,73,237,182,223,4,66,40,26,116,34,27,115,188,229,133,53,80,58,40,217,254,205,162,189,191,165,146,179,170,196,89,212,76,184,229,105,38,155,58,138,36,7,145,237,165,184,212,42,241,206,130,20,149,211,52,87,100,45,33,86,65,254,69,144,85,37,150,161,207,171,50,226,198,220,191,40,179,244,91,129,42,105,61,91,45,93,228,87,116,57,137,162,214,222,252,34,194,157,185,128,140,96,24,243,47,118,124,66,232,154,75,188,245,11,149,145,133,200,121,168,98,13,112,6,62,251,114,72,134,42,125,216,28,199,199,114,226,204,19,49,229,194,0,179,88,94,166,185,107,132,80,88,250,245,214,70,117,161,195,209,174,192,72,220,252,82,113,125,83,127,161,120,223,15,202,4,239,30,14,7,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("3097c718-930c-44c3-b160-b371be20a427"));
		}

		#endregion

	}

	#endregion

}

