namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwIndexingSessionTypeEnumSchema

	/// <exclude/>
	public class KnwIndexingSessionTypeEnumSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwIndexingSessionTypeEnumSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwIndexingSessionTypeEnumSchema(KnwIndexingSessionTypeEnumSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("e18a67a9-10fb-4155-b0eb-3441e5ce0003");
			Name = "KnwIndexingSessionTypeEnum";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,81,205,10,194,48,12,62,111,176,119,8,120,149,13,61,138,122,17,133,225,77,125,129,186,69,41,172,105,105,90,116,136,239,110,93,117,10,226,142,73,190,191,36,36,20,178,17,21,194,202,162,112,82,231,43,109,100,163,93,150,222,178,52,75,147,145,197,179,212,4,107,242,106,6,91,186,148,84,227,85,210,121,143,204,97,112,104,13,62,103,29,184,40,10,152,179,87,74,216,118,249,170,119,104,44,50,146,99,144,47,42,112,228,130,11,100,56,105,11,22,149,118,216,247,171,152,133,242,183,102,241,37,106,252,177,145,21,96,48,29,204,147,196,5,126,66,117,141,141,111,154,159,60,99,240,140,117,23,72,9,242,226,131,200,123,161,239,36,73,73,210,201,0,91,192,100,60,96,86,82,88,72,133,19,136,65,79,110,169,2,111,106,225,144,255,26,126,148,22,48,237,60,239,241,79,72,117,124,213,179,12,189,7,11,5,181,183,218,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("e18a67a9-10fb-4155-b0eb-3441e5ce0003"));
		}

		#endregion

	}

	#endregion

}

