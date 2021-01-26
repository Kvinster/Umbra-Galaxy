using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class LeaderboardController {
		readonly LeaderboardState _state;

		public List<LeaderboardEntry> Entries => _state.Entries;

		public LeaderboardController() {
			_state = LeaderboardState.GetInstance();
		}

		public void AddEntry(string profileName, int highscore) {
			_state.AddEntry(profileName, highscore);
		}

		public void Clear() {
			_state.Clear();
		}
	}
}
