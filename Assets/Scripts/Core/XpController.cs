using UnityEngine;

using System;
using System.Xml;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class XpController : BaseStateController {
		readonly XpState _state;

		XpConfig _xpConfig;

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

		public void AddXp(int value) {
			if ( value < 0 ) {
				Debug.LogWarning($"Strange xp amount {value}. Ignoring");
				return;
			}
			CurXp += value;
		}

		public int GetDestroyedEnemyXp(string enemyName) {
			return _xpConfig.GetDestroyedEnemyXp(enemyName);
		}

		void LoadConfig() {
			_xpConfig = Resources.Load<XpConfig>("XpConfig");
		}
	}
}
