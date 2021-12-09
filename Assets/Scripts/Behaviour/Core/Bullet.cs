using STP.Behaviour.Core.Enemy;
using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class Bullet : GameComponent, IBullet, IVisibleHandler {
		[NotNull] public Rigidbody2D Rigidbody;
		[NotNull] public Collider2D  Collider;

		public bool KillParent;

		[Space]
		public VfxRunner DeathEffectRunner;

		public TransformBulletMover TransformBulletMover;
		public float                PlayerPushForce = 0f;


		public float LifeTime = 3f;

		float _lifeTimer;
		float _damage;

		public virtual bool NeedToDestroy => (_lifeTimer >= LifeTime);

		protected bool IsVisible;

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( NeedToDestroy && !IsVisible ) {
				Die();
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(_damage);
			var rb = other.gameObject.GetComponent<Rigidbody2D>();
			if ( rb ) {
				var movementDirection = Rigidbody.velocity.normalized;
				rb.AddForce(movementDirection * PlayerPushForce, ForceMode2D.Impulse);
			}
			Die();
		}

		public void OnBecomeVisibleForPlayer(Transform playerTransform) {
			IsVisible = true;
		}

		public void OnBecomeInvisibleForPlayer() {
			IsVisible = false;
		}

		public void Init(float damage, float speed, params Collider2D[] ownerColliders) {
			_damage = damage;
			Rigidbody.AddRelativeForce(Vector2.up * (speed * Rigidbody.mass), ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}

			if ( TransformBulletMover ) {
				TransformBulletMover.enabled          = true;
				TransformBulletMover.Velocity         = Rigidbody.velocity;
				TransformBulletMover.transform.parent = transform.parent;
			}
		}

		public void Die() {
			if ( DeathEffectRunner && IsVisible ) {
				DeathEffectRunner.transform.parent = transform.parent;
				DeathEffectRunner.RunVfx(true);
			}

			Destroy(KillParent ? transform.parent.gameObject : gameObject);
		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}
	}
}
