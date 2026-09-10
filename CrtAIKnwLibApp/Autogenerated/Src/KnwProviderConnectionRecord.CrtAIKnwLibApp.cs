namespace Creatio.Copilot
{
	using System;

	#region Class: KnwProviderConnectionRecord

	/// <summary>
	/// Represents a connection record for a knowledge provider, including connection and authentication parameters.
	/// </summary>
	public class KnwProviderConnectionRecord
	{

		#region Constructors: Internal

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwProviderConnectionRecord"/> class with the specified parameters.
		/// </summary>
		/// <param name="id">The unique identifier of the connection record.</param>
		/// <param name="name">The name of the connection record.</param>
		/// <param name="knowledgeProviderId">The unique identifier of the knowledge provider.</param>
		/// <param name="connectionParams">Arbitrary JSON container for connection and authentication parameters.</param>
		internal KnwProviderConnectionRecord(Guid id, string name, Guid knowledgeProviderId, string connectionParams) {
			Id = id;
			Name = name;
			KnowledgeProviderId = knowledgeProviderId;
			ConnectionParams = connectionParams;
		}


		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets the unique identifier of the connection record.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets the name of the connection record.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the unique identifier of the knowledge provider.
		/// </summary>
		public Guid KnowledgeProviderId { get; }

		/// <summary>
		/// Arbitrary JSON container for connection & auth params (e.g. baseUrl, apiKey).
		/// </summary>
		public string ConnectionParams { get; }

		#endregion

	}

	#endregion

}
