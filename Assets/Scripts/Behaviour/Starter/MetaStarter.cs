using STP.Behaviour.Common;
using STP.Behaviour.Meta;
using STP.Behaviour.Meta.UI;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public BaseStarSystem       StartStarSystem;
        public PlayerShip           PlayerShip;
        public MetaUiCanvas         MetaUiCanvas;
        public MetaTimeManager      TimeManager;
        public StarSystemsGraphInfo StarSystemsGraphInfo;
        public InventoryItemInfos   InventoryItemInfos;
        
        void Start() {
            InitComponents();
        }
    }
}
