namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: McpToolValidationIssueSchema

	/// <exclude/>
	public class McpToolValidationIssueSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public McpToolValidationIssueSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public McpToolValidationIssueSchema(McpToolValidationIssueSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("29d13c95-10db-40f6-8f1d-26eec692c688");
			Name = "McpToolValidationIssue";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,133,143,61,10,195,48,12,133,231,4,114,7,65,246,30,160,221,26,40,100,72,201,16,186,59,182,226,154,186,182,145,156,41,244,238,117,28,40,20,250,179,8,233,233,211,147,228,196,29,57,8,137,48,32,145,96,63,197,93,227,221,100,244,76,34,26,239,170,114,169,202,162,38,212,169,128,198,10,230,61,116,50,12,222,219,139,176,70,101,170,101,158,177,42,19,105,92,68,114,194,130,92,209,175,100,177,100,250,101,220,147,15,72,209,96,114,239,231,209,26,185,245,67,206,129,35,25,167,161,241,10,97,1,141,241,0,188,134,199,39,234,100,208,170,115,122,236,7,58,166,163,160,229,188,138,175,71,235,229,109,29,253,235,221,33,179,208,31,157,107,116,106,251,38,215,155,250,46,38,237,9,23,40,129,80,113,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("29d13c95-10db-40f6-8f1d-26eec692c688"));
		}

		#endregion

	}

	#endregion

}

