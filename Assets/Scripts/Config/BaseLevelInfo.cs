using NaughtyAttributes;
using STP.Config.SpawnerSettings;
using UnityEngine;

namespace STP.Config {
	public abstract class BaseLevelInfo : ScriptableObject {
		[Scene] public string SceneName;

		public AsteroidSpawnerSettings AsteroidSpawnerSettings;
		public DroneSpawnerSettings    DroneSpawnerSettings;

		public abstract LevelType LevelType { get; }
	}
}