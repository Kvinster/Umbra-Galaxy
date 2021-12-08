using STP.Behaviour.Starter;
using STP.Core;

namespace STP.Behaviour.Core.Loot {
	public sealed class LifeLootSpawner : BaseLootSpawner {

		PrefabsController _prefabsController;

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_prefabsController = starter.PrefabsController;
		}

		protected override void Spawn() {
			SpawnPrefab(_prefabsController.GetLifePrefab());
		}
	}
}
