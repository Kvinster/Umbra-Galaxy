using UnityEngine.SceneManagement;
using UnityEngine;

using STP.State;
using STP.State.Core;

namespace STP.Gameplay {
    public class CoreManager {
        public PlayerShipState PlayerShipState {get; private set;} = new PlayerShipState();
        public MotherShipState MotherShipState {get; private set;} = new MotherShipState();
        
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

        public void GoToShop(bool sendItems) {
            if ( sendItems ) {
                SendItemsToMothership();
            }
            SceneManager.LoadScene("Meta");
        }

        public void TeleportToMothership() {
            PlayerShipState.Position               = MotherShipState.TeleportPosition;
            PlayerShipState.Velocity               = Vector2.zero;
            PlayerShipState.TriggerChangeEvent();
        }
    }
}