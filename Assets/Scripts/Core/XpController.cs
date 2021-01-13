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

		public int CurXp {
			get => _state.CurXp;
			private set {
				if ( CurXp == value ) {
					return;
				}
				_state.CurXp = value;
				OnXpChanged?.Invoke(CurXp);
			}
		}

		public event Action<int> OnXpChanged;

		public XpController(GameState gameState) : base(gameState) {
			_state = gameState.XpState;

			LoadConfig();
		}

		public void ResetXp() {
			CurXp = 0;
		}

		public void OnLevelStart() {
			_curLevelXp = 0;
		}

		public void OnLevelWon() {
			_state.CurXp += _curLevelXp;
			_curLevelXp  =  0;
			GameState.Save();
		}

		public void AddLevelXp(int value) {
			if ( value < 0 ) {
				Debug.LogWarning($"Strange xp amount {value}. Ignoring");
				return;
			}
			_curLevelXp += value;
		}

		public int GetDestroyedEnemyXp(string enemyName) {
			return _xpConfig.GetDestroyedEnemyXp(enemyName);
		}

		void LoadConfig() {
			_xpConfig = Resources.Load<XpConfig>("XpConfig");
		}
	}
}
