using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel.Enemies {
	public sealed class EndlessLevelDrone : BaseEnemy, IDestructible {
		[Space]
		public float StartHp;
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;

		EndlessLevelPlayer _target;

		float CurHp { get; set; }

		void Update() {
			if ( !_target ) {
				return;
			}
			var dirRaw = _target.transform.position - transform.position;
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
				Die(fromPlayer: false);
			}
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		protected override void InitInternal(EndlessLevelStarter starter) {
			base.InitInternal(starter);
			_target = starter.Player;
			CurHp   = StartHp;
		}

		protected override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);

			Destroy(gameObject);
		}
	}
}
