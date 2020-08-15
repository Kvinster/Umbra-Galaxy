using UnityEngine;

namespace STP.State {
    public sealed class PlayerInventory {
        public const int InventoryPlaces = 32;

        readonly PlayerInventoryPlace[] _places = new PlayerInventoryPlace[InventoryPlaces];

        public PlayerInventory() {
            for ( var i = 0; i < InventoryPlaces; ++i ) {
                _places[i] = new PlayerInventoryPlace();
            }
            _places[0].SetItem(ItemNames.Mineral, 10);
            _places[1].SetItem(ItemNames.FuelTank, 10);
            _places[2].SetItem(ItemNames.Scrap, 10);
        }

        public PlayerInventoryPlace GetPlace(int placeIndex) {
            if ( (placeIndex < 0) || (placeIndex >= InventoryPlaces) ) {
                Debug.LogErrorFormat("Invalid inventory place index '{0}'", placeIndex);
                return null;
            }
            return _places[placeIndex];
        }

        public int GetUsedCapacity() {
            var usedCapacity = 0;
            foreach ( var place in _places ) {
                if (place.ItemAmount == -1) {
                    continue;
                }
                usedCapacity += place.ItemAmount;
            }
            return usedCapacity;
        }

        public int GetItemAmount(string itemName) {
            if ( string.IsNullOrEmpty(itemName) ) {
                Debug.LogError("Item name is null or empty");
                return -1;
            }
            var res = 0;
            foreach ( var place in _places ) {
                if ( place.ItemName == itemName ) {
                    res += place.ItemAmount;
                }
            }
            return res;
        }

        public bool TryAdd(string itemName, int itemAmount) {
            foreach ( var place in _places ) {
                if ( place.ItemName == itemName ) {
                    place.SetItem(itemName, place.ItemAmount + itemAmount);
                    return true;
                }
            }
            foreach ( var place in _places ) {
                if ( place.IsEmpty ) {
                    place.SetItem(itemName, itemAmount);
                    return true;
                }
            }
            // no place for item
            return false;
        }

        public bool TryRemove(string itemName, int itemAmount) {
            if ( GetItemAmount(itemName) < itemAmount ) {
                return false;
            }
            foreach ( var place in _places ) {
                if ( place.ItemName == itemName ) {
                    var toSub = Mathf.Min(itemAmount, place.ItemAmount);
                    place.SetItem(itemName, place.ItemAmount - toSub);
                    itemAmount -= toSub;
                    if ( itemAmount == 0 ) {
                        break;
                    }
                }
            }
            return true;
        }
    }
}
