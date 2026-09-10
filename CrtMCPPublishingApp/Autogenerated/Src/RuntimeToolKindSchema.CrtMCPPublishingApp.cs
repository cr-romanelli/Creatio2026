namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: RuntimeToolKindSchema

	/// <exclude/>
	public class RuntimeToolKindSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public RuntimeToolKindSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public RuntimeToolKindSchema(RuntimeToolKindSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a77afeec-50a9-44a8-a050-cf06cba43f55");
			Name = "RuntimeToolKind";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,101,143,59,11,194,64,16,132,235,4,242,31,14,210,138,248,108,132,20,26,2,130,133,98,34,214,151,220,38,28,38,187,225,30,90,136,255,221,205,89,90,237,206,55,195,50,139,114,0,59,202,6,68,5,198,72,75,173,155,231,132,173,238,188,145,78,19,38,241,59,137,163,212,64,199,66,20,232,135,157,184,122,116,122,128,138,168,63,105,84,73,204,9,141,14,12,202,94,0,71,254,19,209,116,37,186,225,3,233,133,34,19,139,217,164,15,222,106,4,107,47,134,26,30,204,151,129,151,228,77,3,57,41,216,55,83,9,54,86,193,184,67,93,130,121,106,238,155,137,117,64,71,231,198,2,213,72,220,128,225,38,192,51,185,154,197,150,247,79,168,151,2,170,223,15,147,100,246,5,183,183,215,46,249,0,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a77afeec-50a9-44a8-a050-cf06cba43f55"));
		}

		#endregion

	}

	#endregion

}

