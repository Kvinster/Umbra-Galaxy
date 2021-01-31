using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class LeaderboardController : BaseStateController {
		readonly LeaderboardState _state;
		readonly GameState        _gameState;

		public List<LeaderboardEntry> Entries => _state.Entries;

		public LeaderboardController(GameState gameState) {
			_state     = gameState.LeaderboardState;
			_gameState = gameState;
		}

		public void AddEntry(string profileName, int highscore) {
			_state.AddEntry(profileName, highscore);
			_gameState.Save();
		}

		public void Clear() {
			_state.Clear();
		}
	}
}
