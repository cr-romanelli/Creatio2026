namespace Creatio.Copilot
{
	using Creatio.Copilot.Actions;
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;

	#region Class: RetrieveKnwSourceContextAction

	/// <summary>
	/// Contextual document provider code action.
	/// </summary>
	public class RetrieveKnwSourceContextAction : BaseExecutableCodeAction, IUserConnectionRequired
	{

		#region Class: KnowledgeSourceQuery

		/// <summary>
		/// Represents a knowledge source and its associated query for RAG search.
		/// </summary>
		private sealed class KnowledgeSourceQuery
		{

			#region Properties: Public

			/// <summary>
			/// The unique identifier of the knowledge source.
			/// </summary>
			public Guid KnwSourceId { get; set; }

			/// <summary>
			/// The query associated with the knowledge source.
			/// </summary>
			public string KnwSourceQuery { get; set; }

			/// <summary>
			/// Comma-separated keywords extracted from the user query for filtering.
			/// </summary>
			public string Keywords { get; set; }

			#endregion

		}

		#endregion

		#region Class: SearchResultsSummary

		private sealed class SearchResultsSummary
		{

			#region Properties: Public

			public bool HasCitationEnabledResults { get; set; }

			public bool HasCitationDisabledResults { get; set; }

			#endregion

		}

		#endregion

		#region Constants: Private

		private const string CaptionValue = "Retrieve contextual knowledge from indexed sources using semantic search";
		private const string DescriptionValue =
			"Use this action to semantically search indexed knowledge sources and retrieve relevant context for the" +
			" user’s question. Provide a clear query based on intent. Use returned knowledge to ground your " +
			"response and avoid hallucinations.";
		private const string KnwSourcesParameter = "KnwSourceQueries";
		private const string KnwSectionTitle = "#KNOWLEDGE SOURCES:";
		private const string FailedKnwSectionTitle = "#FAILED KNOWLEDGE SOURCES:";
		private const string GroundingSkillName = "KMSGroundingPostprocessorSkill";
		private const string MixedCitationInstructions =
			"### Mixed citation mode\n" +
			"- Cite only chunks that include a source id and source metadata.\n" +
			"- Chunks that appear only as `{content: ...}` are grounding-only and must not be cited or listed in References.\n" +
			"- Do not infer or invent a source name, file name, title, link, path, or location for grounding-only chunks.\n";
		private const string NoCitationInstructions =
			"Use the grounded content below without citation markers or a References section.\n" +
			"- Do not add citation numbers such as [1] or [1][2].\n" +
			"- Do not add a References section.\n" +
			"- Do not infer or invent source names, file names, titles, links, paths, or locations.\n";
		private const string CitationPromptFallback =
			"Use the cited chunks below to ground the answer.\n" +
			"- Every grounded factual sentence must include a citation.\n" +
			"- Add a References section only for cited chunks.\n" +
			"- Do not invent sources or cite unsupported statements.\n";

		#endregion

		#region  Fields: Private

		private static readonly IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> EmptySettings =
			new ReadOnlyDictionary<Guid, KnwSourceAttachmentSettings>(
				new Dictionary<Guid, KnwSourceAttachmentSettings>(0));

		private UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="RetrieveKnwSourceContextAction"/> class.
		/// </summary>
		public RetrieveKnwSourceContextAction()
		{
			Parameters = new List<SourceCodeActionParameter> {
				new SourceCodeActionParameter {
					Name = KnwSourcesParameter,
					Caption = new LocalizableString("Knowledge Source Queries"),
					Description = new LocalizableString(
						"Generate array of objects where each object contains a knowledge source ID and its " +
						"corresponding query. If no knowledge sources send empty array."),
					IsRequired = true,
					DataValueTypeUId = DataValueType.CompositeObjectListDataValueTypeUId,
					ItemProperties = new List<SourceCodeActionParameter> {
						new SourceCodeActionParameter {
							Name = "knwSourceId",
							Caption = new LocalizableString("Knowledge Source ID"),
							Description = new LocalizableString("The unique identifier of the knowledge source."),
							IsRequired = true,
							DataValueTypeUId = DataValueType.GuidDataValueTypeUId
						},
						new SourceCodeActionParameter {
							Name = "knwSourceQuery",
							Caption = new LocalizableString("Knowledge Source Query"),
							Description = new LocalizableString("The query associated with the knowledge source."),
							IsRequired = true,
							DataValueTypeUId = DataValueType.TextDataValueTypeUId
						},
						new SourceCodeActionParameter {
							Name = "keywords",
							Caption = new LocalizableString("Keywords"),
							Description = new LocalizableString(
								"Comma-separated keywords to filter search results. Extract important terms, technical names, " +
								"or specific identifiers from the user's query."),
							IsRequired = false,
							DataValueTypeUId = DataValueType.TextDataValueTypeUId
						}
					}
				}
			};
		}

		#endregion

		#region Methods: Private

		private static void AppendFormattedLineOfKnwItem(StringBuilder contentBuilder, Guid knwSourceId,
				string message, bool useCitations, double? score = null, string source = null, string location = null)
		{
			if (!useCitations)
			{
				contentBuilder.AppendLine($"- {{content: {message}}}");
				return;
			}
			var metadata = new List<string>();
			if (score.HasValue && score.Value > 0)
			{
				metadata.Add($"score={score.Value:F2}");
			}
			if (!string.IsNullOrWhiteSpace(source))
			{
				metadata.Add($"sourceName=\"{source}\"");
			}
			if (!string.IsNullOrWhiteSpace(location))
			{
				metadata.Add($"location=\"{location}\"");
			}
			string metadataStr = metadata.Count > 0 ? $" ({string.Join(", ", metadata)})" : string.Empty;
			contentBuilder.AppendLine($"- [{knwSourceId}]{metadataStr} {{content: {message}}}");
		}

		private string TryGetCitationPrompt()
		{
			try
			{
				var schemaService = _userConnection.GetIntentSchemaService();
				var citationIntent = schemaService.FindSchemaByName(GroundingSkillName);
				if (citationIntent == null || citationIntent.Status != CopilotIntentStatus.Active)
				{
					KnwLibUtils.Logger.Warn(
						$"Grounding intent '{GroundingSkillName}' not found or inactive.");
					return null;
				}
				return citationIntent.Prompt;
			}
			catch (Exception ex)
			{
				KnwLibUtils.Logger.Error("Failed to append citation prompt.", ex);
				return null;
			}
		}

		private void AppendGroundingPrompt(StringBuilder contentBuilder, SearchResultsSummary summary)
		{
			contentBuilder.AppendLine("#Instructions");
			if (!summary.HasCitationEnabledResults)
			{
				contentBuilder.Append(NoCitationInstructions);
				return;
			}
			if (summary.HasCitationDisabledResults)
			{
				contentBuilder.Append(MixedCitationInstructions);
			}
			contentBuilder.Append(TryGetCitationPrompt() ?? CitationPromptFallback);
		}

		private static IEnumerable<string> ParseKeywords(string keywords)
		{
			if (string.IsNullOrWhiteSpace(keywords))
			{
				return Enumerable.Empty<string>();
			}
			return keywords
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(k => k.Trim())
				.Where(k => !string.IsNullOrWhiteSpace(k))
				.ToArray();
		}

		private CopilotSession FindCopilotSessionById(Guid sessionId)
		{
			try
			{
				var sessionManager = ClassFactory.Get<ICopilotSessionManager>(
					new ConstructorArgument("userConnection", _userConnection));
				return sessionManager.FindById(sessionId);
			}
			catch (Exception ex)
			{
				KnwLibUtils.Logger.Error($"Failed to retrieve session {sessionId}", ex);
				return null;
			}
		}

		private IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> GetAttachedSourceSettings(Guid sessionId)
		{
			try
			{
				CopilotSession copilotSession = FindCopilotSessionById(sessionId);
				if (copilotSession == null)
				{
					KnwLibUtils.Logger.Warn($"Session not found: {sessionId}");
					return EmptySettings;
				}

				var sourceSettingsProvider = ClassFactory.Get<IKnwSourceAttachmentSettingsProvider>(
					new ConstructorArgument("userConnection", _userConnection));

				return sourceSettingsProvider.GetSettingsBySession(copilotSession) ?? EmptySettings;
			}
			catch (Exception ex)
			{
				KnwLibUtils.Logger.Error("Failed to retrieve knowledge source attachment settings for session.", ex);
				return EmptySettings;
			}
		}

		private static bool ShouldUseCitations(Guid knwSourceId,
				IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> attachedSourceSettings)
		{
			return attachedSourceSettings == null
				|| !attachedSourceSettings.TryGetValue(knwSourceId, out KnwSourceAttachmentSettings settings)
				|| settings == null
				|| settings.UseCitations;
		}

		private List<KnowledgeSourceQuery> FilterToAttachedSources(
				List<KnowledgeSourceQuery> knwSources,
				IEnumerable<Guid> attachedSourceIds,
				StringBuilder contentBuilder,
				Guid sessionId)
		{
			var filteredSources = new List<KnowledgeSourceQuery>();

			if (attachedSourceIds == null || !attachedSourceIds.Any())
			{
				KnwLibUtils.Logger.Info($"Session {sessionId} has no attached knowledge sources.");
				return filteredSources;
			}

			var attachedSet = new HashSet<Guid>(attachedSourceIds);
			var rejectedSources = new List<Guid>();

			foreach (var source in knwSources)
			{
				if (attachedSet.Contains(source.KnwSourceId))
				{
					filteredSources.Add(source);
				}
				else
				{
					rejectedSources.Add(source.KnwSourceId);
				}
			}

			if (rejectedSources.Any())
			{
				KnwLibUtils.Logger.Warn(
					$"Rejected unauthorized knowledge source access. " +
					$"SessionId={sessionId}, " +
					$"RejectedSourceIds=[{string.Join(", ", rejectedSources)}], " +
					$"AttachedSourceIds=[{string.Join(", ", attachedSourceIds)}]");

				contentBuilder.AppendLine(
					$"Note: {rejectedSources.Count} rejected source(s) excluded from retrieval.");
			}

			if (filteredSources.Count == 0)
			{
				contentBuilder.AppendLine("No requested knowledge sources are attached.");
			}

			return filteredSources;
		}

		private void HandleKnowledgeRetrieval(ActionExecutionOptions options,
				StringBuilder contentBuilder, Guid sessionId)
		{
			contentBuilder.AppendLine(KnwSectionTitle);
			if (!options.ParameterValues.TryGetValue(KnwSourcesParameter, out string value) || value.IsNullOrEmpty())
			{
				contentBuilder.AppendLine("No knowledge sources or queries provided.");
				return;
			}
			List<KnowledgeSourceQuery> knwSources;
			try
			{
				knwSources = JsonConvert.DeserializeObject<List<KnowledgeSourceQuery>>(value);
			}
			catch (Exception ex)
			{
				const string errorMsg = "Failed to parse KnwSourceQueries JSON parameter.";
				KnwLibUtils.Logger.Error($"{errorMsg}, value: {value}", ex);
				contentBuilder.AppendLine(errorMsg);
				return;
			}
			if (knwSources.IsNullOrEmpty())
			{
				contentBuilder.AppendLine("No knowledge sources or queries provided.");
				return;
			}

			IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> attachedSourceSettings =
				GetAttachedSourceSettings(sessionId);
			knwSources = FilterToAttachedSources(knwSources, attachedSourceSettings.Keys, contentBuilder, sessionId);

			if (knwSources.IsNullOrEmpty())
			{
				contentBuilder.AppendLine("No authorized knowledge sources available for this request.");
				return;
			}

			Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>> knwDict = GetKnowledgeSourceQueriesDict(knwSources,
				contentBuilder);
			if (knwDict.Count == 0)
			{
				return;
			}
			IKnwSearchClient_Temp knwSearchClient = ResolveKnwSearchClient(contentBuilder);
			if (knwSearchClient == null)
			{
				return;
			}
			KnwSearchDTO_Temp knwDto = ExecuteSearch(knwSearchClient, knwDict, sessionId, contentBuilder);
			if (knwDto == null)
			{
				return;
			}
			SearchResultsSummary summary = AppendSearchResults(knwDto, attachedSourceSettings, contentBuilder);
			AppendGroundingPrompt(contentBuilder, summary);
		}

		private Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>> GetKnowledgeSourceQueriesDict(
				List<KnowledgeSourceQuery> knwSourceQueries, StringBuilder contentBuilder)
		{
			try
			{
				var knwSourcesDict = knwSourceQueries
					.Where(q => q.KnwSourceId.IsNotEmpty() && q.KnwSourceQuery.IsNotNullOrEmpty())
					.GroupBy(q => q.KnwSourceId)
					.ToDictionary(g => g.Key, g =>
						g.Select(q => new KnwSearchRequestQuery_Temp
						{
							Query = q.KnwSourceQuery,
							Keywords = ParseKeywords(q.Keywords)
						}));
				if (knwSourcesDict.Count == 0)
				{
					contentBuilder.AppendLine("No knowledge sources or queries matched the provided data.");
					return new Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>>();
				}
				return knwSourcesDict;
			}
			catch (Exception ex)
			{
				string errorMsg = "Failed to retrieve KnwSourceQueries parameter";
				KnwLibUtils.Logger.Error(errorMsg, ex);
				contentBuilder.AppendLine(errorMsg);
				return new Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>>();
			}
		}

		private IKnwSearchClient_Temp ResolveKnwSearchClient(StringBuilder contentBuilder)
		{
			try
			{
				return ClassFactory.Get<IKnwSearchClient_Temp>();
			}
			catch (Exception ex)
			{
				KnwLibUtils.Logger.Error("Failed to resolve IKnwSearchClient.", ex);
				contentBuilder.AppendLine("Failed to resolve knowledge search client.");
				return null;
			}
		}

		private KnwSearchDTO_Temp ExecuteSearch(IKnwSearchClient_Temp knwSearchClient,
				Dictionary<Guid, IEnumerable<KnwSearchRequestQuery_Temp>> knwDict, Guid sessionId,
				StringBuilder contentBuilder)
		{
			try
			{
				var request = new KnwSearchRequest_Temp
				{
					KnwSourceQueries = knwDict,
					SessionId = sessionId
				};
				KnwSearchDTO_Temp knwDto = knwSearchClient.Search(request);
				if (knwDto == null || (knwDto.KnwItems.IsNullOrEmpty() && knwDto.ErrorMessage.IsNullOrEmpty()))
				{
					contentBuilder.AppendLine("No relevant documents found for the given data.");
					return null;
				}
				if (knwDto.ErrorMessage.IsNotNullOrEmpty())
				{
					contentBuilder.AppendLine(knwDto.ErrorMessage);
					return null;
				}
				return knwDto;
			}
			catch (Exception ex)
			{
				KnwLibUtils.Logger.Error("Failed to retrieve information.", ex);
				contentBuilder.AppendLine("Failed to retrieve information.");
				return null;
			}
		}

		private static SearchResultsSummary AppendSearchResults(KnwSearchDTO_Temp knwDto,
				IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> attachedSourceSettings,
				StringBuilder contentBuilder)
		{
			var summary = new SearchResultsSummary();
			foreach (KnwSearchItemResponse_Temp result in knwDto.KnwItems.Where(i => i.Error.IsNullOrEmpty()))
			{
				bool useCitations = ShouldUseCitations(result.KnwSourceId, attachedSourceSettings);
				if (useCitations)
				{
					summary.HasCitationEnabledResults = true;
				}
				else
				{
					summary.HasCitationDisabledResults = true;
				}
				double? score = result.Score > 0 ? result.Score : (double?)null;
				string source = !string.IsNullOrWhiteSpace(result.ItemDetail?.Title)
					? result.ItemDetail.Title
					: null;
				string location = result.ItemDetail?.Location;
				AppendFormattedLineOfKnwItem(contentBuilder, result.KnwSourceId, result.Response, useCitations, score,
					source, location);
			}
			IEnumerable<KnwSearchItemResponse_Temp> errorItems = knwDto.KnwItems.Where(i => !i.Error.IsNullOrEmpty());
			if (errorItems.Any())
			{
				contentBuilder.AppendLine(FailedKnwSectionTitle);
				foreach (KnwSearchItemResponse_Temp result in errorItems)
				{
					AppendFormattedLineOfKnwItem(contentBuilder, result.KnwSourceId, result.Error,
						ShouldUseCitations(result.KnwSourceId, attachedSourceSettings));
				}
			}
			return summary;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override LocalizableString GetCaption()
		{
			return new LocalizableString(CaptionValue);
		}

		/// <inheritdoc />
		public override LocalizableString GetDescription()
		{
			return new LocalizableString(DescriptionValue);
		}

		/// <inheritdoc />
		public override CopilotActionExecutionResult Execute(ActionExecutionOptions options)
		{
			var sb = new StringBuilder();
			HandleKnowledgeRetrieval(options, sb, options.CopilotSessionUId);
			return new CopilotActionExecutionResult
			{
				Status = CopilotActionExecutionStatus.Completed,
				Response = sb.ToString()
			};
		}

		/// <summary>Sets the user connection.</summary>
		/// <param name="userConnection">The user connection.</param>
		public void SetUserConnection(UserConnection userConnection)
		{
			_userConnection = userConnection;
		}

		#endregion

	}

	#endregion

}

