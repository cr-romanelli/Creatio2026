namespace CrtSegmentation
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
	using System.Runtime.Serialization;
	using Common.Logging;
	using Creatio.Segmentation;
	using Creatio.Segmentation.Exceptions;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;

	#region Class: SegmentService

	/// <summary>
	/// DataService endpoint of the Data Segmentation Engine.
	/// Exposes manual actualization, truncation and scheduled-actualization operations for a segment.
	/// Resolves the engine services from the platform via <see cref="ClassFactory"/>.
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class SegmentService : BaseService
	{

		#region Fields: Private

		private static readonly ILog _log = LogManager.GetLogger("Segmentation");

		#endregion

		#region Methods: Public

		/// <summary>
		/// Recalculates segment membership synchronously: evaluates the entry/exit filters and
		/// applies the resulting changes to the segment data table.
		/// </summary>
		/// <param name="segmentId">Unique identifier of the segment to actualize.</param>
		/// <param name="excludeCurrentMembersBeforeActualization">When <c>true</c>, soft-deletes all
		/// current members (sets <c>RemovedOn</c> on every active row) and then re-populates the segment
		/// from scratch using the current entry filter. When <c>false</c> or omitted, the segment is
		/// actualized incrementally (current behavior).</param>
		/// <returns>
		/// An <see cref="ActualizeResponse"/> with the membership-change statistics on success,
		/// or with <c>Success = false</c> and an error message on failure.
		/// </returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public ActualizeResponse Actualize(Guid segmentId, bool excludeCurrentMembersBeforeActualization) {
			try {
				var service = ClassFactory.Get<ISegmentActualizationService>(
					new ConstructorArgument("userConnection", UserConnection));
				var result = service.Actualize(segmentId, excludeCurrentMembersBeforeActualization);
				return new ActualizeResponse {
					Success = true,
					SegmentId = result.SegmentId,
					Status = result.Status,
					StartedOn = result.StartedOn,
					CompletedOn = result.CompletedOn,
					RecordsRestored = result.RecordsRestored,
					RecordsInserted = result.RecordsInserted,
					RecordsRemoved = result.RecordsRemoved,
					MissingRecordsCleaned = result.MissingRecordsCleaned,
					TotalMembers = result.TotalMembers,
					TotalActiveMembers = result.TotalActiveMembers
				};
			} catch (SegmentNotFoundException ex) {
				_log.Error(string.Format("Actualize failed: segment {0} not found", segmentId), ex);
				return new ActualizeResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("Segment {0} not found", segmentId)
				};
			} catch (FilterEvaluationException ex) {
				_log.Error(string.Format("Actualize failed: filter evaluation error for segment {0}", segmentId), ex);
				return new ActualizeResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("Filter evaluation error: {0}", ex.Message)
				};
			} catch (Exception ex) {
				_log.Error(string.Format("Actualize failed for segment {0}", segmentId), ex);
				return new ActualizeResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("Actualization failed: {0}", ex.Message)
				};
			}
		}

		/// <summary>
		/// Removes all membership records from the segment's data table (resets the segment to empty).
		/// </summary>
		/// <param name="segmentId">Unique identifier of the segment to truncate.</param>
		/// <returns>
		/// A <see cref="TruncateResponse"/> with the deleted row count on success,
		/// or with <c>Success = false</c> and an error message on failure.
		/// </returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public TruncateResponse Truncate(Guid segmentId) {
			try {
				var service = ClassFactory.Get<ISegmentTruncateService>(
					new ConstructorArgument("userConnection", UserConnection));
				var result = service.Truncate(segmentId);
				return new TruncateResponse {
					Success = true,
					SegmentId = result.SegmentId,
					Status = "truncated",
					RowsDeleted = result.RowsDeleted
				};
			} catch (SegmentNotFoundException ex) {
				_log.Error(string.Format("Truncate failed: segment {0} not found", segmentId), ex);
				return new TruncateResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("Segment {0} not found", segmentId)
				};
			} catch (Exception ex) {
				_log.Error(string.Format("Truncate failed for segment {0}", segmentId), ex);
				return new TruncateResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("Truncate failed: {0}", ex.Message)
				};
			}
		}

		/// <summary>
		/// Queues an asynchronous (background) actualization of the segment via the Quartz scheduler.
		/// Returns immediately; the actualization runs out of process.
		/// </summary>
		/// <param name="segmentId">Unique identifier of the segment to actualize.</param>
		/// <param name="excludeCurrentMembersBeforeActualization">When <c>true</c>, the scheduled
		/// actualization soft-deletes all current members and then re-populates the segment from scratch
		/// using the current entry filter. When <c>false</c> or omitted, the segment is actualized
		/// incrementally.</param>
		/// <returns>
		/// A <see cref="ScheduleActualizationResponse"/> indicating whether the job was queued.
		/// </returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public ScheduleActualizationResponse ScheduleActualization(Guid segmentId,
				bool excludeCurrentMembersBeforeActualization) {
			try {
				var jobManager = ClassFactory.Get<ISegmentActualizationJobManager>();
				jobManager.ScheduleActualization(segmentId, excludeCurrentMembersBeforeActualization);
				return new ScheduleActualizationResponse {
					Success = true,
					SegmentId = segmentId
				};
			} catch (Exception ex) {
				_log.Error(string.Format("ScheduleActualization failed for segment {0}", segmentId), ex);
				return new ScheduleActualizationResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("ScheduleActualization failed: {0}", ex.Message)
				};
			}
		}

		/// <summary>
		/// Cancels a pending or running scheduled actualization job for the segment.
		/// </summary>
		/// <param name="segmentId">Unique identifier of the segment whose job should be removed.</param>
		/// <returns>
		/// An <see cref="UnscheduleActualizationResponse"/> indicating whether the job was removed.
		/// </returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public UnscheduleActualizationResponse UnscheduleActualization(Guid segmentId) {
			try {
				var jobManager = ClassFactory.Get<ISegmentActualizationJobManager>();
				jobManager.UnscheduleActualization(segmentId);
				return new UnscheduleActualizationResponse {
					Success = true,
					SegmentId = segmentId
				};
			} catch (InvalidOperationException ex) {
				_log.Warn(string.Format("DeleteActualization rejected for segment {0}", segmentId), ex);
				return new UnscheduleActualizationResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = ex.Message
				};
			} catch (Exception ex) {
				_log.Error(string.Format("DeleteActualization failed for segment {0}", segmentId), ex);
				return new UnscheduleActualizationResponse {
					Success = false,
					SegmentId = segmentId,
					ErrorMessage = string.Format("DeleteActualization failed: {0}", ex.Message)
				};
			}
		}

		#endregion

	}

	#endregion

	#region Class: ActualizeResponse

	/// <summary>
	/// Response contract for <see cref="SegmentService.Actualize"/>.
	/// </summary>
	[DataContract]
	public class ActualizeResponse
	{

		/// <summary>Indicates whether the actualization completed successfully.</summary>
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		/// <summary>Human-readable error message; present only when <see cref="Success"/> is <c>false</c>.</summary>
		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		/// <summary>Identifier of the actualized segment.</summary>
		[DataMember(Name = "segmentId")]
		public Guid SegmentId { get; set; }

		/// <summary>Final status of the operation (e.g. "completed", "skipped").</summary>
		[DataMember(Name = "status", EmitDefaultValue = false)]
		public string Status { get; set; }

		/// <summary>UTC timestamp when the actualization started.</summary>
		[DataMember(Name = "startedOn", EmitDefaultValue = false)]
		public DateTime? StartedOn { get; set; }

		/// <summary>UTC timestamp when the actualization completed.</summary>
		[DataMember(Name = "completedOn", EmitDefaultValue = false)]
		public DateTime? CompletedOn { get; set; }

		/// <summary>Number of previously-removed members restored during actualization.</summary>
		[DataMember(Name = "recordsRestored", EmitDefaultValue = false)]
		public int? RecordsRestored { get; set; }

		/// <summary>Number of new members inserted during actualization.</summary>
		[DataMember(Name = "recordsInserted", EmitDefaultValue = false)]
		public int? RecordsInserted { get; set; }

		/// <summary>Number of members marked as removed during actualization.</summary>
		[DataMember(Name = "recordsRemoved", EmitDefaultValue = false)]
		public int? RecordsRemoved { get; set; }

		/// <summary>
		/// Number of membership rows deleted because their source record no longer exists
		/// in the target entity.
		/// </summary>
		[DataMember(Name = "missingRecordsCleaned", EmitDefaultValue = false)]
		public int? MissingRecordsCleaned { get; set; }

		/// <summary>Total number of member rows in the segment data table after actualization.</summary>
		[DataMember(Name = "totalMembers", EmitDefaultValue = false)]
		public int? TotalMembers { get; set; }

		/// <summary>Number of currently active members (RemovedOn is null) after actualization.</summary>
		[DataMember(Name = "totalActiveMembers", EmitDefaultValue = false)]
		public int? TotalActiveMembers { get; set; }

	}

	#endregion

	#region Class: TruncateResponse

	/// <summary>
	/// Response contract for <see cref="SegmentService.Truncate"/>.
	/// </summary>
	[DataContract]
	public class TruncateResponse
	{

		/// <summary>Indicates whether the truncation completed successfully.</summary>
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		/// <summary>Human-readable error message; present only when <see cref="Success"/> is <c>false</c>.</summary>
		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		/// <summary>Identifier of the truncated segment.</summary>
		[DataMember(Name = "segmentId")]
		public Guid SegmentId { get; set; }

		/// <summary>Operation status (e.g. "truncated").</summary>
		[DataMember(Name = "status", EmitDefaultValue = false)]
		public string Status { get; set; }

		/// <summary>Number of membership rows deleted from the segment data table.</summary>
		[DataMember(Name = "rowsDeleted", EmitDefaultValue = false)]
		public long RowsDeleted { get; set; }

	}

	#endregion

	#region Class: ScheduleActualizationResponse

	/// <summary>
	/// Response contract for <see cref="SegmentService.ScheduleActualization"/>.
	/// </summary>
	[DataContract]
	public class ScheduleActualizationResponse
	{

		/// <summary>Indicates whether the actualization job was queued successfully.</summary>
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		/// <summary>Human-readable error message; present only when <see cref="Success"/> is <c>false</c>.</summary>
		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		/// <summary>Identifier of the segment whose actualization was queued.</summary>
		[DataMember(Name = "segmentId")]
		public Guid SegmentId { get; set; }

	}

	#endregion

	#region Class: UnscheduleActualizationResponse

	/// <summary>
	/// Response contract for <see cref="SegmentService.UnscheduleActualization"/>.
	/// </summary>
	[DataContract]
	public class UnscheduleActualizationResponse
	{

		/// <summary>Indicates whether the scheduled job was removed successfully.</summary>
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		/// <summary>Human-readable error message; present only when <see cref="Success"/> is <c>false</c>.</summary>
		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		/// <summary>Identifier of the segment whose job was removed.</summary>
		[DataMember(Name = "segmentId")]
		public Guid SegmentId { get; set; }

	}

	#endregion

}

