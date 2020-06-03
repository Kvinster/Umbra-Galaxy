using STP.Gameplay;
using UnityEngine;

using STP.State;
using STP.Utils;

namespace STP.View {
    public class CoreStarter : GameBehaviour {
        public PlayerState   State => PlayerState.Instance;
        
        public CoreShipState   ShipState       {get; private set;}
        public MaterialCreator MaterialCreator {get; private set;}
        public CoreManager     CoreManager     {get; private set;}
        
        void Start() {
            ShipState       = new CoreShipState();
            MaterialCreator = new MaterialCreator(this);
            CoreManager     = new CoreManager(State, ShipState);
            foreach (var comp in CoreBehaviour.Instances) {
                comp.Init(this);
            }        
            //Settings for smooth gameplay
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
        }

        protected override void CheckDescription() { }
    }
}
