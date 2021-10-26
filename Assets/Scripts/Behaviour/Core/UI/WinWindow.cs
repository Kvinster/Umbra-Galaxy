using UnityEngine;
using UnityEngine.UI;

using STP.Core;
using STP.Config;
using STP.Manager;
using STP.Service;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class WinWindow : BaseCoreWindow {
		const string StatsFormat = "Lives: {0}\nXP: {1}";

		[NotNull] public TMP_Text   StatsText;
		[NotNull] public Button     GoToMenuButton;
		[NotNull] public GameObject ContinueButtonRoot;
		[NotNull] public Button     ContinueButton;

		LevelManager     _levelManager;
		PlayerController _playerController;
		XpController     _xpController;
		LevelController  _levelController;

		public void CommonInit(LevelManager levelManager, PlayerController playerController,
			XpController xpController, LevelController levelController) {
			_levelManager     = levelManager;
			_playerController = playerController;
			_xpController     = xpController;
			_levelController  = levelController;

			GoToMenuButton.onClick.AddListener(OnGoToMenuClick);
			var hasNextLevel = levelManager.CurLevelIndex < LevelsConfig.Instance.TotalLevelsCount - 1;
			if ( hasNextLevel ) {
				ContinueButtonRoot.SetActive(true);
				ContinueButton.onClick.AddListener(OnContinueClick);
			} else {
				ContinueButtonRoot.SetActive(false);
			}
		}

		public override IPromise Show() {
			StatsText.text = string.Format(StatsFormat, _playerController.CurLives, _xpController.Xp);

			return base.Show();
		}

		void OnGoToMenuClick() {
			Hide();
			_levelManager.QuitToMenu();
		}

		void OnContinueClick() {
			_levelController.StartLevel(_levelManager.CurLevelIndex + 1);
			Hide();
			SceneService.LoadLevel(_levelManager.CurLevelIndex + 1);
		}
	}
}
