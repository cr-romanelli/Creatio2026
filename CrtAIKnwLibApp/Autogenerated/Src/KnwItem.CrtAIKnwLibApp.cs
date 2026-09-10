namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;

	#region Class: KnwItem

	/// <summary>
	/// Represents a knowledge item discovered or managed by a knowledge provider.
	/// </summary>
	public class KnwItem
	{

		#region Properties: Public

		/// <summary>
		/// Unique identifier of the knowledge item.
		/// </summary>
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Identifier of the knowledge source this item belongs to.
		/// </summary>
		public Guid KnwSourceId { get; set; }

		/// <summary>
		/// Type of the knowledge item (e.g., document, image, etc.).
		/// </summary>
		public string ItemType { get; set; } = string.Empty;

		/// <summary>
		/// Display name or title of the knowledge item.
		/// </summary>
		public string DisplayName { get; set; } = string.Empty;

		/// <summary>
		/// Size of the item content in bytes.
		/// </summary>
		public long SizeBytes { get; set; }

		/// <summary>
		/// Date and time when the item was last modified.
		/// </summary>
		public DateTime ModifiedOn { get; set; }

		/// <summary>
		/// Hash of the item content for integrity or change detection.
		/// </summary>
		public string Hash { get; set; }

		/// <summary>
		/// Additional metadata associated with the knowledge item.
		/// </summary>
		public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Factory function to provide a stream of the item's content.
		/// </summary>
		public Func<CancellationToken, Stream> ContentStreamFactory { get; set; }

		#endregion

	}

	#endregion

}
