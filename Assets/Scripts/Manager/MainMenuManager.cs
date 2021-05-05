using STP.Behaviour.MainMenu;
using STP.Core;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly LeaderboardWindow _leaderboardWindow;
		readonly LevelsScreen      _levelsScreen;
		readonly SettingsScreen    _settingsScreen;

		public MainMenuManager(MainScreen mainScreen, LeaderboardWindow leaderboardWindow, LevelsScreen levelsScreen, SettingsScreen settingsScreen) {
			_mainScreen        = mainScreen;
			_leaderboardWindow = leaderboardWindow;
			_levelsScreen      = levelsScreen;
			_settingsScreen    = settingsScreen;
		}

		public void Init() {
			ShowMain();
		}

		public void ShowMain() {
			HideAll();
			_mainScreen.Show();
		}

		public void ShowSettingsScreen() {
			HideAll();
			_settingsScreen.Show();
		}

		public void ShowLeaderboard() {
			HideAll();
			_leaderboardWindow.Show();
		}

		public void ShowLevelsScreen() {
			HideAll();
			_levelsScreen.Show();
		}

		void HideAll() {
			_mainScreen.Hide();
			_leaderboardWindow.Hide();
			_levelsScreen.Hide();
			_settingsScreen.Hide();
		}
	}
}
