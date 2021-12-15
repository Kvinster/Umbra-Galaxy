using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Manager;

namespace STP.Behaviour.Core {
	public sealed class PowerUpActiveSoundPlayer : BaseCoreComponent {
		public PowerUpType PowerUpType;
		public AudioSource AudioSource;

		PlayerManager _playerManager;

		protected override void InitInternal(CoreStarter starter) {
			_playerManager = starter.PlayerManager;

			_playerManager.OnPowerUpStarted += OnPowerUpStarted;
			_playerManager.OnPowerUpFinished += OnPowerUpFinished;
		}

		void OnPowerUpStarted(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType ) {
				return;
			}
			AudioSource.Play();
		}

		void OnPowerUpFinished(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType ) {
				return;
			}
			AudioSource.Stop();
		}
	}
}
