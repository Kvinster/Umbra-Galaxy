using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Core.Enemy {
    public class Asteroid : GameComponent {
        void OnCollisionEnter2D(Collision2D other) {
            var destructible = other.gameObject.GetComponent<IDestructible>();
            destructible?.TakeDamage(float.MaxValue);
        }
    }
}