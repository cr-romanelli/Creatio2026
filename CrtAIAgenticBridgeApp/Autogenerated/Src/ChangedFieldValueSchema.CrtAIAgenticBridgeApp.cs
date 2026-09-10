namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: ChangedFieldValueSchema

	/// <exclude/>
	public class ChangedFieldValueSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public ChangedFieldValueSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public ChangedFieldValueSchema(ChangedFieldValueSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1001-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "ChangedFieldValue";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1001-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,93,145,205,78,67,33,16,133,215,222,228,190,195,89,234,194,246,1,186,170,245,39,221,116,161,209,61,133,41,37,225,2,97,160,122,109,76,124,8,159,208,39,113,232,223,194,4,38,12,28,62,230,12,149,93,176,120,25,185,208,48,235,187,190,11,106,32,78,74,19,22,185,204,151,115,75,161,56,125,151,157,177,52,79,105,242,92,37,31,168,239,246,125,119,53,157,98,25,10,229,160,60,50,165,76,44,106,85,92,12,136,27,40,232,173,10,150,12,26,163,140,208,209,215,33,160,178,108,173,71,148,45,33,159,113,141,85,34,6,85,244,22,239,45,146,185,61,93,216,56,47,143,240,4,171,88,192,148,157,242,238,83,24,46,200,141,70,137,181,172,99,13,230,132,201,206,90,202,72,106,244,81,25,252,126,255,92,214,244,145,34,19,35,6,127,42,128,56,250,157,192,54,142,188,193,193,62,18,229,35,106,56,22,2,218,137,5,148,49,17,174,153,8,15,45,191,119,210,168,118,158,111,38,34,79,117,237,157,134,246,138,25,139,163,243,199,6,125,83,190,54,135,173,99,103,21,75,145,210,248,149,60,135,61,44,149,153,24,147,240,213,254,224,162,122,170,206,96,113,104,194,235,210,252,23,94,201,148,241,7,208,193,61,98,195,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1001-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

