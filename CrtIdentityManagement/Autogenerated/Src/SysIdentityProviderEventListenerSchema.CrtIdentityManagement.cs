namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: SysIdentityProviderEventListenerSchema

	/// <exclude/>
	public class SysIdentityProviderEventListenerSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public SysIdentityProviderEventListenerSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public SysIdentityProviderEventListenerSchema(SysIdentityProviderEventListenerSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3");
			Name = "SysIdentityProviderEventListener";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("3091ce02-4f25-40d0-9a41-fe334146f13f");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,205,88,219,114,219,54,16,125,86,102,242,15,8,155,73,168,142,74,247,57,190,164,182,236,38,106,173,200,19,217,205,67,38,153,129,200,149,132,150,2,84,0,84,172,58,254,247,46,110,18,73,81,150,146,94,38,15,30,15,200,221,197,217,179,139,131,165,56,157,129,154,211,20,200,53,72,73,149,24,235,164,43,248,152,77,10,73,53,19,60,233,74,221,203,128,107,166,151,125,202,233,4,102,184,120,252,232,238,241,163,86,161,24,159,144,225,82,105,152,29,214,214,201,37,227,127,110,60,188,134,91,157,188,133,73,145,83,121,113,59,151,160,20,238,161,214,118,101,20,179,153,224,205,111,36,108,123,94,5,191,213,234,252,108,235,171,11,147,42,131,45,152,74,6,201,197,2,153,216,110,183,201,218,165,152,52,89,15,78,11,61,237,113,13,147,21,106,180,250,78,194,4,23,164,155,83,165,94,24,2,67,192,43,41,22,44,3,105,247,191,100,72,44,7,105,125,14,14,14,200,145,42,102,51,42,151,39,126,109,173,72,238,205,200,88,200,166,88,196,45,19,239,132,72,21,73,37,80,13,29,82,204,51,251,159,242,140,100,144,131,6,34,230,224,176,42,162,5,209,83,32,180,200,24,110,35,38,33,196,209,65,9,200,123,75,218,178,130,56,30,166,83,152,209,55,216,129,228,152,68,13,160,162,246,7,244,157,23,163,156,165,36,53,60,236,164,129,188,32,103,84,65,195,118,24,233,206,146,180,98,246,103,6,121,134,212,94,73,182,192,4,221,203,185,91,16,165,49,191,148,32,5,153,224,249,146,96,211,194,45,185,202,41,227,166,137,221,242,152,112,248,228,94,197,198,185,245,83,244,241,253,199,163,147,15,223,63,141,58,246,129,125,55,152,91,170,76,75,207,89,14,25,249,76,170,207,139,92,23,18,122,124,65,37,163,92,183,15,61,80,224,153,195,90,5,222,7,61,21,187,144,43,45,77,155,189,2,29,136,50,76,199,142,25,95,238,54,185,179,40,37,224,254,60,244,0,186,92,47,231,144,117,69,94,204,248,111,52,47,224,200,69,59,137,35,19,36,50,0,91,173,251,29,251,118,167,148,79,66,24,117,14,42,149,204,102,236,65,156,142,181,175,222,169,196,118,195,14,107,4,135,164,144,153,200,216,152,173,98,33,241,144,244,43,207,44,76,123,24,91,45,54,38,241,134,7,214,170,200,115,242,249,115,61,24,150,165,192,19,130,6,63,134,45,3,33,198,195,133,188,95,65,73,109,86,6,66,45,142,243,76,222,77,65,66,156,138,156,28,159,16,252,151,216,6,127,130,29,30,0,15,120,68,158,61,107,126,119,182,236,101,81,219,135,26,226,97,75,117,136,229,161,89,16,34,207,108,194,136,194,132,25,248,229,203,228,90,12,45,255,113,155,188,124,73,34,152,205,245,50,58,44,121,98,195,6,207,117,185,75,20,198,1,86,123,71,48,79,209,211,232,46,120,220,191,32,207,239,2,178,251,231,228,135,19,92,135,253,238,159,7,207,123,215,61,33,128,235,151,228,23,193,120,28,29,146,168,19,24,126,168,201,70,2,57,233,169,213,113,140,125,215,45,204,86,181,174,246,27,244,212,27,172,230,64,190,155,50,13,67,115,229,197,222,26,91,162,122,174,209,182,79,117,58,245,6,15,224,88,8,150,17,204,143,25,133,92,163,185,81,32,241,34,226,88,61,115,94,139,202,178,67,202,96,87,43,188,11,69,33,83,176,220,251,12,76,35,151,179,116,120,106,93,90,111,80,188,87,132,236,227,205,138,183,142,87,168,75,145,34,198,191,232,40,7,95,81,23,160,10,12,47,102,7,97,168,133,68,103,167,96,173,38,101,174,40,171,151,186,86,37,3,135,74,79,165,248,100,33,4,146,46,110,83,112,34,80,198,89,225,120,183,236,217,27,193,189,180,87,13,227,120,232,152,206,68,74,14,78,108,153,220,157,33,22,184,9,226,117,117,26,240,30,199,132,53,100,177,24,253,142,57,19,133,59,129,12,186,83,215,163,192,243,8,47,148,164,228,28,188,66,146,117,114,78,211,20,179,18,50,193,9,135,73,232,82,188,231,98,111,107,11,228,84,238,152,120,29,108,187,128,206,192,119,195,188,164,217,104,89,87,113,175,144,213,253,43,131,198,4,100,130,136,53,229,41,36,184,174,131,236,218,203,61,243,145,146,155,90,139,150,247,175,20,231,139,248,190,177,147,195,215,209,29,124,191,53,182,189,139,19,169,108,8,90,227,82,57,175,237,55,30,158,243,127,161,102,46,188,63,188,123,84,174,83,135,249,245,165,60,55,131,159,81,142,198,90,158,1,206,149,208,80,204,157,21,48,6,85,25,90,223,76,213,212,156,253,171,2,225,132,28,123,89,229,26,219,152,90,140,49,206,44,230,74,93,239,150,193,152,226,192,117,85,142,129,18,23,40,50,129,202,222,117,241,142,206,157,255,198,176,218,33,198,33,185,48,183,100,123,61,136,52,108,119,92,74,96,37,230,95,40,221,251,105,247,254,226,221,138,54,118,82,201,150,84,187,148,115,161,207,192,246,4,100,137,165,43,80,92,210,252,243,209,32,124,39,60,36,251,254,238,242,231,126,213,104,255,228,224,255,95,42,234,25,248,111,85,116,72,23,223,192,193,243,148,114,71,229,23,126,42,4,239,108,45,137,123,6,41,137,104,45,22,98,68,162,110,100,190,103,164,97,176,15,113,54,231,182,250,89,55,201,134,41,168,225,120,148,155,169,95,40,253,70,224,36,205,53,70,123,173,103,121,159,202,63,138,121,245,112,236,222,178,196,208,30,59,151,216,217,15,128,81,164,237,3,241,138,210,135,102,204,27,201,72,33,153,123,100,7,113,166,108,94,195,82,65,208,40,185,150,75,55,101,172,227,118,204,139,95,25,207,146,211,145,194,34,153,159,21,68,161,77,60,255,209,131,31,70,49,174,18,251,227,0,24,173,52,161,240,207,61,120,173,245,220,12,236,59,76,84,41,221,39,117,120,21,201,93,65,187,216,71,123,119,136,110,227,175,24,251,203,173,71,248,246,210,212,242,12,78,121,32,201,164,52,144,54,49,124,187,85,111,55,103,108,95,234,159,133,156,81,29,55,230,218,33,165,170,55,232,177,87,159,154,26,55,143,234,238,105,245,33,62,251,27,17,99,55,186,101,20,0,0 };
		}

		protected override void InitializeLocalizableStrings() {
			base.InitializeLocalizableStrings();
			SetLocalizableStringsDefInheritance();
			LocalizableStrings.Add(CreateDefaultIdentityProviderCannotBeDeletedLocalizableString());
			LocalizableStrings.Add(CreateServerURLMustBeAnAbsoluteHttpOrHttpsURLLocalizableString());
			LocalizableStrings.Add(CreateProviderDescriptionMustNotContainHtmlMarkupLocalizableString());
			LocalizableStrings.Add(CreateProviderNameMustNotContainHtmlMarkupLocalizableString());
		}

		protected virtual SchemaLocalizableString CreateDefaultIdentityProviderCannotBeDeletedLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("96f623b3-2f5e-120f-1035-3df1a9fd6506"),
				Name = "DefaultIdentityProviderCannotBeDeleted",
				CreatedInPackageId = new Guid("5c5d2e15-85e3-46de-ba5d-56368b920cde"),
				CreatedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3"),
				ModifiedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateServerURLMustBeAnAbsoluteHttpOrHttpsURLLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("d344ec4d-766c-82d5-f127-1b72f59387bb"),
				Name = "ServerURLMustBeAnAbsoluteHttpOrHttpsURL",
				CreatedInPackageId = new Guid("5c5d2e15-85e3-46de-ba5d-56368b920cde"),
				CreatedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3"),
				ModifiedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateProviderDescriptionMustNotContainHtmlMarkupLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("e6138140-f010-556f-d2b0-0122c57a96ba"),
				Name = "ProviderDescriptionMustNotContainHtmlMarkup",
				CreatedInPackageId = new Guid("5c5d2e15-85e3-46de-ba5d-56368b920cde"),
				CreatedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3"),
				ModifiedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateProviderNameMustNotContainHtmlMarkupLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("d5a84461-bfbd-721f-f494-91b7ca9a14b3"),
				Name = "ProviderNameMustNotContainHtmlMarkup",
				CreatedInPackageId = new Guid("5c5d2e15-85e3-46de-ba5d-56368b920cde"),
				CreatedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3"),
				ModifiedInSchemaUId = new Guid("3688bb08-16f7-468a-9c8e-d625046484b3")
			};
			return localizableString;
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("3688bb08-16f7-468a-9c8e-d625046484b3"));
		}

		#endregion

	}

	#endregion

}

