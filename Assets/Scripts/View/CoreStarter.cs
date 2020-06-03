using UnityEngine;

using STP.State;
using STP.Utils;

namespace STP.View {
    public class CoreStarter : GameBehaviour {
        public PlayerState   State => PlayerState.Instance;
        
        public CoreShipState ShipState;
        
        void Start() {
            ShipState = new CoreShipState();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
            foreach (var comp in CoreBehaviour.Instances) {
                comp.Init(this);
            }        
        }

        protected override void CheckDescription() { }
    }
}
