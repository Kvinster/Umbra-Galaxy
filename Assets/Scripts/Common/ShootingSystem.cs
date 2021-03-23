using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.EndlessLevel;

namespace STP.Common {
	public sealed class ShootingSystem {
		readonly ShootingSystemParams _params;

		float _leftReloadTime;

		public ShootingSystem(ShootingSystemParams shootingParams) {
			_params = shootingParams;
		}
		
		public void TryShoot() {
			if ( _leftReloadTime > 0f ) {
				return;
			}
			Shoot();
			_leftReloadTime = _params.ReloadTime;
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
		}
	}
}