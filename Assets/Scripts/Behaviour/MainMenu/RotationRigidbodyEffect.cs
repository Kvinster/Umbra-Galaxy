using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
	public sealed class RotationRigidbodyEffect : GameComponent {
		public float CircleTime;

		public Rigidbody2D Rigidbody2D;
		
		public void ShowEffect() {
			gameObject.SetActive(true);
		}

		public void HideEffect() {
			gameObject.SetActive(false);
		}

		void FixedUpdate() {
			if ( gameObject.activeSelf ) {
				var newAngle = Rigidbody2D.rotation + 360f / CircleTime * Time.unscaledDeltaTime;
				Rigidbody2D.MoveRotation(newAngle);
			}
		}
	}
}