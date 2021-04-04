using UnityEngine.UI;

using System;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI.LevelUpWindow {
	public class ShipView : GameComponent {
		[NotNull] public Image  Image;
		[NotNull] public Button Button;
		
		public void Init(CoreStarter starter, PrefabsController prefabsController, ShipType shipType, Action hideWindowAction) {
			Image.sprite = prefabsController.GetShipPreview(shipType);
			Button.onClick.AddListener(() => {
				CreateNewShip(starter, shipType);
				hideWindowAction();
			});
		}

		public void Deinit() {
			Button.onClick.RemoveAllListeners();
		}

		void CreateNewShip(CoreStarter starter, ShipType shipType) {
			starter.PlayerController.Ship = shipType;
			var newPlayer = starter.ShipCreator.CreatePlayerShip(shipType);
			if ( !newPlayer ) {
				return;
			}
			newPlayer.Init(starter);
			// Workaround cause player init sets (0,0) start position
			newPlayer.transform.position = starter.Player.transform.position;
			Destroy(starter.Player.gameObject);
			starter.Player = newPlayer;
			EventManager.Fire(new PlayerShipChanged(newPlayer));
		}
	}
}