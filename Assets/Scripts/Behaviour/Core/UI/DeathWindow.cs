﻿using UnityEngine;
using UnityEngine.UI;

using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class DeathWindow : BaseCoreWindow {
		const string LivesTextFormat = "Lives: {0}";

		[NotNull] public TMP_Text   LivesText;
		[NotNull] public Button     QuitButton;
		[NotNull] public GameObject HasLivesRoot;
		[NotNull] public Button     ContinueButton;
		[NotNull] public GameObject NoLivesRoot;
		[NotNull] public Button     RestartButton;

		LevelManager     _levelManager;
		PlayerManager    _playerManager;
		PlayerController _playerController;

		public void CommonInit(LevelManager levelManager, PlayerManager playerManager,
			PlayerController playerController) {
			_levelManager     = levelManager;
			_playerManager    = playerManager;
			_playerController = playerController;

			QuitButton.onClick.AddListener(OnQuitClick);
			ContinueButton.onClick.AddListener(OnContinueClick);
			RestartButton.onClick.AddListener(OnRestartClick);
		}

		public override IPromise Show() {
			var livesLeft = _playerController.CurLives;
			LivesText.text = string.Format(LivesTextFormat, livesLeft);

			HasLivesRoot.SetActive(livesLeft > 0);
			NoLivesRoot.SetActive(livesLeft <= 0);

			return base.Show();
		}

		void OnQuitClick() {
			Hide();
			_levelManager.QuitToMenu();
		}

		void OnContinueClick() {
			_playerManager.Respawn();
			Hide();
		}

		void OnRestartClick() {
			_playerManager.Restart();
			_levelManager.TryReloadLevel();
			Hide();
		}
	}
}
