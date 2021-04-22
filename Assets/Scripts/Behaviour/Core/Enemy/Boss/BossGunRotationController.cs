using UnityEngine;
using UnityEngine.Assertions;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.Boss {
	public sealed class BossGunRotationController : GameComponent {
		[Header("Parameters")]
		public float GunRotationSpeed;
		[Header("Dependencies")]
		[NotNull] public Transform GunTransform;

		Transform _target;

		bool _isActive;

		public bool IsActive {
			get => _isActive;
			set {
				if ( value ) {
					Assert.IsTrue(HasTarget);
				}
				_isActive = value;
			}
		}

		public bool HasTarget => _target;

		public bool IsPointedAtTarget => HasTarget && Mathf.Approximately(AngleToTarget, 0f);

		float AngleToTarget => HasTarget
			? Vector2.SignedAngle(_target.position - GunTransform.position, GunTransform.TransformVector(Vector2.up))
			: 0f;

		public void SetTarget(Transform target) {
			_target = target;
		}

		void Update() {
			if ( IsActive && !IsPointedAtTarget ) {
				var targetAngle = AngleToTarget;
				var step        = GunRotationSpeed * Time.deltaTime;
				if ( step >= Mathf.Abs(targetAngle) ) {
					GunTransform.rotation = Quaternion.Euler(0f, 0f, GunTransform.rotation.eulerAngles.z + targetAngle);
				} else {
					GunTransform.rotation = Quaternion.Euler(0f, 0f, GunTransform.rotation.eulerAngles.z + step *
						((targetAngle > 0) ? -1 : 1));
				}
			}
		}
	}
}
