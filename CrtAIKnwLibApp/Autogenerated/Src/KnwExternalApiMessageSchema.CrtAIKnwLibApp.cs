namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: KnwExternalApiMessageSchema

	/// <exclude/>
	public class KnwExternalApiMessageSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public KnwExternalApiMessageSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public KnwExternalApiMessageSchema(KnwExternalApiMessageSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f");
			Name = "KnwExternalApiMessage";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,149,86,219,110,26,49,16,125,222,72,249,7,151,246,97,145,88,62,0,20,85,21,185,40,202,165,105,147,54,207,142,119,0,43,198,38,182,151,64,35,254,189,190,45,120,111,148,190,68,97,102,124,230,204,197,103,205,241,2,212,18,19,64,19,9,88,83,49,156,136,37,101,66,159,158,124,156,158,36,133,162,124,134,158,64,74,172,196,84,27,231,98,33,248,184,213,35,193,216,141,231,179,132,25,21,28,77,24,86,106,132,110,248,251,197,90,131,228,152,125,91,210,59,80,10,207,192,5,82,238,205,72,105,147,153,32,98,15,116,197,39,31,238,204,30,93,112,115,140,107,147,225,65,210,21,214,30,51,89,250,31,136,88,191,65,150,150,230,79,80,162,144,4,30,201,28,22,248,222,212,140,206,80,175,53,83,111,220,137,114,205,103,160,76,139,248,143,66,104,124,177,38,0,57,228,37,246,13,108,44,104,123,208,1,212,201,188,224,175,79,66,220,98,57,131,26,88,197,119,12,179,115,170,240,11,235,38,85,250,15,98,229,176,190,23,250,82,20,188,137,19,249,14,98,172,48,163,230,244,91,97,242,54,64,98,231,65,20,63,156,11,41,133,108,128,68,190,3,24,87,192,65,82,114,137,25,123,193,228,181,134,82,243,246,198,97,199,128,231,126,205,170,59,119,7,122,46,114,187,113,197,11,163,36,44,156,251,191,220,225,104,225,216,10,110,5,49,165,254,129,60,44,87,250,75,129,52,155,203,129,216,105,160,162,242,115,80,158,6,91,212,68,228,48,176,9,146,36,152,167,129,102,0,235,91,231,135,139,8,1,204,167,179,19,246,5,6,26,81,213,233,14,187,239,154,150,208,41,74,171,52,208,217,25,226,5,99,14,63,36,72,36,232,66,242,58,5,15,177,117,127,181,220,196,7,170,148,118,29,48,164,158,228,230,10,244,174,53,143,46,48,173,183,162,90,75,32,235,216,126,242,208,195,107,117,111,88,126,151,207,115,170,225,209,106,88,90,207,214,247,53,148,156,202,42,234,97,1,124,91,97,62,243,187,81,31,225,209,5,116,111,94,89,76,96,211,93,78,7,133,62,250,90,31,4,26,117,241,141,71,68,176,38,243,255,154,234,246,216,11,209,38,193,173,87,34,222,197,224,33,118,27,163,101,126,167,134,38,74,119,230,146,45,193,10,80,143,150,90,150,189,89,133,205,160,148,216,81,117,200,255,84,235,113,140,186,196,27,38,112,158,105,33,50,230,212,182,6,215,37,211,227,118,110,121,41,180,93,172,90,148,186,2,37,131,61,227,66,103,83,167,183,163,216,175,204,172,108,34,106,37,185,25,180,207,215,174,230,181,92,78,138,51,167,203,246,17,192,187,114,185,136,140,216,94,100,202,108,89,51,93,135,240,215,218,228,197,59,43,56,172,151,230,194,64,158,57,93,170,166,205,97,105,246,14,56,217,152,64,188,194,148,217,142,85,99,140,216,79,141,252,234,38,145,246,111,71,224,145,195,20,23,76,215,14,117,95,217,230,149,104,95,242,118,105,56,78,245,101,164,16,251,219,96,21,175,91,33,226,51,109,122,109,117,60,86,128,90,170,7,172,231,70,208,190,244,110,247,106,235,57,171,225,71,132,189,29,254,198,172,8,47,144,100,133,101,44,207,254,128,129,225,240,142,26,64,169,231,83,173,121,248,44,228,171,123,118,14,119,79,51,45,164,209,158,240,197,107,62,216,6,101,93,123,230,206,18,196,52,20,182,178,52,13,149,6,61,95,192,248,136,150,58,136,106,51,187,81,205,176,39,102,143,10,9,14,63,117,27,132,89,173,168,225,57,76,67,216,192,138,173,42,63,190,126,40,97,86,171,29,195,118,213,245,214,170,209,216,254,2,179,169,173,62,194,11,0,0 };
		}

		protected override void InitializeLocalizableStrings() {
			base.InitializeLocalizableStrings();
			SetLocalizableStringsDefInheritance();
			LocalizableStrings.Add(CreateInternalErrorLocalizableString());
			LocalizableStrings.Add(CreateSessionNotFoundLocalizableString());
			LocalizableStrings.Add(CreateInvalidRequestLocalizableString());
			LocalizableStrings.Add(CreateIndexNotFoundLocalizableString());
			LocalizableStrings.Add(CreateIngestionDisabledLocalizableString());
			LocalizableStrings.Add(CreateChunkTooLargeLocalizableString());
			LocalizableStrings.Add(CreateMaxFileSourcesExceededLocalizableString());
			LocalizableStrings.Add(CreateMaxFilesPerSourceExceededLocalizableString());
			LocalizableStrings.Add(CreateIngestionQuotaExceededLocalizableString());
			LocalizableStrings.Add(CreateGenericFallbackLocalizableString());
		}

		protected virtual SchemaLocalizableString CreateInternalErrorLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("7c59e629-853e-349d-a43d-f37f4816974c"),
				Name = "InternalError",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateSessionNotFoundLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("29489872-dd0c-b45f-5ffd-07565b77f789"),
				Name = "SessionNotFound",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateInvalidRequestLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("71cf5e63-fa13-e0bb-52d8-348e961a37ef"),
				Name = "InvalidRequest",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateIndexNotFoundLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("810432d3-4cda-7c9f-8aef-9b0b1aae8652"),
				Name = "IndexNotFound",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateIngestionDisabledLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("0d67ca44-1fee-aaa3-9520-621255b4e0fc"),
				Name = "IngestionDisabled",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateChunkTooLargeLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("d9ee4a84-630e-7f5d-fea1-29de9a0013cd"),
				Name = "ChunkTooLarge",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateMaxFileSourcesExceededLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("5534c090-8b8c-665b-9df2-cc48a518795a"),
				Name = "MaxFileSourcesExceeded",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateMaxFilesPerSourceExceededLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("28f7268e-0253-c3ad-0fe3-71d9c8d2db89"),
				Name = "MaxFilesPerSourceExceeded",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateIngestionQuotaExceededLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("1f1cd3e7-a77e-1156-3a71-ff577ab9eef7"),
				Name = "IngestionQuotaExceeded",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		protected virtual SchemaLocalizableString CreateGenericFallbackLocalizableString() {
			SchemaLocalizableString localizableString = new SchemaLocalizableString() {
				UId = new Guid("5d088373-01b6-3a3d-5892-cda63791c377"),
				Name = "GenericFallback",
				CreatedInPackageId = new Guid("7fde53f3-8dfb-49d2-af8d-0767dd13dc3b"),
				CreatedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"),
				ModifiedInSchemaUId = new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f")
			};
			return localizableString;
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("ef71342c-1133-4736-a8cf-9376ab60a47f"));
		}

		#endregion

	}

	#endregion

}

