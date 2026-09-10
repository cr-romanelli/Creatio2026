namespace Creatio.Copilot
{

	#region Class: KnwItemCitation

	/// <summary>
	/// Represents citation information for a knowledge item.
	/// Used to provide source links and display names for AI-generated responses.
	/// </summary>
	public class KnwItemCitation
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the unique identifier of the knowledge item.
		/// </summary>
		public string ItemId { get; set; }

		/// <summary>
		/// Gets or sets the source location.
		/// Used for citations to identify the source.
		/// </summary>
		public string SourceLocation { get; set; }

		/// <summary>
		/// Gets or sets the display name of the source.
		/// For files: file name; for sections: record name.
		/// </summary>
		public string SourceDisplayName { get; set; }

		#endregion

	}

	#endregion

}

