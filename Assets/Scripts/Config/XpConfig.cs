using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Config {
	[Serializable]
	public class DestroyedEnemyXpInfo {
		public string EnemyName;
		public int    XpAmount;
	}

	[CreateAssetMenu(fileName = "XpConfig", menuName = "ScriptableObjects/XpConfig", order = 1)]
	public class XpConfig : ScriptableObject {
		public List<DestroyedEnemyXpInfo> EnemyXpInfo;
		public List<PlayerLevelUpInfo>    LevelUpInfos;
		
		public int GetDestroyedEnemyXp(string enemyName) {
			var item = EnemyXpInfo.Find(x => x.EnemyName == enemyName);
			if ( item == null ) {
				Debug.LogError($"Can't find xp amount for enemy {enemyName}");
				return 0;
			}
			return item.XpAmount;
		}
	}

	[Serializable]
	public class PlayerLevelUpInfo {
		public List<GameObject> ShipsToSelect;
		public int              NeededXp;
	}
}