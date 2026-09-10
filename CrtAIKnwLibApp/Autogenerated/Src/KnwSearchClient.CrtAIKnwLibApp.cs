namespace Creatio.Copilot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Terrasoft.Common;
    using Terrasoft.Core;
    using Terrasoft.Core.Factories;

    #region Interface: IKnwSearchClient

    /// <summary>
    /// Provides an interface for searching knowledge sources using a RAG service.
    /// </summary>
    public interface IKnwSearchClient_Temp
    {
        #region Methods: Public

        /// <summary>
        /// Searches multiple knowledge sources by their identifiers and associated queries.
        /// </summary>
        /// <param name="request">The search request containing knowledge source queries, session ID
        /// and pagination options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.
        /// Defaults to <see cref="CancellationToken.None"/> if not specified.</param>
        /// <returns>A <see cref="KnwSearchDTO_Temp"/> object with the search results for each source.</returns>
        KnwSearchDTO_Temp Search(KnwSearchRequest_Temp request, CancellationToken cancellationToken = default);

        #endregion
    }

    #endregion

    #region Interface: IKnwEndUserContextAllowedItemsFilter

    /// <summary>
    /// Applies optional end-user-context filtering to already provider-filtered search results.
    /// </summary>
    public interface IKnwEndUserContextAllowedItemsFilter
    {
        #region Methods: Public

        /// <summary>
        /// Filters item identifiers using end-user context rules.
        /// </summary>
        /// <param name="itemIds">The item identifiers already allowed by provider filtering.</param>
        /// <param name="context">Optional end-user context.</param>
        /// <param name="knwSourceId">Knowledge source identifier for logging/correlation.</param>
        /// <returns>Filtered item identifiers.</returns>
        ISet<string> FilterAlreadyAllowedItems(ISet<string> itemIds, KnwEndUserContext context, Guid knwSourceId);

        #endregion
    }

    #endregion

    #region Class: NoOpKnwEndUserContextAllowedItemsFilter

    /// <summary>
    /// Phase 1 no-op implementation of end-user-context filtering.
    /// </summary>
    [DefaultBinding(typeof(IKnwEndUserContextAllowedItemsFilter))]
    internal class NoOpKnwEndUserContextAllowedItemsFilter : IKnwEndUserContextAllowedItemsFilter
    {
        #region Methods: Public

        /// <inheritdoc />
        public ISet<string> FilterAlreadyAllowedItems(ISet<string> itemIds, KnwEndUserContext context,
            Guid knwSourceId)
        {
            ISet<string> result = itemIds ?? new HashSet<string>();
            if (context != null && (!string.IsNullOrWhiteSpace(context.UserId) ||
                    !string.IsNullOrWhiteSpace(context.DisplayName)))
            {
                KnwLibUtils.Logger.Debug(
                    $"[{nameof(NoOpKnwEndUserContextAllowedItemsFilter)}] End-user context supplied for source " +
                    $"{knwSourceId}, but Phase 1 processor does not apply additional filtering.");
            }
            return result;
        }

        #endregion
    }

    #endregion

    #region Class: KnwSearchClient

    /// <summary>
    /// Implementation of IKnwSearchClient for RAG service integration.
    /// </summary>
    [DefaultBinding(typeof(IKnwSearchClient_Temp))]
    internal class KnwSearchClient_Temp : IKnwSearchClient_Temp
    {
        #region Class: FilteredItemsResult

        private sealed class FilteredItemsResult
        {
            public bool Success { get; set; }

            public List<KnwBaseItemResponse> Items { get; set; } = new List<KnwBaseItemResponse>();
        }

        #endregion

        #region Fields: Private

        private readonly UserConnection _userConnection;
        private const string InvalidRequestErrorCode = "invalid_request";
        private const string RetrievalFailedErrorCode = "knowledge_retrieval_failed";
        private const string NoResultsErrorCode = "no_results";
        private const string ProviderUnavailableErrorCode = "provider_unavailable";
        private const string AccessFilterFailedErrorCode = "access_filter_failed";
        private const string EndUserContextFailedErrorCode = "end_user_context_processing_failed";

        #endregion

        #region Constructors: Public

        /// <summary>
        /// Initializes a new instance of the <see cref="KnwSearchClient_Temp"/> class.
        /// </summary>
        /// <param name="userConnection">The user connection context.</param>
        public KnwSearchClient_Temp(UserConnection userConnection)
        {
            _userConnection = userConnection;
        }

        #endregion

        #region Methods: Private

        private static KnwSearchItemResponse_Temp CreateErrorResponse(Guid knwSourceId, string errorCode,
            string errorMessage)
        {
            return new KnwSearchItemResponse_Temp
            {
                KnwSourceId = knwSourceId,
                ErrorCode = errorCode ?? string.Empty,
                Error = errorMessage ?? string.Empty
            };
        }

        private IKnwProvider GetProviderForSource(Guid knwSourceId)
        {
            try
            {
                var sourceManager = ClassFactory.Get<IKnwSourceManager>(
                    new ConstructorArgument("userConnection", _userConnection));
                KnwSourceRecord knwSource = sourceManager.GetById(knwSourceId);
                if (knwSource == null)
                {
                    KnwLibUtils.Logger.Debug(
                        $"[{nameof(KnwSearchClient_Temp)}] Provider resolution source lookup returned null. " +
                        $"KnwSourceId={knwSourceId}");
                    return null;
                }

                var providerFactory = ClassFactory.Get<IKnwProviderFactory>();
                var options = new KnwProviderInitializationOptions(_userConnection, knwSource);
                return providerFactory.CreateKnwProvider(options);
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Error($"Failed to get provider for source {knwSourceId}", ex);
                return null;
            }
        }

        private static FilteredItemsResult FilterItemsByUserAccess(List<KnwBaseItemResponse> items, IKnwProvider provider)
        {
            var result = new FilteredItemsResult
            {
                Success = true,
                Items = items ?? new List<KnwBaseItemResponse>()
            };
            if (provider == null || items == null || items.Count == 0)
            {
                return result;
            }

            try
            {
                HashSet<string> itemIds =
                    items.Select(item => item.ItemId).Where(id => !id.IsNullOrEmpty()).ToHashSet();
                if (itemIds.Count == 0)
                {
                    return result;
                }

                ISet<string> allowedItemIds = provider.GetUserAllowedItems(itemIds);
                result.Items = items.Where(item => allowedItemIds.Contains(item.ItemId)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Error("Failed to filter items by user access rights", ex);
                return new FilteredItemsResult
                {
                    Success = false,
                    Items = new List<KnwBaseItemResponse>()
                };
            }
        }

        private IKnwEndUserContextAllowedItemsFilter GetEndUserContextAllowedItemsFilter()
        {
            try
            {
                return ClassFactory.Get<IKnwEndUserContextAllowedItemsFilter>();
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Debug(
                    $"[{nameof(KnwSearchClient_Temp)}] Falling back to no-op end-user-context allowed-items filter. " +
                    $"Reason={ex.Message}");
                return new NoOpKnwEndUserContextAllowedItemsFilter();
            }
        }

        private FilteredItemsResult FilterItemsByEndUserContext(List<KnwBaseItemResponse> items, KnwSearchQuery request)
        {
            var result = new FilteredItemsResult
            {
                Success = true,
                Items = items ?? new List<KnwBaseItemResponse>()
            };
            if (items == null || items.Count == 0)
            {
                return result;
            }

            try
            {
                IKnwEndUserContextAllowedItemsFilter allowedItemsFilter = GetEndUserContextAllowedItemsFilter();
                if (allowedItemsFilter == null)
                {
                    return new FilteredItemsResult
                    {
                        Success = false,
                        Items = new List<KnwBaseItemResponse>()
                    };
                }
                HashSet<string> itemIds = items.Select(item => item.ItemId)
                    .Where(id => !id.IsNullOrEmpty())
                    .ToHashSet();
                if (itemIds.Count == 0)
                {
                    return result;
                }
                ISet<string> allowedItemIds = allowedItemsFilter.FilterAlreadyAllowedItems(itemIds, request.EndUserContext,
                    request.KnwSourceId) ?? new HashSet<string>();
                result.Items = items.Where(item => allowedItemIds.Contains(item.ItemId)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Error("Failed to process end-user context for search results", ex);
                return new FilteredItemsResult
                {
                    Success = false,
                    Items = new List<KnwBaseItemResponse>()
                };
            }
        }

        private static Dictionary<string, KnwItemCitation> GetCitationsForItems(List<KnwBaseItemResponse> items,
            IKnwProvider provider)
        {
            if (provider == null || items == null || items.Count == 0)
            {
                return null;
            }

            try
            {
                HashSet<string> itemIds =
                    items.Select(item => item.ItemId).Where(id => !id.IsNullOrEmpty()).ToHashSet();
                if (itemIds.Count == 0)
                {
                    return null;
                }

                return provider.GetItemsCitations(itemIds);
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Error("Failed to retrieve citations for search results", ex);
                return null;
            }
        }

        private static KnwSearchItemResponse_Temp CreateSearchItemResponse(KnwBaseItemResponse item, Guid knwSourceId,
            Dictionary<string, KnwItemCitation> citations)
        {
            KnwItemCitation citation = null;
            citations?.TryGetValue(item.ItemId, out citation);
            Guid? itemId = null;
            if (Guid.TryParse(item.ItemId, out Guid parsedId))
            {
                itemId = parsedId;
            }

            return new KnwSearchItemResponse_Temp
            {
                KnwSourceId = knwSourceId,
                Response = item.ChunkText,
                Score = item.Score,
                ItemDetail = new KnwItemDetail_Temp
                {
                    ItemId = itemId,
                    Title = citation?.SourceDisplayName ?? string.Empty,
                    Location = citation?.SourceLocation ?? string.Empty
                }
            };
        }

        private IEnumerable<KnwSearchItemResponse_Temp> Search(KnwSearchQuery request,
            CancellationToken cancellationToken)
        {
            if (request.KnwSourceId.IsEmpty() || request.Query.IsNullOrEmpty())
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, InvalidRequestErrorCode,
                        "No knowledge base sources provided for search.")
                };
            }

            var serviceClient = ClassFactory.Get<IKnwServiceClient>();
            var searchRequest = new KnwSearchQuery
            {
                KnwSourceId = request.KnwSourceId,
                SessionId = request.SessionId,
                Query = request.Query,
                Skip = request.Skip,
                Take = request.Take,
                Keywords = request.Keywords
            };
            IEnumerable<KnwBaseItemResponse> knwItems;
            try
            {
                knwItems = serviceClient.GetKnowledgeBaseItemsByIds(searchRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                KnwLibUtils.Logger.Error("Knowledge search client error: unable to retrieve knowledge base items.", ex);
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, RetrievalFailedErrorCode,
                        "Knowledge search client error: unable to retrieve knowledge base items.")
                };
            }

            var knwItemsList = knwItems?.ToList();
            if (knwItemsList == null || knwItemsList.Count == 0)
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, NoResultsErrorCode,
                        "No relevant knowledge base item were found for your query.")
                };
            }

            IKnwProvider provider = GetProviderForSource(request.KnwSourceId);
            if (provider == null)
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, ProviderUnavailableErrorCode,
                        "Knowledge source is unavailable for secure access filtering.")
                };
            }

            FilteredItemsResult providerFilteredItems = FilterItemsByUserAccess(knwItemsList, provider);
            if (!providerFilteredItems.Success)
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, AccessFilterFailedErrorCode,
                        "Failed to filter items by user access rights.")
                };
            }
            knwItemsList = providerFilteredItems.Items;

            FilteredItemsResult endUserContextFilteredItems = FilterItemsByEndUserContext(knwItemsList, request);
            if (!endUserContextFilteredItems.Success)
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, EndUserContextFailedErrorCode,
                        "Failed to process end-user context for search results.")
                };
            }
            knwItemsList = endUserContextFilteredItems.Items;

            if (knwItemsList.Count == 0)
            {
                return new[]
                {
                    CreateErrorResponse(request.KnwSourceId, NoResultsErrorCode,
                        "No relevant knowledge base item were found for your query.")
                };
            }

            Dictionary<string, KnwItemCitation> citations = GetCitationsForItems(knwItemsList, provider);
            List<KnwSearchItemResponse_Temp> result = knwItemsList
                .Select(item => CreateSearchItemResponse(item, request.KnwSourceId, citations))
                .ToList();
            KnwLibUtils.Logger.Debug(
                $"[{nameof(KnwSearchClient_Temp)}] Source search completed. " +
                $"KnwSourceId={request.KnwSourceId}, RetrievedItems={knwItemsList.Count}, FinalItems={result.Count}");
            return result;
        }

        #endregion

        #region Methods: Public

        /// <inheritdoc />
        public KnwSearchDTO_Temp Search(KnwSearchRequest_Temp request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return new KnwSearchDTO_Temp
                {
                    ErrorMessage = "No knowledge base sources provided for search."
                };
            }

            if (request.KnwSourceQueries == null || !request.KnwSourceQueries.Any())
            {
                return new KnwSearchDTO_Temp
                {
                    ErrorMessage = "No knowledge base sources provided for search."
                };
            }

            KnwLibUtils.Logger.Debug(
                $"[{nameof(KnwSearchClient_Temp)}] Starting multi-source search. " +
                $"SessionId={request.SessionId}, Skip={request.Skip}, Take={request.Take}, " +
                $"SourceCount={request.KnwSourceQueries.Count}");
            var allResults = new List<KnwSearchItemResponse_Temp>();
            foreach (KeyValuePair<Guid, IEnumerable<KnwSearchRequestQuery_Temp>> pair in request.KnwSourceQueries)
            {
                foreach (KnwSearchRequestQuery_Temp value in pair.Value)
                {
                    try
                    {
                        IEnumerable<KnwSearchItemResponse_Temp> results = Search(new KnwSearchQuery
                        {
                            KnwSourceId = pair.Key,
                            Query = value.Query,
                            SessionId = request.SessionId,
                            Skip = request.Skip,
                            Take = request.Take,
                            Keywords = value.Keywords,
                            EndUserContext = request.EndUserContext
                        }, cancellationToken);
                        allResults.AddRange(results);
                    }
                    catch (Exception ex)
                    {
                        KnwLibUtils.Logger.Error("Knowledge search client error.", ex);
                        return new KnwSearchDTO_Temp
                        {
                            ErrorMessage = $"Knowledge search client error: unable to complete searches. {ex.Message}"
                        };
                    }
                }
            }

            return new KnwSearchDTO_Temp
            {
                KnwItems = allResults.ToArray()
            };
        }

        /// <inheritdoc />
        public Task<KnwSearchDTO_Temp> SearchAsync(KnwSearchRequest_Temp request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Search(request, cancellationToken));
        }

        #endregion
    }

    #endregion
}

