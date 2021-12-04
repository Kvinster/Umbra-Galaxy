using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly SettingsScreen    _settingsScreen;
		readonly LeaderboardScreen _leaderboardScreen;

		public MainMenuManager(MainScreen mainScreen, SettingsScreen settingsScreen, LeaderboardScreen leaderboardScreen) {
			_mainScreen        = mainScreen;
			_settingsScreen    = settingsScreen;
			_leaderboardScreen = leaderboardScreen;
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
			_leaderboardScreen.Show();
		}

		void HideAll() {
			_mainScreen.Hide();
			_settingsScreen.Hide();
			_leaderboardScreen.Hide();
		}
	}
}
