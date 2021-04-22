using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.Boss {
	public sealed class BossMoveAgent : GameComponent {
		[Header("Parameters")]
		public float TargetDistance;
		public float MoveSpeed;
		public float RotationSpeed;
		[Header("Dependencies")]
		[NotNull] public Rigidbody2D Rigidbody;

		Transform _target;

		public bool IsActive { get; set; }

		bool HaveTarget => _target;

		float DistanceToTarget => HaveTarget ? Vector2.Distance(_target.position, Rigidbody.position) : float.MaxValue;

		bool WithinDistanceToTarget => HaveTarget && (DistanceToTarget < TargetDistance);

		public void SetTarget(Transform target) {
			_target = target;
		}

		void FixedUpdate() {
			if ( !IsActive || !HaveTarget ) {
				return;
			}
			if ( !WithinDistanceToTarget ) {
				var diffDistance = DistanceToTarget - TargetDistance;
				var direction    = (Vector2) _target.position - Rigidbody.position;
				var step         = MoveSpeed * Time.fixedDeltaTime;
				if ( step > diffDistance ) {
					step = diffDistance;
				}
				Rigidbody.MovePosition(Rigidbody.position + direction.normalized * step);
			}

			{
				while ( Rigidbody.rotation > 180 ) {
					Rigidbody.rotation -= 360;
				}
				while ( Rigidbody.rotation < -180 ) {
					Rigidbody.rotation += 360;
				}
				Vector2 targetPos   = _target.position;
				var     targetAngle = Vector2.SignedAngle(Vector2.up, targetPos - Rigidbody.position);
				var     curAngle    = Rigidbody.rotation;
				var     diff        = Mathf.Abs(targetAngle - curAngle);
				while ( diff > 180 ) {
					if ( curAngle > targetAngle ) {
						targetAngle += 360;
					} else {
						targetAngle -= 360;
					}
					diff = Mathf.Abs(targetAngle - curAngle);
				}
				if ( diff <= RotationSpeed * Time.fixedDeltaTime ) {
					Rigidbody.rotation = targetAngle;
				} else {
					Rigidbody.rotation =
						curAngle + RotationSpeed * Time.fixedDeltaTime * ((curAngle > targetAngle) ? -1 : 1);
				}
			}
		}
	}
}
