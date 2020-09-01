using System;

using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public abstract class BaseLockZoneChecker : GameComponent {
        public event Action LockZoneConditionReached;

        public abstract bool LockConditionActive { get; }

        protected void TriggerLockEvent() {
            LockZoneConditionReached?.Invoke();
        }

        public virtual void Init(CoreStarter starter) { }
    }
}