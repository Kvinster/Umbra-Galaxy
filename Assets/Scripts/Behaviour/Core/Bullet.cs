using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class Bullet : GameComponent {
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;

		public float LifeTime = 3f;

		float _lifeTimer;

		public void Init(Vector2 force, float rotation, params Collider2D[] ownerColliders) {
			Rigidbody.rotation = rotation;
			Rigidbody.AddRelativeForce(force, ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}
		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( _lifeTimer >= LifeTime ) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(10f);
			Destroy(gameObject);
		}
	}
}
