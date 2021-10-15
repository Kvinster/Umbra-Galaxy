using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly LeaderboardWindow _leaderboardWindow;
		readonly SettingsScreen    _settingsScreen;

		public MainMenuManager(MainScreen mainScreen, LeaderboardWindow leaderboardWindow,
			SettingsScreen settingsScreen) {
			_mainScreen        = mainScreen;
			_leaderboardWindow = leaderboardWindow;
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

		void HideAll() {
			_mainScreen.Hide();
			_leaderboardWindow.Hide();
			_settingsScreen.Hide();
		}
	}
}
