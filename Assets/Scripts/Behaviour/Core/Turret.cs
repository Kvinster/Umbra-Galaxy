using UnityEngine;

namespace STP.Behaviour.Core {
	public sealed class Turret : MonoBehaviour {
		public GameObject BulletPrefab;
		public float      BulletStartForce;
		public float      ReloadDuration;
		public Collider2D Collider;

		float _reloadTimer;

		void Update() {
			TryShoot();
			_reloadTimer -= Time.deltaTime;
		}

		void TryShoot() {
			if ( _reloadTimer <= 0 ) {
				Shoot();
				_reloadTimer = ReloadDuration;
			}
		}

		void Shoot() {
			var bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity, null);
			var bulletRb = bulletGo.GetComponent<Rigidbody2D>();
			bulletRb.rotation = transform.rotation.eulerAngles.z;
			bulletRb.AddRelativeForce(Vector2.up * BulletStartForce, ForceMode2D.Impulse);
			Physics2D.IgnoreCollision(Collider, bulletGo.GetComponent<Collider2D>());
		}
	}
}
