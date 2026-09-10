namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: EventTypeDtoSchema

	/// <exclude/>
	public class EventTypeDtoSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public EventTypeDtoSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public EventTypeDtoSchema(EventTypeDtoSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("f1a2b3c4-0005-4a5b-8c6d-e7f8a9b0c1d2");
			Name = "EventTypeDto";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("d2e3f4a5-0005-4b6c-9d7e-f8a9b0c1d2e3");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,144,207,74,196,48,16,135,207,45,244,29,66,79,10,218,23,88,60,212,182,130,7,69,172,232,65,60,76,211,217,58,144,166,33,51,93,168,139,239,110,226,191,5,237,97,33,9,132,239,55,223,48,51,51,217,65,181,11,11,142,197,253,108,133,70,44,90,244,4,134,222,64,104,178,155,44,205,82,11,35,178,3,141,170,242,82,94,151,3,134,164,190,244,212,15,88,58,87,52,86,252,114,55,145,21,46,158,176,11,130,29,105,228,162,150,41,75,247,89,154,60,215,32,80,77,33,6,90,78,110,131,78,93,168,28,119,193,115,46,139,195,252,244,37,164,220,220,25,210,74,27,96,86,77,132,15,129,125,74,146,104,249,210,220,224,216,161,255,149,232,169,255,46,255,169,103,241,113,170,42,0,181,87,3,202,70,113,124,222,227,40,235,146,158,216,25,88,226,119,213,85,31,248,209,74,100,237,201,197,29,174,43,15,252,88,101,220,186,44,173,126,197,17,174,200,8,122,206,207,84,51,146,212,184,133,217,200,35,152,57,38,183,96,24,215,154,54,255,13,127,155,39,225,134,243,1,142,239,168,160,26,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("f1a2b3c4-0005-4a5b-8c6d-e7f8a9b0c1d2"));
		}

		#endregion

	}

	#endregion

}

