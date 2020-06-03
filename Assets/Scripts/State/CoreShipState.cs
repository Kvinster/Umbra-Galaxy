using System.Collections.Generic;

namespace STP.State {
    public class CoreShipState {
        public readonly Dictionary<string, int> ShipInventory = new Dictionary<string, int>();
        
        public int Capacity => 10;
        
        public int ItemsCount {get; private set;}

        public bool TryAddItem(string itemName, int amount = 1) {
            if ( ItemsCount + amount > Capacity ) {
                return false;
            }
            if ( ShipInventory.ContainsKey(itemName) ) {
                ShipInventory[itemName] += amount;
            }
            else {
                ShipInventory.Add(itemName, amount);
            }
            ItemsCount += amount;
            return true;
        }

        public void DropAllItems() {
            ShipInventory.Clear();
            ItemsCount = 0;
        }
    }
}