namespace Creatio.Copilot
{
	using System;

	#region Class: KnwProviderRecord

	/// <summary>
	/// Represents a knowledge provider record with identification and descriptive information.
	/// </summary>
	public class KnwProviderRecord
	{

		#region Constructors: Internal

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwProviderRecord"/> class with all properties.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="name">The name of the provider.</param>
		/// <param name="description">The description of the provider.</param>
		/// <param name="code">The code of the provider.</param>
		public KnwProviderRecord(Guid id, string name, string description, string code) {
			Id = id;
			Name = name;
			Description = description;
			Code = code;
		}


		#endregion

		#region Properties: Public

		/// <summary>
		/// Gets or sets the unique identifier of the knowledge provider record.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets or sets the name of the knowledge provider.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the description of the knowledge provider.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Gets or sets the code of the knowledge provider.
		/// </summary>
		public string Code { get; }

		#endregion

	}

	#endregion

}
