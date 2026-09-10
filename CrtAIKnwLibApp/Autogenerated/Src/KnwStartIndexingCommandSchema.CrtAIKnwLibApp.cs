namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwStartIndexingCommandSchema

	/// <exclude/>
	public class KnwStartIndexingCommandSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwStartIndexingCommandSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwStartIndexingCommandSchema(KnwStartIndexingCommandSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("c076beb0-4a67-4626-a5b2-8e2728d6b74a");
			Name = "KnwStartIndexingCommand";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8aa4d699-3cf5-485e-8023-1440b0f17cf2");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,144,65,139,194,48,16,133,207,22,250,31,6,246,222,222,85,188,244,32,226,69,240,23,100,155,177,59,208,102,66,102,130,43,197,255,110,154,86,113,23,188,4,230,205,188,247,62,226,204,128,226,77,139,208,4,52,74,92,53,236,169,103,45,139,177,44,86,81,200,117,112,190,137,226,176,41,139,164,124,5,236,136,29,52,189,17,89,195,209,93,207,106,130,30,156,197,223,116,219,240,48,24,103,243,105,93,215,176,149,152,132,112,219,45,243,178,135,40,104,65,25,100,50,131,254,32,208,146,0,62,112,139,34,213,51,161,126,139,240,241,187,167,22,218,169,252,115,247,106,204,253,47,214,83,96,143,65,9,19,240,41,39,204,251,255,128,89,56,58,190,246,104,59,4,225,24,210,199,144,69,167,116,33,12,213,203,244,206,244,132,218,71,178,153,41,219,14,22,70,232,80,55,32,211,115,95,136,208,217,25,42,207,179,250,87,188,63,0,75,135,166,186,147,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("c076beb0-4a67-4626-a5b2-8e2728d6b74a"));
		}

		#endregion

	}

	#endregion

}

