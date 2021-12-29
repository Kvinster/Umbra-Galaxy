using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.MainMenu;
using STP.Core;
using STP.Core.State;
using STP.Service.Achievements;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public ScreensViewController ScreensViewController;

		public GameController GameController => GameController.Instance;

		void Start() {
			TryCreateGameState();
			GameController.CreateGameController(GameState.ActiveInstance);

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
			
			AchievementServiceProvider.Implementation.ResetAllStats();
			AchievementServiceProvider.Implementation.SetAchievement(AchievementType.CompleteGameAchievement);
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
