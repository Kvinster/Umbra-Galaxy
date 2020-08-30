using System;

using STP.Behaviour.Core.AiMovement;
using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects {
    public sealed class EscortShip : BaseShip {
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;

        [NotNull] public PatrolMovementController MovementController;

        public override ConflictSide CurrentSide => ConflictSide.Civilians;

        public event Action<BaseShip> OnReachedRouteEnd;

        protected override void OnShipDestroy() {
            Destroy(gameObject);
        }

        void OnDestroy() {
            MovementController.OnFinishedPatrol -= OnFinishedPatrol;
        }

        protected override void InitInternal(CoreStarter starter) {
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
            MovementController.Init(ShipSpeed, true);
            MovementController.OnFinishedPatrol += OnFinishedPatrol;
        }

        void OnFinishedPatrol() {
            OnReachedRouteEnd?.Invoke(this);
        }
    }
}