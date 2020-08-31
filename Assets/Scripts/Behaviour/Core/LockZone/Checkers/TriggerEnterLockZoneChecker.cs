using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public sealed class TriggerEnterLockZoneChecker : BaseLockZoneChecker {
        [NotNull] public EnemySpawner Spawner;

        bool _lockWasActivated;
        
        public void OnTriggerEnter2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() || _lockWasActivated ) {
                return;
            }
            _lockWasActivated = true;
            Spawner.TryStartSpawn();
            TriggerLockEvent();
        }
    }
}