namespace Creatio.Copilot
{
	using Terrasoft.Core;
	using Terrasoft.Core.Entities;
	using Terrasoft.Core.Entities.Events;
	using Terrasoft.Core.Factories;

	#region Class: KnwFeatureChangeEventListener

	/// <summary>
	/// Listens for changes to the <c>Feature</c> entity and triggers actions when specific features are modified.
	/// </summary>
	[EntityEventListener(SchemaName = "Feature")]
	public class KnwFeatureChangeEventListener : BaseEntityEventListener
	{

		#region Constants: Private

		private const string EnableKnwRetrievalFeatureCode = "GenAIFeatures.EnableKnwRetrieval";

		#endregion

		#region Methods: Private

		private void OnFeatureChanged(object source) {
			var featureEntity = (Entity)source;
			UserConnection userConnection = featureEntity.UserConnection;
			var featureCode = featureEntity.GetTypedColumnValue<string>("Code");
			if (featureCode == EnableKnwRetrievalFeatureCode) {
				var appSchedulerWrapper = ClassFactory.Get<IAppSchedulerWraper>();
				var indexationStatusJob = ClassFactory.Get<IKnwSrcIndexationStatusJobActualizer>();
				indexationStatusJob.Register(userConnection, appSchedulerWrapper);
				var deferredIndexationJob = ClassFactory.Get<IKnwDeferredIndexationJobDispatcher>();
				deferredIndexationJob.Register(userConnection, appSchedulerWrapper);
			}
		}

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public override void OnInserted(object sender, EntityAfterEventArgs e) {
			base.OnInserted(sender, e);
			OnFeatureChanged(sender);
		}

		/// <inheritdoc />
		public override void OnUpdated(object sender, EntityAfterEventArgs e) {
			base.OnUpdated(sender, e);
			OnFeatureChanged(sender);
		}

		/// <inheritdoc />
		public override void OnDeleted(object sender, EntityAfterEventArgs e) {
			base.OnDeleted(sender, e);
			OnFeatureChanged(sender);
		}

		#endregion

	}

	#endregion

}

