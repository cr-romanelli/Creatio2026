namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwProviderRecordSchema

	/// <exclude/>
	public class KnwProviderRecordSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwProviderRecordSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwProviderRecordSchema(KnwProviderRecordSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("6a5435a0-149b-4879-a541-be3b597ae9b0");
			Name = "KnwProviderRecord";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4b4f6d9f-1234-44a2-bd3b-b64923d93c0c");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,83,93,111,226,48,16,124,6,137,255,176,226,94,174,210,137,188,23,194,11,149,42,116,210,9,181,247,7,124,246,66,87,77,214,185,181,83,212,34,254,251,217,78,200,133,143,34,149,23,136,39,222,157,217,217,9,171,18,93,165,52,194,66,80,121,178,147,133,173,168,176,126,52,220,141,134,131,218,17,111,224,249,221,121,44,167,163,97,64,190,9,110,200,50,44,10,229,220,61,252,228,237,74,236,27,25,148,39,212,86,76,186,148,101,25,204,92,93,150,74,222,231,237,249,9,43,65,135,236,29,40,120,101,187,45,208,108,16,170,182,26,36,149,195,150,252,11,4,128,61,173,73,71,69,12,138,13,24,116,90,168,242,244,134,64,188,182,82,166,119,147,3,91,214,163,171,234,63,5,105,208,81,226,37,133,131,93,82,249,127,22,203,206,75,173,189,149,48,210,146,61,10,171,162,185,115,58,74,2,150,76,158,84,65,31,24,135,97,220,6,73,206,43,14,54,218,53,248,23,12,37,136,160,5,215,249,248,76,192,56,155,183,218,210,176,170,40,162,11,21,138,39,116,147,142,52,59,101,157,85,74,84,9,28,86,150,143,201,140,231,191,3,81,205,244,183,198,206,49,148,201,44,75,247,46,151,197,223,166,48,62,29,212,30,150,112,189,182,91,129,229,166,69,15,248,90,39,109,77,171,34,62,93,173,109,119,121,102,226,247,199,154,76,24,251,7,132,213,197,144,198,206,221,161,167,172,195,34,213,29,196,84,15,6,75,3,121,40,158,166,195,175,232,68,158,26,52,192,67,111,174,188,223,171,121,189,136,154,243,212,47,1,251,152,148,148,39,100,211,68,234,56,95,171,110,187,247,176,74,243,92,201,214,35,134,47,196,10,184,248,239,47,109,248,224,215,167,31,209,39,25,106,173,76,198,5,3,118,176,65,63,133,253,87,180,244,67,115,78,127,157,183,93,67,114,251,22,238,11,105,187,81,66,127,191,183,40,233,135,246,70,9,41,67,199,220,39,233,105,208,99,112,255,15,20,167,215,0,175,5,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("6a5435a0-149b-4879-a541-be3b597ae9b0"));
		}

		#endregion

	}

	#endregion

}

