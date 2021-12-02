using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.MainMenu;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public MainScreen        MainScreen;
		[NotNull] public SettingsScreen    SettingsScreen;
		[NotNull] public LeaderboardWindow LeaderboardWindow;

		[NotNull] public ScreensViewController ScreensViewController;

		public GameController GameController => GameController.Instance;

		void Start() {
			TryCreateGameState();
			GameController.CreateGameController(GameState.ActiveInstance);

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;

			DebugGuiController.Instance.SetDrawable(new MainMenuDebugDrawable(this));
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
