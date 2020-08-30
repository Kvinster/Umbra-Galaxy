using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Utils {
    [RequireComponent(typeof(Collider2D))]
    public sealed class TriggerNotifier : GameComponent {
        public event Action<GameObject> OnTriggerEnter;
        public event Action<GameObject> OnTriggerStay;
        public event Action<GameObject> OnTriggerExit;

        public readonly List<GameObject> Exceptions = new List<GameObject>();

        void OnTriggerEnter2D(Collider2D other) {
            if ( !Exceptions.Contains(other.gameObject) ) {
                OnTriggerEnter?.Invoke(other.gameObject);
            }
        }

        void OnTriggerStay2D(Collider2D other) {
            if ( !Exceptions.Contains(other.gameObject) ) {
                OnTriggerStay?.Invoke(other.gameObject);
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if ( !Exceptions.Contains(other.gameObject) ) {
                OnTriggerExit?.Invoke(other.gameObject);
            }
        }
    }
}
