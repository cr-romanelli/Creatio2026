namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: AssemblyInfoSchema

	/// <exclude/>
	public class AssemblyInfoSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public AssemblyInfoSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public AssemblyInfoSchema(AssemblyInfoSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("0bf6ea1d-8e46-4691-9053-f0474c6406a0");
			Name = "AssemblyInfo";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,133,204,177,10,194,48,16,128,225,189,208,119,40,157,116,201,224,168,83,137,32,14,66,176,197,69,28,146,114,232,65,114,9,185,75,105,222,94,17,119,247,255,251,11,35,61,187,177,178,64,80,215,66,130,1,148,142,33,161,135,60,66,94,112,6,62,180,77,219,220,45,51,4,231,235,190,59,147,64,38,235,249,134,140,206,195,20,55,189,206,114,209,198,20,231,145,95,159,231,144,146,154,128,133,251,237,227,47,62,86,178,1,103,147,227,90,79,64,195,47,222,125,237,27,150,58,31,92,163,0,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("0bf6ea1d-8e46-4691-9053-f0474c6406a0"));
		}

		#endregion

	}

	#endregion

}

