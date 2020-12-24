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
			var bulletComp = bulletGo.GetComponent<Bullet>();
			bulletComp.Init(Collider, Vector2.up * BulletStartForce, transform.rotation.eulerAngles.z);
		}
	}
}
