using STP.Behaviour.Common;
using STP.Behaviour.Meta;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public PlayerShip         PlayerShip;
        public MetaTimeManager    TimeManager;
        public InventoryItemInfos InventoryItemInfos;
        
        public StarSystemsManager StarSystemsManager { get; private set; }
        
        void Start() {
            StarSystemsManager = new StarSystemsManager();
            InitComponents();
        }
    }
}
