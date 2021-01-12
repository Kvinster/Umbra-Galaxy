using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;

using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class DeathWindow : GameComponent {
		const string LivesTextFormat = "Lives: {0}";

		[NotNull] public TMP_Text   LivesText;
		[NotNull] public Button     QuitButton;
		[NotNull] public GameObject HasLivesRoot;
		[NotNull] public Button     ContinueButton;
		[NotNull] public GameObject NoLivesRoot;
		[NotNull] public Button     RestartButton;

		PlayerManager    _playerManager;
		LevelGoalManager _levelGoalManager;
		XpController     _xpController;

		Promise _showPromise;

		public void CommonInit(PlayerManager playerManager, LevelGoalManager levelGoalManager, XpController xpController) {
			_playerManager    = playerManager;
			_levelGoalManager = levelGoalManager;
			_xpController     = xpController;

			QuitButton.onClick.AddListener(OnQuitClick);
			ContinueButton.onClick.AddListener(OnContinueClick);
			RestartButton.onClick.AddListener(OnRestartClick);
		}

		public IPromise Show(int livesLeft) {
			if ( _showPromise != null ) {
				return Promise.Rejected(new Exception("DeathWindow is already shown"));
			}

			LivesText.text = string.Format(LivesTextFormat, livesLeft);

			HasLivesRoot.SetActive(livesLeft > 0);
			NoLivesRoot.SetActive(livesLeft <= 0);

			gameObject.SetActive(true);

			_showPromise = new Promise();
			return _showPromise;
		}

		void Hide() {
			gameObject.SetActive(false);
			_showPromise.Resolve();
			_showPromise = null;
		}

		void OnQuitClick() {
			GameState.TryReleaseActiveInstance();
			SceneManager.LoadScene("MainMenu");
		}

		void OnContinueClick() {
			_playerManager.Respawn();
			Hide();
		}

		void OnRestartClick() {
			_playerManager.Restart();
			_levelGoalManager.LoseLevel();
			_xpController.ResetXp();
			Hide();
		}
	}
}
