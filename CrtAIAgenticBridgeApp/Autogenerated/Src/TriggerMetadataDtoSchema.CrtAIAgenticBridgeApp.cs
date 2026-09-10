namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: TriggerMetadataDtoSchema

	/// <exclude/>
	public class TriggerMetadataDtoSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public TriggerMetadataDtoSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public TriggerMetadataDtoSchema(TriggerMetadataDtoSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0003-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "TriggerMetadataDto";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0003-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,208,193,74,196,48,16,6,224,115,11,125,135,176,39,61,216,23,88,60,212,86,193,195,138,184,130,135,101,15,105,58,134,129,38,13,153,169,80,23,223,221,201,118,117,81,122,88,72,66,194,252,124,97,102,36,244,86,109,39,98,112,229,203,232,25,29,148,91,136,168,123,252,212,140,131,95,23,121,145,123,237,128,130,54,160,234,200,213,99,101,65,146,230,46,98,103,161,10,161,188,247,28,167,231,1,61,83,249,6,173,0,31,104,128,202,134,135,34,63,20,121,182,107,52,235,122,144,152,54,124,245,36,156,186,85,43,142,104,45,196,27,7,172,59,9,172,174,247,146,13,99,219,163,81,166,215,68,234,117,142,108,78,137,35,152,37,113,38,55,224,90,136,191,160,25,58,152,145,31,133,228,15,233,176,150,130,58,40,11,188,86,148,142,175,212,214,50,210,33,133,94,79,233,185,104,53,231,250,197,36,144,137,24,210,60,151,201,115,253,82,242,29,161,239,232,175,118,26,214,67,42,201,164,118,123,117,188,210,127,51,147,45,235,27,55,161,188,238,253,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0003-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

