namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IEntityChangeFilterMatcherSchema

	/// <exclude/>
	public class IEntityChangeFilterMatcherSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IEntityChangeFilterMatcherSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IEntityChangeFilterMatcherSchema(IEntityChangeFilterMatcherSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f7a8b9c0-5020-4d1e-2f3a-4b5c6d7e8f9a");
			Name = "IEntityChangeFilterMatcher";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("a8b9c0d1-5020-4e2f-3a4b-5c6d7e8f9a0b");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,37,142,77,14,194,32,16,133,215,37,225,14,44,53,49,94,192,85,109,212,176,112,99,227,1,40,140,116,18,160,100,128,69,53,222,93,168,201,108,222,207,55,121,37,97,176,98,92,83,6,127,226,140,179,160,60,164,168,52,136,129,114,47,123,11,33,163,62,19,26,11,125,140,199,71,169,218,3,103,31,206,186,88,38,135,90,96,200,64,175,198,200,75,77,243,58,204,42,88,184,162,171,254,93,101,61,3,213,118,35,186,105,89,156,144,105,115,119,183,130,70,192,134,140,181,228,213,83,154,131,216,92,2,189,144,105,50,101,106,27,19,16,42,135,111,48,255,191,105,95,247,118,95,206,234,253,0,232,96,17,114,198,0,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f7a8b9c0-5020-4d1e-2f3a-4b5c6d7e8f9a"));
		}

		#endregion

	}

	#endregion

}

