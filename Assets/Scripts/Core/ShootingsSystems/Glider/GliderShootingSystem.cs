using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems.Glider {
	public sealed class GliderShootingSystem : BaseShootingSystem<GliderShootingSystemParams> {
		Transform _target;
		
		public override    bool CanShoot => base.CanShoot && IsTargetInRange;
		bool IsTargetInRange {
			get {
				if ( !_target ) {
					return false;
				}
				var distance = Vector2.Distance(_target.position, Params.BulletOrigin.position);
				return (distance >= Params.MinAttackDistance) && (distance <= Params.MaxAttackDistance);
			}
		}

		public GliderShootingSystem(CoreSpawnHelper spawnHelper, GliderShootingSystemParams shootingParams) : base(spawnHelper, shootingParams) { }

		public void SetTarget(Transform target) {
			_target = target;
		}
		
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