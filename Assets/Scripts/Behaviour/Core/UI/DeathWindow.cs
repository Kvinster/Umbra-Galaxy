using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Sound;
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
		[NotNull] public Button     ContinueButton;

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
		}

		public override IPromise Show() {
			var livesLeft = _playerController.CurLives;
			LivesText.text = string.Format(LivesTextFormat, livesLeft);

			ContinueButton.gameObject.SetActive(livesLeft > 0);

			return base.Show();
		}

		void OnQuitClick() {
			PersistentAudioPlayer.Instance.SetPitch(1f);
			Hide();
			_levelManager.QuitToMenu();
		}

		void OnContinueClick() {
			Hide();
			PersistentAudioPlayer.Instance.SetPitch(1f);
			_playerManager.Respawn();
		}
	}
}
