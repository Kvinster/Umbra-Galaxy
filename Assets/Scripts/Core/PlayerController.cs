using UnityEngine;

using System;

using STP.Core.State;

namespace STP.Core {
	public sealed class PlayerController : BaseStateController {
		public const float MaxPlayerHp = 100f;

		const int   StartPlayerLives = 3;
		const float StartPlayerHp    = MaxPlayerHp;

		int   _curLives;
		float _curHp;
		bool  _isInvincible;
		bool  _isAlive;

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

		public float CurHp {
			get => _curHp;
			private set {
				if ( Mathf.Approximately(_curHp, value) ) {
					return;
				}

				_curHp = value;
				OnCurHpChanged?.Invoke(_curHp);
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

		public bool IsAlive {
			get => _isAlive;
			private set {
				if ( _isAlive == value ) {
					return;
				}
				_isAlive = value;
				OnIsAliveChanged?.Invoke(IsAlive);
			}
		}

		public event Action<int>   OnCurLivesChanged;
		public event Action<float> OnCurHpChanged;
		public event Action<bool>  OnIsInvincibleChanged;
		public event Action<bool>  OnIsAliveChanged;

		public PlayerController(ProfileState profileState) {
			CurLives     = StartPlayerLives;
			CurHp        = StartPlayerHp;
			IsInvincible = false;
		}

		public void TakeDamage(float damage) {
			if ( !IsAlive ) {
				Debug.LogError("Player taking damage when dead");
				return;
			}
			if ( IsInvincible ) {
				return;
			}
			CurHp = Mathf.Max(CurHp - damage, 0f);
			if ( Mathf.Approximately(CurHp, 0f) ) {
				IsAlive = false;
			}
		}

		public void OnRespawn() {
			IsAlive      = true;
			IsInvincible = false;
			RestoreHp();
		}

		public void RestoreHp() {
			CurHp = StartPlayerHp;
		}

		public void AddHp(float hp) {
			if ( hp < 0 ) {
				Debug.LogError($"Can't add negative hp {hp}");
				return;
			}
			if ( Mathf.Approximately(CurHp, MaxPlayerHp) ) {
				return;
			}
			CurHp = Mathf.Clamp(CurHp + hp, 0, MaxPlayerHp);
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
	}
}