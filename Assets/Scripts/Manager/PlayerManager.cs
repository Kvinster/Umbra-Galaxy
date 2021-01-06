using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core;
using STP.Common;
using STP.Controller;
using STP.Events;
using STP.State;
using STP.Utils;
using STP.Utils.Events;

namespace STP.Manager {
	public sealed class PlayerManager {
		readonly Player           _player;
		readonly PlayerController _playerController;
		readonly XpController     _xpController;

		readonly UnityContext _context;

		readonly List<PowerUpState> _powerUpStates = new List<PowerUpState>();

		public PlayerManager(Player player, PlayerController playerController, XpController xpController, UnityContext context) {
			_player           = player;
			_playerController = playerController;
			_context          = context;
			_xpController     = xpController;
			_context.AddUpdateCallback(UpdateTimers);
			EventManager.Subscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		public void Deinit() {
			if ( _context ) {
				_context.RemoveUpdateCallback(UpdateTimers);
			}
			EventManager.Unsubscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		public void AddTimeToPowerUp(string name, float time) {
			var powerUpTimer = _powerUpStates.Find(x => x.Name == name);
			if ( powerUpTimer != null ) {
				powerUpTimer.AddTime(time);
			} else {
				_powerUpStates.Add(new PowerUpState(name, time));
			}
		}

		public List<PowerUpState> GetAllActivePowerUpStates() {
			return _powerUpStates;
		}

		public float GetPowerUpTime(string powerUpName) {
			var powerUp = _powerUpStates.Find(x => x.Name == powerUpName);
			return powerUp?.TimeLeft ?? -1f;
		}

		bool HasActivePowerUp(string name) {
			return _powerUpStates.Find(x => x.Name == name) != null;
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

		void UpdateTimers(float deltaTime) {
			for ( var i = _powerUpStates.Count - 1; i >= 0; i-- ) {
				var powerUp = _powerUpStates[i];
				if ( powerUp.Tick(deltaTime) ) {
					HandlePowerUpFinish(powerUp);
				}
			}
		}

		void HandlePowerUpFinish(PowerUpState powerUpState) {
			var powerUpName = powerUpState.Name;
			switch ( powerUpName ) {
				case PowerUpNames.X2Xp: {
					break;
				}
				case PowerUpNames.Shield: {
					_playerController.IsInvincible = false;
					break;
				}
				default: {
					Debug.LogErrorFormat("Unsupported power up name '{0}'", powerUpName);
					break;
				}
			}
			_powerUpStates.Remove(powerUpState);
		}

		void OnEnemyDestroyed(EnemyDestroyed e) {
			var xpAmount = _xpController.GetDestroyedEnemyXp(e.EnemyName);
			if ( HasActivePowerUp(PowerUpNames.X2Xp) ) {
				xpAmount *= 2;
			}
			_xpController.AddXp(xpAmount);
		}
	}
}
