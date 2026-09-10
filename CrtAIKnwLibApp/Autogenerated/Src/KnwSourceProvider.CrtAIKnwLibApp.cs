namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.Common;

	#region Class: KnwSourceProvider

	/// <summary>
	/// Provides methods to retrieve knowledge sources associated with specific intents from the database.
	/// </summary>
	[DefaultBinding(typeof(IKnwSourceProvider))]
	internal class KnwSourceProvider : IKnwSourceProvider
	{

		#region Fields: Private

		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="KnwSourceProvider"/> class with the specified user connection.
		/// </summary>
		/// <param name="userConnection">The user connection to use for database operations.</param>
		public KnwSourceProvider(UserConnection userConnection) {
			_userConnection = userConnection ?? throw new ArgumentNullException(nameof(userConnection));
		}

		#endregion

		#region Methods: Private

		private void AddIntentIdFilter(EntitySchemaQuery esq, EntitySchemaQueryFilterCollection filterGroup,
				Guid? intentId) {
			if (intentId.HasValue) {
				IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(
					FilterComparisonType.Equal, "IntentId", intentId);
				filterGroup.Add(filter);
			}
		}

		private IEnumerable<KnwSourceDto> QueryByIntentIds(Guid? currentIntentId, Guid? rootIntentId) {
			if (!currentIntentId.HasValue && !rootIntentId.HasValue) {
				return Enumerable.Empty<KnwSourceDto>();
			}
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager, "KnwSourceInSkill");
			esq.AddColumn("KnwSource.Id");
			esq.AddColumn("KnwSource.Description");
			var orGroup = new EntitySchemaQueryFilterCollection(esq, LogicalOperationStrict.Or);
			AddIntentIdFilter(esq, orGroup, currentIntentId);
			if (currentIntentId != rootIntentId) {
				AddIntentIdFilter(esq, orGroup, rootIntentId);
			}
			esq.Filters.Add(orGroup);
			var entities = esq.GetEntityCollection(_userConnection);
			var result = new List<KnwSourceDto>(entities.Count);
			foreach (var entity in entities) {
				var id = entity.GetTypedColumnValue<Guid>("KnwSource_Id");
				var description = entity.GetTypedColumnValue<string>("KnwSource_Description");
				result.Add(new KnwSourceDto { Id = id, Description = description });
			}
			return result;
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public IEnumerable<KnwSourceDto> GetSourcesInSession(CopilotSession session) {
			if (session == null) {
				return Enumerable.Empty<KnwSourceDto>();
			}
			try {
				return QueryByIntentIds(session.CurrentIntentId, session.RootIntentId);
			} catch {
				return Enumerable.Empty<KnwSourceDto>();
			}
		}

		#endregion

	}

	#endregion

}
