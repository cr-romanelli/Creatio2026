namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;

	#region Class: KnwSearchQuery

	/// <summary>
	/// Represents a search query for the knowledge base.
	/// </summary>
	internal class KnwSearchQuery
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the knowledge source identifier.
		/// </summary>
		public Guid KnwSourceId { get; set; }

		/// <summary>
		/// Gets or sets the session identifier.
		/// </summary>
		public Guid SessionId { get; set; }

		/// <summary>
		/// Gets or sets the search query.
		/// </summary>
		public string Query { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the number of items to skip in the search results.
		/// </summary>
		public int Skip { get; set; } = 0;

		/// <summary>
		/// Gets or sets the number of items to take from the search results.
		/// </summary>
		public int Take { get; set; } = 10;

		/// <summary>
		/// Gets or sets the list of keywords to filter search results.
		/// </summary>
		public IEnumerable<string> Keywords { get; set; }

		/// <summary>
		/// Gets or sets optional end-user context for additional downstream filtering.
		/// </summary>
		public KnwEndUserContext EndUserContext { get; set; }

		#endregion

	}

	#endregion

}

