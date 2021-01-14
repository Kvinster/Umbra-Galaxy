using UnityEngine;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public sealed class Turret : BaseCoreComponent {
		public GameObject BulletPrefab;
		public float      BulletStartForce;
		public float      ReloadDuration;
		public Collider2D Collider;

		CoreSpawnHelper _spawnHelper;

		float _reloadTimer;

		void Update() {
			TryShoot();
			_reloadTimer -= Time.deltaTime;
		}

		protected override void InitInternal(CoreStarter starter) {
			_spawnHelper = starter.SpawnHelper;
		}

		void TryShoot() {
			if ( _reloadTimer <= 0 ) {
				Shoot();
				_reloadTimer = ReloadDuration;
			}
		}

		void Shoot() {
			var bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity, null);
			var bulletComp = bulletGo.GetComponent<IBullet>();
			bulletComp.Init(10f, Vector2.up * BulletStartForce, transform.rotation.eulerAngles.z, Collider);
			_spawnHelper.TryInitSpawnedObject(bulletGo);
		}
	}
}
