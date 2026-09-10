namespace Creatio.Copilot
{
	using System;
	using Terrasoft.Core.Factories;

	#region Interface: IKnwServiceProblemHelper

	/// <summary>
	/// Provides helper methods for extracting and handling service problem exceptions.
	/// </summary>
	internal interface IKnwServiceProblemHelper
	{

		#region Methods: Public

		/// <summary>
		/// Finds the first <see cref="KnwServiceProblemException"/> in the exception chain.
		/// </summary>
		/// <param name="exception">The exception to inspect.</param>
		/// <returns>The problem exception if found; otherwise, <c>null</c>.</returns>
		KnwServiceProblemException FindProblemException(Exception exception);

		/// <summary>
		/// Logs technical details of an exception in a structured format.
		/// </summary>
		/// <param name="context">The context message for the log entry.</param>
		/// <param name="exception">The exception to log.</param>
		void LogTechnicalError(string context, Exception exception);

		/// <summary>
		/// Determines whether an upload failure is terminal and should stop processing.
		/// </summary>
		/// <param name="exception">The exception to inspect.</param>
		/// <returns><c>true</c> if the failure is terminal; otherwise, <c>false</c>.</returns>
		bool IsTerminalUploadFailure(Exception exception);

		/// <summary>
		/// Sets user-facing failure details on a session when possible.
		/// </summary>
		/// <param name="sessionManager">Session manager instance.</param>
		/// <param name="session">Current session record.</param>
		/// <param name="exception">Original exception.</param>
		/// <param name="fallbackMessage">Fallback message used when problem details are not available.</param>
		/// <param name="context">Context message for error logging if persistence fails.</param>
		void TrySetSessionFailureDetails(IKnwIndexingSessionManager sessionManager,
			KnwIndexingSessionRecord session, Exception exception, string fallbackMessage, string context);

		#endregion

	}

	#endregion

	#region Class: KnwServiceProblemHelper

	/// <summary>
	/// Default implementation of <see cref="IKnwServiceProblemHelper"/>.
	/// </summary>
	[DefaultBinding(typeof(IKnwServiceProblemHelper))]
	internal class KnwServiceProblemHelper : IKnwServiceProblemHelper
	{

		#region Constants: Private

		private const int TooManyRequestsStatusCode = 429;

		#endregion

		#region Methods: Public

		/// <inheritdoc />
		public KnwServiceProblemException FindProblemException(Exception exception) {
			for (Exception current = exception; current != null; current = current.InnerException) {
				if (current is KnwServiceProblemException problemException) {
					return problemException;
				}
			}
			return null;
		}

		/// <inheritdoc />
		public void LogTechnicalError(string context, Exception exception) {
			KnwServiceProblemException problemException = FindProblemException(exception);
			if (problemException == null) {
				KnwLibUtils.Logger.Error(
					$"{context}. ExceptionType={exception?.GetType().Name}, Message={exception?.Message}",
					exception);
				return;
			}
			KnwLibUtils.Logger.Error($"{context}. Code={problemException.Code}, StatusCode={problemException.StatusCode}, " +
				$"Detail={problemException.Detail}, ParamsJson={problemException.ParamsJson}",
				exception);
		}

		/// <inheritdoc />
		public bool IsTerminalUploadFailure(Exception exception) {
			KnwServiceProblemException problemException = FindProblemException(exception);
			if (problemException == null) {
				return false;
			}
			return problemException.StatusCode >= 400 &&
				problemException.StatusCode <= 499 &&
				problemException.StatusCode != TooManyRequestsStatusCode;
		}

		/// <inheritdoc />
		public void TrySetSessionFailureDetails(IKnwIndexingSessionManager sessionManager,
				KnwIndexingSessionRecord session, Exception exception, string fallbackMessage, string context) {
			if (sessionManager == null || session == null || session.Id == Guid.Empty) {
				return;
			}
			try {
				KnwServiceProblemException problemException = FindProblemException(exception);
				sessionManager.SetSessionFailureDetails(session.Id, problemException?.Code,
					problemException?.Detail ?? fallbackMessage ?? exception?.Message);
			} catch (Exception setDetailsException) {
				KnwLibUtils.Logger.Error($"{context}. SessionId={session.SessionId}, Message={setDetailsException.Message}",
					setDetailsException);
			}
		}

		#endregion

	}

	#endregion

}

