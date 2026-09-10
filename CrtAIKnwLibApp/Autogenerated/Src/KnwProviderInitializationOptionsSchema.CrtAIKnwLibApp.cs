namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwProviderInitializationOptionsSchema

	/// <exclude/>
	public class KnwProviderInitializationOptionsSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwProviderInitializationOptionsSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwProviderInitializationOptionsSchema(KnwProviderInitializationOptionsSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("dae91811-5172-436b-a62b-347ac43bf735");
			Name = "KnwProviderInitializationOptions";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4b4f6d9f-1234-44a2-bd3b-b64923d93c0c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,147,205,110,219,48,16,132,207,54,224,119,88,184,151,22,48,172,123,253,115,241,161,40,122,104,144,38,15,192,146,43,103,17,153,20,150,84,132,214,240,187,119,69,234,199,82,19,196,200,81,36,61,251,205,236,216,170,19,250,82,105,132,3,163,10,228,214,7,87,82,225,194,98,126,94,204,103,149,39,123,132,7,100,86,222,229,65,46,25,55,139,185,220,124,98,60,146,179,112,40,148,247,95,225,135,117,117,129,230,136,119,236,94,200,32,255,44,69,204,250,248,54,203,50,216,250,234,116,82,252,103,223,126,127,183,20,72,21,244,183,25,106,193,165,231,144,59,6,5,207,157,26,148,173,220,10,200,234,162,50,13,78,120,66,240,174,98,129,102,212,142,13,40,107,64,59,107,81,71,49,131,65,81,225,215,221,232,236,106,118,89,253,46,72,131,110,176,133,186,238,120,199,60,61,253,236,28,29,12,118,229,52,112,165,131,99,113,125,23,197,210,139,169,201,177,75,244,98,203,98,45,54,124,80,86,208,93,30,141,108,61,34,104,198,124,183,124,143,102,153,237,19,119,167,94,83,120,74,105,148,168,41,39,52,109,46,171,171,52,86,49,157,46,199,117,143,154,77,89,183,165,98,117,2,43,141,216,45,43,143,124,232,53,150,251,7,153,242,40,103,48,28,66,112,226,166,243,23,57,186,41,145,108,189,205,162,226,235,3,158,109,253,43,178,38,237,97,227,237,102,111,21,135,40,223,110,245,189,4,63,63,142,108,193,216,229,170,249,125,130,186,79,189,234,33,191,64,243,111,152,205,250,7,176,27,46,55,241,106,34,189,155,136,199,71,151,182,76,104,77,234,211,184,92,194,94,34,7,194,91,170,245,13,131,143,185,52,115,174,219,47,5,113,154,84,144,54,244,5,233,179,163,81,44,111,180,161,77,115,226,104,242,121,134,35,134,13,92,110,65,252,111,187,194,108,226,127,253,67,96,211,53,13,91,25,67,77,114,78,167,227,195,203,63,81,60,26,153,1,5,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("dae91811-5172-436b-a62b-347ac43bf735"));
		}

		#endregion

	}

	#endregion

}

