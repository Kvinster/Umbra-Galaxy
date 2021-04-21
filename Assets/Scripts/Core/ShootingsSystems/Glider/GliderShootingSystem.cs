using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems.Glider {
	public sealed class GliderShootingSystem : ShootingSystem<GliderShootingSystemParams> {
		Transform _target;
		
		public override bool CanShoot => base.CanShoot && IsTargetInRange;

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
	}
}