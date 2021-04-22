using UnityEngine;

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
			var bullet = other.gameObject.GetComponent<IBullet>();
			if ( bullet == null ) {
				Explode();
			} else { 
				Destroy(gameObject);
			}
		}

		public void Init(float damage, float speed, params Collider2D[] ownerColliders) {
			_damage = damage;
			Rigidbody.AddRelativeForce(Vector2.up * (speed * Rigidbody.mass), ForceMode2D.Impulse);
			foreach ( var ownerCollider in ownerColliders ) {
				IgnoreCollider(ownerCollider);
			}

			foreach ( var bullet in InnerBullets ) {
				bullet.gameObject.SetActive(false);
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
			var bulletSpeed = Rigidbody.velocity.magnitude;
			var toPlayerDir = (_player.transform.position - transform.position).normalized;
			var baseAngle   = Vector2.SignedAngle(toPlayerDir, Vector2.right) + 90;
			for ( var index = 0; index < InnerBullets.Count; index++ ) {
				var bullet = InnerBullets[index];
				bullet.gameObject.SetActive(true);
				bullet.transform.SetParent(transform.parent);
				bullet.transform.rotation = Quaternion.AngleAxis(-baseAngle + -30 + index * 15, Vector3.forward);
				bullet.Rigidbody.velocity = Vector3.zero;
				bullet.Rigidbody.rotation = -baseAngle + -30 + index * 15;
				bullet.Init(_damage, bulletSpeed, Collider);
			}
			Destroy(gameObject);
		}
	}
}
