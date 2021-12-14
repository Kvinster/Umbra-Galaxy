using UnityEngine.UI;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI {
	public sealed class PauseWindow : BaseCoreWindow {

		[NotNull] public Button   ResumeButton;
		[NotNull] public Button   QuitButton;

		LevelManager _levelManager;

		public void CommonInit(LevelManager levelManager) {
			_levelManager = levelManager;
			ResumeButton.onClick.AddListener(OnResumeClick);
			QuitButton.onClick.AddListener(OnQuitClick);
		}

		void OnResumeClick() {
			Hide();
		}

		void OnQuitClick() {
			Hide();
			_levelManager.QuitToMenu();
		}
	}
}
