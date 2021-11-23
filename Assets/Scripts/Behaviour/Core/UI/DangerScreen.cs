using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class DangerScreen : GameComponent {
		[NotNull] public CanvasGroup CanvasGroup;

		public void Init() {
			CanvasGroup.alpha = 0f;
			gameObject.SetActive(false);
		}

		public Tween Show(float duration) {
			gameObject.SetActive(true);
			CanvasGroup.alpha = 0f;
			return CanvasGroup.DOFade(1f, duration);
		}

		public Tween Hide(float duration) {
			CanvasGroup.alpha = 1f;
			return DOTween.Sequence()
				.Append(CanvasGroup.DOFade(0f, duration))
				.OnComplete(() => gameObject.SetActive(false));
		}
	}
}
