using UnityEngine;
using UnityEngine.Assertions;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class DangerScreen : GameComponent {
		[NotNull] public CanvasGroup CanvasGroup;

		Tween _anim;

		void OnDestroy() {
			_anim?.Kill();
		}

		public void Init() {
			CanvasGroup.alpha = 0f;
			gameObject.SetActive(false);
		}

		public Tween Show(float duration) {
			Assert.IsNull(_anim);
			gameObject.SetActive(true);
			CanvasGroup.alpha = 0f;
			return (_anim = CanvasGroup.DOFade(1f, duration).OnComplete(() => _anim = null));
		}

		public Tween Hide(float duration) {
			Assert.IsNull(_anim);
			CanvasGroup.alpha = 1f;
			return (_anim = DOTween.Sequence()
				.Append(CanvasGroup.DOFade(0f, duration))
				.OnComplete(() => {
					gameObject.SetActive(false);
					_anim = null;
				})
				.SetUpdate(UpdateType.Manual));
		}
	}
}
