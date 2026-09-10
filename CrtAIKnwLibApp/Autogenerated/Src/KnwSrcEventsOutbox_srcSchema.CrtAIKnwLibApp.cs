namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSrcEventsOutbox_srcSchema

	/// <exclude/>
	public class KnwSrcEventsOutbox_srcSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSrcEventsOutbox_srcSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSrcEventsOutbox_srcSchema(KnwSrcEventsOutbox_srcSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("d45def03-e605-42f2-84ef-199a99e64a0e");
			Name = "KnwSrcEventsOutbox_src";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,125,144,177,106,195,64,12,64,103,27,252,15,194,221,237,221,77,179,152,14,165,144,14,254,2,229,44,146,3,159,20,78,231,180,174,233,191,231,114,113,154,132,66,23,129,132,244,244,36,70,71,122,64,67,208,122,194,96,165,106,229,96,7,9,69,62,23,121,246,228,105,103,133,161,29,80,181,129,119,254,236,188,121,61,18,7,253,24,195,86,190,58,179,39,135,69,30,123,235,186,134,149,142,206,161,159,214,75,222,10,7,180,172,96,132,53,96,28,131,81,169,135,237,4,58,177,217,123,97,251,125,94,203,32,137,7,78,122,26,170,43,174,190,227,89,14,228,25,7,136,160,96,13,152,179,211,63,74,217,156,180,110,55,92,21,26,120,91,80,151,134,95,112,146,140,120,111,121,7,155,248,25,120,129,242,239,130,242,121,1,19,247,23,118,202,127,82,124,44,198,218,9,191,234,157,102,98,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("d45def03-e605-42f2-84ef-199a99e64a0e"));
		}

		#endregion

	}

	#endregion

}

