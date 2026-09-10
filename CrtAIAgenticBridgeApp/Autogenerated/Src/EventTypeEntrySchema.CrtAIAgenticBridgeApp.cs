namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: EventTypeEntrySchema

	/// <exclude/>
	public class EventTypeEntrySchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public EventTypeEntrySchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public EventTypeEntrySchema(EventTypeEntrySchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("d4e5f6a7-4010-4c8d-9e0f-1a2b3c4d5e6f");
			Name = "EventTypeEntry";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("e5f6a7b8-4010-4d9e-0f1a-2b3c4d5e6f7a");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,145,77,107,27,49,16,134,207,49,248,63,12,57,181,7,239,222,155,212,224,108,211,212,61,184,166,110,200,121,34,141,119,5,250,88,52,82,140,8,249,239,25,217,113,28,210,82,8,236,7,146,30,205,251,206,59,153,141,239,97,83,56,145,187,152,78,242,155,101,211,5,107,73,37,19,60,55,55,228,41,26,37,200,116,226,209,17,143,168,8,186,152,22,203,69,79,62,25,117,21,141,238,105,49,142,205,239,44,107,71,211,201,227,116,114,54,230,123,107,20,40,139,204,112,253,32,232,159,50,210,181,79,177,200,105,37,142,8,167,88,197,187,160,9,30,161,167,116,1,92,63,79,85,243,61,37,5,76,42,27,53,144,195,239,198,38,138,252,159,75,55,217,104,232,6,244,61,85,245,165,254,23,219,182,45,92,114,118,14,99,153,31,55,214,145,102,35,70,38,93,89,8,91,216,97,18,85,13,42,216,236,60,220,46,191,113,3,171,108,45,56,66,207,176,51,86,43,140,26,62,161,47,47,212,231,230,88,111,21,252,204,159,224,237,222,250,23,208,70,2,149,186,16,188,45,176,27,200,3,38,176,132,44,154,158,64,237,189,107,225,201,234,170,9,134,193,120,72,131,252,197,88,115,106,128,132,142,180,253,122,126,136,232,208,245,107,240,235,24,30,140,166,120,222,206,193,135,232,208,26,38,6,148,92,67,20,1,114,99,42,240,115,243,107,5,24,35,22,72,1,170,223,87,129,117,24,179,197,36,168,24,84,40,81,204,164,9,11,117,226,112,95,62,98,224,100,186,125,27,251,203,204,126,32,15,27,74,151,117,118,115,184,59,164,222,237,227,92,234,191,134,125,38,175,60,207,36,51,2,251,207,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("d4e5f6a7-4010-4c8d-9e0f-1a2b3c4d5e6f"));
		}

		#endregion

	}

	#endregion

}

