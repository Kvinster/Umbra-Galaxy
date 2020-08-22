using UnityEngine.SceneManagement;

using STP.State;
using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class CoreManager {
        const float FastTravelEngineChargingTime = 3f;
        
        readonly PlayerState     _playerState;
        readonly PlayerInventory _inventory;

        public SelfDestructEngine  SelfDestructEngine  { get; } = new SelfDestructEngine();
        public FastTravelEngine    FastTravelEngine    { get; } = new FastTravelEngine();
        public PlayerShipState     PlayerShipState     { get; } = new PlayerShipState();
        public AllianceManager     AllianceManager     { get; } = new AllianceManager();
        
        public CoreManager(PlayerController playerController, UnityContext context) {
            _inventory = playerController.Inventory;
            context.AddUpdateCallback(FastTravelEngine.UpdateEngineState);
            context.AddUpdateCallback(SelfDestructEngine.UpdateSelfDestructionTimers);
            FastTravelEngine.Init(FastTravelEngineChargingTime);
        }

        public bool TryAddItemToShip(string material, int amount = 1) {
            return _inventory.TryAdd(material, amount);
        }

        public void GoToMeta() {
            SceneManager.LoadScene("Meta");
        }
    }
}