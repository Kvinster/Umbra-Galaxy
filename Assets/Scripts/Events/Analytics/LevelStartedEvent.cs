using System.Collections.Generic;
using STP.Service;

namespace STP.Events.Analytics {
	public readonly struct LevelStartedEvent : IAnalyticsEvent {
		public string                     EventName => "level_started";
		public Dictionary<string, object> EventArgs { get; }

		public LevelStartedEvent(int levelIndex) =>
			EventArgs = new Dictionary<string, object> {
				{ "level_index", levelIndex },
			};
	}
}