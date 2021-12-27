using UnityEngine;
using UnityEngine.UI;

using STP.Manager;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using RSG;

namespace STP.Behaviour.Core.UI {
	public sealed class DeathWindow : BaseCoreWindow {
		[NotNull] public Button      QuitButton;
		[NotNull] public CanvasGroup CanvasGroup;
		[Space]
		[NotNull] public JinglePlayer AppearSoundPlayer;

		LevelManager     _levelManager;

		public void CommonInit(LevelManager levelManager) {
			_levelManager = levelManager;

			QuitButton.onClick.AddListener(OnQuitClick);
		}

		public override IPromise Show() {
			var promise = base.Show();
			CanvasGroup.alpha = 0f;
			CanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
			AppearSoundPlayer.Play();
			return promise;
		}

		void OnQuitClick() {
			_levelManager.QuitToMenu();
		}
	}
}
