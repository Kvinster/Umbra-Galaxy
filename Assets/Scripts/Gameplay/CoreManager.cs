using UnityEngine.SceneManagement;
using UnityEngine;

using STP.State;
using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class CoreManager {
        const float FastTravelEngineChargingTime = 3f;
        
        readonly PlayerState     _playerState;
        readonly PlayerInventory _inventory;
        
        public int UsedCapacity { get; private set; }

        public FastTravelEngine FastTravelEngine {get;} = new FastTravelEngine();
        public PlayerShipState  PlayerShipState  {get;} = new PlayerShipState();
        
        public CoreManager(PlayerState state, UnityContext context) {
            _inventory   = state.Inventory;
            UsedCapacity = _inventory.GetUsedCapacity();
            context.AddUpdateCallback(FastTravelEngine.UpdateEngineState);
            FastTravelEngine.Init(FastTravelEngineChargingTime);
        }

        public bool TryAddItemToShip(string material, int amount = 1) {
            if ( UsedCapacity + amount > PlayerInventory.Capacity ) {
                return false;
            }
            _inventory.TryAdd(material, amount);
            UsedCapacity += amount;
            return true;
        }

        public void GoToMeta() {
            SceneManager.LoadScene("Meta");
        }
    }
}