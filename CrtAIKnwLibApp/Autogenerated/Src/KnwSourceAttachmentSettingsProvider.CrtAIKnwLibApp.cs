namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwSourceAttachmentSettingsProvider

	/// <summary>
	/// Provides attachment-level knowledge source settings for a Copilot session.
	/// </summary>
	public interface IKnwSourceAttachmentSettingsProvider
	{

		#region Methods: Public

		/// <summary>
		/// Returns effective attachment settings for knowledge sources attached to the specified session.
		/// </summary>
		/// <param name="session">The Copilot session.</param>
		/// <returns>A dictionary keyed by knowledge source identifier.</returns>
		IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> GetSettingsBySession(CopilotSession session);

		#endregion

	}

	#endregion

	#region Class: KnwSourceAttachmentSettings

	/// <summary>
	/// Represents effective attachment-level settings for a knowledge source in a Copilot session.
	/// </summary>
	public class KnwSourceAttachmentSettings
	{

		#region Properties: Public

		/// <summary>
		/// Gets or sets a value indicating whether citation metadata should be preserved for this source.
		/// </summary>
		public bool UseCitations { get; set; } = true;

		#endregion

	}

	#endregion

	#region Class: KnwSourceAttachmentSettingsProvider

	/// <inheritdoc cref="IKnwSourceAttachmentSettingsProvider" />
	[DefaultBinding(typeof(IKnwSourceAttachmentSettingsProvider))]
	internal class KnwSourceAttachmentSettingsProvider : IKnwSourceAttachmentSettingsProvider
	{

		#region Fields: Private

		private static readonly IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> EmptySettings =
			new ReadOnlyDictionary<Guid, KnwSourceAttachmentSettings>(
				new Dictionary<Guid, KnwSourceAttachmentSettings>(0));
		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwSourceAttachmentSettingsProvider"/> class.
		/// </summary>
		/// <param name="userConnection">The user connection to use for database operations.</param>
		public KnwSourceAttachmentSettingsProvider(UserConnection userConnection) {
			_userConnection = userConnection ?? throw new ArgumentNullException(nameof(userConnection));
		}

		#endregion

		#region Methods: Private

		private static void AddIntentIdFilter(EntitySchemaQuery esq, EntitySchemaQueryFilterCollection filterGroup,
				Guid? intentId) {
			if (intentId.HasValue) {
				IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(
					FilterComparisonType.Equal, "IntentId", intentId);
				filterGroup.Add(filter);
			}
		}

		private static bool ReadUseCitations(Entity entity) {
			object rawValue = entity.GetColumnValue("UseCitations");
			return rawValue == null || rawValue == DBNull.Value || rawValue is bool useCitations && useCitations;
		}

		private IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> GetSettingsByIntentIds(Guid? currentIntentId,
				Guid? rootIntentId) {
			if (!currentIntentId.HasValue && !rootIntentId.HasValue) {
				return EmptySettings;
			}
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager, "KnwSourceInSkill");
			esq.AddColumn("IntentId");
			esq.AddColumn("KnwSource.Id");
			esq.AddColumn("UseCitations");
			var orGroup = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.Or);
			AddIntentIdFilter(esq, orGroup, currentIntentId);
			if (currentIntentId != rootIntentId) {
				AddIntentIdFilter(esq, orGroup, rootIntentId);
			}
			esq.Filters.Add(orGroup);
			EntityCollection entities = esq.GetEntityCollection(_userConnection);
			var result = new Dictionary<Guid, KnwSourceAttachmentSettings>();
			foreach (Entity entity in entities) {
				if (entity == null) {
					continue;
				}
				Guid sourceId = entity.GetTypedColumnValue<Guid>("KnwSource_Id");
				if (sourceId == Guid.Empty) {
					continue;
				}
				Guid intentId = entity.GetTypedColumnValue<Guid>("IntentId");
				bool useCitations = ReadUseCitations(entity);
				if (intentId == rootIntentId && result.ContainsKey(sourceId)) {
					continue;
				}
				result[sourceId] = new KnwSourceAttachmentSettings {
					UseCitations = useCitations
				};
			}
			return result;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public IReadOnlyDictionary<Guid, KnwSourceAttachmentSettings> GetSettingsBySession(CopilotSession session) {
			if (session == null) {
				return EmptySettings;
			}
			try {
				return GetSettingsByIntentIds(session.CurrentIntentId, session.RootIntentId);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error("Failed to retrieve knowledge source attachment settings for session " +
				                         $"CurrentIntentId={session.CurrentIntentId}, RootIntentId={session.RootIntentId}", e);
				return EmptySettings;
			}
		}

		#endregion

	}

	#endregion

}

