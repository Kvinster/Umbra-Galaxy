﻿using System;
using System.Collections.Generic;
using STP.Behaviour.Core;
using STP.Common;

namespace STP.Core {
	public sealed class PlayerController : BaseStateController {
		const int StartPlayerLives = 3;

		public ShipType Ship;

		public readonly HpSystem HpSystem;

		readonly UpgradesController _upgradesController;

		int  _curLives;
		bool _isInvincible;

		readonly Dictionary<PowerUpType, bool> _powerUpStates = new Dictionary<PowerUpType, bool>();

		public int CurLives {
			get => _curLives;
			private set {
				if ( CurLives == value ) {
					return;
				}

				_curLives = value;
				OnCurLivesChanged?.Invoke(CurLives);
			}
		}

		public bool IsInvincible {
			get => _isInvincible;
			set {
				if ( _isInvincible == value ) {
					return;
				}
				_isInvincible = value;
				OnIsInvincibleChanged?.Invoke(_isInvincible);
			}
		}

		public event Action<int>               OnCurLivesChanged;
		public event Action<bool>              OnIsInvincibleChanged;
		public event Action<PowerUpType, bool> OnPowerUpStateChanged;
		public event Action                    OnRespawned;

		public PlayerController(UpgradesController upgradesController) {
			_upgradesController = upgradesController;

			CurLives     = StartPlayerLives;
			HpSystem     = new HpSystem(upgradesController.GetCurConfigMaxHp());
			IsInvincible = false;

			foreach ( var powerUpType in PowerUpTypeHelper.PowerUpTypes ) {
				_powerUpStates.Add(powerUpType, false);
			}
		}

		public void OnLevelStart() {
			HpSystem.SetMaxHp(_upgradesController.GetCurConfigMaxHp());
		}

		public void TakeDamage(float damage) {
			if ( IsInvincible ) {
				return;
			}
			HpSystem.TakeDamage(damage);
		}

		public void Respawn() {
			IsInvincible = false;
			RestoreHp();
			foreach ( var powerUpType in PowerUpTypeHelper.PowerUpTypes ) {
				SetPowerUpState(powerUpType, false);
			}
			OnRespawned?.Invoke();
		}

		public void RestoreHp() {
			HpSystem.ResetHp();
		}

		public void AddHp(float hp) {
			if ( !HpSystem.IsAlive ) {
				return;
			}
			HpSystem.AddHp(hp);
		}

		public void RestoreLives() {
			CurLives = StartPlayerLives;
		}

		public bool TrySubLives(int subLives = 1) {
			if ( CurLives >= subLives ) {
				CurLives -= subLives;
				return true;
			}

			return false;
		}

		public void AddLives(int addLives = 1) {
			CurLives += addLives;
		}

		public bool TryPickupPowerUp(PowerUpType powerUpType) {
			if ( !GetPowerUpState(powerUpType) ) {
				SetPowerUpState(powerUpType, true);
				return true;
			}
			return false;
		}

		public bool TryUsePowerUp(PowerUpType powerUpType) {
			if ( GetPowerUpState(powerUpType) ) {
				SetPowerUpState(powerUpType, false);
				return true;
			}
			return false;
		}

		public bool GetPowerUpState(PowerUpType powerUpType) {
			return _powerUpStates[powerUpType];
		}

		void SetPowerUpState(PowerUpType powerUpType, bool state) {
			if ( GetPowerUpState(powerUpType) == state ) {
				return;
			}
			_powerUpStates[powerUpType] = state;
			OnPowerUpStateChanged?.Invoke(powerUpType, state);
		}
	}
}