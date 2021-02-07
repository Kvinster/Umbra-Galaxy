using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class Asteroid : GameComponent {
        public float LifeTime = 10f;
        public float RotationPower = 100000f;

        [NotNull]
        public Rigidbody2D Rigidbody;

        readonly Timer _lifeTimer = new Timer();

        void OnCollisionEnter2D(Collision2D other) {
            var destructible = other.gameObject.GetComponent<IDestructible>();
            destructible?.TakeDamage(float.MaxValue);
        }

        void Update() {
            if ( _lifeTimer.DeltaTick() ) {
                Destroy(gameObject);
            }
        }

        public void Init(Vector2 direction, float speed) {
            Rigidbody.AddRelativeForce(speed * Rigidbody.mass * direction, ForceMode2D.Impulse);
            Rigidbody.AddTorque(RotationPower, ForceMode2D.Impulse);
            _lifeTimer.Start(LifeTime);
        }
    }
}