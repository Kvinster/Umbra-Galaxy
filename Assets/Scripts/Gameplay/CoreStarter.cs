using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour;
using STP.Gameplay.DebugGUI;
using STP.Gameplay.Weapon.Common;
using STP.State;
using STP.Utils;
using STP.View;
using STP.View.DebugGUI;

namespace STP.Gameplay {
    public class CoreStarter : GameBehaviour {
        public Transform        BulletSpawnStock;
        public Transform        MaterialSpawnStock;
        public UnityContext     UnityContext;
        public BaseLevelWrapper LevelWrapper;

        public OverlayManager OverlayManager;
        
        public WeaponCreator     WeaponCreator     { get; private set; }
        public BulletCreator     BulletCreator     { get; private set; }
        public MaterialCreator   MaterialCreator   { get; private set; }
        public CoreManager       CoreManager       { get; private set; }
        public CoreOverlayHelper OverlayHelper     { get; private set; }
        public WeaponViewCreator WeaponViewCreator { get; private set; }

        public PlayerController PlayerController => GameState.Instance.PlayerController;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, BulletSpawnStock, MaterialSpawnStock, UnityContext);
        
        void Start() {
            BulletCreator     = new BulletCreator(this);
            WeaponCreator     = new WeaponCreator();
            WeaponViewCreator = new WeaponViewCreator(this);
            MaterialCreator   = new MaterialCreator(this);
            CoreManager       = new CoreManager(PlayerController, UnityContext);
            OverlayHelper     = new CoreOverlayHelper(this);
            var behaviours = new HashSet<CoreBehaviour>(CoreBehaviour.Instances);
            foreach (var comp in behaviours) {
                comp.Init(this);
            }        
            //Settings for smooth gameplay
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
            DebugGuiController.Instance.SetDrawable(new CoreDebugGUI(this));
        }
    }
}
