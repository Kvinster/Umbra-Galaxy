using System.Collections.Generic;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class Spawner : GameComponent {
		[NotNullOrEmpty] public List<GameObject> SpawningEnemies;

		CoreSpawnHelper _spawnHelper;
		
		public void Init(CoreSpawnHelper spawnHelper) {
			_spawnHelper = spawnHelper;
		}

		public void Spawn() {
			var randomEnemy = RandomUtils.GetRandomElement(SpawningEnemies);
			var enemy = Instantiate(randomEnemy, transform.position, Quaternion.identity);
			_spawnHelper.TryInitSpawnedObject(enemy);
		}
	}
}