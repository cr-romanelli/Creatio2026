namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: ActualizeKnwSrcEventListenerSchema

	/// <exclude/>
	public class ActualizeKnwSrcEventListenerSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public ActualizeKnwSrcEventListenerSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public ActualizeKnwSrcEventListenerSchema(ActualizeKnwSrcEventListenerSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("3adc49de-3cf0-480d-b2be-5b68e2fc6945");
			Name = "ActualizeKnwSrcEventListener";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d5e01d64-57e6-47d0-82bc-33ac8cbaf486");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,189,84,193,106,219,64,16,61,59,144,127,24,220,139,3,69,186,199,177,33,56,105,73,211,210,82,181,228,80,122,88,175,198,246,22,105,87,204,174,156,184,33,255,222,209,174,28,73,142,148,148,18,122,18,59,243,244,246,189,153,39,105,145,163,45,132,68,88,16,10,167,76,180,48,133,202,140,59,62,186,63,62,26,149,86,233,53,124,67,34,97,205,202,113,147,112,58,80,143,222,9,233,12,41,180,125,136,27,92,50,42,207,141,230,46,247,223,16,174,149,209,176,200,132,181,167,112,46,93,41,50,245,27,175,245,109,66,242,114,139,218,125,84,214,161,70,242,248,56,142,225,204,150,121,46,104,55,175,207,30,5,89,13,131,149,33,184,214,230,54,195,116,141,144,152,146,216,150,210,41,222,85,198,52,88,39,92,105,65,212,87,133,226,47,179,140,246,244,113,139,191,40,151,153,146,96,81,48,29,200,74,229,179,34,225,20,174,206,139,226,64,248,232,222,139,127,116,251,9,221,198,164,236,247,11,169,173,112,24,186,69,56,192,119,139,180,48,90,163,244,210,222,163,235,86,38,251,11,184,228,240,206,129,12,207,19,168,118,53,26,109,5,129,40,138,22,197,12,170,119,154,194,73,253,70,196,85,182,231,71,240,99,220,129,140,127,78,61,25,161,43,73,119,249,162,100,199,206,242,174,42,15,127,168,125,162,78,131,213,33,223,126,172,161,233,103,174,244,6,73,185,212,72,144,132,171,217,184,47,50,209,147,209,70,159,53,151,18,39,200,141,227,185,31,98,88,216,214,168,20,154,230,75,35,59,24,121,217,61,206,122,118,176,39,152,182,71,158,200,13,166,101,134,116,67,124,226,52,204,66,176,195,39,177,139,152,230,172,242,208,193,33,205,39,45,154,38,169,137,15,234,7,179,236,165,9,225,187,122,138,126,140,103,195,219,195,25,125,229,149,240,16,105,210,53,251,182,207,71,75,94,138,43,222,12,166,205,197,207,8,188,232,3,95,40,254,211,56,190,161,209,215,75,250,15,10,31,94,43,81,151,58,29,202,19,183,94,72,211,235,168,72,208,218,176,176,129,108,183,1,255,83,209,208,108,154,246,223,169,57,248,73,132,106,183,200,181,63,224,245,120,139,154,6,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("3adc49de-3cf0-480d-b2be-5b68e2fc6945"));
		}

		#endregion

	}

	#endregion

}

