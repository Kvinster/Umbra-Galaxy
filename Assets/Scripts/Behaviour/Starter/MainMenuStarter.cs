using UnityEngine;

using STP.Behaviour.MainMenu;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public ProfilesScreen    ProfilesScreen;
		[NotNull] public ProfileNameScreen ProfileNameScreen;
		[NotNull] public MainScreen        MainScreen;
		[NotNull] public LeaderboardWindow LeaderboardWindow;

		public MainMenuManager       MainMenuManager       { get; private set; }
		public LeaderboardController LeaderboardController { get; private set; }

		void Start() {
			MainMenuManager =
				new MainMenuManager(ProfilesScreen, ProfileNameScreen, MainScreen, LeaderboardWindow);
			LeaderboardController = new LeaderboardController();

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;

			MainMenuManager.Init();
		}
	}
}
