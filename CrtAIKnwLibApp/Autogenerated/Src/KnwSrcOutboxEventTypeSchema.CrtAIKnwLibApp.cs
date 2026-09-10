namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSrcOutboxEventTypeSchema

	/// <exclude/>
	public class KnwSrcOutboxEventTypeSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSrcOutboxEventTypeSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSrcOutboxEventTypeSchema(KnwSrcOutboxEventTypeSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("9db1daae-0cbd-45c9-bb8b-d313e5528da0");
			Name = "KnwSrcOutboxEventType";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,145,193,74,196,64,12,134,207,45,244,29,2,94,165,69,189,137,122,217,237,105,65,15,234,3,204,206,252,116,7,218,76,201,76,237,86,241,221,157,78,85,10,178,11,30,147,252,249,146,63,97,213,193,247,74,131,54,2,21,172,43,55,174,183,173,11,69,254,81,228,69,158,93,8,26,235,152,106,30,186,91,218,241,248,44,250,105,8,123,119,172,223,192,225,101,234,145,116,85,85,209,157,31,186,78,201,244,240,29,111,225,181,216,61,60,121,232,48,83,244,65,113,3,194,220,74,33,246,82,15,241,214,7,24,178,76,126,98,125,16,199,246,93,37,185,75,131,202,31,124,181,226,91,14,16,86,45,33,46,118,106,175,108,241,240,103,185,148,120,196,72,2,237,196,208,168,60,233,217,63,76,249,171,95,79,203,210,117,64,247,116,117,121,6,89,31,163,21,203,205,154,59,244,230,12,247,53,85,35,247,250,191,92,131,22,167,185,219,84,141,220,155,132,253,92,126,9,54,203,59,231,48,230,190,0,231,116,36,11,254,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("9db1daae-0cbd-45c9-bb8b-d313e5528da0"));
		}

		#endregion

	}

	#endregion

}

