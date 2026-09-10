namespace Creatio.Copilot
{
	using System;
	using System.Runtime.Serialization;

	#region Class: RetrieveSettingsOverride

	/// <summary>
	/// Represents override values for retrieval settings that can be applied at the knowledge source or client level.
	/// </summary>
	[Serializable]
	[DataContract]
	public class RetrieveSettingsOverride
	{
		#region Properties: Public

		[DataMember(Name = "maxDegreeOfParallelism")]
		public int? MaxDegreeOfParallelism { get; set; }

		[DataMember(Name = "scoreThreshold")]
		public float? ScoreThreshold { get; set; }

		[DataMember(Name = "keywordBoostingEnabled")]
		public bool? KeywordBoostingEnabled { get; set; }

		#endregion
	}

	#endregion

	#region Class: RankingSettingsOverride

	/// <summary>
	/// Represents override values for ranking settings that control result re-ranking behavior.
	/// </summary>
	[Serializable]
	[DataContract]
	public class RankingSettingsOverride
	{
		#region Properties: Public

		// Reserved for future expansion (freshness, sourceType, contentLength, similarity)

		#endregion
	}

	#endregion

	#region Class: ConfigurationScopeRequest

	/// <summary>
	/// Represents a request to create or update a configuration scope in the knowledge management microservice.
	/// </summary>
	[Serializable]
	[DataContract]
	public class ConfigurationScopeRequest
	{
		#region Properties: Public

		[DataMember(Name = "knowledgeSourceId")]
		public Guid? KnowledgeSourceId { get; set; }

		[DataMember(Name = "retrieveSettings")]
		public RetrieveSettingsOverride RetrieveSettings { get; set; }

		[DataMember(Name = "rankingSettings")]
		public RankingSettingsOverride RankingSettings { get; set; }

		#endregion
	}

	#endregion

	#region Class: ConfigurationMetadata

	/// <summary>
	/// Provides metadata about the resolved configuration, including the source of each setting group.
	/// </summary>
	[Serializable]
	[DataContract]
	public class ConfigurationMetadata
	{
		#region Properties: Public

		[DataMember(Name = "retrieveSettingsSource")]
		public string RetrieveSettingsSource { get; set; }

		[DataMember(Name = "rankingSettingsSource")]
		public string RankingSettingsSource { get; set; }

		[DataMember(Name = "fromCache")]
		public bool FromCache { get; set; }

		[DataMember(Name = "resolvedAt")]
		public DateTime? ResolvedAt { get; set; }

		#endregion
	}

	#endregion

	#region Class: ConfigurationScopeResponse

	/// <summary>
	/// Represents the response from the microservice when retrieving or saving configuration.
	/// </summary>
	[Serializable]
	[DataContract]
	public class ConfigurationScopeResponse
	{
		#region Properties: Public

		[DataMember(Name = "id")]
		public Guid Id { get; set; }

		[DataMember(Name = "knowledgeSourceId")]
		public Guid? KnowledgeSourceId { get; set; }

		[DataMember(Name = "clientId")]
		public string ClientId { get; set; }

		[DataMember(Name = "retrieveSettings")]
		public RetrieveSettingsOverride RetrieveSettings { get; set; }

		[DataMember(Name = "rankingSettings")]
		public RankingSettingsOverride RankingSettings { get; set; }

		[DataMember(Name = "metadata")]
		public ConfigurationMetadata Metadata { get; set; }

		#endregion
	}

	#endregion

	#region Class: KnwSourceConfigRequest

	/// <summary>
	/// Represents a simplified request for saving knowledge source configuration from the UI.
	/// </summary>
	[Serializable]
	[DataContract]
	public class KnwSourceConfigRequest
	{
		#region Properties: Public

		[DataMember(Name = "knwSourceId")]
		public Guid KnwSourceId { get; set; }

		[DataMember(Name = "scoreThreshold")]
		public float? ScoreThreshold { get; set; }

		[DataMember(Name = "keywordBoostingEnabled")]
		public bool? KeywordBoostingEnabled { get; set; }

		#endregion
	}

	#endregion
}

