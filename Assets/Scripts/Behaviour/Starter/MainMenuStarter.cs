using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.MainMenu;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public ProfilesScreen    ProfilesScreen;
		[NotNull] public ProfileNameScreen ProfileNameScreen;
		[NotNull] public MainScreen        MainScreen;
		[NotNull] public LeaderboardWindow LeaderboardWindow;
		[NotNull] public LevelsScreen      LevelsScreen;
		[NotNull] public SettingsScreen    SettingsScreen;

		public MainMenuManager MainMenuManager { get; private set; }

		public GameController GameController { get; private set; }


		void Start() {
			TryCreateGameState();
			GameController = new GameController(GameState.ActiveInstance);

			MainMenuManager = new MainMenuManager(ProfilesScreen, ProfileNameScreen, MainScreen, LeaderboardWindow,
				LevelsScreen, SettingsScreen);

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;

			MainMenuManager.Init();
		}

		void TryCreateGameState() {
			if ( GameState.IsActiveInstanceExists ) {
				return;
			}
			var gs = GameState.TryLoadActiveGameState() ?? GameState.CreateNewActiveGameState();
			Assert.IsNotNull(gs);
		}
	}
}
