namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;

	#region Interface: IMcpToolValidator

	internal interface IMcpToolValidator
	{

		#region Methods: Public

		/// <summary>
		/// Validates the admin-configured tool record against the authorized tool set
		/// the caller already resolved for this request. Membership in
		/// <paramref name="authorizedTools"/> (keyed by process SysSchema.Id) is the
		/// runtime-availability check; the validator does not query the runtime itself.
		/// </summary>
		McpToolValidationResult Validate(McpToolModel tool,
			IReadOnlyDictionary<Guid, RuntimeToolDefinition> authorizedTools);

		#endregion

	}

	#endregion

}

