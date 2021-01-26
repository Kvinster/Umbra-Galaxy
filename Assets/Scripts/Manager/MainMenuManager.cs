using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen        _mainScreen;
		readonly LoadWindow        _loadWindow;
		readonly LeaderboardWindow _leaderboardWindow;

		public MainMenuManager(MainScreen mainScreen, LoadWindow loadWindow, LeaderboardWindow leaderboardWindow) {
			_mainScreen        = mainScreen;
			_loadWindow        = loadWindow;
			_leaderboardWindow = leaderboardWindow;
		}

		public void Init() {
			HideAll();
			_mainScreen.Show();
		}

		public void ShowLoad() {
			HideAll();
			_loadWindow.Show();
		}

		public void ShowMain() {
			HideAll();
			_mainScreen.Show();
		}

		public void ShowLeaderboard() {
			HideAll();
			_leaderboardWindow.Show();
		}

		void HideAll() {
			_mainScreen.Hide();
			_loadWindow.Hide();
			_leaderboardWindow.Hide();
		}
	}
}
