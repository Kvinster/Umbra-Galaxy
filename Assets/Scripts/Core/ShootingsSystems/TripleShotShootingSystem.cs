using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems {
	public sealed class TripleShotShootingSystem : BaseShootingSystem<ShootingSystemParams> {
		readonly float _angle;

		public TripleShotShootingSystem(float angle, CoreSpawnHelper spawnHelper, ShootingSystemParams shootingParams) :
			base(spawnHelper, shootingParams) {
			_angle = angle;
		}

		protected override void Shoot() {
			for ( var i = -1; i <= 1; ++i ) {
				var angle = Params.RotationSource.rotation.eulerAngles.z + i * _angle;
				var bulletGo = Object.Instantiate(Params.BulletPrefab, Params.BulletOrigin.position, Quaternion.AngleAxis(angle, Vector3.forward));
				var bullet   = bulletGo.GetComponent<IBullet>();
				// Init custom bullet component if necessary
				bullet?.Init(Params.BulletDamage, Params.BulletSpeed, Params.IgnoreColliders);
				// Init all other BaseCoreComponent components
				SpawnHelper.TryInitSpawnedObject(bulletGo);
			}
		}
	}
}
