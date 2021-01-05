using System;
using UnityEngine;

using STP.Config;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;

namespace STP.Controller {
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
			EventManager.Subscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		public void ResetXp() {
			CurXp = 0;
		}

		void OnEnemyDestroyed(EnemyDestroyed e) {
			CurXp += _xpConfig.GetDestroyedEnemyXp(e.EnemyName);
		}
		
		void LoadConfig() {
			_xpConfig = Resources.Load<XpConfig>("XpConfig");
		}
	}
}