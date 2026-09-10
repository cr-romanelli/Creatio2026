namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: IEventSenderSchema

	/// <exclude/>
	public class IEventSenderSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public IEventSenderSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public IEventSenderSchema(IEventSenderSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1007-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "IEventSender";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1007-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,203,75,204,77,45,46,72,76,78,85,112,46,42,113,244,116,76,79,205,43,201,76,118,42,202,76,73,79,117,44,40,208,11,42,5,242,115,83,121,185,170,121,185,56,11,74,147,114,50,147,21,50,243,74,82,139,210,64,122,60,93,203,128,234,131,83,243,82,82,139,128,242,32,53,156,101,249,153,41,10,32,33,155,16,151,196,146,68,59,13,176,26,255,228,228,210,162,162,212,188,228,212,160,212,194,210,212,226,18,168,172,66,17,132,171,105,13,212,91,203,203,5,68,0,152,190,183,17,147,0,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1007-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

