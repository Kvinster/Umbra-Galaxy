using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class PowerUpButton : BaseCoreComponent {
		public PowerUpType PowerUpType;
		public KeyCode     HotKeyCode;

		[NotNull] public TMP_Text    PowerUpTypeText;
		[NotNull] public Button      Button;
		[NotNull] public CanvasGroup CanvasGroup;

		Player           _player;
		PlayerManager    _playerManager;
		PlayerController _playerController;

		void Update() {
			if ( Input.GetKeyDown(HotKeyCode) ) {
				OnButtonClick();
			}
		}

		void OnDestroy() {
			if ( _player ) {
				_player.OnPlayerRespawn -= OnPlayerRespawn;
			}
			if ( _playerController != null ) {
				_playerController.OnPowerUpStateChanged -= OnPowerUpStateChanged;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_player           = starter.Player;
			_playerManager    = starter.PlayerManager;
			_playerController = starter.PlayerController;

			PowerUpTypeText.text = PowerUpType.ToString();

			Button.onClick.AddListener(OnButtonClick);

			_player.OnPlayerRespawn                 += OnPlayerRespawn;
			_playerController.OnPowerUpStateChanged += OnPowerUpStateChanged;
			OnPlayerRespawn();
		}

		void OnPlayerRespawn() {
			SetEnabled(false);
		}

		void OnPowerUpStateChanged(PowerUpType powerUpType, bool state) {
			if ( powerUpType != PowerUpType ) {
				return;
			}
			SetEnabled(state);
		}

		void SetEnabled(bool isEnabled) {
			CanvasGroup.alpha        = isEnabled ? 1f : 0.5f;
			CanvasGroup.interactable = isEnabled;
			Button.enabled           = isEnabled;
		}

		void OnButtonClick() {
			_playerManager.TryUsePowerUp(PowerUpType);
		}
	}
}
