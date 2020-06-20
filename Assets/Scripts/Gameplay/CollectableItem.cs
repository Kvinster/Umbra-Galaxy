using UnityEngine;

using STP.State;
using STP.View;

namespace STP.Gameplay {
    public class CollectableItem : CoreBehaviour {
        CoreManager _coreManager;
        
        public string ItemName;

        void OnTriggerEnter2D(Collider2D other) {
            if ( !other.GetComponent<PlayerShip>() ) {
                return;
            }
            if ( _coreManager.TryAddItemToShip(ItemName) ) {
                Destroy(gameObject);
            }
        }

        protected override void CheckDescription() {
            var foundName = false;
            foreach ( var item in ItemNames.AllItems ) {
                if ( item == ItemName ) {
                    foundName = true;
                }
            }
            if ( !foundName ) {
                Debug.LogError(string.Format("Can't find item {0} in all items collection.", ItemName));
            }
        }

        public override void Init(CoreStarter starter) {
            _coreManager = starter.CoreManager;
        }
    }
}