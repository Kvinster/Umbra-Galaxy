using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems {
	public abstract class BaseShootingSystem {
		public abstract bool TryShoot();

		public abstract void ForceRecharge();

		public abstract void DeltaTick();
	}

	public abstract class BaseShootingSystem<T> : BaseShootingSystem where T : ShootingSystemParams {
		protected readonly T Params;

		protected readonly CoreSpawnHelper SpawnHelper;

		float _leftReloadTime;

		public virtual bool CanShoot => (_leftReloadTime <= 0);

		protected BaseShootingSystem(CoreSpawnHelper spawnHelper, T shootingParams) {
			Params      = shootingParams;
			SpawnHelper = spawnHelper;
		}

		public override bool TryShoot() {
			if ( !CanShoot ) {
				return false;
			}
			Shoot();
			ForceRecharge();
			return true;
		}

		public override void ForceRecharge() {
			_leftReloadTime = Params.ReloadTime;
		}

		public override void DeltaTick() {
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