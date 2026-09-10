namespace Creatio.Copilot
{
	using Terrasoft.Common;
	using Terrasoft.Core;

	#region Class: KnwExternalApiMessage

	internal static class KnwExternalApiMessage
	{

		#region Constants: Private

		private const string ResourceSchemaName = "KnwExternalApiMessage";
		private const string IngestionQuotaExceededResourceKey = "IngestionQuotaExceeded";
		private const string ChunkTooLargeResourceKey = "ChunkTooLarge";
		private const string IngestionDisabledResourceKey = "IngestionDisabled";
		private const string IndexNotFoundResourceKey = "IndexNotFound";
		private const string InvalidRequestResourceKey = "InvalidRequest";
		private const string InternalErrorResourceKey = "InternalError";
		private const string GenericFallbackResourceKey = "GenericFallback";

		#endregion

		#region Methods: Public

		public static string ResolveLocalizedMessage(UserConnection userConnection, string errorCode,
				string fallbackMessage)
		{
			string localizableKey = ResolveResourceKey(errorCode);
			if (userConnection == null)
			{
				return fallbackMessage;
			}
			try
			{
				string localizedMessage = TryGetLocalizedString(userConnection, localizableKey);
				if (!string.IsNullOrWhiteSpace(localizedMessage))
				{
					return localizedMessage;
				}
				string genericLocalizedMessage = TryGetLocalizedString(userConnection, GenericFallbackResourceKey);
				return string.IsNullOrWhiteSpace(genericLocalizedMessage) ? fallbackMessage : genericLocalizedMessage;
			}
			catch
			{
				return fallbackMessage;
			}
		}

		#endregion

		#region Methods: Private

		private static string ResolveResourceKey(string code)
		{
			switch (code)
			{
				case "ingestion-quota-exceeded":
					return IngestionQuotaExceededResourceKey;
				case "payload-too-large":
					return ChunkTooLargeResourceKey;
				case "ingestion-disabled":
					return IngestionDisabledResourceKey;
				case "resource-not-found":
				case "session-index-not-found":
					return IndexNotFoundResourceKey;
				case "request-validation":
				case "session-invalid-chunk-size":
					return InvalidRequestResourceKey;
				case "internal-unexpected-error":
				case "dependency-unavailable":
				case "conflict":
					return InternalErrorResourceKey;
				default:
					return GenericFallbackResourceKey;
			}
		}

		private static string TryGetLocalizedString(UserConnection userConnection, string resourceKey)
		{
			if (string.IsNullOrWhiteSpace(resourceKey))
			{
				return null;
			}
			string resourcePath = $"LocalizableStrings.{resourceKey}.Value";
			var localizableString = new LocalizableString(
				userConnection.Workspace.ResourceStorage,
				ResourceSchemaName,
				resourcePath
			);
			string value = localizableString.Value;
			if (string.IsNullOrWhiteSpace(value))
			{
				value = localizableString.GetCultureValue(GeneralResourceStorage.DefCulture, false);
			}
			return value;
		}

		#endregion

	}

	#endregion

}

