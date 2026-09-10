namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: ITriggerMetadataProviderSchema

	/// <exclude/>
	public class ITriggerMetadataProviderSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public ITriggerMetadataProviderSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public ITriggerMetadataProviderSchema(ITriggerMetadataProviderSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0008-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "ITriggerMetadataProvider";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0008-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,133,144,177,106,195,48,16,134,103,27,244,14,30,211,197,47,208,201,113,66,201,144,98,156,64,135,210,65,145,175,138,192,150,156,187,147,193,148,190,123,79,193,25,66,105,10,90,164,255,251,126,116,23,201,121,91,212,200,213,174,178,224,217,153,53,186,206,66,53,142,101,27,229,62,192,179,202,227,3,106,235,25,231,38,56,207,84,190,193,233,0,56,57,3,84,110,56,136,169,114,175,7,160,81,27,248,195,63,162,179,22,112,227,200,132,9,112,86,249,151,202,179,49,158,122,103,10,105,5,252,76,242,110,225,246,192,186,211,172,27,12,147,235,0,133,77,124,182,196,181,68,125,176,77,164,115,11,151,8,196,239,31,197,11,112,213,247,11,65,171,167,235,191,30,40,73,88,194,21,49,166,217,121,97,67,7,55,93,160,237,36,195,28,231,17,168,149,25,131,39,40,238,94,255,177,101,17,60,31,204,25,6,93,135,62,14,254,190,230,119,124,235,163,235,227,171,108,54,213,101,223,42,151,243,3,74,221,133,142,203,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0008-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

