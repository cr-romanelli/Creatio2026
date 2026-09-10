namespace Terrasoft.Core.Process
{

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Drawing;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.DB;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Process;
	using Terrasoft.Core.Process.Configuration;

	#region Class: ConfActivityLogCleanerMethodsWrapper

	/// <exclude/>
	public class ConfActivityLogCleanerMethodsWrapper : ProcessModel
	{

		public ConfActivityLogCleanerMethodsWrapper(Process process)
			: base(process) {
			AddScriptTaskMethod("ScriptTask2Execute", ScriptTask2Execute);
		}

		#region Methods: Private

		private bool ScriptTask2Execute(ProcessExecutingContext context) {
			// Get required process parameters
			var userConnection = Get<UserConnection>("UserConnection");
			var dateForCleaner = Get<DateTime>("DateForCleaner");
			
			// Convert Date parameter to start of the day (00:00:00)
			DateTime cutoff = dateForCleaner.Date;
			
			// Batch size to avoid timeouts and large transactions
			const int batchSize = 1000;
			
			// Counters for deleted records by type
			var totalConfActivityLog = 0;
			var totalCompilationHistory = 0;
			var totalSysFile = 0;
			var totalLocalizationChangeLog = 0;
			
			/* ============================================================
			 * Phase A:
			 * Delete ConfActivityLog records in leaf-first order
			 * (children first, then parents)
			 * ============================================================ */
			
			while (true)
			{
			    // 1) Select leaf ConfActivityLog IDs older than cutoff
			    // Leaf = no child rows where Child.ParentId = This.Id
			    var leafSelect = new Select(userConnection)
			        .Column("l", "Id")
			        .From("ConfActivityLog").As("l")
			        .Where("l", "CreatedOn").IsLess(Column.Parameter(cutoff))
			        .And()
			        .Not()
			        .Exists(
			            new Select(userConnection)
			                .Column(Column.Const(1))   // SELECT 1
			                .From("ConfActivityLog").As("c")
			                .Where("c", "ParentId").IsEqual(Column.SourceColumn("l", "Id"))
			        )
			        .OrderByAsc("l", "CreatedOn")
			        .OrderByAsc("l", "Id") as Select;
			
			    var ids = new List<Guid>();
			
			    using (var dbExecutor = userConnection.EnsureDBConnection())
			    using (var reader = leafSelect.ExecuteReader(dbExecutor))
			    {
			        int counter = 0;
			
			        while (reader.Read())
			        {
			            ids.Add(reader.GetColumnValue<Guid>("Id"));
			            counter++;
			
			            // Manual batch limit (replacement for TOP)
			            if (counter >= batchSize)
			            {
			                break;
			            }
			        }
			    }
			
			    if (ids.Count == 0)
			    {
			        break; // no more deletable leaf records
			    }
			
			    // 2) Delete dependent records from CompilationHistory first
			    int deletedCompHistory = new Delete(userConnection)
			        .From("CompilationHistory")
			        .Where("ConfActivityLog")
			        .In(Column.Parameters(ids.Cast<object>().ToArray()))
			        .Execute();
			    
			    totalCompilationHistory += deletedCompHistory;
			
			    // 3) Delete SysFile records using Entity abstraction (S3-aware)
			    var sysFileSchema = userConnection.EntitySchemaManager.GetInstanceByName("SysFile");
			    foreach (var recordId in ids)
			    {
			        var esq = new EntitySchemaQuery(sysFileSchema);
			        esq.PrimaryQueryColumn.IsVisible = true;
			        esq.AddAllSchemaColumns();
			        esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "RecordId", recordId));
			        
			        var sysFileEntities = esq.GetEntityCollection(userConnection);
			        
			        // Create a copy to avoid "Collection was modified" error
			        var entitiesToDelete = new List<Entity>();
			        foreach (var entity in sysFileEntities)
			        {
			            entitiesToDelete.Add(entity);
			        }
			        
			        // Now iterate over the copy
			        foreach (var sysFileEntity in entitiesToDelete)
			        {
			            var fileId = sysFileEntity.PrimaryColumnValue;
			            var deleted = false;
			            try
			            {
			                sysFileEntity.Delete();
			                deleted = true;
			                totalSysFile++; // Count successfully deleted file
			            }
			            catch (System.Exception ex)
			            {
			                // S3 delete failed
			            }
			            
			            if (deleted == false)
			            {
			                try
			                {
			                    int deletedFile = new Delete(userConnection)
			                        .From("SysFile")
			                        .Where("Id").IsEqual(Column.Parameter(fileId))
			                        .Execute();
			                    totalSysFile += deletedFile;
			                }
			                catch (System.Exception ex)
			                {
			                    // DB delete failed too, ignore
			                }
			            }
			        }
			    }
			    
			    // 4) Delete dependent records from LocalizationChangeLog
			    int deletedLocChangeLog = new Delete(userConnection)
			        .From("LocalizationChangeLog")
			        .Where("ConfActivityLog")
			        .In(Column.Parameters(ids.Cast<object>().ToArray()))
			        .Execute();
			    
			    totalLocalizationChangeLog += deletedLocChangeLog;
			
			    // 5) Delete leaf records from ConfActivityLog
			    int deletedConfActivityLog = new Delete(userConnection)
			        .From("ConfActivityLog")
			        .Where("Id")
			        .In(Column.Parameters(ids.Cast<object>().ToArray()))
			        .Execute();
			
			    totalConfActivityLog += deletedConfActivityLog;
			
			    if (deletedConfActivityLog <= 0)
			    {
			        break;
			    }
			}
			
			/* ============================================================
			 * Phase B:
			 * Delete orphan CompilationHistory rows older than cutoff
			 * ============================================================ */
			
			while (true)
			{
			    var esqHistory = new EntitySchemaQuery(userConnection.EntitySchemaManager, "CompilationHistory")
			    {
			        RowCount = batchSize
			    };
			
			    esqHistory.PrimaryQueryColumn.IsVisible = true;
			    esqHistory.AddColumn("Id");
			
			    var historyCreatedOn = esqHistory.AddColumn("CreatedOn");
			    historyCreatedOn.OrderByAsc();
			
			    esqHistory.Filters.Add(esqHistory.CreateFilterWithParameters(
			        FilterComparisonType.Less, "CreatedOn", cutoff
			    ));
			
			    esqHistory.Filters.Add(esqHistory.CreateIsNullFilter("ConfActivityLog"));
			
			    var historyEntities = esqHistory.GetEntityCollection(userConnection);
			    if (historyEntities.Count == 0)
			    {
			        break;
			    }
			
			    var historyIds = historyEntities
			        .Select(e => e.PrimaryColumnValue)
			        .Where(id => id != Guid.Empty)
			        .ToList();
			
			    if (historyIds.Count == 0)
			    {
			        break;
			    }
			
			    int deletedOrphanHistory = new Delete(userConnection)
			        .From("CompilationHistory")
			        .Where("Id")
			        .In(Column.Parameters(historyIds.Cast<object>().ToArray()))
			        .Execute();
			
			    totalCompilationHistory += deletedOrphanHistory;
			
			    if (deletedOrphanHistory <= 0)
			    {
			        break;
			    }
			}
			
			
			// Create detailed log message
			var logMessage = string.Format(
			    "Configuration Activity Log cleanup completed at {0:yyyy-MM-dd HH:mm:ss}<br>" +
			    "Cutoff date: {1:yyyy-MM-dd}<br><br>" +
			    "Deleted records:<br>" +
			    "- ConfActivityLog: {2}<br>" +
			    "- CompilationHistory: {3}<br>" +
			    "- SysFile: {4}<br>" +
			    "- LocalizationChangeLog: {5}<br><br>" +
			    "Total deleted: {6}",
			    DateTime.Now,
			    cutoff,
			    totalConfActivityLog,
			    totalCompilationHistory,
			    totalSysFile,
			    totalLocalizationChangeLog,
			    totalConfActivityLog + totalCompilationHistory + totalSysFile + totalLocalizationChangeLog
			);
			
			Set<string>("CleanupLogMessage", logMessage);
			
			return true;
		}

		#endregion

	}

	#endregion

}

