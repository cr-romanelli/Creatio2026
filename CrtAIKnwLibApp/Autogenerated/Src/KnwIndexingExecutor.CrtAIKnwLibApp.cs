namespace Creatio.Copilot
{
	using Creatio.Messaging.MessageBus;
	using System;
	using Terrasoft.Common;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.ServiceBus;
	using Terrasoft.Core.Tasks;
	using Terrasoft.ServiceBus;

	#region Interface: IKnwIndexingRunner

	/// <summary>
	/// An interface for starting knowledge source indexing process.
	/// </summary>
	public interface IKnwIndexingRunner
	{

		#region Methods: Public

		/// <summary>
		/// Starts the indexing process for the specified knowledge source.
		/// </summary>
		/// <param name="knwSourceId">Knowledge source identifier.</param>
		/// <param name="userConnection">An instance of the <see cref="UserConnection"/> type to use.</param>
		void StartIndexing(Guid knwSourceId, UserConnection userConnection);

		/// <summary>
		/// Cancels the indexing process for the specified knowledge source.
		/// </summary>
		/// <param name="knwSourceId">Knowledge source identifier.</param>
		/// <param name="userConnection">An instance of the <see cref="UserConnection"/> type to use.</param>
		void CancelIndexing(Guid knwSourceId, UserConnection userConnection);

		#endregion

	}

	#endregion

	#region Class: KnwIndexingExecutor

	/// <summary>
	/// Executes the knowledge source indexing process and handles related commands.
	/// </summary>
	[DefaultBinding(typeof(IKnwIndexingRunner))]
	[DefaultBinding(typeof(KnwIndexingExecutor))]
	public class KnwIndexingExecutor : IKnwIndexingRunner, IMessageConsumer<KnwStartIndexingCommand>, IMessageConsumer<KnwCancelSessionCommand>
	{

		#region Methods: Public

		/// <inheritdoc cref="IMessageConsumer{TMessage,TContext}.Consume" />
		void IMessageConsumer<KnwStartIndexingCommand, IConsumingContext>.Consume(KnwStartIndexingCommand message,
				IConsumingContext context) {
			Task.StartNewWithUserConnection<KnwIndexingBackgroundTask, KnwStartIndexingCommand>(message);
		}

		/// <inheritdoc cref="IMessageConsumer{TMessage,TContext}.Consume" />
		void IMessageConsumer<KnwCancelSessionCommand, IConsumingContext>.Consume(KnwCancelSessionCommand message,
				IConsumingContext context) {
			Task.StartNewWithUserConnection<KnwCancelSessionBackgroundTask, KnwCancelSessionCommand>(message);
		}

		/// <inheritdoc cref="IKnwIndexingRunner.StartIndexing" />
		public void StartIndexing(Guid knwSourceId, UserConnection userConnection) {
			userConnection.CheckArgumentNull(nameof(userConnection));
			userConnection.GetMessageBus().Send(new KnwStartIndexingCommand {
				KnwSourceId = knwSourceId
			});
		}

		/// <inheritdoc cref="IKnwIndexingRunner.CancelIndexing" />
		public void CancelIndexing(Guid knwSourceId, UserConnection userConnection) {
			userConnection.CheckArgumentNull(nameof(userConnection));
			userConnection.GetMessageBus().Send(new KnwCancelSessionCommand {
				KnwSourceId = knwSourceId
			});

			#endregion

		}

		#endregion

	}
}
