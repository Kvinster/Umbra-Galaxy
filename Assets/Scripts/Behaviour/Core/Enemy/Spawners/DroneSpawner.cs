using STP.Behaviour.Starter;
using STP.Config;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class DroneSpawner : BaseSpawner {
		protected override BaseSpawnerSettings InitSettings(CoreStarter starter) {
			return starter.LevelController.GetCurLevelConfig().DroneSpawnerSettings;
		}
	}
}