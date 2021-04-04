using UnityEngine;

using System;
using System.Collections.Generic;
using STP.Behaviour.Core;
using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PowerUpPrefabsCatalogue", menuName = "ScriptableObjects/PowerUpPrefabsCatalogue", order = 1)]
	public class PrefabConfig : ScriptableObject {
		public class EnumToEntry<T, T1> where T : Enum {
			public T  Type;
			public T1 Entry;
		}
		
		[Serializable]
		public class PowerUpEntry : EnumToEntry<PowerUpType, GameObject> { }
		
		[Serializable]
		public class BulletEntry : EnumToEntry<BulletType, GameObject> { }

		[Serializable]
		public class ShipEntry {
			public ShipType   Type;
			public GameObject Entry;
			public Sprite     PreviewSprite;
		}

		public List<PowerUpEntry> PowerUps;
		public List<ShipEntry>    Ships;
		public List<BulletEntry>  PlayerBullets;

		public GameObject GetPowerUpPrefab(PowerUpType powerUp) {
			return PowerUps.Find(x => x.Type == powerUp)?.Entry;
		}

		public GameObject GetBulletPrefab(BulletType bulletType) {
			return PlayerBullets.Find(x => x.Type == bulletType)?.Entry;
		} 
		
		public GameObject GetShipPrefab(ShipType ship) {
			return Ships.Find(x => x.Type == ship)?.Entry;
		}
		
		public Sprite GetShipPreviewSprite(ShipType ship) {
			return Ships.Find(x => x.Type == ship)?.PreviewSprite;
		}
 	}
}