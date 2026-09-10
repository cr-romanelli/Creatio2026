namespace Creatio.Copilot
{
	
	#region Class: KnwBaseItemResponse

	/// <summary>
	/// Represents a response item from the knowledge base.
	/// </summary>
	internal class KnwBaseItemResponse
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the unique identifier of the knowledge base item.
		/// </summary>
		public string ItemId { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the source link of the knowledge base item.
		/// </summary>
		public string SourceLink { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the display name of the source.
		/// </summary>
		public string SourceDisplayName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the chunk text of the knowledge base item.
		/// </summary>
		public string ChunkText { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the score of the knowledge base item.
		/// </summary>
		public double Score { get; set; }

		#endregion

	}

	#endregion

}

