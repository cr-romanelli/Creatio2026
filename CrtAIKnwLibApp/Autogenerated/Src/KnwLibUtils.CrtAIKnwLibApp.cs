namespace Creatio.Copilot
{
	using System;
	using Common.Logging;

	#region Class: KnwLibUtils

	/// <summary>
	/// Provides utility functions for the Knowledge Library.
	/// </summary>
	internal static class KnwLibUtils
	{
		
		#region Constants: Internal

		/// <summary>
		/// Enrichment service URL used by the stage environment.
		/// </summary>
		internal const string StageEnrichmentServiceUrl = "https://enrichment-stage.bpmonline.com";

		/// <summary>
		/// Knowledge service URL used by the stage environment and as the default fallback.
		/// </summary>
		internal const string StageKnowledgeServiceUrl = "https://kms-stage.bpmonline.com";

		/// <summary>
		/// Knowledge service URL used by the preproduction environment.
		/// </summary>
		internal const string PreprodKnowledgeServiceUrl = "https://kms-pre.creatio.com";

		/// <summary>
		/// Knowledge service URL used by the demo environment.
		/// </summary>
		internal const string DemoKnowledgeServiceUrl = "https://demo-kms.creatio.com";

		#endregion

		#region Fields: Private

		private static readonly object _logLocker = new object();

		#endregion

		#region Properties: Public

		private static ILog _logger;

		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		public static ILog Logger {
			get {
				if (_logger != null) {
					return _logger;
				}
				lock (_logLocker) {
					_logger = _logger ?? LogManager.GetLogger("CreatioAI.Knw");
				}
				return _logger;
			}
			set {
				lock (_logLocker) {
					_logger = value;
				}
			}
		}

		#endregion

		#region Methods: Private

		private static string NormalizeUrl(string url) {
			return (url ?? string.Empty).Trim().TrimEnd('/').ToLowerInvariant();
		}

		private static string ExtractEnvironmentValue(string enrichmentServiceUrl) {
			if (!Uri.TryCreate(enrichmentServiceUrl, UriKind.Absolute, out Uri uri) || string.IsNullOrWhiteSpace(uri.Host)) {
				return string.Empty;
			}
			string firstHostLabel = uri.Host.Split('.')[0];
			if (string.IsNullOrWhiteSpace(firstHostLabel)) {
				return string.Empty;
			}
			int dashIndex = firstHostLabel.IndexOf('-');
			string token = dashIndex >= 0 ? firstHostLabel.Substring(0, dashIndex) : firstHostLabel;
			return NormalizeUrl(token);
		}

		#endregion

		#region Methods: Public

		/// <summary>
		/// Resolves the knowledge service URL based on the account enrichment service URL.
		/// </summary>
		/// <param name="enrichmentServiceUrl">The account enrichment service URL used as a base value.</param>
		/// <returns>
		/// The resolved knowledge service URL, or <c>null</c> when the enrichment URL is empty or is not a
		/// parseable absolute URL. A valid URL without a recognizable environment prefix resolves to the stage
		/// knowledge service URL.
		/// </returns>
		public static string ResolveKnowledgeServiceUrl(string enrichmentServiceUrl) {
			string normalizedEnrichmentServiceUrl = NormalizeUrl(enrichmentServiceUrl);
			if (string.IsNullOrWhiteSpace(normalizedEnrichmentServiceUrl)) {
				return null;
			}
			if (normalizedEnrichmentServiceUrl == NormalizeUrl(StageEnrichmentServiceUrl)) {
				return StageKnowledgeServiceUrl;
			}

			string environmentValue = ExtractEnvironmentValue(normalizedEnrichmentServiceUrl);
			if (string.IsNullOrWhiteSpace(environmentValue)) {
				// The enrichment URL is not a parseable absolute URL, so a knowledge service URL cannot be derived.
				return null;
			}
			// exception for enrichment service URL without environment prefix, which is used in some non-production environments
			if (environmentValue == "enrichment") {
				return StageKnowledgeServiceUrl;
			}
			// exception for preproduction environment
			if (environmentValue == "stage") {
				return PreprodKnowledgeServiceUrl;
			}
			// exception for demo environment
			if (environmentValue == "demo") {
				return DemoKnowledgeServiceUrl;
			}
			// default pattern for environment-specific URLs
			return $"https://kms-{environmentValue}.creatio.com";
		}

		#endregion

	}

	#endregion

}
