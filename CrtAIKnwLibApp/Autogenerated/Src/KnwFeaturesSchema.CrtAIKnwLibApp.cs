namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwFeaturesSchema

	/// <exclude/>
	public class KnwFeaturesSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwFeaturesSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwFeaturesSchema(KnwFeaturesSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("034eea79-d275-4e60-a0fa-2477d62e55c2");
			Name = "KnwFeatures";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("fc5a15eb-1c52-4ea9-828b-8fee971a8bbd");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,101,145,203,106,195,48,16,69,215,14,228,31,134,116,211,110,236,125,94,80,66,11,161,148,134,62,62,64,177,39,142,168,44,25,141,20,211,134,254,123,101,61,156,215,202,248,204,245,245,25,73,178,6,169,101,37,194,74,35,51,92,229,43,213,114,161,204,120,116,28,143,50,75,92,214,195,232,217,61,172,198,79,85,215,194,241,217,120,228,34,119,26,107,174,36,172,4,35,154,194,139,236,98,140,252,184,40,10,152,147,109,26,166,127,150,241,125,163,213,129,87,72,176,11,73,104,208,176,138,25,6,101,95,210,15,148,134,111,169,58,129,85,141,240,129,250,192,157,98,140,83,158,122,139,179,98,46,13,106,201,4,144,113,178,101,168,186,212,201,142,94,233,90,249,139,240,237,209,154,125,24,94,11,123,240,142,173,107,64,105,8,204,254,204,183,247,236,65,170,72,138,249,208,116,174,152,181,118,43,6,181,225,155,41,68,197,215,88,219,71,163,234,201,85,73,50,182,52,74,59,227,141,239,137,137,27,97,79,214,146,27,206,4,255,117,167,201,64,98,7,220,21,48,233,142,81,237,188,243,156,16,161,212,184,91,76,146,202,164,88,70,185,142,59,177,62,149,174,8,37,219,186,219,200,79,191,188,216,44,173,150,154,238,31,224,232,121,182,166,167,240,41,44,192,104,139,51,143,255,210,122,40,171,176,97,0,145,95,225,64,47,161,99,255,114,124,193,211,189,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("034eea79-d275-4e60-a0fa-2477d62e55c2"));
		}

		#endregion

	}

	#endregion

}

