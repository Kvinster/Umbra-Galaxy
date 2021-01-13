using UnityEngine;

using System;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class XpController : BaseStateController {
		readonly XpState         _state;
		readonly LevelController _levelController;

		XpConfig _xpConfig;

		int _curLevelXp;

		public int CurTotalXp => _curLevelXp + CurXp;

		int CurLevelXp {
			get => _curLevelXp;
			set {
				if ( _curLevelXp == value ) {
					return;
				}
				_curLevelXp = value;
				OnXpChanged?.Invoke(CurTotalXp);
			}
		}

		int CurXp {
			get => _state.CurXp;
			set {
				if ( CurXp == value ) {
					return;
				}
				_state.CurXp = value;
				GameState.Save();
			}
		}

		public event Action<int> OnXpChanged;

		public XpController(GameState gameState) : base(gameState) {
			_state = gameState.XpState;

			LoadConfig();
		}

		public void ResetXp() {
			CurLevelXp = 0;
			CurXp      = 0;
		}

		public void OnLevelStart() {
			CurLevelXp = 0;
		}

		public void OnLevelWon() {
			CurXp      += CurLevelXp;
			CurLevelXp =  0;
		}

		public void AddLevelXp(int value) {
			if ( value < 0 ) {
				Debug.LogWarning($"Strange xp amount {value}. Ignoring");
				return;
			}
			CurLevelXp += value;
		}

		public int GetDestroyedEnemyXp(string enemyName) {
			return _xpConfig.GetDestroyedEnemyXp(enemyName);
		}

		void LoadConfig() {
			_xpConfig = Resources.Load<XpConfig>("XpConfig");
		}
	}
}
