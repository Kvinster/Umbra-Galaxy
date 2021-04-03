using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core;

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
		public List<ShipVisual>           ShipViews;
		
		public int GetDestroyedEnemyXp(string enemyName) {
			var item = EnemyXpInfo.Find(x => x.EnemyName == enemyName);
			if ( item == null ) {
				Debug.LogError($"Can't find xp amount for enemy {enemyName}");
				return 0;
			}
			return item.XpAmount;
		}

		public ShipVisual GetShipVisual(ShipType shipType) {
			return ShipViews.Find(x => x.ShipType == shipType);
		}
	}

	[Serializable]
	public class ShipVisual {
		public ShipType   ShipType;
		public Sprite     ShipSprite;
	}

	[Serializable]
	public class PlayerLevelUpInfo {
		public List<ShipType> ShipsToSelect;
		public int            NeededXp;
	}
}