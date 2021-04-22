using UnityEngine;

using System;

using NaughtyAttributes;

namespace STP.Config {
	[Serializable]
	public abstract class BaseSpawnerSettings {
		public bool Enabled;
		[EnableIf(nameof(Enabled))] [AllowNesting]
		public float SpawnPeriod = 1f;
		[EnableIf(nameof(Enabled))] [AllowNesting]
		public float SpawnRange = 1000f;
		[EnableIf(nameof(Enabled))] [AllowNesting]
		public GameObject Prefab;
	}
}
