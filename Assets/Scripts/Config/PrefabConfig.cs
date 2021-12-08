using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PowerUpPrefabsCatalogue", menuName = "ScriptableObjects/PowerUpPrefabsCatalogue", order = 1)]
	public class PrefabConfig : ScriptableObject {
		public class EnumToEntry<T, T1> where T : Enum {
			public T  Type;
			public T1 Entry;
		}

		[Serializable]
		public sealed class PowerUpEntry : EnumToEntry<PowerUpType, GameObject> { }

		[Serializable]
		public sealed class BulletEntry : EnumToEntry<BulletType, GameObject> { }

		public List<PowerUpEntry> PowerUps;
		public List<BulletEntry>  PlayerBullets;

		public GameObject GetPowerUpPrefab(PowerUpType powerUp) {
			return PowerUps.Find(x => x.Type == powerUp)?.Entry;
		}

		public GameObject GetBulletPrefab(BulletType bulletType) {
			return PlayerBullets.Find(x => x.Type == bulletType)?.Entry;
		}
	}
}