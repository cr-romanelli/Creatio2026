namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using Common.Logging;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;

	/// <summary>
	/// APD-1464 live-wiring + APD-1530 resume routing: the <see cref="IEventSender"/>
	/// entry point. This is a pure <b>router</b> — it does no HTTP itself; it chooses
	/// the transport by trigger mode and delegates:
	/// <list type="bullet">
	/// <item>Start-mode triggers (high-volume entity changes) are buffered into the
	/// process-wide <see cref="OutboundEventBatcher"/> and flushed as bulk requests
	/// through <see cref="BulkEventSender{TData}"/> — with throttle (R4), backoff +
	/// jitter (R1) and <c>Retry-After</c> floor (R2).</item>
	/// <item>Resume-mode triggers (e.g. <c>creatio_process_completion</c>) resume a
	/// suspended workflow on the control plane and are rejected by the bulk ingress
	/// with <c>RESUME_NOT_SUPPORTED_IN_BULK</c>. They are low-volume and go through
	/// <see cref="SingleEventSender"/> to the single-event endpoint.</item>
	/// </list>
	/// Keeping the HTTP in the two dedicated transports (both
	/// <see cref="AIPlatformHttpSenderBase"/>) and the routing here gives each class a
	/// single responsibility.
	/// </summary>
	[DefaultBinding(typeof(IEventSender))]
	internal class EventSender : IEventSender
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private readonly UserConnection _userConnection;

		public EventSender(UserConnection userConnection) {
			_userConnection = userConnection;
		}

		public void Send<TData>(EventOccurrenceRequest<TData> request) {
			if (request == null) {
				throw new ArgumentNullException(nameof(request));
			}
			if (string.IsNullOrWhiteSpace(request.TriggerCode)) {
				Log.Warn("EventOccurrenceRequest.TriggerCode is empty. Skipping send.");
				return;
			}
			// Resume-mode events resume a suspended workflow (correlated by id) and the
			// control-plane bulk ingress rejects them with RESUME_NOT_SUPPORTED_IN_BULK.
			// Deliver them through the single-event transport; only start-mode events
			// are batched onto the bulk route.
			if (Constants.TriggerModes.IsResume(request.TriggerCode)) {
				ClassFactory.Get<SingleEventSender>(
					new ConstructorArgument("userConnection", _userConnection)).Send(request);
				return;
			}
			OutboundEventBatcher.Current.Enqueue(request, _userConnection);
		}
	}
}

