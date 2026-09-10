namespace CrtAIAgenticBridgeApp.Runtime
{
	using System.Collections.Generic;
	using Terrasoft.Core.Process;

	#region Interface: ILookupResponseEnricher

	/// <summary>
	/// Enriches a process result-parameter set so lookup values carry both the raw identifier and a
	/// human-readable display value. Mirrors the platform's
	/// <c>CopilotParametrizedActionResponseProvider</c> logic, retargeted from Copilot action metadata
	/// onto <see cref="ProcessSchemaParameter"/> so it can run against any agentic process result.
	/// </summary>
	public interface ILookupResponseEnricher
	{

		#region Methods: Public

		/// <summary>
		/// Returns a copy of <paramref name="resultValues"/> in which every lookup-typed value (including
		/// lookups nested inside composite-object / object-list parameters) is wrapped as an object so the
		/// structured shape never drifts. With enrichment enabled the wrapper carries a resolved label
		/// (<c>{ value, displayValue }</c>); with enrichment disabled it carries the identifier only
		/// (<c>{ value }</c>) — the entity reads are skipped, but the wrapper is unconditional, so a lookup
		/// is never emitted as a bare identifier. Non-lookup values are passed through untouched.
		/// </summary>
		/// <param name="resultValues">The raw result parameter values read from the completed process.</param>
		/// <param name="parameters">The process result parameters providing the type metadata.</param>
		/// <returns>The enriched values (lookups wrapped); display labels added when enrichment is enabled.</returns>
		IDictionary<string, object> Enrich(IDictionary<string, object> resultValues,
			IEnumerable<ProcessSchemaParameter> parameters);

		#endregion

	}

	#endregion

}

