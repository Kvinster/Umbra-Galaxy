﻿using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class FragBullet : BaseCoreComponent, IBullet {
		[NotNullOrEmpty]
		public List<Bullet> InnerBullets;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;
		[NotNull]
		public TriggerNotifier Notifier;

		public float LifeTime = 3f;

		float _lifeTimer;
		float _damage;

		Player _player;

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( _lifeTimer >= LifeTime ) {
				Destroy(gameObject);
			}
		}

		void OnDestroy() {
			Notifier.OnTriggerEnter -= OnRangeEnter;
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(_damage);
			Explode();
		}

		public void Init(float damage, Vector2 force, float rotation, params Collider2D[] ownerColliders) {
			_damage = damage;
			foreach ( var bullet in InnerBullets ) {
				bullet.gameObject.SetActive(false);
			}
			Rigidbody.rotation = rotation;
			Rigidbody.AddRelativeForce(force, ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}

			Notifier.OnTriggerEnter += OnRangeEnter;
		}

		protected override void InitInternal(CoreStarter starter) {
			_player = starter.Player;
		}

		void IgnoreCollider(Collider2D ignoreCollider) {
			Physics2D.IgnoreCollision(ignoreCollider, Collider);
		}

		void OnRangeEnter(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( player ) {
				Explode();
			}
		}

		void Explode() {
			for ( var index = 0; index < InnerBullets.Count; index++ ) {
				var bullet = InnerBullets[index];
				bullet.transform.SetParent(transform.parent);
				bullet.gameObject.SetActive(true);
				var toPlayerDir = (_player.transform.position - transform.position).normalized;
				var baseAngle   = Vector2.SignedAngle(toPlayerDir, Vector2.right);
				bullet.Init(_damage, Vector2.right * Rigidbody.velocity.magnitude / 3,  -baseAngle + -30 + index * 15, Collider);
			}

			Destroy(gameObject);
		}
	}
}