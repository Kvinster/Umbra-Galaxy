using STP.State;
using STP.Utils;

namespace STP.View {
    public class CoreStarter : GameBehaviour {
        public PlayerState   State => PlayerState.Instance;
        
        public CoreShipState ShipState;
        
        void Start() {
            ShipState = new CoreShipState();
            foreach (var comp in CoreBehaviour.Instances) {
                comp.Init(this);
            }        
        }

        protected override void CheckDescription() { }
    }
}
