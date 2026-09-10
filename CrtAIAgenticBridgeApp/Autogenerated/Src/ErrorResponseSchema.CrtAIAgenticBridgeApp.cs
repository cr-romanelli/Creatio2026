namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: ErrorResponseSchema

	/// <exclude/>
	public class ErrorResponseSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public ErrorResponseSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public ErrorResponseSchema(ErrorResponseSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0007-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "ErrorResponse";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0007-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,109,208,177,106,195,48,16,6,224,57,6,191,195,145,169,29,170,23,8,25,220,36,67,135,148,18,15,25,66,7,89,61,196,129,45,153,187,115,33,13,121,247,156,112,90,104,41,72,90,254,159,79,220,77,66,41,66,123,22,197,193,29,166,164,52,160,107,145,201,247,244,229,149,114,90,213,85,93,37,63,160,140,62,32,108,88,155,151,38,162,53,195,51,211,71,196,102,28,221,46,41,159,223,50,37,21,119,196,206,128,79,10,40,110,171,185,174,46,117,181,56,109,189,250,77,182,154,15,250,240,106,28,172,97,137,204,153,159,216,232,156,4,151,143,239,214,28,167,174,167,0,161,247,34,176,43,133,195,61,183,176,80,179,181,199,161,67,254,145,100,10,246,159,204,196,183,209,229,220,67,59,39,112,129,136,186,2,41,207,181,204,244,63,100,115,138,143,248,27,18,229,178,166,253,156,253,165,22,118,237,220,0,183,159,26,134,75,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0007-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

