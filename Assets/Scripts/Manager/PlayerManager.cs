using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core;
using STP.Common;
using STP.Core;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;

namespace STP.Manager {
	public sealed class PlayerManager {
		const float HealPowerPerSecond = 10;

		readonly Player           _player;
		readonly PlayerController _playerController;
		readonly XpController     _xpController;

		readonly UnityContext _context;

		readonly List<PowerUpState> _powerUpStates = new List<PowerUpState>();

		public event Action<PowerUpType> OnPowerUpStarted;
		public event Action<PowerUpType> OnPowerUpFinished;

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

		public void AddTimeToPowerUp(PowerUpType type, float time) {
			var powerUpTimer = _powerUpStates.Find(x => x.Type == type);
			if ( powerUpTimer != null ) {
				powerUpTimer.AddTime(time);
			} else {
				_powerUpStates.Add(new PowerUpState(type, time));
				OnPowerUpStarted?.Invoke(type);
			}
		}

		public List<PowerUpState> GetAllActivePowerUpStates() {
			return _powerUpStates;
		}

		public float GetPowerUpTime(PowerUpType type) {
			var powerUp = _powerUpStates.Find(x => x.Type == type);
			return powerUp?.TimeLeft ?? -1f;
		}

		public bool HasActivePowerUp(PowerUpType type) {
			return _powerUpStates.Find(x => x.Type == type) != null;
		}

		public bool OnPlayerDied() {
			_playerController.TrySubLives(1);
			return (_playerController.CurLives > 0);
		}

		public void Respawn() {
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
				else {
					HandlePowerUpProgress(powerUp, deltaTime);
				}
			}
		}

		void HandlePowerUpFinish(PowerUpState powerUpState) {
			var powerUpName = powerUpState.Type;
			switch ( powerUpName ) {
				case PowerUpType.X2Xp:
				case PowerUpType.Heal:
				case PowerUpType.X2Damage:
				case PowerUpType.IncFireRate: {
					break;
				}
				case PowerUpType.Shield: {
					_playerController.IsInvincible = false;
					break;
				}
				default: {
					Debug.LogErrorFormat("Unsupported power up name '{0}'", powerUpName);
					break;
				}
			}
			_powerUpStates.Remove(powerUpState);
			OnPowerUpFinished?.Invoke(powerUpName);
		}

		void HandlePowerUpProgress(PowerUpState state, float timePassed) {
			var powerUpType = state.Type;
			switch ( powerUpType ) {
				case PowerUpType.X2Xp:
				case PowerUpType.Shield:
				case PowerUpType.X2Damage:
				case PowerUpType.IncFireRate: {
					break;
				}
				case PowerUpType.Heal: {
					var hpToAdd = HealPowerPerSecond * timePassed;
					_playerController.AddHp(hpToAdd);
					break;
				}
				default: {
					Debug.LogErrorFormat("Unsupported power up type '{0}'", powerUpType);
					break;
				}
			}
		}

		void OnEnemyDestroyed(EnemyDestroyed e) {
			var xpAmount = _xpController.GetDestroyedEnemyXp(e.EnemyName);
			if ( HasActivePowerUp(PowerUpType.X2Xp) ) {
				xpAmount *= 2;
			}
			_xpController.AddLevelXp(xpAmount);
		}
	}
}
