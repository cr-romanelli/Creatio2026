namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: McpServerInfoSchema

	/// <exclude/>
	public class McpServerInfoSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public McpServerInfoSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public McpServerInfoSchema(McpServerInfoSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("b1a2c3d4-1374-4006-8a06-000000000006");
			Name = "McpServerInfo";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,145,65,79,2,49,16,133,207,144,240,31,38,112,209,3,187,235,26,61,232,202,101,47,106,2,110,132,120,47,101,22,154,116,219,205,116,144,160,241,191,59,197,69,137,9,199,105,223,123,125,243,213,169,6,67,171,52,194,2,137,84,240,53,39,165,119,181,89,111,73,177,241,110,208,255,28,244,123,35,194,181,12,80,90,21,194,29,76,117,59,71,122,71,122,114,181,31,244,69,144,166,41,20,97,219,52,138,246,147,110,254,145,128,89,161,99,195,123,32,108,61,49,174,192,56,224,13,194,180,172,160,208,19,227,12,27,101,205,7,22,169,158,136,42,180,222,5,76,186,148,138,60,123,237,45,72,86,136,29,106,111,173,223,133,223,136,60,203,111,198,217,245,56,191,133,208,162,134,139,57,19,170,70,45,45,194,227,98,81,117,57,76,202,133,88,0,118,134,55,240,60,127,153,141,95,171,18,242,36,131,154,84,99,220,250,242,248,102,145,158,172,98,28,35,57,101,33,176,16,209,160,35,131,255,8,122,145,82,175,221,46,109,84,72,127,22,57,73,38,204,132,48,60,192,176,148,82,2,244,80,185,138,186,176,145,235,225,253,57,223,91,183,174,88,175,146,44,201,206,43,143,132,78,28,127,76,14,182,175,195,31,141,208,173,126,254,49,142,114,246,13,198,69,134,35,253,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("b1a2c3d4-1374-4006-8a06-000000000006"));
		}

		#endregion

	}

	#endregion

}

