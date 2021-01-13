using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;

using STP.Core;
using STP.Core.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class WinWindow : GameComponent {
		const string StatsFormat = "Lives: {0}\nXP: {1}";

		[NotNull] public TMP_Text StatsText;
		[NotNull] public Button   GoToMenuButton;

		PlayerController _playerController;
		XpController     _xpController;

		Promise _showPromise;

		public void CommonInit(PlayerController playerController, XpController xpController) {
			_playerController = playerController;
			_xpController     = xpController;

			GoToMenuButton.onClick.AddListener(OnGoToMenuClick);
		}

		public IPromise Show() {
			if ( _showPromise != null ) {
				return Promise.Rejected(new Exception("WinWindow is already shown"));
			}

			StatsText.text = string.Format(StatsFormat, _playerController.CurLives, _xpController.CurTotalXp);

			gameObject.SetActive(true);

			_showPromise = new Promise();
			return _showPromise;
		}

		void OnGoToMenuClick() {
			var profileName = GameState.ActiveInstance.ProfileName;
			GameState.TryReleaseActiveInstance();
			GameState.TryRemoveSave(profileName);
			SceneManager.LoadScene("MainMenu");
		}
	}
}
