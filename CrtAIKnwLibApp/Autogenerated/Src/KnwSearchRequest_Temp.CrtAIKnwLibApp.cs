namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;

	#region Class: KnwEndUserContext

	/// <summary>
	/// Represents optional end-user identity context carried with a search request.
	/// </summary>
	public class KnwEndUserContext
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the end-user identifier in the external system.
		/// </summary>
		public string UserId { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the end-user display name.
		/// </summary>
		public string DisplayName { get; set; } = string.Empty;

		#endregion

	}

	#endregion

	#region Class: KnwSearchRequest

	/// <summary>
	/// Represents a request for searching knowledge sources with pagination and session tracking.
	/// </summary>
	/// <remarks>
	/// <para>Use this class to encapsulate the parameters required for a knowledge search operation, including the sources to query, session context, and pagination options.</para>
	/// </remarks>
	public class KnwSearchRequest_Temp
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets the mapping of knowledge source identifiers to their associated search queries.
		/// </summary>
		/// <remarks>
		/// Each key is a unique identifier for a knowledge source, and the value is a collection
		/// of search query objects for that source.
		/// </remarks>
		public IDictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>> KnwSourceQueries { get; set; }
			= new Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>>();

		/// <summary>
		/// The unique identifier for the search session.
		/// </summary>
		public Guid SessionId { get; set; } = Guid.Empty;

		/// <summary>
		/// The number of search results to skip (for pagination).
		/// </summary>
		public int Skip { get; set; } = 0;

		/// <summary>
		/// The maximum number of search results to return.
		/// </summary>
		public int Take { get; set; } = 10;

		/// <summary>
		/// Optional end-user identity context for downstream filtering.
		/// </summary>
		public KnwEndUserContext EndUserContext { get; set; }

		#endregion

	}

	#endregion

}

