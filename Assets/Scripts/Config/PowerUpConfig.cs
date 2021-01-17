using UnityEngine;

using System;
using System.Collections.Generic;
using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PowerUpPrefabsCatalogue", menuName = "ScriptableObjects/PowerUpPrefabsCatalogue", order = 1)]
	public class PowerUpConfig : ScriptableObject {
		[Serializable]
		public class PowerUpEntry {
			public PowerUpType Type;
			public GameObject  Prefab;
		}

		public List<PowerUpEntry> PowerUps;

		public GameObject GetPowerUpPrefab(PowerUpType powerUp) {
			return GetPowerUpEntry(powerUp)?.Prefab;
		}

		PowerUpEntry GetPowerUpEntry(PowerUpType powerUp) {
			return PowerUps.Find(x => x.Type == powerUp);
		}
 	}
}