using STP.Behaviour.Starter;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShip : BaseMetaComponent {
        public PlayerShipMovementController MovementController;
        
        protected override void InitInternal(MetaStarter starter) {
        }

        public void TryMoveTo(BaseStarSystem starSystem) {
            if ( MovementController.CanMoveTo(starSystem) ) {
                MovementController.MoveTo(starSystem);
            }
        }
    }
}
