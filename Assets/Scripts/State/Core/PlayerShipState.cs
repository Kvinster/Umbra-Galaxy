using UnityEngine;

namespace STP.State.Core {
    public class PlayerShipState {
        public Vector2 Position;
        public Vector2 Velocity;
        
        public delegate void OnStateChanged();
        public event OnStateChanged StateChangedManually;

        public void TriggerChangeEvent() {
            StateChangedManually?.Invoke();
        }
    }
}