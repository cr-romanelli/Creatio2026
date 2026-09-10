namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IKnwProviderFactorySchema

	/// <exclude/>
	public class IKnwProviderFactorySchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IKnwProviderFactorySchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IKnwProviderFactorySchema(IKnwProviderFactorySchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f35ca9e1-d8d1-4a27-96fe-363192c8f392");
			Name = "IKnwProviderFactory";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4b4f6d9f-1234-44a2-bd3b-b64923d93c0c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,173,84,61,111,219,48,16,157,109,192,255,225,160,46,242,34,238,137,44,160,117,208,194,40,138,6,104,182,162,3,35,157,108,162,20,41,28,41,7,110,224,255,94,138,34,101,37,81,11,15,217,172,251,120,239,221,189,163,21,111,208,180,188,68,216,18,114,43,116,182,213,173,144,218,174,150,207,171,229,162,51,66,237,225,1,137,184,209,181,117,73,194,236,51,47,173,38,129,230,118,181,116,53,31,8,247,66,43,216,41,139,84,59,168,27,216,125,85,79,247,164,143,162,66,26,170,79,190,148,49,6,185,233,154,134,211,169,8,223,33,15,34,182,67,173,9,8,141,150,199,158,252,183,210,79,18,171,61,66,27,16,65,52,173,196,6,149,237,5,43,227,27,246,226,136,106,82,108,116,71,37,154,44,178,178,9,109,219,61,74,81,78,24,103,245,46,158,189,230,113,190,111,104,15,186,50,55,112,239,219,135,228,235,137,124,192,175,18,13,240,89,241,202,88,174,28,233,176,91,123,112,90,91,44,69,45,176,114,73,97,5,151,226,143,159,12,116,235,7,204,70,38,246,154,42,111,57,241,6,148,179,113,147,132,242,164,120,112,160,243,80,126,87,61,231,91,101,89,206,60,216,5,155,208,118,164,76,241,81,185,25,17,161,36,172,55,201,116,89,9,43,46,3,69,232,193,138,168,61,103,17,166,199,157,54,135,61,77,34,233,228,247,238,133,254,239,65,126,64,93,223,6,111,80,85,131,61,254,251,60,28,228,203,96,244,111,43,185,113,238,253,239,52,133,58,32,9,91,233,114,102,214,80,157,0,235,39,249,121,135,53,239,164,253,36,84,229,124,76,237,169,69,93,167,51,13,235,245,47,87,239,143,77,113,9,101,47,99,70,5,252,227,217,92,127,134,215,168,207,222,236,60,204,19,31,197,123,25,4,253,223,199,98,113,228,116,57,253,205,224,65,148,242,5,109,62,165,43,210,120,51,46,248,195,191,223,44,230,182,186,194,222,116,7,57,222,235,40,0,211,201,89,184,138,225,224,70,94,31,60,95,125,48,231,191,149,154,69,199,20,5,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f35ca9e1-d8d1-4a27-96fe-363192c8f392"));
		}

		#endregion

	}

	#endregion

}

