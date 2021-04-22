using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class Bullet : GameComponent, IBullet {
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;

		public float LifeTime = 3f;

		float _lifeTimer;
		float _damage;

		public virtual bool NeedToDestroy => (_lifeTimer >= LifeTime);

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( NeedToDestroy ) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(_damage);
			Destroy(gameObject);
		}

		public void Init(float damage, float speed, params Collider2D[] ownerColliders) {
			_damage = damage;
			Rigidbody.AddRelativeForce(Vector2.up * (speed * Rigidbody.mass), ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}
		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}
	}
}
