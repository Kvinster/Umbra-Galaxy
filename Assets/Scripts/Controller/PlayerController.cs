using UnityEngine;

using System;

using STP.Utils;

namespace STP.Controller {
	public sealed class PlayerController : Singleton<PlayerController> {
		public const float MaxPlayerHp = 100f;

		const int   StartPlayerLives = 3;
		const float StartPlayerHp    = MaxPlayerHp;

		int   _curLives;
		float _curHp;
		bool  _isInvincible;

		public int CurLives {
			get => _curLives;
			private set {
				if ( _curLives == value ) {
					return;
				}

				_curLives = value;
				OnCurLivesChanged?.Invoke(_curLives);
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

		public event Action<int>   OnCurLivesChanged;
		public event Action<float> OnCurHpChanged;
		public event Action<bool>  OnIsInvincibleChanged;

		public PlayerController() {
			CurLives     = StartPlayerLives;
			CurHp        = StartPlayerHp;
			IsInvincible = false;
		}

		public bool TakeDamage(float damage) {
			if ( IsInvincible ) {
				return false;
			}
			CurHp = Mathf.Max(CurHp - damage, 0f);
			return Mathf.Approximately(CurHp, 0f);
		}

		public void RestoreHp() {
			CurHp = StartPlayerHp;
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