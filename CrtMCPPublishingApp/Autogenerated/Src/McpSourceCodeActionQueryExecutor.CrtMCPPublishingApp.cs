namespace Terrasoft.Configuration
{
	using global::Common.Logging;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Security.Cryptography;
	using System.Text;
	using Creatio.Copilot.Actions;
	using Terrasoft.AppFeatures;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;

	#region Class: McpSourceCodeActionQueryExecutor

	/// <summary>
	/// Data provider for the virtual <c>McpSourceCodeAction</c> entity. The entity has
	/// no physical table (<c>IsVirtual = true</c>); its rows are produced here by a
	/// reflection scan of the source-code actions (classes deriving Creatio Copilot's
	/// <c>BaseExecutableCodeAction</c>) that can be published as MCP tools — the
	/// source-code analogue of the <c>VwProcessLib</c> view the process picker uses.
	///
	/// Resolved by convention: <c>EntitySchemaQueryExecutorFactory</c> looks up
	/// <c>{EntitySchemaName}QueryExecutor</c>, so the class name and the
	/// <see cref="DefaultBindingAttribute"/> <c>Name</c> must stay
	/// <c>McpSourceCodeActionQueryExecutor</c>. The standard selection window
	/// (<c>crt.OpenSelectionWindowRequest</c> with <c>entitySchemaName</c>) then reads
	/// it transparently, exactly like a table-backed entity.
	///
	/// Each row's <c>Id</c> is a STABLE hash of its <c>FullTypeName</c> so that the
	/// post-selection reload-by-Id (which arrives here as a primary-column filter)
	/// returns the same row. The scan is cached for the process lifetime (the action
	/// set only changes on a configuration rebuild, which reloads the app domain).
	/// </summary>
	[DefaultBinding(typeof(IEntityQueryExecutor), Name = nameof(McpSourceCodeActionQueryExecutor))]
	public class McpSourceCodeActionQueryExecutor : BaseQueryExecutor, IEntityQueryExecutor
	{

		#region Constants: Private

		private const string EntitySchemaName = "McpSourceCodeAction";
		private const string AbstractionsAssemblyName = "Creatio.Copilot.Abstractions";

		#endregion

		#region Fields: Private

		private static readonly ILog Log = LogManager.GetLogger(nameof(McpSourceCodeActionQueryExecutor));
		private static volatile List<SourceCodeActionInfo> _cachedActions;
		private static readonly object CacheLock = new object();

		#endregion

		#region Constructors: Public

		public McpSourceCodeActionQueryExecutor(UserConnection userConnection)
			: base(userConnection, EntitySchemaName) {
		}

		#endregion

		#region Methods: Public

		public EntityCollection GetEntityCollection(EntitySchemaQuery esq) {
			QueryFilterInfo filterInfo = esq.Filters.ParseFilters();
			// Fast path: a reload-by-Id (the post-selection lookup) is a primary-column
			// filter. Anything else (the lookup search box's Name "contains", plus any
			// master filters) is honored generically so the search box actually filters.
			bool filterById = GetIsPrimaryColumnValueFilter(filterInfo, out Guid requestedId);
			var matched = new List<SourceCodeActionInfo>();
			foreach (SourceCodeActionInfo action in GetActions()) {
				if (filterById) {
					if (action.Id != requestedId) {
						continue;
					}
				} else if (!MatchesFilter(filterInfo, action)) {
					continue;
				}
				matched.Add(action);
			}
			// Like filtering, ordering has no DB to defer to for a virtual entity, so the
			// column sort the grid puts on the ESQ must be applied in-memory here; without
			// this the list would always stay in the default scan order (Caption asc) and
			// clicking a column header would do nothing.
			List<SourceCodeActionInfo> ordered = ApplySort(esq, matched);
			var collection = new EntityCollection(UserConnection, EntitySchema);
			foreach (SourceCodeActionInfo action in ordered) {
				Entity entity = EntitySchema.CreateEntity(UserConnection);
				entity.LoadColumnValue("Id", action.Id);
				entity.LoadColumnValue("Name", action.Caption);
				entity.LoadColumnValue("FullTypeName", action.FullTypeName);
				entity.LoadColumnValue("Description", action.Description);
				collection.Add(entity);
			}
			return collection;
		}

		#endregion

		#region Methods: Private

		// Evaluates the parsed ESQ filter tree against a scanned action in-memory (no DB
		// to defer to for a virtual entity). Honors AND/OR filter collections and
		// column-value comparisons (equals, contains, starts/ends-with — the lookup
		// search uses "contains" on the display column). Unknown filter types / columns
		// are treated as a match so an unrecognized filter never hides everything.
		private static bool MatchesFilter(QueryFilterInfo filter, SourceCodeActionInfo action) {
			if (filter == null) {
				return true;
			}
			if (filter is FilterCollection collection) {
				if (collection.Filters == null || collection.Filters.Count == 0) {
					return true;
				}
				return collection.LogicalOperation == LogicalOperationStrict.Or
					? collection.Filters.Any(child => MatchesFilter(child, action))
					: collection.Filters.All(child => MatchesFilter(child, action));
			}
			if (filter is CompareColumnWithValueFilter compare) {
				return MatchesColumn(compare, action);
			}
			return true;
		}

		private static bool MatchesColumn(CompareColumnWithValueFilter compare, SourceCodeActionInfo action) {
			switch (compare.ColumnPath) {
				case "Id":
					return compare.GetIsValueMatched(action.Id);
				case "Name":
					return compare.GetIsValueMatched(action.Caption);
				case "FullTypeName":
					return compare.GetIsValueMatched(action.FullTypeName);
				case "Description":
					return compare.GetIsValueMatched(action.Description);
				default:
					return true;
			}
		}

		// Translates the ESQ column ordering (which the grid sets on a header click) into
		// the executor-agnostic sort spec and delegates to the pure, unit-tested
		// McpSourceCodeActionQuery.Sort. Reading OrderDirection/OrderPosition/Path is the
		// only ESQ-coupled part; all ordering semantics live in the helper.
		private static List<SourceCodeActionInfo> ApplySort(EntitySchemaQuery esq, List<SourceCodeActionInfo> actions) {
			List<SourceCodeActionSortColumn> sortColumns = esq.Columns
				.Where(column => column.OrderDirection != OrderDirection.None)
				.Select(column => new SourceCodeActionSortColumn(
					column.Path,
					column.OrderDirection == OrderDirection.Ascending,
					column.OrderPosition))
				.ToList();
			return McpSourceCodeActionQuery.Sort(actions, sortColumns);
		}

		// Deterministic id from the full type name so the same action always maps to
		// the same row id across calls (list query + reload-by-id). Hash only — not
		// security-sensitive; MD5 conveniently yields 16 bytes for a Guid.
		private static Guid ToStableId(string fullTypeName) {
			using (MD5 md5 = MD5.Create()) {
				return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(fullTypeName ?? string.Empty)));
			}
		}

		private static List<SourceCodeActionInfo> GetActions() {
			if (_cachedActions != null) {
				return _cachedActions;
			}
			// Scan (which instantiates each action to read its metadata) OUTSIDE the lock,
			// so a slow/side-effecting action constructor never stalls other first-openers.
			// Concurrent first-callers may each scan once (idempotent); the lock only
			// publishes the first result. NOTE: action constructors are expected to be
			// cheap and side-effect-free — they run during this admin picker scan.
			List<SourceCodeActionInfo> scanned = ScanActions();
			lock (CacheLock) {
				if (_cachedActions == null) {
					_cachedActions = scanned;
				}
				return _cachedActions;
			}
		}

		private static List<SourceCodeActionInfo> ScanActions() {
			var actions = new List<SourceCodeActionInfo>();
			foreach (Assembly assembly in GetCandidateAssemblies()) {
				foreach (Type type in GetLoadableTypes(assembly)) {
					if (type == null || type.IsAbstract || !type.IsClass ||
							!typeof(BaseExecutableCodeAction).IsAssignableFrom(type)) {
						continue;
					}
					if (McpSourceCodeActionQuery.IsExcludedActionType(type.FullName)) {
						continue;
					}
					SourceCodeActionInfo info = TryDescribe(type);
					if (info != null) {
						actions.Add(info);
					}
				}
			}
			return actions
				.OrderBy(action => action.Caption, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		// Only assemblies referencing Creatio.Copilot.Abstractions can contain an action
		// subclass, so the rest of the app domain is skipped without loading its types.
		private static IEnumerable<Assembly> GetCandidateAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => {
				try {
					if (string.Equals(assembly.GetName().Name, AbstractionsAssemblyName,
							StringComparison.OrdinalIgnoreCase)) {
						return false;
					}
					return assembly.GetReferencedAssemblies().Any(reference =>
						string.Equals(reference.Name, AbstractionsAssemblyName, StringComparison.OrdinalIgnoreCase));
				} catch {
					return false;
				}
			});
		}

		private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
			try {
				return assembly.GetTypes();
			} catch (ReflectionTypeLoadException exception) {
				return exception.Types.Where(type => type != null);
			} catch (Exception exception) {
				Log.Debug($"Failed to read types from assembly '{assembly.FullName}'.", exception);
				return Array.Empty<Type>();
			}
		}

		private static SourceCodeActionInfo TryDescribe(Type type) {
			try {
				var action = (BaseExecutableCodeAction)Activator.CreateInstance(type);
				if (!action.IsEnabled) {
					return null;
				}
				string caption = action.GetCaption() != null ? action.GetCaption().ToString() : null;
				string description = action.GetDescription() != null ? action.GetDescription().ToString() : null;
				string fullTypeName = type.FullName + ", " + type.Assembly.GetName().Name;
				return new SourceCodeActionInfo {
					Id = ToStableId(fullTypeName),
					FullTypeName = fullTypeName,
					Caption = caption.IsNullOrWhiteSpace() ? type.Name : caption,
					Description = description ?? string.Empty
				};
			} catch (Exception exception) {
				Log.Debug($"Skipping source code action '{type.FullName}': could not instantiate.", exception);
				return null;
			}
		}

		#endregion

	}

	#endregion

	#region Class: SourceCodeActionInfo

	/// <summary>
	/// One scanned source-code action row surfaced by <see cref="McpSourceCodeActionQueryExecutor"/>.
	/// Public so the pure query helpers (<see cref="McpSourceCodeActionQuery"/>) and their unit
	/// tests can operate on it without a live UserConnection.
	/// </summary>
	public sealed class SourceCodeActionInfo
	{
		// Stable row id (hash of FullTypeName), computed once at scan time so the
		// per-query GetEntityCollection loop doesn't recompute an MD5 per row.
		public Guid Id { get; set; }
		public string FullTypeName { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
	}

	#endregion

	#region Class: SourceCodeActionSortColumn

	/// <summary>
	/// Executor-agnostic description of one requested sort column: the action field
	/// (<see cref="ColumnPath"/>), direction (<see cref="Ascending"/>) and the grid's
	/// multi-sort <see cref="Position"/>. Decouples the pure sort from EntitySchemaQuery.
	/// </summary>
	public sealed class SourceCodeActionSortColumn
	{
		public SourceCodeActionSortColumn(string columnPath, bool ascending, int position) {
			ColumnPath = columnPath;
			Ascending = ascending;
			Position = position;
		}

		public string ColumnPath { get; }
		public bool Ascending { get; }
		public int Position { get; }
	}

	#endregion

	#region Class: McpSourceCodeActionQuery

	/// <summary>
	/// Pure, UserConnection-free query helpers for the source-code-action picker: the
	/// built-in-action exclusion predicate and the in-memory column sort. Kept out of
	/// <see cref="McpSourceCodeActionQueryExecutor"/> (which needs a live UserConnection and
	/// derives from a CrtCoreBase type) so the logic is unit-testable in isolation.
	/// </summary>
	public static class McpSourceCodeActionQuery
	{

		// Built-in Copilot conversation actions that make no sense as standalone MCP tools
		// (they act on the live Copilot conversation), so they are hidden from the picker.
		// Keyed by Type.FullName rather than caption/Id: the virtual rows have no persistent
		// Id, and captions are localizable/renameable, whereas the CLR type name is stable.
		private static readonly HashSet<string> ExcludedActionFullTypeNames =
			new HashSet<string>(StringComparer.Ordinal) {
				"Creatio.Copilot.CancelExecutionAction",
				"Creatio.Copilot.NotifyUserAction",
				"Creatio.Copilot.SendMessageToUserAction"
			};

		/// <summary>
		/// True when the action type (by <c>Type.FullName</c>, without assembly) is a built-in
		/// action that must not appear in the picker.
		/// </summary>
		public static bool IsExcludedActionType(string fullTypeName) {
			return fullTypeName != null && ExcludedActionFullTypeNames.Contains(fullTypeName);
		}

		/// <summary>
		/// Orders <paramref name="actions"/> by <paramref name="sortColumns"/> (applied in
		/// <see cref="SourceCodeActionSortColumn.Position"/> order, ascending/descending,
		/// OrdinalIgnoreCase). Columns with no sort key (primary key / unknown) are skipped;
		/// when nothing sortable is requested the input order is preserved.
		/// </summary>
		public static List<SourceCodeActionInfo> Sort(IReadOnlyList<SourceCodeActionInfo> actions,
				IReadOnlyList<SourceCodeActionSortColumn> sortColumns) {
			if (actions == null) {
				return new List<SourceCodeActionInfo>();
			}
			IEnumerable<SourceCodeActionSortColumn> orderedColumns =
				(sortColumns ?? new List<SourceCodeActionSortColumn>()).OrderBy(column => column.Position);
			IOrderedEnumerable<SourceCodeActionInfo> ordered = null;
			foreach (SourceCodeActionSortColumn column in orderedColumns) {
				Func<SourceCodeActionInfo, string> keySelector = GetSortKeySelector(column.ColumnPath);
				if (keySelector == null) {
					continue;
				}
				if (ordered == null) {
					ordered = column.Ascending
						? actions.OrderBy(keySelector, StringComparer.OrdinalIgnoreCase)
						: actions.OrderByDescending(keySelector, StringComparer.OrdinalIgnoreCase);
				} else {
					ordered = column.Ascending
						? ordered.ThenBy(keySelector, StringComparer.OrdinalIgnoreCase)
						: ordered.ThenByDescending(keySelector, StringComparer.OrdinalIgnoreCase);
				}
			}
			return ordered != null ? ordered.ToList() : actions.ToList();
		}

		// Maps a column path to the action field used as the sort key. Returns null for the
		// primary key / unknown columns, which then don't participate in sorting.
		private static Func<SourceCodeActionInfo, string> GetSortKeySelector(string columnPath) {
			switch (columnPath) {
				case "Name":
					return action => action.Caption;
				case "Description":
					return action => action.Description;
				case "FullTypeName":
					return action => action.FullTypeName;
				default:
					return null;
			}
		}

	}

	#endregion

}

