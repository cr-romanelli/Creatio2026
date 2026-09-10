namespace Terrasoft.Configuration
{
	using System.Linq;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;

    #region Interface: IOAuthIntegrationReader

    /// <summary>
    /// OAuth2.0 integration reader.
    /// </summary>
    public interface IOAuthIntegrationReader
	{
		/// <summary>
		/// Gets the integration information.
		/// </summary>
		/// <param name="userConnection">The user connection.</param>
		/// <param name="integrationName">Name of the integration.</param>
		/// <returns>The integration information.</returns>
		IntegrationInfo GetIntegrationInfo(UserConnection userConnection, string integrationName);
	}

	#endregion

    #region Class: OAuthIntegrationReader

    /// <inheritdoc cref="IOAuthIntegrationReader"/>
    public class OAuthIntegrationReader : IOAuthIntegrationReader
	{

		#region Methods: Private

		private Entity[] GetIntegrationEntity(UserConnection userConnection, string integrationName) {
			var esq = new EntitySchemaQuery(userConnection.EntitySchemaManager, "OAuthClientApp");
			IEntitySchemaQueryFilterItem filter =
				esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", integrationName);
			esq.Filters.Add(filter);
			esq.PrimaryQueryColumn.IsAlwaysSelect = true;
			esq.AddColumn("ClientId");
			esq.AddColumn("ClientSecret");
			return esq.GetEntityCollection(userConnection).ToArray();
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc cref="IOAuthIntegrationReader.GetIntegrationInfo"/>
		public IntegrationInfo GetIntegrationInfo(UserConnection userConnection, string integrationName) {
			Entity[] collection = GetIntegrationEntity(userConnection, integrationName);
			bool isSuccess = collection.Length == 1;
			var integrationInfo = new IntegrationInfo {
				Name = integrationName,
				Success = isSuccess,
			};
			if (isSuccess) {
				integrationInfo.ClientId = collection[0].SafeGetColumnValue<string>("ClientId");
				integrationInfo.ClientSecret = collection[0].SafeGetColumnValue<string>("ClientSecret");
			}
			return integrationInfo;
		}

		#endregion

	}

	#endregion

}

