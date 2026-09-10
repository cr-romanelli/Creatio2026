namespace Creatio.Copilot
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Factories;
	using Terrasoft.File;
	using Terrasoft.File.Abstractions;

	#region Class: FileKnwSourceProvider

	/// <summary>
	/// Implementation of <see cref="IKnwProvider"/> for file-based knowledge sources.
	/// </summary>
	[DefaultBinding(typeof(IKnwProvider), Name = "FileKnowledgeSourceProvider")]
	public class FileKnwSourceProvider : IKnwProvider
	{

		#region Constants: Private

		private const string KnwSourceFileSchemaName = "KnwSourceFile";
		private const string FileDownloadRelativeUrlFormat = "rest/FileService/Download/KnwSourceFile/{0}";

		#endregion

		#region Fields: Private

		private UserConnection _userConnection;
		private Guid _knwSourceId;
		private readonly IBaseUriResolver _baseUriResolver;

		#endregion

		#region Constructors: Public

		/// <summary>
		/// Initializes a new instance of the <see cref="FileKnwSourceProvider"/> type.
		/// </summary>
		/// <param name="baseUriResolver">Base uri resolver.</param>
		public FileKnwSourceProvider(IBaseUriResolver baseUriResolver) {
			_baseUriResolver = baseUriResolver;
		}

		#endregion

		#region Methods: Private

		private void WriteLog(string context, Exception exception, Guid? fileId = null) {
			string sourcePart = _knwSourceId.IsNotEmpty() ? $"SourceId={_knwSourceId}" : "SourceId=<empty>";
			string filePart = fileId.HasValue && fileId.Value.IsNotEmpty() ? $", FileId={fileId.Value}" : string.Empty;
			KnwLibUtils.Logger.Error($"[{nameof(FileKnwSourceProvider)}] {context}. {sourcePart}{filePart}",
				exception);
		}

		private EntitySchemaQuery GetEsq(DateTime? modifiedSince) {
			var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager, KnwSourceFileSchemaName) {
				PrimaryQueryColumn = {
					IsAlwaysSelect = true
				}
			};
			esq.AddColumn("Name");
			esq.AddColumn("TotalSize");
			esq.AddColumn("ModifiedOn");
			IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(FilterComparisonType.Equal,
				"KnwSource", _knwSourceId);
			esq.Filters.Add(filter);
			if (modifiedSince.HasValue) {
				IEntitySchemaQueryFilterItem modifiedOnFilter = esq.CreateFilterWithParameters(
					FilterComparisonType.Greater, "ModifiedOn", modifiedSince.Value);
				esq.Filters.Add(modifiedOnFilter);
			}
			return esq;
		}

		private Func<CancellationToken, Stream> CreateContentStreamFactory(Guid id) {
			return cancellationToken => {
				if (cancellationToken.IsCancellationRequested) {
					return Stream.Null;
				}
				try {
					var locator = new EntityFileLocator(KnwSourceFileSchemaName, id);
					IFile file = _userConnection.GetFile(locator);
					return file == null
						? Stream.Null
						: file.Read();
				} catch (Exception e) {
					WriteLog($"Read content for file {id}", e, id);
					return Stream.Null;
				}
			};
		}

		private KnwItem GetKnwItem(Entity entity) {
			Guid id = entity.PrimaryColumnValue;
			string name = entity.GetTypedColumnValue<string>("Name");
			int size = entity.GetTypedColumnValue<int>("TotalSize");
			DateTime modifiedOn = entity.GetTypedColumnValue<DateTime>("ModifiedOn");
			if (name.IsNullOrWhiteSpace()) {
				name = id.ToString("N");
			}
			return new KnwItem {
				Id = id.ToString("D"),
				KnwSourceId = _knwSourceId,
				ItemType = "File",
				DisplayName = name,
				SizeBytes = size,
				ModifiedOn = modifiedOn == DateTime.MinValue ? DateTime.UtcNow : modifiedOn,
				Hash = string.Empty,
				Metadata = new Dictionary<string, string> {
					{ "FileId", id.ToString() },
					{ "KnwSourceFileId", id.ToString() },
					{ "FileSchemaName", KnwSourceFileSchemaName }
				},
				ContentStreamFactory = CreateContentStreamFactory(id)
			};
		}

		private string GetUrl(string fileId) {
			string relativeUrl = string.Format(FileDownloadRelativeUrlFormat, fileId);
			return new Uri(_baseUriResolver.ResolveBaseUri(), relativeUrl).ToString();
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public IEnumerable<KnwItem> Discover(KnwProviderDiscoveryOptions options,
				CancellationToken ct = default) {
			if (ct.IsCancellationRequested || _userConnection == null || _knwSourceId.IsEmpty()) {
				return Enumerable.Empty<KnwItem>();
			}
			EntitySchemaQuery esq = GetEsq(options.ModifiedSince);
			EntityCollection entities;
			try {
				entities = esq.GetEntityCollection(_userConnection);
			} catch (Exception e) {
				WriteLog("Failed to execute ESQ for knowledge source file", e);
				entities = new EntityCollection(_userConnection, KnwSourceFileSchemaName);
			}
			var result = new List<KnwItem>(entities.Count);
			foreach (Entity entity in entities) {
				if (ct.IsCancellationRequested) {
					break;
				}
				KnwItem item = GetKnwItem(entity);
				result.Add(item);
			}
			return result;
		}

		/// <inheritdoc />

		public void Initialize(KnwProviderInitializationOptions initializationOptions) {
			_knwSourceId = initializationOptions.KnwSource?.Id ?? Guid.Empty;
			_userConnection = initializationOptions.UserConnection;
		}

		/// <inheritdoc />
		public Dictionary<string, KnwItemCitation> GetItemsCitations(ISet<string> itemIds) {
			var result = new Dictionary<string, KnwItemCitation>();
			if (itemIds == null || _userConnection == null || _knwSourceId.IsEmpty()) {
				return result;
			}
			var itemIdList = itemIds.ToList();
			if (itemIdList.Count == 0) {
				return result;
			}
			var guidList = new List<string>();
			foreach (string itemId in itemIdList) {
				if (Guid.TryParse(itemId, out Guid guid) && guid != Guid.Empty) {
					guidList.Add(guid.ToString());
				}
			}
			if (guidList.Count == 0) {
				return result;
			}
			try {
				var esq = new EntitySchemaQuery(_userConnection.EntitySchemaManager, KnwSourceFileSchemaName) {
					PrimaryQueryColumn = { IsAlwaysSelect = true }
				};
				esq.AddColumn("Name");
				IEntitySchemaQueryFilterItem filter = esq.CreateFilterWithParameters(
					FilterComparisonType.Equal, "Id", guidList);
				esq.Filters.Add(filter);
				EntityCollection entities = esq.GetEntityCollection(_userConnection);
				foreach (Entity entity in entities) {
					Guid id = entity.PrimaryColumnValue;
					string name = entity.GetTypedColumnValue<string>("Name");
					if (name.IsNullOrWhiteSpace()) {
						name = id.ToString("N");
					}
					string formattedId = id.ToString();
					result[formattedId] = new KnwItemCitation {
						ItemId = formattedId,
						SourceLocation = GetUrl(formattedId),
						SourceDisplayName = name
					};
				}
			} catch (Exception e) {
				WriteLog("Failed to retrieve citations for file items", e);
			}
			return result;
		}

		/// <inheritdoc />
		public ISet<string> GetUserAllowedItems(ISet<string> itemIds) {

			// TODO RND-81183: Implement access control filtering for file-based sources
			return itemIds;
		}

		#endregion

	}

	#endregion

}
