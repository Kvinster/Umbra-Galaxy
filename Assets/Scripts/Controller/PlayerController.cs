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

		public event Action<int>   OnCurLivesChanged;
		public event Action<float> OnCurHpChanged;

		public PlayerController() {
			CurLives = StartPlayerLives;
			CurHp    = StartPlayerHp;
		}

		public bool TakeDamage(float damage) {
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
	}
}