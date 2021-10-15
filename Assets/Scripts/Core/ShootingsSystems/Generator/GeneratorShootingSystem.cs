using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core.ShootingsSystems.Generator {
	public sealed class GeneratorShootingSystem : BaseShootingSystem<GeneratorShootingSystemParams> {
		readonly Transform _generatorTransform;
		Transform _playerTransform;

		public GeneratorShootingSystem(CoreSpawnHelper spawnHelper, GeneratorShootingSystemParams shootingParams,
			Transform generatorTransform) : base(spawnHelper, shootingParams) {
			_generatorTransform = generatorTransform;
		}

		protected override void Shoot() {
			if ( !_playerTransform ) {
				// Can't shoot cause not added target transform
				return;
			}
			var playerDistance = (_generatorTransform.position - _playerTransform.position).magnitude;
			var speed = InterpolateSpeed(Params.BulletSpeed, Params.MinBulletSpeed, Params.MaxSpeedDegradationDistance,
				playerDistance);
			var angle = Params.RotationSource.rotation.eulerAngles.z;
			var bulletGo = Object.Instantiate(Params.BulletPrefab, Params.BulletOrigin.position, Quaternion.AngleAxis(angle, Vector3.forward));
			var bullet   = bulletGo.GetComponent<IBullet>();
			// Init custom bullet component if necessary
			bullet?.Init(Params.BulletDamage, speed, Params.IgnoreColliders);
			// Init all other BaseCoreComponent components
			SpawnHelper.TryInitSpawnedObject(bulletGo);
		}

		float InterpolateSpeed(float maxSpeed, float minSpeed, float maxDistance, float curDistance) {
			// Ignore speed coeff if distance is exceeded max distance
			curDistance = Mathf.Min(curDistance, maxDistance);
			var res = (maxSpeed - minSpeed) * (curDistance - 0) / (maxDistance - 0) + minSpeed;
			return res;
		}

		public void SetTarget(Transform target) {
			_playerTransform = target;
		}
	}
}