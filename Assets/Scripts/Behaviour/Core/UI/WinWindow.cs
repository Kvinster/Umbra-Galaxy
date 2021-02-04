using UnityEngine.UI;

using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class WinWindow : BaseCoreWindow {
		const string StatsFormat = "Lives: {0}\nXP: {1}";

		[NotNull] public TMP_Text StatsText;
		[NotNull] public Button   GoToMenuButton;

		LevelManager     _levelManager;
		PlayerController _playerController;
		XpController     _xpController;

		public void CommonInit(LevelManager levelManager, PlayerController playerController,
			XpController xpController) {
			_levelManager     = levelManager;
			_playerController = playerController;
			_xpController     = xpController;

			GoToMenuButton.onClick.AddListener(OnGoToMenuClick);
		}

		public override IPromise Show() {
			StatsText.text = string.Format(StatsFormat, _playerController.CurLives, _xpController.CurTotalXp);

			return base.Show();
		}

		void OnGoToMenuClick() {
			Hide();
			_levelManager.QuitToMenu();
		}
	}
}
