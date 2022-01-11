using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Config {
	[Serializable]
	public class DestroyedEnemyScoreInfo {
		public string EnemyName;
		public int    XpAmount;
		// needed for counting boss kills
		public bool   IsBoss;
	}

	[CreateAssetMenu(fileName = "XpConfig", menuName = "ScriptableObjects/XpConfig", order = 1)]
	public sealed class ScoreConfig : ScriptableObject {
		public List<DestroyedEnemyScoreInfo> EnemyXpInfo;

		public int GetDestroyedEnemyScore(string enemyName) {
			var item = GetConfigItem(enemyName);
			return item?.XpAmount ?? -1;
		}

		public bool IsBossEnemy(string enemyName) {
			var item = GetConfigItem(enemyName);
			return item?.IsBoss ?? false;
		}

		DestroyedEnemyScoreInfo GetConfigItem(string enemyName) {
			var item = EnemyXpInfo.Find(x => x.EnemyName == enemyName);
			if ( item == null ) {
				Debug.LogError($"Can't find xp amount for enemy {enemyName}");
			}
			return item;
		}
	}
}