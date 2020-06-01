using System.Collections.Generic;

namespace STP.State {
    public class CoreShipState {
        public int Capacity => 10;
        
        public readonly Dictionary<string, int> ShipInventory = new Dictionary<string, int>();
    }
}