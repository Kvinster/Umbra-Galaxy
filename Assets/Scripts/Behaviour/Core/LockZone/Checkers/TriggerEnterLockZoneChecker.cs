using UnityEngine;

using STP.Behaviour.Core.Objects;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public sealed class TriggerEnterLockZoneChecker : BaseLockZoneChecker {
        bool _playerInArea;
        
        public override bool LockConditionActive => _playerInArea;
        
        public void OnTriggerEnter2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            _playerInArea = true;
            TriggerLockEvent();
        }

        public void OnTriggerExit2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            _playerInArea = false;
        }
    }
}