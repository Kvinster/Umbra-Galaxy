using UnityEngine;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShip : BaseMetaComponent {
        public PlayerShipMovementController MovementController;

        MetaTimeManager _timeManager;

        void Update() {
            if ( Input.GetKeyDown(KeyCode.Space) ) {
                if ( _timeManager.IsPaused ) {
                    _timeManager.Unpause(_timeManager.CurDay + 1);
                } else if ( MovementController.IsMoving ) {
                    MovementController.InterruptMoving();
                }
            } 
        }

        protected override void InitInternal(MetaStarter starter) {
            _timeManager = starter.TimeManager;
        }
    }
}
