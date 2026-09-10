namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IMcpToolValidatorSchema

	/// <exclude/>
	public class IMcpToolValidatorSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IMcpToolValidatorSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IMcpToolValidatorSchema(IMcpToolValidatorSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("d1298cfc-a7ae-4d97-ab8c-d8a0bea0ec07");
			Name = "IMcpToolValidator";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,109,146,77,79,27,65,12,134,207,65,226,63,88,112,1,9,118,239,4,114,1,9,229,16,181,10,136,187,51,227,77,44,102,103,182,246,108,164,45,234,127,175,247,43,45,136,227,216,143,95,219,175,39,98,77,218,160,35,120,37,17,212,84,229,226,49,197,138,247,173,96,230,20,207,207,62,206,207,22,173,114,220,195,75,167,153,234,229,151,183,241,33,144,235,97,45,158,41,146,176,51,198,168,75,161,189,69,97,29,51,73,101,77,238,96,189,113,205,107,74,225,13,3,123,204,73,6,144,123,32,98,0,158,201,239,192,197,199,0,159,100,55,148,15,201,235,29,252,108,119,129,221,152,44,203,18,238,181,173,107,148,110,53,7,38,17,82,200,7,2,244,53,199,91,55,109,73,30,178,245,1,33,151,196,3,238,145,163,230,17,108,173,129,240,239,25,81,202,179,98,159,118,104,123,11,96,16,66,223,153,128,166,112,52,182,74,98,121,86,139,252,106,73,115,97,147,214,59,18,61,112,99,27,158,166,108,80,176,22,170,32,218,17,30,46,254,117,235,247,214,139,114,5,87,239,212,153,224,174,131,70,146,35,213,222,242,23,119,160,26,139,181,191,6,30,22,154,5,165,141,153,107,186,197,35,114,192,29,7,206,29,24,236,222,151,195,188,199,217,74,240,201,172,136,41,131,205,39,221,144,156,138,129,179,82,168,138,211,144,229,255,94,126,190,137,221,96,75,218,134,124,242,247,106,2,54,201,83,24,60,187,233,203,22,235,173,57,244,35,134,238,137,135,127,98,122,247,207,45,251,27,216,142,109,251,162,39,170,56,114,159,94,193,23,47,174,151,211,229,41,250,241,248,195,251,207,248,203,62,5,45,246,23,205,245,87,26,212,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("d1298cfc-a7ae-4d97-ab8c-d8a0bea0ec07"));
		}

		#endregion

	}

	#endregion

}

