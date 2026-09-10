namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: McpPingHandlerSchema

	/// <exclude/>
	public class McpPingHandlerSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public McpPingHandlerSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public McpPingHandlerSchema(McpPingHandlerSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("b1a2c3d4-1374-400b-8a0b-00000000000b");
			Name = "McpPingHandler";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,145,205,106,195,48,16,132,207,54,228,29,22,122,73,160,216,247,196,248,146,64,219,67,168,105,250,2,138,178,73,84,228,149,208,74,41,38,244,221,187,254,105,232,15,189,121,103,180,163,111,44,82,45,178,87,26,225,21,67,80,236,142,177,88,59,58,154,83,10,42,26,71,179,252,58,203,179,196,134,78,176,235,56,98,43,190,181,168,123,147,139,7,36,12,70,175,102,185,156,186,11,120,18,21,214,86,49,47,97,171,125,35,107,143,138,14,22,195,112,162,44,75,168,56,181,173,10,93,61,205,163,207,176,93,55,80,233,218,203,74,85,234,186,128,23,140,41,16,131,34,192,214,199,14,220,254,77,238,5,143,1,216,163,94,129,182,6,41,242,20,148,24,33,158,13,67,116,112,17,170,99,39,35,130,118,68,35,46,136,167,172,185,96,241,133,82,126,99,49,20,49,144,178,146,42,248,191,232,97,9,79,162,108,49,158,221,225,214,40,187,14,173,110,197,71,91,170,55,105,111,141,30,77,63,124,247,121,211,222,115,138,218,181,56,21,159,111,204,0,39,20,21,199,32,87,222,79,69,107,240,42,200,251,8,22,47,160,127,134,44,11,195,63,249,27,86,236,146,214,200,60,39,124,135,255,19,231,139,197,170,207,249,152,184,145,14,35,250,48,143,234,79,81,180,79,0,171,138,113,36,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("b1a2c3d4-1374-400b-8a0b-00000000000b"));
		}

		#endregion

	}

	#endregion

}

