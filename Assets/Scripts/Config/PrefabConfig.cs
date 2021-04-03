using UnityEngine;

using System;
using System.Collections.Generic;
using STP.Behaviour.Core;
using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PowerUpPrefabsCatalogue", menuName = "ScriptableObjects/PowerUpPrefabsCatalogue", order = 1)]
	public class PrefabConfig : ScriptableObject {
		public class EnumToPrefabEntry<T> where T : Enum {
			public T          Type;
			public GameObject Prefab;
		}
		
		[Serializable]
		public class PowerUpEntry : EnumToPrefabEntry<PowerUpType> { }

		[Serializable]
		public class ShipEntry : EnumToPrefabEntry<ShipType> { }

		public List<PowerUpEntry> PowerUps;
		public List<ShipEntry>    Ships;

		public GameObject GetPowerUpPrefab(PowerUpType powerUp) {
			return PowerUps.Find(x => x.Type == powerUp)?.Prefab;
		}
		
		public GameObject GetShipPrefab(ShipType ship) {
			return Ships.Find(x => x.Type == ship)?.Prefab;
		}
 	}
}