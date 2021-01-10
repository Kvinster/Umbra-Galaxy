using UnityEngine;

using System;

using STP.Config;
using STP.Utils;

namespace STP.Core {
	public class XpController : Singleton<XpController> {
		XpConfig _xpConfig;

		int _curXp;

		public int CurXp {
			get {
				return _curXp;
			}
			private set {
				_curXp = value;
				if ( _curXp != value ) {
					OnXpChanged?.Invoke(value);
				}
			}
		}

		public event Action<int> OnXpChanged;

		public XpController() {
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
