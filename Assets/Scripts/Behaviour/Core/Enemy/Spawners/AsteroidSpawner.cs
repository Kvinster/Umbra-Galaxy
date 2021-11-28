using UnityEngine;

using STP.Behaviour.Starter;
using STP.Config;
using STP.Config.SpawnerSettings;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class AsteroidSpawner : BaseSpawner {
		CoreSpawnHelper         _spawnHelper;
		AsteroidSpawnerSettings _spawnerSettings;

		protected override BaseSpawnerSettings Settings => _spawnerSettings;

		protected override void InitInternal(CoreStarter starter) {
			_spawnHelper     = starter.SpawnHelper;
			_spawnerSettings = starter.LevelController.CurLevelConfig.AsteroidSpawnerSettings;
			base.InitInternal(starter);
		}

		protected override void InitItem(GameObject go) {
			var asteroid = go.GetComponent<Asteroid>();
			if ( !asteroid ) {
				Debug.LogError("Can't init asteroid");
				return;
			}
			_spawnHelper.TryInitSpawnedObject(go);
			var dirToPlayer = Player.transform.position - go.transform.position;
			var speed       = Random.Range(_spawnerSettings.AsteroidMinSpeed, _spawnerSettings.AsteroidMaxSpeed);
			asteroid.SetParams(dirToPlayer.normalized, speed);
		}
	}
}