using DG.Tweening;
using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
	public sealed class RotationEffect : GameComponent {
		public float CircleTime;

		Sequence _seq;

		public void ShowEffect() {
			gameObject.SetActive(true);
		}

		public void HideEffect() {
			gameObject.SetActive(false);
		}

		void OnEnable() {
			_seq?.Kill();
			_seq = DOTween.Sequence();
			var endRotation = transform.rotation.eulerAngles + Vector3.forward * 360;
			_seq.Append(transform.DORotate(endRotation, CircleTime, RotateMode.FastBeyond360))
				.SetAutoKill(false)
				.SetLoops(-1);
		}

		void OnDisable() {
			_seq?.Kill();
		}
	}
}