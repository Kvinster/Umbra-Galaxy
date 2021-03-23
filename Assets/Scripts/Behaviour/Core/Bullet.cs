using UnityEngine;

using STP.Behaviour.EndlessLevel;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class Bullet : GameComponent, IBullet {
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

		//TODO: Remove this init method. Use only Init method with reduced args.
		public void Init(float damage, float speed, float rotation, params Collider2D[] ownerColliders) {
			_damage = damage;

			transform.rotation = Quaternion.Euler(0, 0, rotation);
			Rigidbody.rotation = rotation;
			Rigidbody.AddRelativeForce(speed * Rigidbody.mass * Vector2.up, ForceMode2D.Impulse);
			Rigidbody.AddRelativeForce(Vector2.up * (speed * Rigidbody.mass), ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}
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
