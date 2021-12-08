using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;

namespace STP.Behaviour.Core.Loot {
	public sealed class PowerUpLootSpawner : BaseLootSpawner {
		[Header("Parameters")]
		public PowerUpType PowerUpType;

		PrefabsController _prefabsController;

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_prefabsController = starter.PrefabsController;
		}

		protected override void Spawn() {
			SpawnPrefab(_prefabsController.GetPowerUpPrefab(PowerUpType));
		}
	}
}
