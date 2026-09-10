namespace Creatio.Copilot
{
	using System;

	#region Class: KnwSourceRecord

	/// <summary>
	/// Represents a knowledge source record, including its name, connection, and state information.
	/// </summary>
	public class KnwSourceRecord
	{

		#region Constructors: Internal

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwSourceRecord"/> class with the specified property values.
		/// </summary>
		/// <param name="id">The unique identifier of the knowledge source.</param>
		/// <param name="name">The name of the knowledge source.</param>
		/// <param name="description">The description of the knowledge source.</param>
		/// <param name="status">The current status of the knowledge source.</param>
		/// <param name="knwProvider">The provider record associated with the knowledge source.</param>
		/// <param name="knwProviderConnection">The provider connection record associated with the knowledge source.</param>
		/// <param name="lastIndexedOn">The last indexed timestamp of the knowledge source.</param>
		/// <param name="knwSourceConfig">The configuration JSON for the knowledge source.</param>
		internal KnwSourceRecord(Guid id, string name, string description, DateTime? lastIndexedOn,
				KnwSourceStatusEnum status, KnwProviderRecord knwProvider,
				KnwProviderConnectionRecord knwProviderConnection, string knwSourceConfig) {
			Id = id;
			Name = name;
			Description = description;
			LastIndexedOn = lastIndexedOn;
			Status = status;
			KnwProvider = knwProvider;
			KnwProviderConnection = knwProviderConnection;
			KnwSourceConfig = knwSourceConfig;
		}

		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets or sets the unique identifier of the knowledge source.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets or sets the name of the knowledge source.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the description of the knowledge source.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Gets or sets the current status of the knowledge source.
		/// </summary>
		public KnwSourceStatusEnum Status { get; }

		/// <summary>
		/// Gets the provider record associated with the knowledge source.
		/// </summary>
		public KnwProviderRecord KnwProvider { get; }

		/// <summary>
		/// Gets the provider connection record associated with the knowledge source.
		/// </summary>
		public KnwProviderConnectionRecord KnwProviderConnection { get; }

		/// <summary>
		/// Gets the connection parameters for the knowledge source provider connection.
		/// </summary>
		public string ConnectionParams => KnwProviderConnection?.ConnectionParams;

		/// <summary>
		/// Gets the provider code for the knowledge source.
		/// </summary>
		public string ProviderCode => KnwProvider?.Code;

		/// <summary>
		/// Gets the configuration JSON for the knowledge source.
		/// </summary>
		public string KnwSourceConfig { get; }

		/// <summary>
		/// Gets or sets the last indexed timestamp of the knowledge source. Can be null if never indexed.
		/// </summary>
		public DateTime? LastIndexedOn { get; }

		#endregion

		#region Methods: Public

		/// <summary>
		/// Validates that required properties are not null.
		/// </summary>
		public virtual void Validate() {
			if (KnwProvider == null) {
				throw new InvalidOperationException("Cannot find a valid KnwProvider.");
			}
			if (KnwProviderConnection == null) {
				throw new InvalidOperationException("Cannot find a valid KnwProviderConnection.");
			}
		}

		#endregion

	}

	#endregion

}
