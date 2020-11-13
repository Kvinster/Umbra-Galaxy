using STP.Behaviour.Starter;
using STP.Gameplay;

namespace STP.Behaviour.Core.Objects {
    public sealed class CoreItem : BaseCoreComponent, ICollectable {
        public string ItemName;

        CoreManager _coreManager;

        protected override void InitInternal(CoreStarter starter) {
            _coreManager = starter.CoreManager;
        }

        public void CollectItem() {
            if ( _coreManager.TryAddItemToShip(ItemName) ) {
                Destroy(gameObject);
            }
        }
    }
}