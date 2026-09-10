namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: RuntimeToolSchemaResultSchema

	/// <exclude/>
	public class RuntimeToolSchemaResultSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public RuntimeToolSchemaResultSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public RuntimeToolSchemaResultSchema(RuntimeToolSchemaResultSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("4de92bc3-c3f0-4a78-959d-cbcbd4a3ec0c");
			Name = "RuntimeToolSchemaResult";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,207,209,170,194,48,12,6,224,107,7,123,135,128,247,62,128,94,142,115,113,4,65,54,95,32,214,216,83,232,210,145,164,122,49,124,247,211,109,32,8,234,77,105,211,47,225,15,99,79,58,160,35,56,145,8,106,186,218,166,73,124,13,62,11,90,72,92,87,99,93,173,178,6,246,47,68,104,211,145,220,130,163,67,186,80,44,61,38,232,108,87,87,133,175,133,124,233,133,38,162,234,22,218,204,22,122,58,165,20,59,247,71,61,182,164,57,218,76,3,27,9,99,4,55,217,207,116,53,206,252,57,250,40,105,32,177,64,101,254,49,159,99,112,203,255,48,223,65,77,166,196,63,34,73,154,18,16,70,240,100,59,208,233,120,124,164,7,82,69,255,77,159,75,50,248,213,46,59,87,236,23,184,215,196,45,222,97,89,227,29,92,19,95,150,109,230,247,82,125,45,150,218,63,111,167,86,39,162,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("4de92bc3-c3f0-4a78-959d-cbcbd4a3ec0c"));
		}

		#endregion

	}

	#endregion

}

