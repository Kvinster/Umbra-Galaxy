using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Config {
	[Serializable]
	public class DestroyedEnemyScoreInfo {
		public string EnemyName;
		public int    XpAmount;
	}

	[CreateAssetMenu(fileName = "XpConfig", menuName = "ScriptableObjects/XpConfig", order = 1)]
	public sealed class ScoreConfig : ScriptableObject {
		public List<DestroyedEnemyScoreInfo> EnemyXpInfo;

		public int GetDestroyedEnemyScore(string enemyName) {
			var item = EnemyXpInfo.Find(x => x.EnemyName == enemyName);
			if ( item == null ) {
				Debug.LogError($"Can't find xp amount for enemy {enemyName}");
				return 0;
			}
			return item.XpAmount;
		}
	}
}