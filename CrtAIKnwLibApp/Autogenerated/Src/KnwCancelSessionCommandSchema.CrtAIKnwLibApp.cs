namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwCancelSessionCommandSchema

	/// <exclude/>
	public class KnwCancelSessionCommandSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwCancelSessionCommandSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwCancelSessionCommandSchema(KnwCancelSessionCommandSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("4ffdf83e-e36e-4610-9a87-b6b32fa65166");
			Name = "KnwCancelSessionCommand";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d6384572-e40b-4c76-9160-d17156fa7b07");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,117,144,193,106,195,48,12,134,207,13,228,29,4,187,39,247,101,236,146,67,25,187,20,242,4,174,173,101,34,137,20,44,155,174,132,189,251,28,39,45,221,96,39,163,79,214,239,79,102,51,161,206,198,34,180,30,77,32,169,90,153,105,148,80,22,75,89,28,162,18,247,208,93,53,224,212,148,69,34,79,30,123,18,134,118,52,170,207,240,206,151,214,176,197,177,67,213,196,91,153,38,195,46,95,173,235,26,94,52,38,224,175,175,123,189,247,33,42,58,8,2,54,15,131,97,32,118,248,181,190,166,91,82,2,96,224,108,236,208,123,137,105,36,24,29,170,91,108,253,144,59,199,243,72,22,236,106,244,191,208,97,201,82,247,5,78,94,102,244,129,48,109,113,202,9,91,255,175,117,6,71,12,10,226,147,91,58,195,39,194,192,114,25,209,245,8,42,209,167,239,35,135,28,232,131,208,87,247,148,71,201,155,229,49,146,91,37,187,60,246,230,96,129,30,67,179,38,55,240,189,43,34,187,205,50,215,27,253,13,19,251,1,87,250,198,58,187,1,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("4ffdf83e-e36e-4610-9a87-b6b32fa65166"));
		}

		#endregion

	}

	#endregion

}

