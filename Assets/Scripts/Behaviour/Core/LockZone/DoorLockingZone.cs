using System.Collections.Generic;

using STP.Behaviour.Core.LockZone.Checkers;
using STP.Behaviour.Core.Objects.DoorObject;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.LockZone {
    public class DoorLockingZone : GameComponent {
        [NotNull] public BaseLockZoneChecker   LockZoneChecker;
        [NotNull] public BaseUnlockZoneChecker UnlockConditionChecker;

        public List<BaseDoor> Doors;

        void OnEnable() {
            LockZoneChecker.LockZoneConditionReached += LockZone;
            UnlockConditionChecker.UnlockConditionReached += UnlockZone;
        }

        void OnDestroy() {
            LockZoneChecker.LockZoneConditionReached -= LockZone;
            UnlockConditionChecker.UnlockConditionReached -= UnlockZone;
        }

        void OnDisable() {
            OnDestroy();
        }

        void UnlockZone() {
            foreach ( var door in Doors ) {
                door.UnblockZone();
            }
        }

        void LockZone() {
            foreach ( var door in Doors ) {
                door.BlockDoor();
            }
        }
    }
}