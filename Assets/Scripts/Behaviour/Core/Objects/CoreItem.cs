using STP.Gameplay;

namespace STP.Behaviour.Core.Objects {
    public class CoreItem : CoreComponent, ICollectable {
        public string ItemName;
        
        CoreManager _coreManager;
        
        public override void Init(CoreStarter starter) {
            _coreManager = starter.CoreManager;
        }

        public void CollectItem() {
            if ( _coreManager.TryAddItemToShip(ItemName) ) {
                Destroy(gameObject);
            }
        }
    }
}