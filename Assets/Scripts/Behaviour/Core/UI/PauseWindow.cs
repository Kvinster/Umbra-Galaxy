using UnityEngine.UI;

using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class PauseWindow : BaseCoreWindow {
		const string InfoTextTemplate = "Lives: {0}\nXP: {1}\nGoal: {2}";

		[NotNull] public TMP_Text InfoText;
		[NotNull] public Button   ResumeButton;
		[NotNull] public Button   QuitButton;

		LevelManager     _levelManager;
		LevelGoalManager _levelGoalManager;
		XpController     _xpController;
		PlayerController _playerController;

		public void CommonInit(LevelManager levelManager, LevelGoalManager levelGoalManager, XpController xpController,
			PlayerController playerController) {
			_levelManager     = levelManager;
			_levelGoalManager = levelGoalManager;
			_xpController     = xpController;
			_playerController = playerController;

			ResumeButton.onClick.AddListener(OnResumeClick);
			QuitButton.onClick.AddListener(OnQuitClick);
		}

		public override IPromise Show() {
			InfoText.text = string.Format(InfoTextTemplate, _playerController.CurLives, _xpController.Xp,
				$"{_levelGoalManager.CurLevelGoalProgress}/{_levelGoalManager.LevelGoal}");
			return base.Show();
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
