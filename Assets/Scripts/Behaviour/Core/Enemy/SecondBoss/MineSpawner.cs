using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class MineSpawner : GameComponent, ISpawner {
		[NotNullOrEmpty] public GameObject MinePrefab;

		public float MaxStartSpeed = 100f;
		public float LinearDrag    = 1f;

		
		CoreSpawnHelper _spawnHelper;
		
		
		public void Init(CoreSpawnHelper spawnHelper) {
			_spawnHelper   = spawnHelper;
		}

		public void Spawn() {
			var enemy = Instantiate(MinePrefab, transform.position, Quaternion.identity);
			_spawnHelper.TryInitSpawnedObject(enemy);
			var enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
			if ( !enemyRigidbody ) {
				Debug.LogError("Can't get rigidbody from spawned mines");
				return;
			}
			enemyRigidbody.velocity = Random.insideUnitCircle * MaxStartSpeed;
			enemyRigidbody.drag     = LinearDrag;
		}
	}
}