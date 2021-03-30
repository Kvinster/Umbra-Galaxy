using UnityEngine;
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
		
		public void Init(CoreStarter starter, XpController xpController, ShipType shipType, Action hideWindowAction) {
			var shipVisual = xpController.GetShipVisuals(shipType);
			Image.sprite = shipVisual.ShipSprite;
			Button.onClick.AddListener(() => {
				CreateNewShip(starter, shipVisual.ShipPrefab);
				hideWindowAction();
			});
		}

		public void Deinit() {
			Button.onClick.RemoveAllListeners();
		}

		void CreateNewShip(CoreStarter starter, GameObject newPlayerShip) {
			var newShipInstance = Instantiate(newPlayerShip, starter.Player.transform.position, Quaternion.identity);
			var playerComp      = newShipInstance.GetComponent<Player>();
			if ( !playerComp ) {
				Debug.LogError($"can't find player component. GO {newShipInstance}");
				return;
			}
			playerComp.Init(starter);
			Destroy(starter.Player.gameObject);
			starter.Player = playerComp;
			EventManager.Fire(new PlayerShipChanged(playerComp));
		}
	}
}