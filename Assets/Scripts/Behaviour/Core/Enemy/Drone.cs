using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Drone : BaseEnemy, IDestructible {
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;

		Transform _target;

		void Update() {
			if ( !_target ) {
				return;
			}
			var dirRaw = _target.position - transform.position;
			Rigidbody.MoveRotation(Rigidbody.rotation +
			                       MathUtils.GetSmoothRotationAngleOffset(transform.up, dirRaw, RotationSpeed));
		}

		void FixedUpdate() {
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			if ( destructible != null ) {
				destructible.TakeDamage(20);
				Die(fromPlayer: false);
			}
		}

		public void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			HpSystem.OnDied += DieFromPlayer;
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
			_target = target;
		}

		void DieFromPlayer() {
			Die();
		}
	}
}
