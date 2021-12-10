using System.Collections.Generic;
using STP.Service;

namespace STP.Manager {
	public readonly struct UnhandledExceptionEvent : IAnalyticsEvent {
		public string                     EventName => "unhandled_exception";
		public Dictionary<string, object> EventArgs { get; }

		public UnhandledExceptionEvent(string condition, string stackTrace) {
			EventArgs = new Dictionary<string, object> {
				{ "condition", condition},
				{ "stackTrace", stackTrace},
			};
		}
	}
}