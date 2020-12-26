using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Drone : BaseStarterCoreComponent, IDestructible {
		public int   StartHp;
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public TriggerNotifier DetectRangeNotifier;

		Transform _target;

		int CurHp { get; set; }

		void Update() {
			if ( !_target ) {
				return;
			}
			var dirRaw = _target.position - transform.position;
			Rigidbody.rotation += MathUtils.LerpFloat(0f, Vector2.SignedAngle(transform.up, dirRaw), RotationSpeed);
		}

		void FixedUpdate() {
			if ( !_target ) {
				return;
			}
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		protected override void InitInternal(CoreStarter starter) {
			CurHp = StartHp;

			DetectRangeNotifier.OnTriggerEnter += OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  += OnDetectRangeExit;
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(Mathf.CeilToInt(CurHp - damage), 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		void Die() {
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

		void OnCollisionEnter2D(Collision2D other) {
			var player = other.gameObject.GetComponent<Player>();
			if ( player ) {
				player.TakeDamage(20);
				Die();
			}
		}
	}
}
