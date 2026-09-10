namespace Creatio.Copilot
{
	using Terrasoft.Core;

	#region Class: KnowledgeProviderOptions

	/// <summary>
	/// Initialization options for a knowledge provider, including the source record and connection details.
	/// </summary>
	public class KnwProviderInitializationOptions
	{

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwProviderInitializationOptions"/> class
		/// with the specified source, connection, and provider.
		/// </summary>
		/// <param name="userConnection">The User Connection to initialize the provider with.</param>
		/// <param name="knwSource">The knowledge source to initialize the provider with.</param> >
		public KnwProviderInitializationOptions(UserConnection userConnection, KnwSourceRecord knwSource) {
			KnwSource = knwSource;
			UserConnection = userConnection;
		}

		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets the user connection associated with the provider initialization.
		/// </summary>
		public UserConnection UserConnection { get; }

		/// <summary>
		/// Gets the knowledge source used for provider initialization.
		/// </summary>
		public KnwSourceRecord KnwSource { get; }

		#endregion

	}

	#endregion

}
