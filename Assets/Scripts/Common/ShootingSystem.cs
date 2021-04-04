using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Common {
	public sealed class ShootingSystem {
		readonly ShootingSystemParams _params;
		readonly CoreSpawnHelper      _spawnHelper;

		float _leftReloadTime;

		public ShootingSystem(CoreSpawnHelper spawnHelper, ShootingSystemParams shootingParams) {
			_params      = shootingParams;
			_spawnHelper = spawnHelper;
		}
		
		public bool TryShoot() {
			if ( _leftReloadTime > 0f ) {
				return false;
			}
			Shoot();
			_leftReloadTime = _params.ReloadTime;
			return true;
		}

		public void DeltaTick() {
			_leftReloadTime -= Time.deltaTime;
		}
		
		void Shoot() {
			var angle = _params.RotationSource.rotation.eulerAngles.z;
			var bulletGo = Object.Instantiate(_params.BulletPrefab, _params.BulletOrigin.position, Quaternion.AngleAxis(angle, Vector3.forward));
			var bullet   = bulletGo.GetComponent<IBullet>();
			if ( bullet != null ) {
				bullet.Init(_params.BulletDamage, _params.BulletSpeed, _params.IgnoreColliders);
			} else {
				Debug.LogError("No bullet component on current bullet prefab");
			}
			_spawnHelper.TryInitSpawnedObject(bulletGo);
		}
	}
}