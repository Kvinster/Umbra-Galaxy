using System.Collections.Generic;
using STP.Service;

namespace STP.Manager {
	public struct PlayerDiedEvent : IAnalyticsEvent {
		public string                     EventName => "player_died";
		public Dictionary<string, object> EventArgs => new Dictionary<string, object>();
	}
}