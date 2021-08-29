using STP.Behaviour.Starter;
using STP.Config;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class DroneSpawner : BaseSpawner {
		BaseSpawnerSettings _settings;
		
		protected override BaseSpawnerSettings Settings => _settings;
		
		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_settings = starter.LevelController.CurLevelConfig.DroneSpawnerSettings;
		}
	}
}