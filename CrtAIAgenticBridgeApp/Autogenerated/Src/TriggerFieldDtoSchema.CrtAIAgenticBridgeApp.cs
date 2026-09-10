namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: TriggerFieldDtoSchema

	/// <exclude/>
	public class TriggerFieldDtoSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public TriggerFieldDtoSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public TriggerFieldDtoSchema(TriggerFieldDtoSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0002-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "TriggerFieldDto";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0002-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,146,205,74,3,49,16,199,207,93,216,119,8,61,41,232,190,64,241,80,187,43,244,160,72,91,244,80,122,200,102,167,203,64,54,137,249,16,214,226,187,59,217,106,139,186,66,16,146,144,204,127,230,55,51,97,130,67,213,178,117,239,60,116,197,42,40,143,29,20,107,176,200,37,190,113,143,90,205,242,44,207,20,239,192,25,46,128,45,172,159,47,231,45,144,167,184,181,216,180,48,55,166,168,148,183,253,163,70,229,93,241,12,53,1,94,81,128,43,74,175,243,236,144,103,147,109,201,61,95,104,114,227,194,95,60,16,142,221,176,169,183,216,182,96,175,247,8,178,153,94,238,200,209,132,90,162,96,66,114,231,216,230,168,223,69,121,64,77,34,235,8,187,135,174,6,123,66,9,221,192,145,240,133,112,68,167,222,22,36,176,3,107,193,207,152,139,199,123,108,104,28,210,160,51,146,247,241,57,202,42,207,122,50,18,156,176,104,226,79,142,35,207,122,50,146,76,155,222,252,81,226,167,152,10,67,183,130,151,128,22,154,239,184,90,107,201,150,39,49,21,7,42,116,79,92,6,112,211,43,86,117,232,75,216,243,32,253,96,35,135,61,151,14,70,234,222,238,88,117,10,77,174,157,134,54,57,207,143,89,98,203,24,156,154,105,24,208,255,166,162,222,134,235,175,108,19,218,180,62,0,206,82,242,48,132,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0002-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

