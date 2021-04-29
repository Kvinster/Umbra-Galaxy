using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core.ShootingsSystems.Glider;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Glider : BaseEnemy, IDestructible {
		public GliderShootingSystemParams ShootingParams;
		public float           MovementSpeed;
		public float           RotationSpeed;
		[NotNull]
		public Rigidbody2D     Rigidbody;
		[Header("Sound")]
		[NotNull]
		public BaseSimpleSoundPlayer ShotSoundPlayer;

		Transform _target;

		GliderShootingSystem _shootingSystem;

		bool _rotateClockwise;

		float _reloadTimer;

		void Update() {
			if ( !IsInit ) {
				return;
			}
			_shootingSystem.DeltaTick();
			if ( _shootingSystem.TryShoot() ) {
				ShotSoundPlayer.Play();
			}
		}

		void FixedUpdate() {
			if ( !_target ) {
				return;
			}
			var targetPos = (Vector2) _target.position;
			var dirRaw    = targetPos - Rigidbody.position;
			Rigidbody.rotation += MathUtils.LerpFloat(0f, Vector2.SignedAngle(transform.up, dirRaw), RotationSpeed);

			var distance = Vector2.Distance(targetPos, Rigidbody.position);
			var curAngle = Vector2.SignedAngle(Vector2.right, (Rigidbody.position - targetPos));
			var nextAngle = (curAngle + RotationSpeed * Time.fixedDeltaTime * (_rotateClockwise ? -1 : 1)) *
			                Mathf.Deg2Rad;
			if ( distance > ShootingParams.MaxAttackDistance) {
				distance -= 0.5f * MovementSpeed * Time.fixedDeltaTime;
			} else if ( distance < ShootingParams.MinAttackDistance ) {
				distance += 0.5f * MovementSpeed * Time.fixedDeltaTime;
			}
			Rigidbody.MovePosition(targetPos + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * distance);
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_shootingSystem = new GliderShootingSystem(starter.SpawnHelper, ShootingParams);
			HpSystem.OnDied += DieFromPlayer;
			_rotateClockwise = (Random.Range(0, 2) == 1);
		}

		public void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
		}

		protected override void Die(bool fromPlayer = true) {
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
			if ( !IsInit ) {
				return;
			}
			_shootingSystem.SetTarget(target);
			_target = target;
		}

		void OnCollisionEnter2D(Collision2D other) {
			var player = other.gameObject.GetComponent<Player>();
			if ( player ) {
				player.TakeDamage(20);
				Die();
			}
		}
        
		void DieFromPlayer() {
			Die();
		}
	}
}
