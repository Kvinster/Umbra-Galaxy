﻿using STP.Behaviour.Common;
using STP.Behaviour.Meta;
using STP.State;
using STP.State.Meta;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public PlayerShip         PlayerShip;
        public MetaTimeManager    TimeManager;
        public InventoryItemInfos InventoryItemInfos;
        
        public StarSystemsManager StarSystemsManager { get; private set; }

        public TimeController        TimeController        => GameState.Instance.TimeController;
        public ProgressController    ProgressController    => GameState.Instance.ProgressController;
        public StarSystemsController StarSystemsController => GameState.Instance.StarSystemsController;
        public MetaAiShipsController MetaAiShipsController => GameState.Instance.MetaAiShipsController;
        public PlayerController      PlayerController      => GameState.Instance.PlayerController;
        public DarknessController    DarknessController    => GameState.Instance.DarknessController;
        
        void Start() {
            StarSystemsManager = new StarSystemsManager(StarSystemsController);
            InitComponents();
        }
    }
}
