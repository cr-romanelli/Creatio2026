namespace Terrasoft.Configuration
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;

	#region Class: RuntimeToolDefinitionSchema

	/// <exclude/>
	public class RuntimeToolDefinitionSchema : Terrasoft.Core.SourceCodeSchema
	{

		#region Constructors: Public

		public RuntimeToolDefinitionSchema(SourceCodeSchemaManager sourceCodeSchemaManager)
			: base(sourceCodeSchemaManager) {
		}

		public RuntimeToolDefinitionSchema(RuntimeToolDefinitionSchema source)
			: base( source) {
		}

		#endregion

		#region Methods: Protected

		protected override void InitializeProperties() {
			base.InitializeProperties();
			UId = new Guid("65e0e57e-15b3-4168-9e33-de8dd84bd7e0");
			Name = "RuntimeToolDefinition";
			ParentSchemaUId = new Guid("50e3acc0-26fc-4237-a095-849a1d534bd3");
			CreatedInPackageId = new Guid("8bbb0fd1-b7b5-4078-a4ae-8d28ce49d028");
			ZipBody = new byte[] { 31,139,8,0,0,0,0,0,4,0,141,145,209,106,195,48,12,69,159,27,200,63,8,250,190,15,88,31,147,50,202,96,148,38,253,0,199,81,50,129,35,7,217,30,43,97,255,62,59,25,99,131,46,217,139,176,175,142,46,87,136,213,128,110,84,26,161,70,17,229,108,231,31,10,203,29,245,65,148,39,203,121,54,229,217,46,56,226,30,170,155,243,56,28,242,44,42,123,193,62,182,161,48,202,185,71,184,4,246,52,96,109,173,41,177,35,166,101,54,130,196,30,133,149,1,157,200,191,192,221,52,195,223,182,103,177,35,138,39,140,222,231,208,24,210,75,127,156,223,208,196,113,40,20,151,40,244,134,149,126,197,65,193,4,61,250,3,184,84,62,238,226,199,119,212,193,227,10,233,188,164,77,75,116,90,104,76,217,182,108,79,238,200,170,49,216,174,128,63,150,126,38,110,97,46,155,33,94,226,109,182,169,47,239,202,6,209,255,192,107,242,102,13,123,10,212,66,10,122,61,221,141,184,71,110,151,27,205,255,69,253,45,70,237,19,117,49,247,33,87,2,0,0 };
		}

		#endregion

		#region Methods: Public

		public override void GetParentRealUIds(Collection<Guid> realUIds) {
			base.GetParentRealUIds(realUIds);
			realUIds.Add(new Guid("65e0e57e-15b3-4168-9e33-de8dd84bd7e0"));
		}

		#endregion

	}

	#endregion

}

