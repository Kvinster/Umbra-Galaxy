﻿using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core;
using STP.Common;
using STP.Core;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;

using Object = UnityEngine.Object;

namespace STP.Manager {
	public sealed class PlayerManager {
		static readonly Dictionary<PowerUpType, float> PowerUpTypeTimes = new Dictionary<PowerUpType, float> {
			{ PowerUpType.X2Xp, 10f },
			{ PowerUpType.Shield, 10f },
			{ PowerUpType.IncFireRate, 10f },
			{ PowerUpType.X2Damage, 10f },
		};

		Player _player;
		readonly PlayerController _playerController;
		readonly XpController     _xpController;

		readonly UnityContext _context;

		readonly List<PowerUpState> _powerUpStates = new List<PowerUpState>();

		readonly Transform _tempObjectsRoot;

		public event Action<PowerUpType> OnPowerUpStarted;
		public event Action<PowerUpType> OnPowerUpFinished;

		public PlayerManager(Player player, PlayerController playerController, XpController xpController, UnityContext context, Transform tempObjectsRoot) {
			_player           = player;
			_playerController = playerController;
			_context          = context;
			_xpController     = xpController;
			_tempObjectsRoot  = tempObjectsRoot;
			_context.AddUpdateCallback(UpdateTimers);
			_playerController.OnRespawn();
			EventManager.Subscribe<PlayerShipChanged>(OnPlayerChanged);
			EventManager.Subscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		public void Deinit() {
			if ( _context ) {
				_context.RemoveUpdateCallback(UpdateTimers);
			}
			EventManager.Unsubscribe<PlayerShipChanged>(OnPlayerChanged);
			EventManager.Unsubscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		public bool TryPickupPowerUp(PowerUpType powerUpType) {
			return _playerController.TryPickupPowerUp(powerUpType);
		}

		public bool TryUsePowerUp(PowerUpType powerUpType) {
			if ( !_playerController.TryUsePowerUp(powerUpType) ) {
				return false;
			}
			HandlePowerUpStart(powerUpType);
			return true;
		}

		public List<PowerUpState> GetAllActivePowerUpStates() {
			return _powerUpStates;
		}

		public float GetPowerUpCurTime(PowerUpType powerUpType) {
			var powerUp = _powerUpStates.Find(x => x.Type == powerUpType);
			return powerUp?.TimeLeft ?? -1f;
		}

		public float GetPowerUpTotalTime(PowerUpType powerUpType) {
			if ( PowerUpTypeTimes.TryGetValue(powerUpType, out var time) ) {
				return time;
			}
			Debug.LogErrorFormat("No time for power up type '{0}'", powerUpType.ToString());
			return -1f;
		}

		public bool HasActivePowerUp(PowerUpType type) {
			return _powerUpStates.Find(x => x.Type == type) != null;
		}

		public bool OnPlayerDied() {
			if ( !_playerController.TrySubLives(1) ) {
				Debug.LogError("Can's sub life");
			}
			return (_playerController.CurLives > 0);
		}

		public void Respawn() {
			_playerController.OnRespawn();
			for ( var i = _powerUpStates.Count - 1; i >= 0; i-- ) {
				HandlePowerUpFinish(_powerUpStates[i]);
			}

			if ( _tempObjectsRoot ) {
				for ( var i = _tempObjectsRoot.childCount - 1; i >= 0; i-- ) {
					Object.Destroy(_tempObjectsRoot.GetChild(i).gameObject);
				}
			}

			_player.OnRespawn();
		}

		public void Restart() {
			_playerController.OnRespawn();
			_playerController.RestoreLives();
			_player.OnRestart();
		}

		void OnPlayerChanged(PlayerShipChanged e) {
			_player = e.NewPlayer;
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

		void HandlePowerUpStart(PowerUpType powerUpType) {
			switch ( powerUpType ) {
				case PowerUpType.Shield: {
					_playerController.IsInvincible = true;
					AddTimeToPowerUp(powerUpType, PowerUpTypeTimes[PowerUpType.Shield]);
					break;
				}
				case PowerUpType.X2Xp:
				case PowerUpType.IncFireRate:
				case PowerUpType.X2Damage: {
					AddTimeToPowerUp(powerUpType, PowerUpTypeTimes[powerUpType]);
					break;
				}
				case PowerUpType.RestoreHp: {
					_playerController.RestoreHp();
					break;
				}
				default: {
					Debug.LogErrorFormat("Unsupported power up type '{0}'", powerUpType.ToString());
					break;
				}
			}
		}

		void AddTimeToPowerUp(PowerUpType type, float time) {
			var powerUpTimer = _powerUpStates.Find(x => x.Type == type);
			if ( powerUpTimer != null ) {
				powerUpTimer.AddTime(time);
			} else {
				_powerUpStates.Add(new PowerUpState(type, time));
				OnPowerUpStarted?.Invoke(type);
			}
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
				default: {
					Debug.LogErrorFormat("Unsupported power up type '{0}'", powerUpType);
					break;
				}
			}
		}

		void HandlePowerUpFinish(PowerUpState powerUpState) {
			var powerUpName = powerUpState.Type;
			switch ( powerUpName ) {
				case PowerUpType.X2Xp:
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

		void OnEnemyDestroyed(EnemyDestroyed e) {
			var xpAmount = _xpController.GetDestroyedEnemyXp(e.EnemyName);
			if ( HasActivePowerUp(PowerUpType.X2Xp) ) {
				xpAmount *= 2;
			}
			_xpController.AddLevelXp(xpAmount);
		}
	}
}
