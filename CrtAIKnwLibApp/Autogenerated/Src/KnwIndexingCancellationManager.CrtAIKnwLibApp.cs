namespace Creatio.Copilot
{
	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using System.Threading.Tasks;
	using Terrasoft.Common.Messaging;
	using Terrasoft.Core.Factories;

	#region Class: KnwIndexingCancellationContext

	/// <summary>
	/// Represents a context for knowledge indexing cancellation.
	/// </summary>
	internal interface IKnwIndexingCancellationContext : IDisposable
	{

		#region Properties: Internal

		/// <summary>
		/// Gets the cancellation token for the context.
		/// </summary>
		CancellationToken CancellationToken { get; }

		#endregion

	}

	#endregion

	#region Interface: IKnwIndexingCancellationManager

	/// <summary>
	/// Provides methods to manage cancellation of knowledge indexing operations.
	/// </summary>
	internal interface IKnwIndexingCancellationManager
	{

		#region Methods: Internal

		/// <summary>
		/// Registers a new cancellation context for the specified indexing session record ID.
		/// </summary>
		/// <param name="sessionRecordId">The unique identifier of the indexing session record.</param>
		/// <returns>An <see cref="IKnwIndexingCancellationContext"/> instance associated with the session record.</returns>
		IKnwIndexingCancellationContext RegisterCancellation(Guid sessionRecordId);

		/// <summary>
		/// Cancels and disposes the cancellation token source associated with the specified knowledge source ID.
		/// </summary>
		/// <param name="knwSourceId">The unique identifier of the knowledge source.</param>
		void Cancel(Guid knwSourceId);

		#endregion

	}

	#endregion

	#region Class: KnwIndexingCancellationManager

	/// <summary>
	/// Handles cancellation management for knowledge indexing sessions.
	/// </summary>
	[DefaultBinding(typeof(IKnwIndexingCancellationManager))]
	[DefaultBinding(typeof(INotificationHandler<CancelKnwSessionIndexingNotification>))]
	[DistributedNotification]
	internal class KnwIndexingCancellationManager : IKnwIndexingCancellationManager, INotificationHandler<KnwIndexingCancellationManager.CancelKnwSessionIndexingNotification>
	{

		#region Class: CancelKnwSessionIndexingNotification

		/// <summary>
		/// Notification for cancelling a knowledge indexing session by its local session record ID.
		/// </summary>
		internal class CancelKnwSessionIndexingNotification : INotification
		{

			#region Properties: Public

			/// <summary>
			/// Gets the unique identifier of the indexing session record.
			/// </summary>
			public Guid KnwIndexingSessionId { get; }

			#endregion

			#region Constructors: Public

			/// <summary>
			/// Initializes a new instance of the <see cref="CancelKnwSessionIndexingNotification"/> class.
			/// </summary>
			/// <param name="knwIndexingSessionId">Knowledge indexing session record identifier.</param>
			public CancelKnwSessionIndexingNotification(Guid knwIndexingSessionId) {
				KnwIndexingSessionId = knwIndexingSessionId;
			}

			#endregion

		}

		#endregion

		#region Class: KnwIndexingCancellationContext

		/// <summary>
		/// Represents a context for a single knowledge indexing cancellation operation.
		/// </summary>
		private class KnwIndexingCancellationContext : IKnwIndexingCancellationContext
		{
			/// <inheritdoc/>
			public CancellationToken CancellationToken { get; }

			private readonly Action _onDispose;

			/// <summary>
			/// Initializes a new instance of the <see cref="KnwIndexingCancellationContext"/> class.
			/// </summary>
			/// <param name="cancellationTokenSource">The cancellation token source.</param>
			/// <param name="onDispose">The action to perform on dispose.</param>
			public KnwIndexingCancellationContext(CancellationTokenSource cancellationTokenSource, Action onDispose) {
				CancellationToken = cancellationTokenSource.Token;
				_onDispose = onDispose;
			}

			/// <inheritdoc/>
			public void Dispose() {
				_onDispose();
			}
		}

		#endregion

		#region Fields: Private

		private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellationTokenSources =
			new ConcurrentDictionary<Guid, CancellationTokenSource>();
		private readonly IKnwIndexingSessionManager _sessionManager;

		#endregion

		#region Constructors: Public

		public KnwIndexingCancellationManager(IKnwIndexingSessionManager sessionManager, IKnwSourceManager sourceManager) {
			_sessionManager = sessionManager;
		}

		#endregion

		#region Methods: Private

		private static void CancelToken(CancellationTokenSource cts) {
			try {
				cts.Cancel();
			} catch (ObjectDisposedException) {
				// Already disposed by another thread — safe to ignore.
			}
			try {
				cts.Dispose();
			} catch (ObjectDisposedException) {
				// Already disposed by another thread — safe to ignore.
			}
		}

		private void CancelInternal(KnwIndexingSessionRecord session, bool fromNotification) {
			if (!_cancellationTokenSources.TryRemove(session.Id, out CancellationTokenSource cts)) {
				if (fromNotification) {
					return;
				}
				var notification = new CancelKnwSessionIndexingNotification(session.Id);
				MessageHub.Instance.Publish(notification);
				return;
			}
			CancelToken(cts);
			_sessionManager.CancelSession(session, CancellationToken.None);
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc/>
		public IKnwIndexingCancellationContext RegisterCancellation(Guid sessionRecordId) {
			var cts = new CancellationTokenSource();
			_cancellationTokenSources[sessionRecordId] = cts;
			return new KnwIndexingCancellationContext(cts, () => {
				if (_cancellationTokenSources.TryRemove(sessionRecordId, out _)) {
					CancelToken(cts);
				}
			});
		}

		/// <inheritdoc />
		public void Cancel(Guid knwSourceId) {
			KnwIndexingSessionRecord session = _sessionManager.SyncLatestIndexingStatus(knwSourceId,
				CancellationToken.None);
			if (session == null) {
				return;
			}
			if (session.State.IsTerminal()) {
				return;
			}
			if (session.State == KnwIndexingStatusEnum.Indexing) {
				_sessionManager.CancelSession(session, CancellationToken.None);
				return;
			}
			CancelInternal(session, false);
		}

		/// <inheritdoc cref="INotificationHandler{TNotification}" />
		Task INotificationHandler<CancelKnwSessionIndexingNotification>.HandleAsync(
				CancelKnwSessionIndexingNotification notification, CancellationToken cancellationToken) {
			KnwIndexingSessionRecord session = _sessionManager.GetById(notification.KnwIndexingSessionId);
			if (session == null) {
				return Task.CompletedTask;
			}
			CancelInternal(session, true);
			return Task.CompletedTask;
		}

		#endregion

	}

	#endregion

}

