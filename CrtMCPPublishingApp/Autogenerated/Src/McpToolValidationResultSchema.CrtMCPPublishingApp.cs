namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: McpToolValidationResultSchema

	/// <exclude/>
	public class McpToolValidationResultSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public McpToolValidationResultSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public McpToolValidationResultSchema(McpToolValidationResultSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("1977b176-8e60-4bdb-aba7-9fed0fd83423");
			Name = "McpToolValidationResult";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,144,65,75,3,49,16,133,207,91,232,127,24,241,178,123,217,31,224,218,130,246,32,133,10,69,139,247,108,58,93,7,167,201,154,73,16,89,252,239,38,217,162,84,219,222,30,111,190,188,121,19,163,246,40,189,210,8,27,116,78,137,221,249,122,97,205,142,186,224,148,39,107,166,147,97,58,41,130,144,233,224,249,83,60,238,227,156,25,117,26,74,253,128,6,29,233,230,47,179,34,243,30,205,104,95,59,236,34,10,11,86,34,55,240,168,251,141,181,252,162,152,182,121,193,19,74,96,159,81,50,30,157,81,12,58,177,231,209,98,200,248,111,116,108,226,93,208,222,186,184,97,29,90,38,61,18,125,214,231,146,202,10,210,113,69,177,20,9,40,48,3,131,31,176,34,241,183,255,94,100,100,94,86,233,210,226,235,176,31,205,118,172,112,220,103,237,108,143,206,19,158,110,211,198,96,88,74,158,200,171,106,25,97,54,135,177,67,125,199,92,82,146,201,187,202,170,254,97,239,217,234,183,248,203,85,115,20,120,169,241,33,23,6,232,208,55,112,186,249,232,30,155,209,251,6,235,137,195,18,29,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("1977b176-8e60-4bdb-aba7-9fed0fd83423"));
		}

		#endregion

	}

	#endregion

}

