namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwBaseItemResponseSchema

	/// <exclude/>
	public class KnwBaseItemResponseSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwBaseItemResponseSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwBaseItemResponseSchema(KnwBaseItemResponseSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("2db2de7c-3985-4baa-b51c-0a001ac66c13");
			Name = "KnwBaseItemResponse";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d5e01d64-57e6-47d0-82bc-33ac8cbaf486");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,173,211,81,107,194,48,16,7,240,103,5,191,195,193,222,219,247,185,237,97,221,24,178,49,68,247,5,98,123,214,195,52,201,46,41,78,100,223,125,151,180,138,14,95,54,133,66,232,37,249,231,151,166,49,170,65,239,84,137,80,48,170,64,54,43,172,35,109,195,104,184,27,13,7,242,220,48,214,100,13,20,90,121,127,11,175,102,243,168,60,78,2,54,51,153,105,141,199,209,80,134,229,121,14,119,190,109,26,197,219,135,254,125,134,142,209,163,9,30,20,112,63,26,72,166,194,146,109,3,97,133,176,54,118,163,177,170,17,22,18,155,237,147,242,163,40,50,1,217,40,13,101,36,156,23,12,118,73,113,208,78,217,58,228,64,40,228,105,187,208,84,118,253,191,153,169,240,130,34,180,12,62,182,17,213,26,250,108,69,90,137,157,150,132,12,118,121,70,155,182,146,29,98,143,205,3,151,22,5,31,152,76,13,17,59,169,96,7,53,134,113,92,103,12,223,112,223,247,102,207,141,11,219,241,95,128,222,182,44,103,166,201,172,47,165,205,83,212,91,76,186,26,175,34,239,180,218,130,145,191,107,239,235,200,127,32,61,117,33,239,49,227,106,178,114,213,202,70,3,126,133,75,191,91,17,147,62,98,208,245,78,181,180,140,255,118,85,86,90,132,121,10,57,49,245,87,3,77,213,221,142,244,222,85,79,139,82,251,1,115,89,254,22,17,4,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("2db2de7c-3985-4baa-b51c-0a001ac66c13"));
		}

		#endregion

	}

	#endregion

}

