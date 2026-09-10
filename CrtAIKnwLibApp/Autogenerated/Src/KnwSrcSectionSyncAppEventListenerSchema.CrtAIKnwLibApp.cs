namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSrcSectionSyncAppEventListenerSchema

	/// <exclude/>
	public class KnwSrcSectionSyncAppEventListenerSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSrcSectionSyncAppEventListenerSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSrcSectionSyncAppEventListenerSchema(KnwSrcSectionSyncAppEventListenerSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("82c85e43-5d29-4c90-9399-79cb4af1219a");
			Name = "KnwSrcSectionSyncAppEventListener";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("4a74766e-2bb9-4ad6-970c-8d2455cd5d2f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,82,77,79,227,48,16,61,23,137,255,48,130,75,43,173,146,59,165,149,170,10,86,251,165,93,17,16,7,180,7,215,25,168,87,201,216,26,59,101,11,226,191,51,137,83,72,160,168,229,100,205,215,243,123,111,134,84,137,222,41,141,48,103,84,193,216,100,110,157,41,108,56,60,120,60,60,24,84,222,208,29,92,34,179,242,246,54,72,145,113,252,65,62,57,87,58,88,54,232,183,117,92,227,66,186,202,210,146,84,165,126,204,120,103,44,193,188,80,222,159,192,15,186,207,88,103,168,133,2,101,107,210,51,231,206,86,72,225,167,241,1,9,185,25,74,211,20,78,125,85,150,138,215,211,54,190,16,28,105,97,15,62,78,131,151,241,37,91,50,15,170,137,255,217,5,200,163,156,43,140,142,41,31,20,135,202,37,27,204,180,3,234,170,133,244,9,154,42,48,7,93,243,219,77,15,78,224,219,123,202,131,199,134,246,139,216,95,24,150,54,23,185,127,216,172,84,192,88,117,49,128,43,143,60,183,68,173,140,175,24,250,153,225,230,3,73,5,252,31,64,199,119,4,245,170,6,131,149,226,90,100,7,98,2,245,204,107,98,212,78,36,179,87,47,110,142,122,45,71,127,199,13,24,99,168,152,250,120,73,182,22,101,101,159,85,211,254,212,234,68,202,163,212,143,116,55,222,198,98,99,188,161,37,178,9,185,213,144,78,27,51,162,251,43,107,114,248,77,66,45,171,87,181,75,250,27,235,170,126,56,217,226,229,6,96,220,181,46,211,75,204,171,2,249,154,37,146,173,78,226,125,198,203,94,39,2,115,90,175,185,215,135,60,29,118,96,234,235,251,46,23,183,109,244,221,25,73,227,76,135,74,21,230,161,3,211,66,36,155,211,30,246,245,124,217,70,117,212,221,195,158,214,158,81,190,195,216,79,224,101,232,125,173,106,143,117,125,30,117,111,166,111,46,48,102,251,73,201,61,3,138,144,151,139,246,4,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("82c85e43-5d29-4c90-9399-79cb4af1219a"));
		}

		#endregion

	}

	#endregion

}

