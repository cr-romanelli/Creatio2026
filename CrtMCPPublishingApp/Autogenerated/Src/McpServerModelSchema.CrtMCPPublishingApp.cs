namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: McpServerModelSchema

	/// <exclude/>
	public class McpServerModelSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public McpServerModelSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public McpServerModelSchema(McpServerModelSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("b6e4f274-fd10-408b-96ec-644f1b993794");
			Name = "McpServerModel";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("76c4ad3d-328b-49d6-bd5e-eaf12305041a");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,133,82,193,106,2,49,16,61,43,248,15,131,94,218,67,13,94,213,10,197,66,241,96,43,104,63,32,102,103,215,192,110,18,102,102,45,34,254,123,147,117,149,74,75,189,132,201,204,203,155,55,111,226,116,133,28,180,65,216,32,145,102,159,203,112,238,93,110,139,154,180,88,239,122,221,99,175,219,169,217,186,2,214,7,22,172,38,189,110,204,12,8,139,88,134,121,169,153,199,176,52,97,141,180,71,90,250,12,203,6,161,148,130,41,215,85,165,233,48,107,239,27,210,142,131,39,121,210,133,243,44,214,64,166,69,3,239,116,64,200,61,129,6,211,182,199,12,150,243,21,112,67,219,190,127,152,154,217,181,213,84,153,217,227,16,94,90,8,248,47,199,96,133,83,0,226,125,9,165,101,129,135,209,248,29,246,86,95,36,53,12,155,88,30,222,50,77,82,231,168,23,137,83,228,208,72,100,137,130,90,250,237,33,145,95,7,67,4,67,152,63,247,231,113,226,190,154,65,208,178,139,216,162,66,39,141,80,117,76,165,147,218,143,84,101,194,89,237,229,185,250,97,140,117,130,228,116,9,38,121,249,203,202,206,177,177,243,234,248,138,124,64,18,139,209,246,85,189,45,173,57,215,67,19,195,91,109,51,248,92,100,112,132,2,101,18,21,197,227,116,3,97,161,180,206,141,149,18,239,195,210,16,247,81,175,200,134,108,72,95,230,31,240,54,109,101,193,31,174,180,238,79,210,1,186,236,60,103,115,63,103,111,147,49,247,13,95,93,39,65,182,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("b6e4f274-fd10-408b-96ec-644f1b993794"));
		}

		#endregion

	}

	#endregion

}

