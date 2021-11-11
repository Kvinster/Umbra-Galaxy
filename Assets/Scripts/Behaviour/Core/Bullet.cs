﻿using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class Bullet : GameComponent, IBullet {
		[NotNull] public Rigidbody2D Rigidbody;
		[NotNull] public Collider2D  Collider;
		[Space]
		public VfxRunner DeathEffectRunner;

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
			other.TryTakeDamage(_damage);
			Die();
		}

		public void Init(float damage, float speed, params Collider2D[] ownerColliders) {
			_damage = damage;
			Rigidbody.AddRelativeForce(Vector2.up * (speed * Rigidbody.mass), ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}
		}

		public void Die() {
			if ( DeathEffectRunner ) {
				DeathEffectRunner.transform.parent = transform.parent;
				DeathEffectRunner.RunVfx(true);
			}
			Destroy(gameObject);
		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}
	}
}
