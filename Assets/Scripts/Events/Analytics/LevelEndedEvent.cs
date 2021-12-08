using System.Collections.Generic;
using STP.Service;

namespace STP.Events.Analytics {
	public readonly struct LevelEndedEvent : IAnalyticsEvent {
		public string                     EventName => "level_won";
		public Dictionary<string, object> EventArgs { get; }

		public LevelEndedEvent(int levelIndex) =>
			EventArgs = new Dictionary<string, object> {
				{ "level_index", levelIndex },
			};
	}
}