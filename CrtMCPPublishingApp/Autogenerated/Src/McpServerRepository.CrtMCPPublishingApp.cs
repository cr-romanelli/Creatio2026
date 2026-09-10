using Terrasoft.Core.Factories;

namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;

	#region Class: McpServerRepository

	/// <summary>
	/// Default <see cref="IMcpServerRepository"/> implementation over
	/// <c>McpServer</c> / <c>McpTool</c>. Reads use raw <see cref="Select"/>
	/// against physical columns (resilient to ESQ lookup-column quirks); writes
	/// use the entity API. Tools are server-scoped via the <c>McpServerId</c> FK
	/// and mapped onto <see cref="McpToolModel"/> for the shared tools pipeline.
	/// </summary>
	[DefaultBinding(typeof(IMcpServerRepository))]
	internal class McpServerRepository : IMcpServerRepository
	{

		#region Constants: Private

		private const string ServerSchemaName = "McpServer";
		private const string ToolSchemaName = "McpTool";

		#endregion

		#region Fields: Private

		private readonly UserConnection _userConnection;

		#endregion

		#region Constructors: Public

		public McpServerRepository(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		#endregion

		#region Methods: Private

		private static int GetOrdinal(IDataReader reader, string columnName) {
			for (int index = 0; index < reader.FieldCount; index++) {
				if (string.Equals(reader.GetName(index), columnName, StringComparison.OrdinalIgnoreCase)) {
					return index;
				}
			}
			return -1;
		}

		private static string GetString(IDataReader reader, string columnName) {
			int ordinal = GetOrdinal(reader, columnName);
			return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetString(ordinal) : null;
		}

		private static Guid GetGuid(IDataReader reader, string columnName) {
			int ordinal = GetOrdinal(reader, columnName);
			return ordinal >= 0 && !reader.IsDBNull(ordinal) ? reader.GetGuid(ordinal) : Guid.Empty;
		}

		private static bool GetBoolean(IDataReader reader, string columnName) {
			int ordinal = GetOrdinal(reader, columnName);
			return ordinal >= 0 && !reader.IsDBNull(ordinal) && reader.GetBoolean(ordinal);
		}
		
		private static McpServerModel MapServer(IDataReader reader) {
			return new McpServerModel {
				UId = GetGuid(reader, "Id"),
				Title = GetString(reader, "Name"),
				Code = GetString(reader, "Code"),
				Description = GetString(reader, "Description"),
				IsOnline = GetBoolean(reader, "IsOnline")
			};
		}

		private static McpToolModel MapTool(IDataReader reader) {
			return new McpToolModel {
				Id = GetGuid(reader, "Id"),
				McpServerId = GetGuid(reader, "McpServerId"),
				ProcessId = GetGuid(reader, "BusinessProcessId"),
				ToolSourceTypeId = GetGuid(reader, "ToolSourceTypeId"),
				SourceCodeAction = GetString(reader, "SourceCodeAction"),
				Title = GetString(reader, "Title"),
				ExternalName = GetString(reader, "ExternalName"),
				Description = GetString(reader, "Description"),
				InputSchema = GetString(reader, "InputSchema"),
				OutputSchema = GetString(reader, "OutputSchema"),
				Annotations = GetString(reader, "Annotations"),
				IsEnabled = GetBoolean(reader, "IsEnabled")
			};
		}

		private Select CreateServerSelect() {
			return (Select)new Select(_userConnection)
				.Column("Id")
				.Column("Name")
				.Column("Code")
				.Column("Description")
				.Column("IsOnline")
				.From(ServerSchemaName);
		}

		private Select CreateToolSelect() {
			return (Select)new Select(_userConnection)
				.Column("Id")
				.Column("McpServerId")
				.Column("BusinessProcessId")
				.Column("ToolSourceTypeId")
				.Column("SourceCodeAction")
				.Column("Title")
				.Column("ExternalName")
				.Column("Description")
				.Column("InputSchema")
				.Column("OutputSchema")
				.Column("Annotations")
				.Column("IsEnabled")
				.From(ToolSchemaName);
		}

		private List<T> ExecuteRead<T>(Select select, Func<IDataReader, T> map) {
			var result = new List<T>();
			using (DBExecutor dbExecutor = _userConnection.EnsureDBConnection()) {
				using (IDataReader reader = select.ExecuteReader(dbExecutor)) {
					while (reader.Read()) {
						result.Add(map(reader));
					}
				}
			}
			return result;
		}

		#endregion

		#region Methods: Public

		public McpServerModel GetServerByCode(string code) {
			if (code.IsNullOrWhiteSpace()) {
				return null;
			}
			// Match Code in memory (OrdinalIgnoreCase) instead of via a SQL
			// "Code = :code" filter. A case-sensitive DB collation (e.g. on
			// PostgreSQL, which Creatio supports) would make the SQL equality
			// case-sensitive and drop rows that differ only by case BEFORE any
			// in-memory check — breaking case-insensitive routing. The server set
			// is small (admin-managed), so the full read is cheap. Same rationale
			// in GetToolForServerByName.
			return ExecuteRead(CreateServerSelect(), MapServer)
				.FirstOrDefault(server => string.Equals(server.Code, code, StringComparison.OrdinalIgnoreCase));
		}

		public IReadOnlyCollection<McpToolModel> GetEnabledToolsForServer(Guid serverId) {
			if (serverId.IsEmpty()) {
				return Array.Empty<McpToolModel>();
			}
			Select select = CreateToolSelect();
			select.Where("McpServerId").IsEqual(Column.Parameter(serverId))
				.And("IsEnabled").IsEqual(Column.Parameter(true));
			select.OrderByAsc("ExternalName");
			return ExecuteRead(select, MapTool);
		}

		public McpToolModel GetToolForServerByName(Guid serverId, string externalToolName) {
			if (serverId.IsEmpty() || externalToolName.IsNullOrWhiteSpace()) {
				return null;
			}
			// Keep the indexed server + IsEnabled filters in SQL, but match the
			// tool name in memory (OrdinalIgnoreCase) — collation-independent, over
			// the small set of one server's enabled tools.
			Select select = CreateToolSelect();
			select.Where("McpServerId").IsEqual(Column.Parameter(serverId))
				.And("IsEnabled").IsEqual(Column.Parameter(true));
			return ExecuteRead(select, MapTool).FirstOrDefault(tool =>
				string.Equals(tool.ExternalName, externalToolName, StringComparison.OrdinalIgnoreCase));
		}

		#endregion

	}

	#endregion

}

