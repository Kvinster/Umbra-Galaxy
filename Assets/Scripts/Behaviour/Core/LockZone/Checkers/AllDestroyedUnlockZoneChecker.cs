using JetBrains.Annotations;
using STP.Behaviour.Core.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public sealed class AllDestroyedUnlockZoneChecker : BaseUnlockZoneChecker {
        [NotNull] public EnemySpawner EnemySpawner;

        List<BaseShip> _activeShips = new List<BaseShip>();
        
        public void Awake() {
            EnemySpawner.OnEnemySpawned += OnShipCreated; 
        }

        void OnShipCreated(GameObject gameObject) {
            var shipComp = gameObject.GetComponent<BaseShip>();
            if ( !shipComp ) {
                Debug.LogError("Can't handle GO: Not a ship was created by spawner");
                return;
            }
            _activeShips.Add(shipComp);
            shipComp.OnShipDestroyed += OnShipDestroyed;
        }

        void OnShipDestroyed(BaseShip ship) {
            ship.OnShipDestroyed -= OnShipDestroyed;
            _activeShips.Remove(ship);
            if ( !EnemySpawner.IsSpawnActive && (_activeShips.Count == 0) ) {
                TriggerUnlockEvent();
            }
        }
    }
}