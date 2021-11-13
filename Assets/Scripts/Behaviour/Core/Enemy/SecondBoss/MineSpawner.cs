using System.Collections.Generic;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class MineSpawner : GameComponent {
		[NotNullOrEmpty] public GameObject MinePrefab;

		
		CoreSpawnHelper _spawnHelper;
		Transform       _bossTransform;
		
		
		public void Init(CoreSpawnHelper spawnHelper, Transform bossTransform) {
			_spawnHelper = spawnHelper;
			_bossTransform        = bossTransform;
		}

		public void Spawn() {
			var enemy = Instantiate(MinePrefab, transform.position, Quaternion.identity);
			_spawnHelper.TryInitSpawnedObject(enemy);
			var enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
			if ( !enemyRigidbody ) {
				Debug.LogError("Can't get rigidbody from spawned mines");
				return;
			}
		}
	}
}