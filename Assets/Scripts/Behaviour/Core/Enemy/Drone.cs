using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Drone : BaseEnemy, IDestructible {
		public float StartHp;
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public TriggerNotifier DetectRangeNotifier;

		Transform _target;

		float CurHp { get; set; }

		void Update() {
			if ( !_target ) {
				return;
			}
			var dirRaw = _target.position - transform.position;
			Rigidbody.MoveRotation(Rigidbody.rotation +
			                       MathUtils.GetSmoothRotationAngleOffset(transform.up, dirRaw, RotationSpeed));
		}

		void FixedUpdate() {
			if ( !_target ) {
				return;
			}
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			if ( destructible != null ) {
				destructible.TakeDamage(20);
				Die();
			}
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			CurHp = StartHp;

			DetectRangeNotifier.OnTriggerEnter += OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  += OnDetectRangeExit;
		}

		protected override void Die() {
			base.Die();
			DetectRangeNotifier.OnTriggerEnter -= OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  -= OnDetectRangeExit;

			Destroy(gameObject);
		}

		void OnDetectRangeEnter(GameObject other) {
			if ( other.GetComponent<Player>() ) {
				_target = other.transform;
			}
		}

		void OnDetectRangeExit(GameObject other) {
			if ( _target && (other.transform == _target) ) {
				_target = null;
			}
		}
	}
}
