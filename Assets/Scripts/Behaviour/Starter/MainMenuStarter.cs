using UnityEngine;

using STP.Behaviour.MainMenu;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class MainMenuStarter : BaseStarter<MainMenuStarter> {
		[NotNull] public MainScreen MainScreen;
		[NotNull] public LoadWindow LoadWindow;

		public MainMenuManager MainMenuManager { get; private set; }

		void Start() {
			MainMenuManager = new MainMenuManager(MainScreen, LoadWindow);

			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;

			MainMenuManager.Init();
		}
	}
}
