using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly SettingsScreen    _settingsScreen;

		public MainMenuManager(MainScreen mainScreen, SettingsScreen settingsScreen) {
			_mainScreen        = mainScreen;
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
		}

		void HideAll() {
			_mainScreen.Hide();
			_settingsScreen.Hide();
		}
	}
}
