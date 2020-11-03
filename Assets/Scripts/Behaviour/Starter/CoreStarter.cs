using UnityEngine;

using STP.Behaviour.Core;
using STP.Gameplay;
using STP.Gameplay.DebugGUI;
using STP.Gameplay.Weapon.Common;
using STP.View.DebugGUI;
using STP.State;
using STP.State.Meta;
using STP.Utils;

namespace STP.Behaviour.Starter {
    public class CoreStarter : BaseStarter<CoreStarter> {
        public Transform        BulletSpawnStock;
        public Transform        MaterialSpawnStock;
        public UnityContext     UnityContext;
        public BaseLevelWrapper LevelWrapper;

        public OverlayManager OverlayManager;

        public WeaponCreator     WeaponCreator     { get; private set; }
        public CoreItemCreator   CoreItemCreator   { get; private set; }
        public WeaponViewCreator WeaponViewCreator { get; private set; }

        public CoreManager       CoreManager   { get; private set; }
        public CoreOverlayHelper OverlayHelper { get; private set; }

        public LevelController       LevelController       => GameState.Instance.LevelController;
        public CorePlayerController  CorePlayerController  => GameState.Instance.CorePlayerController;
        public StarSystemsController StarSystemsController => GameState.Instance.StarSystemsController;
        public PlayerController      PlayerController      => GameState.Instance.PlayerController;
        public QuestsController      QuestsController      => GameState.Instance.QuestsController;

        protected override void CheckDescription() =>
            ProblemChecker.LogErrorIfNullOrEmpty(this, BulletSpawnStock, MaterialSpawnStock, UnityContext);

        void Start() {
            WeaponViewCreator = new WeaponViewCreator(this);
            CoreItemCreator   = new CoreItemCreator(this);
            CoreManager       = new CoreManager(PlayerController, UnityContext);
            OverlayHelper     = new CoreOverlayHelper(this);
            WeaponCreator     = new WeaponCreator(new BulletCreator(BulletSpawnStock, CoreManager.AllianceManager));
            InitComponents();
            LevelWrapper.Init(this);
            //Settings for smooth gameplay
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount  = 0;
            DebugGuiController.Instance.SetDrawable(new CoreDebugGUI(this));
        }

        void OnDestroy() {
            if ( DebugGuiController.HasInstance ) {
                DebugGuiController.Instance.SetDrawable(null);
            }
        }
    }
}
