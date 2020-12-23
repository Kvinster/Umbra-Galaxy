using UnityEngine;

using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
    public class CoreStarter : BaseStarter<CoreStarter> {
        void Start() {
            InitComponents();
            // Settings for smooth gameplay
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
        }

        void OnDestroy() {
            if ( DebugGuiController.HasInstance ) {
                DebugGuiController.Instance.SetDrawable(null);
            }
        }
    }
}
