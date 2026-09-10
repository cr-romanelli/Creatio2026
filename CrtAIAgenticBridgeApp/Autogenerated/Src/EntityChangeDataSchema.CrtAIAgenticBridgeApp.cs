namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: EntityChangeDataSchema

	/// <exclude/>
	public class EntityChangeDataSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public EntityChangeDataSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public EntityChangeDataSchema(EntityChangeDataSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1002-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "EntityChangeData";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1002-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,143,49,107,195,48,16,133,231,24,252,31,14,79,45,148,252,129,208,193,117,92,200,208,14,73,233,18,50,92,164,139,42,144,100,33,157,6,55,244,191,87,74,157,12,197,67,11,146,64,122,239,125,239,148,162,118,10,118,99,100,178,203,109,114,172,45,45,119,20,52,26,253,137,172,7,183,170,171,186,114,104,41,122,20,4,93,224,118,211,42,202,78,241,20,180,84,212,122,127,77,214,213,185,174,22,251,53,50,118,131,227,128,130,15,249,193,167,163,209,2,132,193,24,161,207,78,30,187,15,116,138,138,49,235,37,244,147,122,33,123,164,112,247,154,235,224,17,26,186,120,203,173,185,47,160,43,41,114,40,99,247,55,25,206,160,136,87,16,203,241,85,38,158,7,6,18,67,144,27,57,139,219,78,226,95,97,226,242,135,183,209,207,79,215,221,228,255,1,229,179,38,35,99,243,0,189,213,188,166,19,38,195,239,104,82,241,156,208,68,154,105,219,31,166,190,41,253,187,114,145,119,94,223,103,205,135,136,239,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1002-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

