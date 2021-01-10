using UnityEngine;

namespace STP.Behaviour.Starter {
	public class MainMenuStarter : BaseStarter<MainMenuStarter> {
		void Start() {
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
		}
	}
}
