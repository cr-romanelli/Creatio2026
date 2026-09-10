namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IEventDispatcherSchema

	/// <exclude/>
	public class IEventDispatcherSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IEventDispatcherSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IEventDispatcherSchema(IEventDispatcherSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1006-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "IEventDispatcher";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1006-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,203,75,204,77,45,46,72,76,78,85,112,46,42,113,244,116,76,79,205,43,201,76,118,42,202,76,73,79,117,44,40,208,11,42,5,242,115,83,121,185,170,121,185,56,51,243,74,82,139,242,18,115,20,192,140,52,144,46,79,215,50,160,14,151,76,160,25,37,201,25,169,69,64,85,32,149,156,101,249,153,41,10,48,97,13,87,160,33,37,149,206,25,137,121,233,169,48,65,199,162,244,98,133,68,32,161,105,13,212,80,203,203,5,68,0,155,142,66,21,142,0,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1006-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

