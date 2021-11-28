using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Starter;
using STP.Config;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class DroneSpawner : BaseSpawner {
		BaseSpawnerSettings _settings;

		Transform _playerTransform;

		protected override BaseSpawnerSettings Settings => _settings;

		protected override void InitInternal(CoreStarter starter) {
			_settings        = starter.LevelController.CurLevelConfig.DroneSpawnerSettings;
			_playerTransform = starter.Player.transform;
			base.InitInternal(starter);
		}

		protected override void InitItem(GameObject go) {
			var drone = go.GetComponent<BaseEnemy>();
			Assert.IsTrue(drone);
			drone.SetTarget(_playerTransform);
		}
	}
}