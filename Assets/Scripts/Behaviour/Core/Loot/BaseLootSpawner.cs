using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Loot {
	public abstract class BaseLootSpawner : BaseCoreComponent {
		[Header("Dependencies")]
		[NotNull] public BaseEnemy Enemy;

		CoreSpawnHelper _spawnHelper;

		protected virtual void Reset() {
			Enemy = GetComponentInParent<BaseEnemy>();
		}

		protected override void OnDisable() {
			base.OnDisable();
			if ( Enemy ) {
				Enemy.OnDestroyed -= OnEnemyDestroyed;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_spawnHelper = starter.SpawnHelper;

			Enemy.OnDestroyed += OnEnemyDestroyed;
		}

		protected void SpawnPrefab(GameObject prefab) {
			var go = Instantiate(prefab, transform.parent);
			go.transform.position = transform.position;
			_spawnHelper.TryInitSpawnedObject(go);
		}

		protected abstract void Spawn();

		void OnEnemyDestroyed(BaseEnemy obj) {
			Spawn();
		}
	}
}
