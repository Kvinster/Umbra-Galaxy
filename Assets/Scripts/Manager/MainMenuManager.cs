using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly LeaderboardWindow _leaderboardWindow;
		readonly LevelsScreen      _levelsScreen;
		readonly SettingsScreen    _settingsScreen;
		readonly UpgradesScreen    _upgradesScreen;

		public MainMenuManager(MainScreen mainScreen, LeaderboardWindow leaderboardWindow, LevelsScreen levelsScreen,
			SettingsScreen settingsScreen, UpgradesScreen upgradesScreen) {
			_mainScreen        = mainScreen;
			_leaderboardWindow = leaderboardWindow;
			_levelsScreen      = levelsScreen;
			_settingsScreen    = settingsScreen;
			_upgradesScreen    = upgradesScreen;
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

		public void ShowUpgradesScreen() {
			HideAll();
			_upgradesScreen.Show();
		}

		void HideAll() {
			_mainScreen.Hide();
			_leaderboardWindow.Hide();
			_levelsScreen.Hide();
			_settingsScreen.Hide();
			_upgradesScreen.Hide();
		}
	}
}
