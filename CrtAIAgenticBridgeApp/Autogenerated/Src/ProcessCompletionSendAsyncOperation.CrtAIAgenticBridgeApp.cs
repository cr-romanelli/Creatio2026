namespace CrtAIAgenticBridgeApp.Runtime
{
	using System;
	using Common.Logging;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.Tasks;

	/// <summary>
	/// MessagePack-safe argument for <see cref="ProcessCompletionSendAsyncOperation"/>: carries
	/// only the trigger code and the already-serialized event body so the process-completion
	/// resume send (APD-2470) can cross the background-task boundary without serializing the
	/// typed <c>EventOccurrenceRequest&lt;ProcessCompletionData&gt;</c> (whose
	/// <c>IDictionary&lt;string, object&gt;</c> result parameters break that transport).
	/// <para>
	/// <see cref="JsonBody"/> may contain arbitrary process result parameters — never log it.
	/// </para>
	/// </summary>
	internal sealed class ProcessCompletionSendArgs
	{
		public string TriggerCode { get; set; }
		public string JsonBody { get; set; }
	}

	/// <summary>
	/// APD-2470: background task that delivers the process-completion resume event off the
	/// user-facing process-resume thread. Runs with its own system <see cref="UserConnection"/>
	/// (injected via <see cref="IUserConnectionRequired"/>) rather than reusing the request
	/// connection, which may already be disposed by the time the task runs. The actual HTTP —
	/// including the bounded retry/back-off — happens in <see cref="SingleEventSender"/>; failures
	/// are logged and swallowed so the background worker never propagates.
	/// <para>
	/// Intentionally NOT gated by <c>EnableAIPlatformOutboundTriggerDelivery</c> (unlike
	/// <c>DispatchEntityChangeAsyncOperation</c>): that toggle governs the high-volume
	/// entity-change outbound lane, whereas resuming a suspended agentic process is core
	/// behavior that must run regardless. The originating hook was never gated either.
	/// </para>
	/// </summary>
	internal class ProcessCompletionSendAsyncOperation
		: IBackgroundTask<ProcessCompletionSendArgs>, IUserConnectionRequired
	{
		private static readonly ILog Log = LogManager.GetLogger("CrtAIAgenticBridgeApp");

		private UserConnection _userConnection;

		public void Run(ProcessCompletionSendArgs arguments) {
			try {
				if (arguments == null
						|| string.IsNullOrWhiteSpace(arguments.TriggerCode)
						|| string.IsNullOrWhiteSpace(arguments.JsonBody)) {
					return;
				}
				var sender = ClassFactory.Get<SingleEventSender>(
					new ConstructorArgument("userConnection", _userConnection));
				sender.SendSerialized(arguments.TriggerCode, arguments.JsonBody);
			} catch (Exception ex) {
				Log.Error("Failed to send process completion resume event asynchronously.", ex);
			}
		}

		public void SetUserConnection(UserConnection userConnection) {
			_userConnection = userConnection;
		}
	}
}

