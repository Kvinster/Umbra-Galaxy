using System.Collections.Generic;

namespace STP.Service {
	public interface IAnalyticsEvent {
		public string EventName { get; }
		public Dictionary<string, object> EventArgs { get; }
	}
}