using UnityEngine;

using STP.Behaviour.Core;
using STP.Controller;

namespace STP.Manager {
	public sealed class PlayerManager {
		readonly Player           _player;
		readonly PlayerController _playerController;

		public PlayerManager(Player player, PlayerController playerController) {
			_player           = player;
			_playerController = playerController;
		}

		public void Respawn() {
			if ( !_playerController.TrySubLives() ) {
				Debug.LogError("Can't subtract live");
			}
			_playerController.RestoreHp();
			_player.OnRespawn();
		}

		public void Restart() {
			_playerController.RestoreHp();
			_playerController.RestoreLives();
			_player.OnRestart();
		}
	}
}
