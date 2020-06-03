using STP.State;

namespace STP.Gameplay {
    public class CoreManager {
        readonly PlayerState   _playerState;
        readonly CoreShipState _shipState;

        public CoreManager(PlayerState state, CoreShipState shipState) {
            _playerState = state;
            _shipState   = shipState;
        }

        public bool TryAddItemToShip(string material, int amount = 1) {
            return _shipState.TryAddItem(material, amount);
        }

        public void SendItemsToMothership() {
            foreach ( var item in _shipState.ShipInventory ) {
                _playerState.AddToInventory(item.Key, item.Value);
            }
            _shipState.DropAllItems();
        }
    }
}