using UnityEngine;

using System;

namespace STP.Utils {
    [RequireComponent(typeof(Collider2D))]
    public sealed class TriggerNotifier : GameComponent {
        public event Action<GameObject> OnTriggerEnter;
        public event Action<GameObject> OnTriggerStay;
        public event Action<GameObject> OnTriggerExit;

        void OnTriggerEnter2D(Collider2D other) {
            OnTriggerEnter?.Invoke(other.gameObject);
        }

        void OnTriggerStay2D(Collider2D other) {
            OnTriggerStay?.Invoke(other.gameObject);
        }

        void OnTriggerExit2D(Collider2D other) {
            OnTriggerExit?.Invoke(other.gameObject);
        }
    }
}
