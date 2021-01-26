using UnityEngine;

using STP.Behaviour.MainMenu;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public MainScreen        MainScreen;
		[NotNull] public LoadWindow        LoadWindow;
		[NotNull] public LeaderboardWindow LeaderboardWindow;

		public MainMenuManager       MainMenuManager       { get; private set; }
		public LeaderboardController LeaderboardController { get; private set; }

		void Start() {
			MainMenuManager       = new MainMenuManager(MainScreen, LoadWindow, LeaderboardWindow);
			LeaderboardController = new LeaderboardController();

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;

			MainMenuManager.Init();
		}
	}
}
