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
        public MotherShipState  MotherShipState  {get;} = new MotherShipState();
        
        readonly PlayerState   _playerState;
        readonly CoreShipState _shipState;
        readonly UnityContext  _unityContext;

        public CoreManager(PlayerState state, CoreShipState shipState, UnityContext context) {
            _playerState  = state;
            _shipState    = shipState;
            _unityContext = context;
            _unityContext.AddUpdateCallback(FastTravelEngine.UpdateEngineState);
            FastTravelEngine.Init(FastTravelEngineChargingTime);
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