using UnityEngine.SceneManagement;

using System;

using STP.State;
using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class CoreManager {
        const float FastTravelEngineChargingTime = 3f;

        readonly PlayerState     _playerState;
        readonly PlayerInventory _inventory;

        readonly PlayerShipState _metaShipState;

        public SelfDestructEngine  SelfDestructEngine  { get; } = new SelfDestructEngine();
        public FastTravelEngine    FastTravelEngine    { get; } = new FastTravelEngine();
        public CoreShipState       CorePlayerShipState { get; }
        public AllianceManager     AllianceManager     { get; } = new AllianceManager();

        public event Action OnPreGoToMeta;

        public CoreManager(PlayerController playerController, UnityContext context) {
            _inventory     = playerController.Inventory;
            _metaShipState = playerController.CurPlayerShipState;
            CorePlayerShipState = new CoreShipState(_metaShipState);
            context.AddUpdateCallback(FastTravelEngine.UpdateEngineState);
            context.AddUpdateCallback(SelfDestructEngine.UpdateSelfDestructionTimers);
            FastTravelEngine.Init(FastTravelEngineChargingTime);
        }

        public bool TryAddItemToShip(string material, int amount = 1) {
            return _inventory.TryAdd(material, amount);
        }

        public void GoToMeta() {
            _metaShipState.Hp = CorePlayerShipState.Hp;
            OnPreGoToMeta?.Invoke();
            SceneManager.LoadScene("Meta");
        }
    }
}