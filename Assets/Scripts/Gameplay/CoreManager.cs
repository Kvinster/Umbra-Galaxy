using UnityEngine.SceneManagement;
using UnityEngine;

using STP.State;
using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class CoreManager {
        const float FastTravelEngineChargingTime = 3f;
        
        public FastTravelEngine FastTravelEngine {get;} = new FastTravelEngine();
        public PlayerShipState  PlayerShipState  {get;} = new PlayerShipState();
        
        readonly PlayerState   _playerState;
        readonly CoreShipState _shipState;

        public CoreManager(PlayerState state, CoreShipState shipState, UnityContext context) {
            _playerState  = state;
            _shipState    = shipState;
            context.AddUpdateCallback(FastTravelEngine.UpdateEngineState);
            FastTravelEngine.Init(FastTravelEngineChargingTime);
        }

        public bool TryAddItemToShip(string material, int amount = 1) {
            return _shipState.TryAddItem(material, amount);
        }

        public void SendItemsToMothership() {
            foreach ( var item in _shipState.ShipInventory ) {
                // TODO: check result, act somehow
                _playerState.Inventory.TryAdd(item.Key, item.Value);
            }
            _shipState.DropAllItems();
        }

        public void GoToMeta() {
            SendItemsToMothership();
            SceneManager.LoadScene("Meta");
        }
    }
}