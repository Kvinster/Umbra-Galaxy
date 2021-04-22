using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems {
	public abstract class ShootingSystem<T> where T : ShootingSystemParams {
		protected readonly T Params;
		
		readonly CoreSpawnHelper _spawnHelper;

		float _leftReloadTime;

		public virtual bool CanShoot => (_leftReloadTime <= 0);

		public ShootingSystem(CoreSpawnHelper spawnHelper, T shootingParams) {
			Params       = shootingParams;
			_spawnHelper = spawnHelper;
		}
		
		public bool TryShoot() {
			if ( !CanShoot ) {
				return false;
			}
			Shoot();
			_leftReloadTime = Params.ReloadTime;
			return true;
		}

		public void DeltaTick() {
			_leftReloadTime -= Time.deltaTime;
		}
		
		void Shoot() {
			var angle = Params.RotationSource.rotation.eulerAngles.z;
			var bulletGo = Object.Instantiate(Params.BulletPrefab, Params.BulletOrigin.position, Quaternion.AngleAxis(angle, Vector3.forward));
			var bullet   = bulletGo.GetComponent<IBullet>();
			// Init custom bullet component if necessary
			bullet?.Init(Params.BulletDamage, Params.BulletSpeed, Params.IgnoreColliders);
			// Init all other BaseCoreComponent components
			_spawnHelper.TryInitSpawnedObject(bulletGo);
		}
	}
	
	public sealed class ShootingSystem : ShootingSystem<ShootingSystemParams> {
		public ShootingSystem(CoreSpawnHelper spawnHelper, ShootingSystemParams shootingParams) : base(spawnHelper,
			shootingParams) { }
	}
}