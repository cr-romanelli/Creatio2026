namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: EntityChangeDispatchArgsSchema

	/// <exclude/>
	public class EntityChangeDispatchArgsSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public EntityChangeDispatchArgsSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public EntityChangeDispatchArgsSchema(EntityChangeDispatchArgsSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1003-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "EntityChangeDispatchArgs";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1003-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,143,193,10,194,48,12,134,207,27,236,29,242,4,190,128,167,234,84,188,40,56,245,34,30,106,27,186,66,87,74,147,30,198,240,221,237,152,8,122,24,66,18,72,242,37,249,147,200,122,3,77,79,140,221,178,42,171,210,203,14,41,72,133,176,142,44,246,194,160,103,171,86,209,106,131,34,132,197,41,229,188,195,170,28,170,178,176,158,49,122,233,64,57,73,4,155,220,226,126,221,74,111,176,182,121,11,171,86,68,67,153,28,233,34,164,135,179,10,136,227,120,116,162,27,213,98,39,15,249,42,12,96,144,151,64,99,120,142,90,62,19,187,100,245,23,127,217,235,255,240,89,238,45,100,18,124,238,195,156,132,90,50,158,243,227,112,84,42,197,136,90,240,12,61,173,212,91,139,78,95,165,75,120,187,127,213,232,119,182,200,158,237,5,46,93,203,117,143,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1003-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

