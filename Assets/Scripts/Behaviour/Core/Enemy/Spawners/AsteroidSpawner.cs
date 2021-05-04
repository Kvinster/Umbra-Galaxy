using UnityEngine;

using STP.Behaviour.Starter;
using STP.Config;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public sealed class AsteroidSpawner : BaseSpawner {
		AsteroidSpawnerSettings _spawnerSettings;

		protected override BaseSpawnerSettings InitSettings(CoreStarter starter) {
			_spawnerSettings = starter.LevelController.CurLevelConfig.AsteroidSpawnerSettings;
			return _spawnerSettings;
		}

		protected override void InitItem(GameObject go) {
			var asteroid = go.GetComponent<Asteroid>();
			if ( !asteroid ) {
				Debug.LogError("Can't init asteroid");
				return;
			}
			var dirToPlayer = Player.transform.position - go.transform.position;
			asteroid.Init(dirToPlayer.normalized, _spawnerSettings.AsteroidSpeed);
		}
	}
}