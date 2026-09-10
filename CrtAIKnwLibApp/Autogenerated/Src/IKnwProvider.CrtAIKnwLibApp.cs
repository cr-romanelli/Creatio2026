namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	#region Class: KnwProviderDiscoveryOptions

	/// <summary>
	/// Options for discovering knowledge items from a provider.
	/// </summary>
	public class KnwProviderDiscoveryOptions
	{

		#region Properties: Public

		/// <summary>
		/// Optional set of item identifiers to discover.
		/// </summary>
		public ISet<string> ItemIds { get; set; }

		/// <summary>
		/// Only return items modified since this date.
		/// </summary>
		public DateTime? ModifiedSince { get; set; }

		#endregion

	}

	#endregion

	#region Interface: IKnwProvider

	/// <summary>
	/// Interface for a knowledge provider that can discover and initialize knowledge items.
	/// </summary>
	public interface IKnwProvider
	{

		#region Methods: Public

		/// <summary>
		/// Discovers knowledge items based on the provided options.
		/// </summary>
		/// <param name="options">Discovery options, such as modification date filter.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>An enumerable collection of discovered <see cref="KnwItem"/> objects.</returns>
		IEnumerable<KnwItem> Discover(KnwProviderDiscoveryOptions options, CancellationToken ct = default);

		/// <summary>
		/// Initializes the provider with the specified initialization options.
		/// </summary>
		/// <param name="initializationOptions">Initialization options for the provider.</param>
		void Initialize(KnwProviderInitializationOptions initializationOptions);

		/// <summary>
		/// Retrieves citation information for multiple knowledge items in bulk.
		/// </summary>
		/// <param name="itemIds">Collection of item identifiers to retrieve citations for.</param>
		/// <returns>Dictionary mapping item IDs to their citation information. Missing items are omitted.</returns>
		Dictionary<string, KnwItemCitation> GetItemsCitations(ISet<string> itemIds);

		/// <summary>
		/// Filters the provided item IDs based on user access rights.
		/// </summary>
		/// <param name="itemIds">Set of item identifiers to filter.</param>
		/// <returns>Set of item IDs that the current user has permission to access.</returns>
		ISet<string> GetUserAllowedItems(ISet<string> itemIds);

		#endregion

	}

	#endregion

	#region Interface: IKnwBatchProvider

	/// <summary>
	/// Extended provider interface for knowledge sources that support batch processing.
	/// Implement this interface for providers that need to process large datasets incrementally.
	/// </summary>
	public interface IKnwBatchProvider : IKnwProvider
	{

		#region Methods: Public

		/// <summary>
		/// Discovers knowledge items in batches.
		/// Yields batches of items lazily, allowing the caller to process each batch
		/// without loading the entire dataset into memory at once.
		/// </summary>
		/// <param name="options">Discovery options, such as modification date filter.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>An enumerable stream of batches, where each batch contains a read-only collection of items.</returns>
		IEnumerable<IReadOnlyList<KnwItem>> DiscoverInBatches(
			KnwProviderDiscoveryOptions options,
			CancellationToken ct = default);

		#endregion

	}

	#endregion
}

