namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: TrnCRealtyEventsSchema

	/// <exclude/>
	public class TrnCRealtyEventsSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public TrnCRealtyEventsSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public TrnCRealtyEventsSchema(TrnCRealtyEventsSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("91846a0a-6a6e-4ff9-9d08-12370949afd5");
			Name = "TrnCRealtyEvents";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("fa25937e-2123-40d7-8812-413aaeede544");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,82,77,79,227,48,16,189,35,241,31,70,17,135,68,170,44,184,242,81,105,91,149,21,18,130,21,13,92,208,30,92,103,26,188,242,71,100,59,101,187,43,254,59,227,56,133,144,130,196,92,18,207,188,121,51,239,105,192,112,141,190,225,2,161,68,231,184,183,235,192,230,214,172,101,221,58,30,164,53,135,7,255,15,15,128,162,245,210,212,176,220,250,128,250,108,152,26,54,106,109,205,151,69,135,108,97,130,12,18,253,119,48,108,177,65,19,118,208,199,46,189,237,114,215,146,150,48,232,242,165,120,66,205,111,72,3,92,64,86,58,51,191,67,174,194,54,43,126,167,174,166,93,41,41,64,40,238,61,164,218,39,60,112,10,51,238,241,147,74,98,233,29,24,16,218,13,237,44,43,132,141,149,21,220,154,37,223,144,146,220,174,254,160,8,224,209,84,232,38,144,8,103,184,38,89,29,237,15,87,123,192,226,157,110,192,28,99,69,91,176,55,182,29,13,22,103,31,97,137,23,92,167,135,148,231,41,81,164,134,17,184,66,33,53,87,208,56,41,162,77,169,139,253,196,80,110,27,172,230,86,181,218,60,112,213,226,121,15,157,230,157,149,191,98,67,54,158,45,215,144,39,170,41,156,28,239,162,248,8,26,201,138,129,236,202,207,185,17,168,176,162,45,130,107,145,152,247,113,62,184,120,19,116,149,158,215,88,162,110,20,15,113,111,131,207,112,109,5,87,242,31,95,41,92,118,184,188,87,115,239,209,209,217,26,114,159,110,150,221,161,183,173,19,4,178,142,88,38,176,63,39,198,224,98,210,173,101,19,200,246,102,120,214,185,115,229,75,107,103,178,78,175,172,96,165,237,119,40,190,33,132,4,164,4,187,180,78,243,144,143,4,210,224,19,118,60,59,218,243,59,70,120,114,246,185,51,96,241,87,96,19,37,238,250,199,240,151,247,103,255,75,159,151,87,101,30,229,78,231,3,0,0 };
		}

		protected override void InitializeLocalizableStrings() {
			base.InitializeLocalizableStrings();
			SetLocalizableStringsDefInheritance();
			LocalizableStrings.Add(CreateValueIsTooBigLocalizableString());
		}

		protected virtual SchemaLocalizableString CreateValueIsTooBigLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("08527c5f-5190-7f4b-1071-ccd65460ad56"),
				Name = "ValueIsTooBig",
				CreatedInPackageId = new Guid("fa25937e-2123-40d7-8812-413aaeede544"),
				CreatedInSchemaUId = new Guid("91846a0a-6a6e-4ff9-9d08-12370949afd5"),
				ModifiedInSchemaUId = new Guid("91846a0a-6a6e-4ff9-9d08-12370949afd5")
			};
			return localizableString;
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("91846a0a-6a6e-4ff9-9d08-12370949afd5"));
		}

		#endregion

	}

	#endregion

}

