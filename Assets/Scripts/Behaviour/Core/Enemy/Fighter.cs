using UnityEngine;

using STP.Behaviour.Starter;
using STP.Core.ShootingsSystems;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Fighter : BaseEnemy, IDestructible {
		public ShootingSystemParams ShootingParams;
		[Space]
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;

		[Header("Sound")]
		[NotNull]
		public BaseSimpleSoundPlayer ShotSoundPlayer;

		Transform       _target;

		DefaultShootingSystem  _defaultShootingSystem;

		void Update() {
			if ( !IsInit ) {
				return;
			}
			_defaultShootingSystem.DeltaTick();
			if ( !_target ) {
				return;
			}
			if ( _defaultShootingSystem.TryShoot() ) {
				ShotSoundPlayer.Play();
			}
			var dirRaw = _target.position - transform.position;
			Rigidbody.rotation += MathUtils.GetSmoothRotationAngleOffset(transform.up, dirRaw, RotationSpeed);
		}

		void FixedUpdate() {
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( other.TryTakeDamage(20) ) {
				Die();
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_defaultShootingSystem = new DefaultShootingSystem(starter.SpawnHelper, ShootingParams);
			HpSystem.OnDied += DieFromPlayer;
		}

		public void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
		}

		public override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			Destroy(gameObject);
		}

		public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
			SetTarget(playerTransform);
		}

		public override void OnBecomeInvisibleForPlayer() {
			// Do nothing
		}

		public override void SetTarget(Transform target) {
			_target = target;
		}

		void DieFromPlayer() {
			Die();
		}
	}
}
