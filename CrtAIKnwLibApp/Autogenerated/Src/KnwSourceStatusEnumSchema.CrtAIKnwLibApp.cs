namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwSourceStatusEnumSchema

	/// <exclude/>
	public class KnwSourceStatusEnumSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwSourceStatusEnumSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwSourceStatusEnumSchema(KnwSourceStatusEnumSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("3a98d530-1f26-4182-b6ad-86117cec24fc");
			Name = "KnwSourceStatusEnum";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("7af68358-3b0d-447f-959b-5895d05db239");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,157,147,221,74,195,64,16,133,175,91,232,59,12,120,43,13,254,221,136,17,164,120,33,130,23,86,31,96,178,59,173,139,155,217,176,63,141,65,124,119,39,27,91,42,181,84,122,57,179,179,231,59,103,39,97,172,41,52,168,8,102,158,48,26,55,157,185,198,88,23,39,227,207,201,120,50,30,157,120,90,26,199,112,207,169,190,134,71,110,231,46,121,69,243,136,49,133,190,153,167,138,162,128,155,144,234,26,125,119,251,83,63,83,227,41,16,199,0,241,141,32,228,27,224,22,128,240,206,174,181,164,151,210,205,106,96,120,152,233,66,164,122,186,22,44,182,20,155,84,89,163,128,132,248,183,139,209,224,119,199,74,110,188,136,248,46,52,0,83,107,59,80,125,116,210,128,172,129,93,132,142,162,56,210,244,65,122,186,145,220,54,51,122,162,22,74,56,59,61,2,169,146,247,242,40,130,173,200,240,242,0,232,161,63,237,199,74,56,63,134,134,43,52,22,43,75,176,112,30,82,160,61,156,187,205,92,9,23,135,64,102,109,74,36,27,239,20,133,144,171,69,94,226,142,141,22,37,53,178,34,107,247,230,156,173,207,133,127,249,47,126,159,46,241,239,124,158,162,55,180,66,155,87,105,88,22,91,203,91,75,157,26,45,43,14,123,232,175,91,58,37,92,101,252,215,240,249,19,235,225,15,232,75,233,125,3,75,183,184,251,49,3,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("3a98d530-1f26-4182-b6ad-86117cec24fc"));
		}

		#endregion

	}

	#endregion

}

