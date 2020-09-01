using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public class LogicAndZoneChecker : BaseLockZoneChecker {
        [NotNullOrEmpty] public List<BaseLockZoneChecker> LockCheckers;

        public override bool LockConditionActive => LockCheckers.All(lockChecker => lockChecker.LockConditionActive);

        public override void Init(CoreStarter starter) {
            foreach ( var lockChecker in LockCheckers ) {
                lockChecker.Init(starter);
                lockChecker.LockZoneConditionReached += TryTriggerEvent;
            }
            TryTriggerEvent();
        }

        void TryTriggerEvent() {
            if ( LockConditionActive ) {
                TriggerLockEvent();
            }
        }
    }
}