using System;

using STP.Utils;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public abstract class BaseLockZoneChecker : GameComponent {
        public event Action LockZoneConditionReached;

        protected void TriggerLockEvent() {
            LockZoneConditionReached?.Invoke();
        }
    }
}