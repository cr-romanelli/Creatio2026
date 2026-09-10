namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: DispatchEntityChangeAsyncOperationSchema

	/// <exclude/>
	public class DispatchEntityChangeAsyncOperationSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public DispatchEntityChangeAsyncOperationSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public DispatchEntityChangeAsyncOperationSchema(DispatchEntityChangeAsyncOperationSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("a1b2c3d4-1010-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "DispatchEntityChangeAsyncOperation";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("b2c3d4e5-1010-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,83,219,142,155,48,16,125,38,82,254,97,202,19,145,34,62,160,155,70,98,73,182,66,106,181,213,54,125,94,57,48,97,173,128,77,199,54,21,170,246,223,59,230,178,155,176,141,4,88,182,207,204,153,115,102,112,70,170,18,82,93,215,90,197,223,116,89,242,246,110,185,112,195,49,161,176,82,199,15,188,56,194,3,95,87,151,247,7,36,18,70,159,108,156,106,194,27,199,241,131,200,173,38,137,230,22,224,32,204,217,95,46,23,74,212,104,26,145,35,83,219,36,75,74,84,86,230,247,36,139,18,147,166,137,159,28,239,107,92,46,254,46,23,129,84,22,73,137,10,242,74,24,3,59,201,145,54,127,217,51,196,118,233,139,80,28,99,58,149,63,54,72,94,134,226,152,224,51,100,247,34,63,151,164,157,42,60,241,230,18,63,229,72,168,52,219,53,100,191,12,82,170,149,194,220,199,63,225,111,39,9,11,206,227,249,131,134,100,43,44,130,177,156,62,7,54,171,208,170,234,32,99,31,193,191,95,252,247,187,80,162,68,138,191,162,245,254,34,69,225,127,197,133,171,187,203,164,215,212,240,236,174,246,189,91,12,118,199,138,153,91,45,11,96,111,162,91,90,64,80,233,106,230,51,43,232,75,15,90,65,32,205,216,216,189,18,199,10,11,174,119,60,48,190,218,204,140,231,155,97,77,178,31,149,176,39,77,245,163,179,199,222,63,146,94,208,14,43,217,34,117,219,104,144,16,200,19,68,159,230,217,39,230,128,93,136,119,120,116,101,20,142,8,40,164,233,49,107,48,103,217,52,126,70,138,177,124,144,10,132,111,35,232,169,143,241,104,85,16,16,114,184,26,54,175,111,186,166,80,36,86,148,250,225,24,70,176,243,170,54,217,190,101,35,118,111,152,109,52,228,82,248,135,255,3,101,44,57,15,78,70,199,162,240,218,249,112,61,239,197,106,172,230,157,54,158,178,71,239,190,247,152,215,143,109,251,137,246,186,213,209,172,243,51,178,209,197,89,13,44,244,195,128,12,150,240,203,207,63,148,151,189,232,231,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("a1b2c3d4-1010-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

