namespace Creatio.Copilot
{
	using Creatio.FeatureToggling;

	#region Class: KnwFeatures

	/// <summary>
	/// Provides feature metadata classes for knowledge Service features.
	/// </summary>
	internal static class KnwFeatures
	{

		#region Class: UseOAuth

		/// <summary>
		/// Represents the metadata for the UseOAuth feature.
		/// </summary>
		public class UseOAuth : FeatureMetadata
		{

			#region Constuctors: Public

			/// <summary>
			/// Initializes a new instance of the <see cref="UseOAuth"/> class with the feature enabled.
			/// </summary>
			public UseOAuth() {
				IsEnabled = true;
			}

			#endregion

		}

		#endregion

	}

	#endregion

}

