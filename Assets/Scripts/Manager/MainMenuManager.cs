using STP.Behaviour.MainMenu;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly MainScreen _mainScreen;
		readonly LoadWindow _loadWindow;

		public MainMenuManager(MainScreen mainScreen, LoadWindow loadWindow) {
			_mainScreen = mainScreen;
			_loadWindow = loadWindow;
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

		void HideAll() {
			_mainScreen.Hide();
			_loadWindow.Hide();
		}
	}
}
