using System.Collections.Generic;
using STP.Service;

namespace STP.Manager {
	public struct ReturnToMenuEvent : IAnalyticsEvent {
		public string                     EventName => "return_to_menu";
		public Dictionary<string, object> EventArgs => new Dictionary<string, object>();
	}
}