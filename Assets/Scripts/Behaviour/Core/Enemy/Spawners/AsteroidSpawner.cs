using UnityEngine;

using STP.Behaviour.Starter;
using STP.Config;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class AsteroidSpawner : BaseSpawner {
		CoreSpawnHelper         _spawnHelper;
		AsteroidSpawnerSettings _spawnerSettings;
		
		protected override BaseSpawnerSettings Settings => _spawnerSettings;
		
		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_spawnHelper     = starter.SpawnHelper;
			_spawnerSettings = starter.LevelController.CurLevelConfig.AsteroidSpawnerSettings;
		}

		protected override void InitItem(GameObject go) {
			var asteroid = go.GetComponent<Asteroid>();
			if ( !asteroid ) {
				Debug.LogError("Can't init asteroid");
				return;
			}
			_spawnHelper.TryInitSpawnedObject(go);
			var dirToPlayer = Player.transform.position - go.transform.position;
			asteroid.SetParams(dirToPlayer.normalized, _spawnerSettings.AsteroidSpeed);
		}
	}
}