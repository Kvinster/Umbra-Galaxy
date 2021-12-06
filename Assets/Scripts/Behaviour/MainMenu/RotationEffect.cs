using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
	public sealed class RotationEffect : GameComponent {
		public float CircleTime;

		public void ShowEffect() {
			gameObject.SetActive(true);
		}

		public void HideEffect() {
			gameObject.SetActive(false);
		}

		void Update() {
			if ( gameObject.activeSelf ) {
				transform.Rotate(Vector3.forward * (360f / CircleTime) * Time.unscaledDeltaTime);
			}
		}
	}
}