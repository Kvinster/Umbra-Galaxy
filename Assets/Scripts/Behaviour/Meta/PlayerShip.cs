using System;

using STP.Behaviour.Starter;
using STP.State;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShip : BaseMetaComponent {
        public PlayerShipMovementController MovementController;

        public BaseStarSystem CurSystem => MovementController.CurSystem;

        public event Action<string> OnCusSystemChanged;
        
        protected override void InitInternal(MetaStarter starter) {
            MovementController.OnCurSystemChanged += OnCurSystemChanged;
        }

        public bool TryMoveTo(BaseStarSystem starSystem, out IPromise movePromise) {
            if ( MovementController.CanMoveTo(starSystem) ) {
                movePromise = MovementController.MoveTo(starSystem)
                    .Then(() => {
                        PlayerState.Instance.CurSystem = starSystem.Name;
                    });
                return true;
            }
            movePromise = null;
            return false;
        }

        void OnCurSystemChanged(string curSystem) {
            OnCusSystemChanged?.Invoke(curSystem);
        }
    }
}
