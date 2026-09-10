namespace Creatio.Copilot
{
	using Terrasoft.Core.Factories;

	#region Interface: IKnwProviderFactory

	/// <summary>
	/// Factory interface for resolving knowledge provider implementations for given knowledge sources.
	/// </summary>
	public interface IKnwProviderFactory
	{

		#region Methods: Public

		/// <summary>
		/// Creates a knowledge provider instance using the specified initialization options.
		/// </summary>
		/// <param name="options">The initialization options for the knowledge provider.</param>
		/// <returns>An <see cref="IKnwProvider"/> instance for the given options.</returns>
		IKnwProvider CreateKnwProvider(KnwProviderInitializationOptions options);

		#endregion

	}

	#endregion

	#region Class: KnwProviderFactory

	/// <inheritdoc cref="IKnwProviderFactory" />
	[DefaultBinding(typeof(IKnwProviderFactory))]
	internal class KnwProviderFactory : IKnwProviderFactory
	{

		#region Methods: Public

		/// <inheritdoc cref="IKnwProviderFactory.CreateKnwProvider" />
		public IKnwProvider CreateKnwProvider(KnwProviderInitializationOptions options) {
			var provider = ClassFactory.Get<IKnwProvider>(options.KnwSource.ProviderCode);
			provider.Initialize(options);
			return provider;
		}

		#endregion

	}

	#endregion

}
