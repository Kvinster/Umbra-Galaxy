using System;
using DG.Tweening;
using UnityEngine.UI;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
	public sealed class QuitScreen : GameComponent, IScreen {
		const float AnimationDuration = 0.5f;
		
		[NotNull] public Button CancelButton;
		[NotNull] public Button CrossButton;
		[NotNull] public Button ExitButton;

		[NotNull] public Transform AnimationBottomPosition;
		[NotNull] public Transform WindowRoot;

		[NotNull] public CanvasGroup BackgroundCanvasGroup;
		
		IScreenShower _screenShower;

		Sequence _activeSequence;
		
		void OnDestroy() {
			_activeSequence?.Kill(true);
		}

		public void Init(IScreenShower screenShower) {
			_screenShower = screenShower;
			CancelButton.onClick.AddListener(_screenShower.Show<MainScreen>);
			CrossButton.onClick.AddListener(_screenShower.Show<MainScreen>);
			
			ExitButton.onClick.AddListener(Exit);
		}

		public void Show() {
			gameObject.SetActive(true);

			WindowRoot.position         = AnimationBottomPosition.position;
			BackgroundCanvasGroup.alpha = 0f;
			
			_activeSequence?.Kill(true);
			_activeSequence = DOTween.Sequence()
				.Append(WindowRoot.DOMove(Vector3.zero, AnimationDuration))
				.Join(BackgroundCanvasGroup.DOFade(1f, AnimationDuration))
				.SetEase(Ease.OutSine);
		}

		public void Hide() {
			if ( !gameObject.activeSelf ) {
				return;
			}
			
			WindowRoot.position         = Vector3.zero;
			BackgroundCanvasGroup.alpha = 0f;
			
			_activeSequence?.Kill(true);
			_activeSequence = DOTween.Sequence()
				.Append(WindowRoot.DOMove(AnimationBottomPosition.position, AnimationDuration))
				.Join(BackgroundCanvasGroup.DOFade(0f, AnimationDuration))
				.AppendCallback(() => gameObject.SetActive(false));
		}

		void Exit() {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
