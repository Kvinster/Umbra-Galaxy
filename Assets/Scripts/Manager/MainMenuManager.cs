using STP.Behaviour.MainMenu;
using STP.Core.State;

namespace STP.Manager {
	public sealed class MainMenuManager {
		readonly ProfilesScreen    _profilesScreen;
		readonly ProfileNameScreen _profileNameScreen;
		readonly MainScreen        _mainScreen;
		readonly LeaderboardWindow _leaderboardWindow;
		readonly LevelsScreen      _levelsScreen;
		readonly SettingsScreen    _settingsScreen;

		public MainMenuManager(ProfilesScreen profilesScreen, ProfileNameScreen profileNameScreen,
			MainScreen mainScreen, LeaderboardWindow leaderboardWindow, LevelsScreen levelsScreen, SettingsScreen settingsScreen) {
			_profilesScreen    = profilesScreen;
			_profileNameScreen = profileNameScreen;
			_mainScreen        = mainScreen;
			_leaderboardWindow = leaderboardWindow;
			_levelsScreen      = levelsScreen;
			_settingsScreen    = settingsScreen;
		}

		public void Init() {
			if ( ProfileState.IsActiveInstanceExists ) {
				ShowMain();
			} else {
				ShowProfiles();
			}
		}

		public void ShowProfiles() {
			HideAll();
			_profilesScreen.Show();
		}

		public void ShowProfileNameScreen(string stateName) {
			HideAll();
			_profileNameScreen.Show(stateName);
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
			_profilesScreen.Hide();
			_profileNameScreen.Hide();
			_mainScreen.Hide();
			_leaderboardWindow.Hide();
			_levelsScreen.Hide();
			_settingsScreen.Hide();
		}
	}
}
