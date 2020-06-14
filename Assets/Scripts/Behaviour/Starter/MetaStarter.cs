using STP.Behaviour.Common;
using STP.Behaviour.Meta;
using STP.Behaviour.Meta.UI;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public BaseStarSystem     StartStarSystem;
        public PlayerShip         PlayerShip;
        public MetaUiCanvas       MetaUiCanvas;
        public MetaTimeManager    TimeManager;
        public InventoryItemInfos InventoryItemInfos;
        
        public StarSystemsManager StarSystemsManager { get; private set; }
        
        void Start() {
            StarSystemsManager = new StarSystemsManager();
            InitComponents();
        }
    }
}
