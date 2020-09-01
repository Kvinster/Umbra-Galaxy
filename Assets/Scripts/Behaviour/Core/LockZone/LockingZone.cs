using System.Collections.Generic;

using STP.Behaviour.Core.LockZone.Checkers;
using STP.Behaviour.Core.Objects.DoorObject;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.LockZone {
    public class LockingZone : CoreComponent {
        [NotNull] public BaseLockZoneChecker   LockZoneChecker;
        [NotNull] public BaseUnlockZoneChecker UnlockConditionChecker;
        [NotNull] public EnemySpawner          Spawner;
        
        public bool OnetimeLock = true;
        
        bool _lockWasUsed;

        public List<BaseDoor> Doors;

        new void OnEnable() {
            base.OnEnable();
            LockZoneChecker.LockZoneConditionReached      += LockZone;
            UnlockConditionChecker.UnlockConditionReached += UnlockZone;
        }

        void OnDestroy() {
            LockZoneChecker.LockZoneConditionReached      -= LockZone;
            UnlockConditionChecker.UnlockConditionReached -= UnlockZone;
        }

        new void OnDisable() {
            base.OnDisable();
            OnDestroy();
        }
        
        protected override void InitInternal(CoreStarter starter) {
            LockZoneChecker.Init(starter);
        }

        void UnlockZone() {
            foreach ( var door in Doors ) {
                door.UnblockZone();
            }
        }

        void LockZone() {
            if ( _lockWasUsed && OnetimeLock) {
                return;
            }
            Spawner.TryStartSpawn();
            _lockWasUsed = true;
            foreach ( var door in Doors ) {
                door.BlockDoor();
            }
        }
    }
}