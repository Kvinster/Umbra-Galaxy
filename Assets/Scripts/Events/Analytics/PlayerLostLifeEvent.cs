using System.Collections.Generic;
using STP.Service;

namespace STP.Manager {
	public struct PlayerLostLifeEvent : IAnalyticsEvent {
		public string                     EventName => "player_lost_life";
		public Dictionary<string, object> EventArgs => new Dictionary<string, object>();
	}
}