using STP.Behaviour.Starter;

namespace STP.Gameplay {
    public class CivilianShip : RoutedShip{
        
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;
        
        public override ConflictSide CurrentSide => ConflictSide.Civilians;

        protected override void OnShipDestroy() {
            Destroy(gameObject);
        }

        
        public override void Init(CoreStarter starter) {
            base.Init(starter);
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
        }

        void Update() {
            Move();
        }
    }
}