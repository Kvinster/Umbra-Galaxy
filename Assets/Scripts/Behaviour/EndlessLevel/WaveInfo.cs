using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Behaviour.EndlessLevel {
	[Serializable]
	public class WaveInfo {
		public int                 MaxTimeUntilNextWave;
		public List<Transform>     SelectedSpawnPoints;
		public List<WaveEnemyInfo> EnemiesToSpawn;
	}

	[Serializable]
	public class WaveEnemyInfo {
		public GameObject Prefab;
		public int        Count;
	}
}