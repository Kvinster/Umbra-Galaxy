using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public  class Bullet : BaseCoreComponent, IBullet {
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;

		public float LifeTime = 3f;

		float _lifeTimer;
		float _damage;

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( _lifeTimer >= LifeTime ) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(_damage);
			Destroy(gameObject);
		}

		public void Init(float damage, Vector2 force, float rotation, params Collider2D[] ownerColliders) {
			_damage = damage;

			Rigidbody.rotation = rotation;
			Rigidbody.AddRelativeForce(force, ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}
		}

		protected override void InitInternal(CoreStarter starter) {

		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}
	}
}
