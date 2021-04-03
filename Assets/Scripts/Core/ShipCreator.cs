using UnityEngine;

using STP.Behaviour.Core;

namespace STP.Core {
	public class ShipCreator {
		readonly PrefabsController _prefabsController;
		readonly Transform         _root;

		public ShipCreator(Transform root, PrefabsController prefabsController) {
			_prefabsController = prefabsController;
			_root              = root;
		}

		public Player CreatePlayerShip(ShipType ship) {
			return CreatePlayerShip(ship, Vector3.zero);
		}

		public Player CreatePlayerShip(ShipType ship, Vector3 position) {
			var go         = GameObject.Instantiate(_prefabsController.GetShipPrefab(ship), position, Quaternion.identity, _root);
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				Debug.LogError("Can't find a player component");
			}
			return playerComp;
		}
	}
}