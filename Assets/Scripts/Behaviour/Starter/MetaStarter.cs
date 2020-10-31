﻿using STP.Behaviour.Common;
using STP.Behaviour.Meta;
using STP.Common;
using STP.Config.ScriptableObjects;
using STP.Gameplay.DebugGUI;
using STP.State;
using STP.State.Meta;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public PlayerShip          PlayerShip;
        public MetaTimeManager     TimeManager;
        public InventoryItemInfos  InventoryItemInfos;
        public CoreLevelsCatalogue LevelsCatalogue;

        public StarSystemsManager StarSystemsManager { get; private set; }
        public QuestHelper        QuestHelper        { get; private set; }

        public TimeController        TimeController        => GameState.Instance.TimeController;
        public LevelController       LevelController       => GameState.Instance.LevelController;
        public ProgressController    ProgressController    => GameState.Instance.ProgressController;
        public DialogController      DialogController      => GameState.Instance.DialogController;
        public QuestsController      QuestsController      => GameState.Instance.QuestsController;
        public StarSystemsController StarSystemsController => GameState.Instance.StarSystemsController;
        public MetaAiShipsController MetaAiShipsController => GameState.Instance.MetaAiShipsController;
        public PlayerController      PlayerController      => GameState.Instance.PlayerController;
        public DarknessController    DarknessController    => GameState.Instance.DarknessController;

        void Start() {
            StarSystemsManager = new StarSystemsManager(StarSystemsController);
            QuestHelper        = new QuestHelper(QuestsController, PlayerController);
            InitComponents();

            // TODO: move this somewhere else
            if ( LevelController.IsLevelActive ) {
                LevelController.FinishLevel();
            }
            DebugGuiController.Instance.SetDrawable(new MetaDebugGUI(this));
        }

        void OnDestroy() {
            DebugGuiController.Instance.SetDrawable(null);
        }
    }
}
