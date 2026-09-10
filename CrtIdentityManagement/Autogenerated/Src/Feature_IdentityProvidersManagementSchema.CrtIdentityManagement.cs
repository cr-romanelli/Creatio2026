namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: Feature_IdentityProvidersManagementSchema

	/// <exclude/>
	public class Feature_IdentityProvidersManagementSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public Feature_IdentityProvidersManagementSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public Feature_IdentityProvidersManagementSchema(Feature_IdentityProvidersManagementSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("adb16da3-ca67-4de3-8303-c1259ef305a9");
			Name = "Feature_IdentityProvidersManagement";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("3091ce02-4f25-40d0-9a41-fe334146f13f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,78,189,10,194,48,16,222,11,125,135,3,23,93,250,0,45,78,69,193,65,112,240,5,206,230,12,129,122,145,203,69,144,226,187,155,90,171,237,210,91,66,190,127,198,27,133,59,54,4,103,18,193,224,175,90,212,158,175,206,70,65,117,158,139,90,244,96,136,213,233,243,136,140,150,110,233,147,103,93,158,65,186,24,28,91,168,133,122,113,177,79,79,20,58,123,107,219,132,87,121,54,168,86,66,54,101,65,221,98,8,37,140,121,39,241,15,103,72,194,52,120,112,220,227,165,117,13,52,189,97,73,15,37,124,75,143,164,104,80,113,240,119,99,208,172,222,115,80,137,141,122,73,43,78,159,138,169,238,91,186,80,183,222,64,247,215,247,119,8,59,198,75,75,6,182,144,178,169,250,211,175,217,6,98,51,204,24,209,31,61,167,18,252,6,13,82,193,254,149,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("adb16da3-ca67-4de3-8303-c1259ef305a9"));
		}

		#endregion

	}

	#endregion

}

