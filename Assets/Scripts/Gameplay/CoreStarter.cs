using UnityEngine;

using STP.Gameplay.WeaponGroup;
using STP.Gameplay.WeaponViews;
using STP.State;
using STP.Utils;
using STP.View;

namespace STP.Gameplay {
    public class CoreStarter : GameBehaviour {
        public Transform    BulletSpawnStock;
        public Transform    MaterialSpawnStock;
        public UnityContext UnityContext;
        
        public OverlayManager  OverlayManager;
        public PlayerState     State => PlayerState.Instance;
        
        public WeaponCreator     WeaponCreator     {get; private set;}
        public CoreShipState     ShipState         {get; private set;}
        public BulletCreator     BulletCreator     {get; private set;}
        public MaterialCreator   MaterialCreator   {get; private set;}
        public CoreManager       CoreManager       {get; private set;}
        public CoreOverlayHelper OverlayHelper     {get; private set;}
        public WeaponViewCreator WeaponViewCreator {get; private set;}
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, BulletSpawnStock, MaterialSpawnStock, UnityContext);
        
        void Start() {
            ShipState         = new CoreShipState();
            BulletCreator     = new BulletCreator(this);
            WeaponCreator     = new WeaponCreator(BulletCreator);
            WeaponViewCreator = new WeaponViewCreator(this);
            MaterialCreator   = new MaterialCreator(this);
            CoreManager       = new CoreManager(State, ShipState, UnityContext);
            OverlayHelper     = new CoreOverlayHelper(this);
            foreach (var comp in CoreBehaviour.Instances) {
                comp.Init(this);
            }        
            //Settings for smooth gameplay
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
        }
    }
}
