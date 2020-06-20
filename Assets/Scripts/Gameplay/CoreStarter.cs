using UnityEngine;

using STP.State;
using STP.Utils;
using STP.View;

namespace STP.Gameplay {
    public class CoreStarter : GameBehaviour {
        public Transform BulletSpawnStock;
        public Transform MaterialSpawnStock;
        
        public OverlayManager  OverlayManager;
        public PlayerState     State => PlayerState.Instance;
        
        public CoreShipState     ShipState       {get; private set;}
        public BulletCreator     BulletCreator   {get; private set;}
        public MaterialCreator   MaterialCreator {get; private set;}
        public CoreManager       CoreManager     {get; private set;}
        public CoreOverlayHelper OverlayHelper   {get; private set;}
        
        void Start() {
            ShipState       = new CoreShipState();
            BulletCreator   = new BulletCreator(this);
            MaterialCreator = new MaterialCreator(this);
            CoreManager     = new CoreManager(State, ShipState);
            OverlayHelper   = new CoreOverlayHelper(this);
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
