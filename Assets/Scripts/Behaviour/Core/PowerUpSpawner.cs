using System;

using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class PowerUpSpawner : BaseCoreComponent {
		[Header("Parameters")]
		public PowerUpType PowerUpType;
		[Header("Dependencies")]
		[NotNull] public BaseEnemy Enemy;

		CoreSpawnHelper   _spawnHelper;
		PrefabsController _prefabsController;

		void Reset() {
			Enemy = GetComponentInParent<BaseEnemy>();
		}

		protected override void OnDisable() {
			base.OnDisable();
			if ( Enemy ) {
				Enemy.OnDestroyed -= OnEnemyDestroyed;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_spawnHelper       = starter.SpawnHelper;
			_prefabsController = starter.PrefabsController;

			Enemy.OnDestroyed += OnEnemyDestroyed;
		}

		void OnEnemyDestroyed(BaseEnemy obj) {
			Spawn();
		}

		void Spawn() {
			var powerUpGo = Instantiate(_prefabsController.GetPowerUpPrefab(PowerUpType), transform.parent);
			powerUpGo.transform.position = transform.position;
			_spawnHelper.TryInitSpawnedObject(powerUpGo);
		}
	}
}
