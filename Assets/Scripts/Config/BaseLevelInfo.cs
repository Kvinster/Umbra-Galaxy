using UnityEngine;

namespace STP.Config {
	public abstract class BaseLevelInfo : ScriptableObject {
		public AsteroidSpawnerSettings AsteroidSpawnerSettings;
		public DroneSpawnerSettings    DroneSpawnerSettings;

		public abstract LevelType LevelType { get; }
	}
}
