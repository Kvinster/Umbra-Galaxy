using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly SettingsScreen    _settingsScreen;
		readonly LeaderboardWindow _leaderboardWindow;

		public MainMenuManager(MainScreen mainScreen, SettingsScreen settingsScreen, LeaderboardWindow leaderboardWindow) {
			_mainScreen        = mainScreen;
			_settingsScreen    = settingsScreen;
			_leaderboardWindow = leaderboardWindow;
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

		void HideAll() {
			_mainScreen.Hide();
			_settingsScreen.Hide();
			_leaderboardWindow.Hide();
		}
	}
}
