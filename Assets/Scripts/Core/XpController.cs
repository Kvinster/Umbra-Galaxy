using UnityEngine;

using System;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class XpController : BaseStateController {
		readonly LevelController _levelController;

		XpConfig _xpConfig;

		int _curPrevXp;
		int _curLevelXp;

		public int CurTotalXp => _curLevelXp + _curPrevXp;

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

		public event Action<int> OnXpChanged;

		public XpController(ProfileState profileState) {
			LoadConfig();
		}

		public void ResetXp() {
			CurLevelXp = 0;
		}

		public void OnLevelStart() {
			CurLevelXp = 0;
		}

		public void OnLevelWon() {
			_curPrevXp += CurLevelXp;
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
