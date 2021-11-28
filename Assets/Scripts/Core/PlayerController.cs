using UnityEngine;

using System;

using STP.Behaviour.Core;
using STP.Config;

namespace STP.Core {
	public sealed class PlayerController : BaseStateController {
		const int StartPlayerLives = 3;

		public ShipType Ship;

		public readonly HpSystem HpSystem;

		int  _curLives;

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

		public bool IsInvincible { get; set; }

		public PlayerConfig Config => PlayerConfig.Instance;

		public event Action<int> OnCurLivesChanged;
		public event Action      OnLifeAdded;

		public PlayerController() {
			CurLives     = StartPlayerLives;
			HpSystem     = new HpSystem(Config.MaxHp);
			IsInvincible = false;
		}

		public void OnLevelStart() {
			HpSystem.SetMaxHp(Config.MaxHp);
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

		public void SubLife() {
			if ( CurLives < 1 ) {
				Debug.LogErrorFormat("PlayerController.SubLife: can't sub life from '{0}'", CurLives);
				return;
			}
			CurLives -= 1;
		}

		public void AddLife() {
			++CurLives;
			OnLifeAdded?.Invoke();
		}
	}
}