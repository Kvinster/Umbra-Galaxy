using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems {
	public abstract class BaseShootingSystem<T> where T : ShootingSystemParams {
		protected readonly T Params;

		protected readonly CoreSpawnHelper SpawnHelper;

		float _leftReloadTime;

		public virtual bool CanShoot => (_leftReloadTime <= 0);

		public BaseShootingSystem(CoreSpawnHelper spawnHelper, T shootingParams) {
			Params      = shootingParams;
			SpawnHelper = spawnHelper;
		}

		public bool TryShoot() {
			if ( !CanShoot ) {
				return false;
			}
			Shoot();
			ForceRecharge();
			return true;
		}

		public void ForceRecharge() {
			_leftReloadTime = Params.ReloadTime;
		}

		public void DeltaTick() {
			_leftReloadTime -= Time.deltaTime;
		}

		protected abstract void Shoot();
	}

	// Default Shooting system
	public sealed class DefaultShootingSystem : BaseShootingSystem<ShootingSystemParams> {
		public DefaultShootingSystem(CoreSpawnHelper spawnHelper, ShootingSystemParams shootingParams) : base(spawnHelper,
			shootingParams) { }

		protected override void Shoot() {
			var angle = Params.RotationSource.rotation.eulerAngles.z;
			var bulletGo = Object.Instantiate(Params.BulletPrefab, Params.BulletOrigin.position, Quaternion.AngleAxis(angle, Vector3.forward));
			var bullet   = bulletGo.GetComponent<IBullet>();
			// Init custom bullet component if necessary
			bullet?.Init(Params.BulletDamage, Params.BulletSpeed, Params.IgnoreColliders);
			// Init all other BaseCoreComponent components
			SpawnHelper.TryInitSpawnedObject(bulletGo);
		}
	}
}