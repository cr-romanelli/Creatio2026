namespace Terrasoft.Configuration
{

    #region Class: IntegrationInfo

    /// <summary>
    /// Data from OAuth2.0 integration
    /// </summary>
    public class IntegrationInfo
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the client identifier.
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// Gets or sets the client secret.
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IntegrationInfo"/> is success.
		/// </summary>
		public bool Success { get; set; }

		#endregion

	}

	#endregion

}

