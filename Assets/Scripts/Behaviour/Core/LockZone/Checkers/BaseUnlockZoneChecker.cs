using STP.Utils;
using System;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public abstract class BaseUnlockZoneChecker : GameComponent {
        public event Action UnlockConditionReached;
        
        protected void TriggerUnlockEvent() {
            UnlockConditionReached?.Invoke();
        }
    }
}