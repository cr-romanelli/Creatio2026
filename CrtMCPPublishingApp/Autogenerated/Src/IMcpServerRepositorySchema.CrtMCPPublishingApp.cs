namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IMcpServerRepositorySchema

	/// <exclude/>
	public class IMcpServerRepositorySchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IMcpServerRepositorySchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IMcpServerRepositorySchema(IMcpServerRepositorySchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("e7ccd1ed-ac09-4ab4-a3c4-cf17ddaa3802");
			Name = "IMcpServerRepository";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("76c4ad3d-328b-49d6-bd5e-eaf12305041a");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,82,203,78,35,49,16,60,7,137,127,104,193,37,145,80,70,92,67,118,14,68,108,148,67,216,8,242,3,142,167,39,49,242,184,71,109,15,218,17,218,127,167,237,121,144,128,246,230,110,87,85,63,170,157,170,208,215,74,35,236,145,89,121,42,195,124,69,174,52,199,134,85,48,228,174,175,62,174,175,38,141,55,238,8,175,173,15,88,61,124,139,5,111,45,234,8,246,243,53,58,100,163,5,35,168,91,198,163,100,97,227,2,114,41,69,22,176,217,234,250,21,249,29,249,5,107,242,38,16,183,9,155,101,25,44,125,83,85,138,219,188,143,119,200,222,72,9,39,237,29,168,113,133,252,65,73,12,219,213,14,124,82,241,48,93,234,124,20,93,102,58,159,129,114,5,132,19,26,238,117,188,166,26,37,69,100,71,252,94,130,136,190,131,251,197,51,188,27,5,63,116,230,176,79,12,197,216,11,213,76,111,50,169,104,145,11,36,253,34,130,102,44,127,221,244,138,91,42,208,222,100,57,76,149,232,222,67,101,152,165,95,42,99,63,195,148,23,245,65,147,109,42,231,103,105,176,3,133,83,132,2,55,46,152,10,35,56,181,157,89,89,68,194,103,95,50,221,143,86,182,83,170,77,141,214,56,76,243,87,202,169,35,86,232,194,124,32,100,103,235,53,209,18,167,44,152,193,155,255,88,51,249,72,246,140,94,110,49,156,168,240,11,216,53,7,107,116,247,57,50,211,252,176,198,208,133,143,237,74,18,83,31,56,158,139,150,247,172,187,140,201,230,5,85,241,199,217,246,235,120,150,231,59,204,163,200,147,83,7,139,69,114,225,55,113,167,57,93,55,166,232,221,223,20,131,222,57,55,82,99,48,82,30,219,103,57,243,75,226,29,244,93,225,223,110,19,145,17,97,131,226,45,186,162,155,57,197,255,186,139,190,72,74,238,19,158,131,180,50,64,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("e7ccd1ed-ac09-4ab4-a3c4-cf17ddaa3802"));
		}

		#endregion

	}

	#endregion

}

