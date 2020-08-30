using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;

namespace STP.Gameplay {
    public sealed class CivilianShip : RoutedShip {
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;

        public override ConflictSide CurrentSide => ConflictSide.Civilians;

        protected override void OnShipDestroy() {
            Destroy(gameObject);
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
        }

        void Update() {
            Move();
        }
    }
}