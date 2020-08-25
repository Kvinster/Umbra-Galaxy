using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core;
using STP.Gameplay;
using STP.Gameplay.DebugGUI;
using STP.Gameplay.Weapon.Common;
using STP.State;
using STP.Utils;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
    public class CoreStarter : GameComponent {
        public Transform        BulletSpawnStock;
        public Transform        MaterialSpawnStock;
        public UnityContext     UnityContext;
        public BaseLevelWrapper LevelWrapper;

        public OverlayManager OverlayManager;

        public WeaponCreator     WeaponCreator     { get; private set; }
        public BulletCreator     BulletCreator     { get; private set; }
        public CoreItemCreator   CoreItemCreator   { get; private set; }
        public WeaponViewCreator WeaponViewCreator { get; private set; }

        public CoreManager       CoreManager   { get; private set; }
        public CoreOverlayHelper OverlayHelper { get; private set; }

        public CorePlayerController CorePlayerController => GameState.Instance.CorePlayerController;
        public PlayerController     PlayerController     => GameState.Instance.PlayerController;

        protected override void CheckDescription() =>
            ProblemChecker.LogErrorIfNullOrEmpty(this, BulletSpawnStock, MaterialSpawnStock, UnityContext);

        void Start() {
            WeaponCreator     = new WeaponCreator();
            WeaponViewCreator = new WeaponViewCreator(this);
            CoreItemCreator   = new CoreItemCreator(this);
            CoreManager       = new CoreManager(PlayerController, UnityContext);
            OverlayHelper     = new CoreOverlayHelper(this);
            BulletCreator     = new BulletCreator(BulletSpawnStock, CoreManager.AllianceManager);
            var behaviours    = new HashSet<CoreComponent>(CoreComponent.Instances);
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
