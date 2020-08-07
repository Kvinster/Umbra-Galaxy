using UnityEngine;

using System.Collections.Generic;

using STP.State.Meta;
using STP.Utils;

namespace STP.State {
    public sealed class GameState : Singleton<GameState> {
        public PlayerController       PlayerController       { get; private set; }
        public ProgressController     ProgressController     { get; private set; }
        public TimeController         TimeController         { get; private set; }
        public StarSystemsController  StarSystemsController  { get; private set; }
        public MetaAiShipsController  MetaAiShipsController  { get; private set; }
        public DarknessController     DarknessController     { get; private set; }
        public ShardsActiveController ShardsActiveController { get; private set; }

        readonly HashSet<BaseStateController> _controllers = new HashSet<BaseStateController>();

        protected override void Init() {
            CreateControllers();
            InitControllers();
        }

        void InitControllers() {
            foreach ( var controller in _controllers ) {
                controller.Init();
            }
        }

        T CreateController<T>(T controller) where T : BaseStateController{
            if ( controller == null ) {
                Debug.LogErrorFormat("Controller '{0}' is null", typeof(T).Name);
                return null;
            }
            if ( _controllers.Contains(controller) ) {
                Debug.LogErrorFormat("_controllers already contains '{0}'", typeof(T).Name);
                return null;
            }
            _controllers.Add(controller);
            return controller;
        }

        void CreateControllers() {
            PlayerController       = CreateController(new PlayerController());
            ProgressController     = CreateController(new ProgressController());
            TimeController         = CreateController(new TimeController());
            StarSystemsController  = CreateController(new StarSystemsController());
            MetaAiShipsController  = CreateController(new MetaAiShipsController(TimeController));
            DarknessController     = CreateController(new DarknessController(TimeController, StarSystemsController, ProgressController));
            ShardsActiveController = CreateController(new ShardsActiveController(TimeController, StarSystemsController));
        }
    }
}
