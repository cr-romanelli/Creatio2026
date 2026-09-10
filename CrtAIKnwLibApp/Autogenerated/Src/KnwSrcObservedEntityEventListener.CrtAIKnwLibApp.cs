namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Newtonsoft.Json;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Requests;
	using SysSettings = Terrasoft.Core.Configuration.SysSettings;

	#region Class: KnwSrcObservedEntityEventListener

	/// <summary>
	/// Tracks section record changes and stores deduplicated sync events in outbox.
	/// </summary>
	[EntityEventListener(IsGlobal = true, Name = nameof(KnwSrcObservedEntityEventListener))]
	public class KnwSrcObservedEntityEventListener : BaseEntityEventListener
	{

		#region Constants: Private

		private const string SectionProviderCode = "SectionKnowledgeSourceProvider";
		private const string DynamicFolderScopeType = "DynamicFolder";
		private const string IgnoredSchemaPatternsSettingCode = "KnwSrcAutoSyncIgnoredSchemaPatterns";
		private const string ProviderCodeColumnValueName = "KnwProvider_Code";
		private const string SourceStatusValueColumnPath = "KnwSourceStatus.Value";
		private const string SourceStatusValueColumnName = "KnwSourceStatus_Value";

		#endregion

		#region Constants: Internal

		internal const string SourceSchemaName = "KnwSource";
		internal const string IsDeletedColumnName = "IsDeleted";
		internal const string FolderTreeSchemaName = "FolderTree";
		internal const string ProviderCodeColumnPath = "KnwProvider.Code";
		internal const string SourceConfigColumnPath = "KnwSourceConfig";

		#endregion

		#region Fields: Private

		private static readonly char[] _ignoredSchemaPatternSeparators = {
			',',
			';',
			'\r',
			'\n'
		};

		private static readonly HashSet<string> _technicalSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
			"Feature",
			KnwSrcEventsOutboxSchema.Name,
			SourceSchemaName,
			"KnwSourceStatus",
			"KnwProvider",
			"KnwProviderConnection",
			"KnwIndexingSession",
			"KnwIndexingStatus",
			"KnwSourceFile",
			"KnwSourceInSkill"
		};
		private static readonly HashSet<string> _folderSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
			FolderTreeSchemaName
		};

		#endregion

		#region Class: KnwSrcObservedBinding

		internal sealed class KnwSrcObservedBinding
		{
			public Guid SourceId { get; set; }

			public HashSet<string> RootColumns { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region Class: SectionProviderConfig

		private sealed class SectionProviderConfig
		{
			public Guid RootSchemaUId { get; set; }

			public string ScopeType { get; set; }

			public Guid? DynamicFolderId { get; set; }

			public List<string> Columns { get; set; }
		}

		#endregion

		#region Methods: Private

		private static IEnumerable<string> GetIgnoredSchemaPatterns(UserConnection userConnection) {
			if (userConnection == null) {
				return Enumerable.Empty<string>();
			}
			string settingValue = SysSettings.GetValue(userConnection, IgnoredSchemaPatternsSettingCode, string.Empty);
			if (settingValue.IsNullOrWhiteSpace()) {
				return Enumerable.Empty<string>();
			}
			return settingValue
				.Split(_ignoredSchemaPatternSeparators, StringSplitOptions.RemoveEmptyEntries)
				.Select(token => token?.Trim())
				.Where(token => !token.IsNullOrWhiteSpace())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();
		}

		private static bool IsSchemaIgnoredByPattern(string schemaName, string pattern) {
			if (schemaName.IsNullOrWhiteSpace() || pattern.IsNullOrWhiteSpace()) {
				return false;
			}
			if (pattern.EndsWith("*", StringComparison.Ordinal)) {
				string prefix = pattern.Substring(0, pattern.Length - 1);
				return prefix.Length == 0 || schemaName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
			}
			return string.Equals(schemaName, pattern, StringComparison.OrdinalIgnoreCase);
		}

		private static bool ShouldIgnoreSchema(UserConnection userConnection, string schemaName) {
			if (schemaName.IsNullOrWhiteSpace()) {
				return true;
			}
			if (_technicalSchemas.Contains(schemaName)) {
				return true;
			}
			return GetIgnoredSchemaPatterns(userConnection).Any(pattern =>
				IsSchemaIgnoredByPattern(schemaName, pattern));
		}

		private static bool IsFolderSchemaName(string schemaName) {
			if (schemaName.IsNullOrWhiteSpace()) {
				return false;
			}
			if (_folderSchemas.Contains(schemaName)) {
				return true;
			}
			return schemaName.EndsWith("Folder", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsFolderDeletionEvent(Entity entity, EntityAfterEventArgs eventArgs,
				KnwSrcOutboxEventType eventType, ISet<string> changedColumns) {
			if (!IsFolderSchemaName(entity?.Schema?.Name)) {
				return false;
			}
			if (eventType == KnwSrcOutboxEventType.Delete) {
				return true;
			}
			if (eventType != KnwSrcOutboxEventType.Update) {
				return false;
			}
			if (entity.Schema.Columns.FindByName(IsDeletedColumnName) == null) {
				return false;
			}
			return entity.GetTypedColumnValue<bool>(IsDeletedColumnName);
		}

		internal static EntitySchemaQuery CreateSourcesUsingDynamicFolderEsq(EntitySchema sourceSchema) {
			var esq = new EntitySchemaQuery(sourceSchema) {
				IgnoreDisplayValues = true,
				UnmaskColumnValues = true,
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn(SourceConfigColumnPath);
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal,
				ProviderCodeColumnPath, SectionProviderCode));
			return esq;
		}

		private static IEnumerable<Guid> GetSourcesUsingDynamicFolder(UserConnection userConnection, Guid folderId) {
			if (folderId.IsEmpty()) {
				return Enumerable.Empty<Guid>();
			}
			EntitySchema sourceSchema = userConnection.EntitySchemaManager.GetInstanceByName(SourceSchemaName);
			EntitySchemaQuery esq = CreateSourcesUsingDynamicFolderEsq(sourceSchema);
			EntityCollection sourceEntities = esq.GetEntityCollection(userConnection);
			return sourceEntities
				.Select(entity => new {
					SourceId = entity.PrimaryColumnValue,
					ConfigJson = entity.GetTypedColumnValue<string>(SourceConfigColumnPath)
				})
				.Where(item => !item.ConfigJson.IsNullOrWhiteSpace())
				.Select(item => new {
					item.SourceId,
					Config = TryParseSectionProviderConfig(item.ConfigJson, item.SourceId)
				})
				.Where(item => item.Config != null
					&& string.Equals(item.Config.ScopeType, DynamicFolderScopeType,
						StringComparison.OrdinalIgnoreCase)
					&& item.Config.DynamicFolderId.HasValue
					&& item.Config.DynamicFolderId.Value == folderId)
				.Select(item => item.SourceId)
				.Distinct()
				.ToList();
		}

		private static SectionProviderConfig TryParseSectionProviderConfig(string configJson, Guid sourceId) {
			try {
				return JsonConvert.DeserializeObject<SectionProviderConfig>(configJson);
			} catch (Exception e) {
				KnwLibUtils.Logger.Error($"Failed to parse KnwSourceConfig for source {sourceId}.", e);
				return null;
			}
		}

		private static IKnwServiceClient ResolveServiceClient(UserConnection userConnection) {
			return ClassFactory.Get<IKnwServiceClient>(
				new ConstructorArgument("userConnection", userConnection),
				new ConstructorArgument("httpRequestClient", ClassFactory.Get<IHttpRequestClient>()));
		}

		private static void InvalidateSourcesByFolder(UserConnection userConnection, Guid folderId) {
			IEnumerable<Guid> sourceIds = GetSourcesUsingDynamicFolder(userConnection, folderId);
			IList<Guid> sourceIdList = sourceIds?.ToList() ?? new List<Guid>();
			if (!sourceIdList.Any()) {
				return;
			}
			IKnwServiceClient serviceClient = ResolveServiceClient(userConnection);
			IKnwSourceManager sourceManager = ClassFactory.Get<IKnwSourceManager>(
				new ConstructorArgument("userConnection", userConnection));
			foreach (Guid sourceId in sourceIdList) {
				try {
					serviceClient.InvalidateKnowledgeSource(sourceId, CancellationToken.None);
					sourceManager.UpdateStatus(sourceId, KnwSourceStatusEnum.Unavailable);
					KnwLibUtils.Logger.Info($"Knowledge source {sourceId} invalidated due to deleted folder {folderId}.");
				} catch (Exception e) {
					KnwLibUtils.Logger.Error($"Failed to invalidate source {sourceId} after folder {folderId} deletion.", e);
				}
			}
		}

		private static ISet<string> GetChangedColumns(EntityAfterEventArgs eventArgs) {
			var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (eventArgs?.ModifiedColumnValues == null) {
				return columns;
			}
			foreach (EntityColumnValue columnValue in eventArgs.ModifiedColumnValues) {
				if (columnValue != null && !columnValue.Name.IsNullOrWhiteSpace()) {
					columns.Add(columnValue.Name);
				}
			}
			return columns;
		}

		private static HashSet<string> ResolveRootColumns(UserConnection userConnection, Guid schemaUId,
				IEnumerable<string> configuredSpecs) {
			var rootColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (configuredSpecs == null) {
				return rootColumns;
			}
			EntitySchema schema = userConnection.EntitySchemaManager.GetInstanceByUId(schemaUId);
			foreach (string spec in configuredSpecs) {
				if (spec.IsNullOrWhiteSpace()) {
					continue;
				}
				if (Guid.TryParse(spec, out Guid columnUId)) {
					EntitySchemaColumn column = schema.Columns.FindByUId(columnUId);
					if (column != null) {
						rootColumns.Add(column.Name);
					}
					continue;
				}
				int dotIndex = spec.IndexOf('.');
				if (dotIndex > 0) {
					rootColumns.Add(spec.Substring(0, dotIndex));
					continue;
				}
				rootColumns.Add(spec);
			}
			return rootColumns;
		}

		private static bool IsViewToBaseSchemaMatch(string viewSchemaName, string baseSchemaName) {
			if (viewSchemaName.IsNullOrWhiteSpace() || baseSchemaName.IsNullOrWhiteSpace()) {
				return false;
			}
			if (!viewSchemaName.StartsWith("Vw", StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			string viewBasePart = viewSchemaName.Substring(2);
			if (viewBasePart.IsNullOrWhiteSpace()) {
				return false;
			}
			return string.Equals(viewBasePart, baseSchemaName, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsSchemaObservedForRoot(EntitySchemaManager schemaManager, Guid schemaUId,
				Guid rootSchemaUId) {
			if (schemaManager == null || schemaUId == Guid.Empty || rootSchemaUId == Guid.Empty) {
				return false;
			}
			if (schemaUId == rootSchemaUId) {
				return true;
			}
			HashSet<Guid> ancestry = BuildSchemaAncestry(schemaManager, schemaUId);
			if (ancestry.Contains(rootSchemaUId)) {
				return true;
			}
			try {
				EntitySchema observedSchema = schemaManager.GetInstanceByUId(schemaUId);
				EntitySchema rootSchema = schemaManager.GetInstanceByUId(rootSchemaUId);
				bool isCompatible = IsViewToBaseSchemaMatch(observedSchema?.Name, rootSchema?.Name)
					|| IsViewToBaseSchemaMatch(rootSchema?.Name, observedSchema?.Name);
				if (isCompatible) {
					KnwLibUtils.Logger.Debug($"Schema compatibility matched by Vw/base naming for observed " +
						$"schema {schemaUId} ({observedSchema?.Name ?? "<unknown>"}) and root schema " +
						$"{rootSchemaUId} ({rootSchema?.Name ?? "<unknown>"}).");
				}
				return isCompatible;
			} catch (Exception e) {
				KnwLibUtils.Logger.Debug($"Failed to resolve schema compatibility for observed schema {schemaUId} " +
					$"and root schema {rootSchemaUId}.", e);
				return false;
			}
		}

		internal static HashSet<Guid> BuildSchemaAncestry(EntitySchemaManager schemaManager, Guid schemaUId) {
			var ancestry = new HashSet<Guid>();
			Guid current = schemaUId;
			while (current != Guid.Empty) {
				if (!ancestry.Add(current)) {
					break;
				}
				try {
					EntitySchema schema = schemaManager.GetInstanceByUId(current);
					current = schema.ParentSchemaUId;
				} catch (ItemNotFoundException e) {
					KnwLibUtils.Logger.Debug($"Parent schema {current} was not found while building ancestry chain. " +
						$"Current ancestry: [{string.Join(", ", ancestry)}].", e);
					break;
				} catch (Exception e) {
					KnwLibUtils.Logger.Warn($"Failed to resolve parent for schema {current} " +
						$"while building ancestry chain.", e);
					break;
				}
			}
			return ancestry;
		}

		private static List<KnwSrcObservedBinding> GetBindings(UserConnection userConnection, Guid schemaUId) {
			EntitySchema schema = userConnection.EntitySchemaManager.GetInstanceByName(SourceSchemaName);
			var esq = new EntitySchemaQuery(schema) {
				IgnoreDisplayValues = true,
				UnmaskColumnValues = true,
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn(SourceConfigColumnPath);
			esq.AddColumn(ProviderCodeColumnPath);
			esq.AddColumn(SourceStatusValueColumnPath);
			var sourceEntities = esq.GetEntityCollection(userConnection);
			HashSet<Guid> ancestry = BuildSchemaAncestry(userConnection.EntitySchemaManager, schemaUId);
			KnwLibUtils.Logger.Debug($"Resolving observed bindings for schema {schemaUId}. " +
				$"Candidate sources: {sourceEntities.Count}. " +
				$"Schema ancestry: [{string.Join(", ", ancestry)}].");
			var result = new List<KnwSrcObservedBinding>();
			foreach (Entity sourceEntity in sourceEntities) {
				string providerCode = sourceEntity.GetTypedColumnValue<string>(ProviderCodeColumnValueName);
				int statusValue = sourceEntity.GetTypedColumnValue<int>(SourceStatusValueColumnName);
				if (!string.Equals(providerCode, SectionProviderCode, StringComparison.OrdinalIgnoreCase)
						|| statusValue != (int)KnwSourceStatusEnum.Available) {
					KnwLibUtils.Logger.Debug($"Skipping source {sourceEntity.PrimaryColumnValue} while resolving " +
						$"bindings. Provider={providerCode ?? "<null>"}, Status={statusValue}.");
					continue;
				}
				string configJson = sourceEntity.GetTypedColumnValue<string>(SourceConfigColumnPath);
				if (configJson.IsNullOrWhiteSpace()) {
					KnwLibUtils.Logger.Debug($"Skipping source {sourceEntity.PrimaryColumnValue} because " +
						"KnwSourceConfig is empty.");
					continue;
				}
				SectionProviderConfig config = null;
				try {
					config = JsonConvert.DeserializeObject<SectionProviderConfig>(configJson);
				} catch (Exception e) {
					KnwLibUtils.Logger.Error($"Failed to parse KnwSourceConfig for source " +
						$"{sourceEntity.PrimaryColumnValue}.", e);
					continue;
				}
				if (config == null || config.RootSchemaUId == Guid.Empty
						|| !IsSchemaObservedForRoot(userConnection.EntitySchemaManager, schemaUId,
							config.RootSchemaUId)) {
					KnwLibUtils.Logger.Debug($"Skipping source {sourceEntity.PrimaryColumnValue} because " +
						$"schema is not observed for root. Config={config?.RootSchemaUId ?? Guid.Empty}, " +
						$"Ancestry=[{string.Join(", ", ancestry)}].");
					continue;
				}
				result.Add(new KnwSrcObservedBinding {
					SourceId = sourceEntity.PrimaryColumnValue,
					RootColumns = ResolveRootColumns(userConnection, schemaUId, config.Columns ?? new List<string>())
				});
			}
			KnwLibUtils.Logger.Debug($"Resolved {result.Count} observed bindings for schema {schemaUId}.");
			return result;
		}

		private static bool HasIntersect(ISet<string> changedColumns, ISet<string> configuredColumns) {
			if (configuredColumns == null || configuredColumns.Count == 0) {
				return true;
			}
			if (changedColumns == null || changedColumns.Count == 0) {
				return false;
			}
			return changedColumns.Any(configuredColumns.Contains);
		}

		#endregion

		#region Properties: Internal

		internal Func<UserConnection, Guid, List<KnwSrcObservedBinding>> BindingsProvider { get; set; }

		internal Action<UserConnection, Guid> FolderDeletionInvalidator { get; set; }

		internal IKnwSrcOutboxRepository OutboxRepository { get; set; }

		#endregion

		#region Methods: Private

		private void HandleEntityChange(object sender, EntityAfterEventArgs eventArgs, KnwSrcOutboxEventType eventType) {
			if (!(sender is Entity entity) || entity.PrimaryColumnValue == Guid.Empty) {
				KnwLibUtils.Logger.Debug($"Skipping entity change handling because sender is invalid. " +
					$"EventType={eventType}.");
				return;
			}
			UserConnection userConnection = entity.UserConnection;
			string schemaName = entity.Schema?.Name;
			ISet<string> changedColumns = GetChangedColumns(eventArgs);
			if (IsFolderDeletionEvent(entity, eventArgs, eventType, changedColumns)) {
				Action<UserConnection, Guid> invalidator = FolderDeletionInvalidator ?? InvalidateSourcesByFolder;
				try {
					invalidator(userConnection, entity.PrimaryColumnValue);
				} catch (Exception e) {
					KnwLibUtils.Logger.Error($"Failed to invalidate sources for deleted folder " +
						$"{entity.PrimaryColumnValue}.", e);
				}
			}
			if (ShouldIgnoreSchema(userConnection, schemaName)) {
				KnwLibUtils.Logger.Debug($"Ignoring {eventType} event for schema {schemaName ?? "<null>"} " +
					$"and record {entity.PrimaryColumnValue}.");
				return;
			}
			Guid schemaUId = entity.Schema?.UId ?? Guid.Empty;
			if (schemaUId == Guid.Empty) {
				KnwLibUtils.Logger.Debug($"Skipping {eventType} event for record {entity.PrimaryColumnValue} " +
					"because schema UId is empty.");
				return;
			}
			string changedColumnsList = changedColumns.Count == 0
				? "<none>"
				: string.Join(",", changedColumns);
			KnwLibUtils.Logger.Debug($"Handling {eventType} event for schema {schemaName} ({schemaUId}), " +
				$"record {entity.PrimaryColumnValue}. Changed columns: {changedColumnsList}.");
			List<KnwSrcObservedBinding> bindings = BindingsProvider == null
				? GetBindings(userConnection, schemaUId)
				: BindingsProvider(userConnection, schemaUId);
			if (bindings.Count == 0) {
				KnwLibUtils.Logger.Debug($"No matching section bindings found for schema {schemaUId} and " +
					$"record {entity.PrimaryColumnValue}. Event type: {eventType}.");
				return;
			}
			IKnwSrcOutboxRepository repository = OutboxRepository ?? ClassFactory.Get<IKnwSrcOutboxRepository>(
				new ConstructorArgument("userConnection", userConnection));
			int upsertedCount = 0;
			foreach (KnwSrcObservedBinding binding in bindings) {
				if (eventType == KnwSrcOutboxEventType.Update && !HasIntersect(changedColumns, binding.RootColumns)) {
					KnwLibUtils.Logger.Debug($"Skipping update outbox upsert for source {binding.SourceId} " +
						$"and record {entity.PrimaryColumnValue} because changed columns do not intersect.");
					continue;
				}
				KnwLibUtils.Logger.Debug($"Upserting outbox event. Source={binding.SourceId}, " +
					$"Record={entity.PrimaryColumnValue}, SchemaUId={schemaUId}, EventType={eventType}.");
				repository.Upsert(binding.SourceId, entity.PrimaryColumnValue, schemaUId, eventType, changedColumns,
					DateTime.UtcNow);
				upsertedCount++;
			}
			KnwLibUtils.Logger.Debug($"Completed handling {eventType} event for record {entity.PrimaryColumnValue}. " +
				$"Outbox upserts: {upsertedCount}.");
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			HandleEntityChange(sender, e, KnwSrcOutboxEventType.Create);
		}

		/// <inheritdoc />
		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			HandleEntityChange(sender, e, KnwSrcOutboxEventType.Update);
		}

		/// <inheritdoc />
		public override void OnDeleted(object sender, EntityAfterEventArgs e) {
			base.OnDeleted(sender, e);
			HandleEntityChange(sender, e, KnwSrcOutboxEventType.Delete);
		}

		#endregion

	}

	#endregion

}

