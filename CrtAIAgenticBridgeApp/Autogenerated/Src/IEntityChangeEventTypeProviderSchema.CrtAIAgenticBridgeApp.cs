namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IEntityChangeEventTypeProviderSchema

	/// <exclude/>
	public class IEntityChangeEventTypeProviderSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IEntityChangeEventTypeProviderSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IEntityChangeEventTypeProviderSchema(IEntityChangeEventTypeProviderSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1005-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "IEntityChangeEventTypeProvider";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1005-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,133,144,205,106,195,48,16,132,207,54,248,29,4,185,164,16,242,0,233,201,53,33,232,16,8,249,57,149,30,84,105,42,47,196,178,144,100,131,9,121,247,110,101,18,200,169,199,157,217,111,70,43,167,58,68,175,52,68,19,82,45,107,11,151,72,127,4,50,22,181,247,235,227,192,115,135,170,188,85,101,49,68,114,86,156,166,152,208,189,87,37,43,139,0,75,189,19,210,37,132,31,142,217,8,185,101,34,77,77,171,156,197,118,228,188,243,228,113,8,253,72,6,33,83,126,248,190,146,22,244,128,254,101,138,91,230,158,117,123,164,182,55,113,35,14,57,105,54,159,28,135,133,233,243,75,236,144,94,52,66,92,238,6,50,2,185,237,164,91,116,234,34,205,74,68,54,249,50,157,251,255,214,223,230,243,138,177,231,117,233,70,117,37,163,18,26,197,204,242,97,46,224,204,252,160,60,223,231,31,121,17,89,251,5,214,153,15,186,98,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1005-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

