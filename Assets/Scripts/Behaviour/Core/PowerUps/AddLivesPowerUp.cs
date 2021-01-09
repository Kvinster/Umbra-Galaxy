using UnityEngine;

using STP.Behaviour.Starter;
using STP.Controller;

namespace STP.Behaviour.Core.PowerUps {
	public class AddLivesPowerUp : BasePowerUp {
		const int TempAddLivesValue = 1;

		PlayerController _playerController;
		
		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_playerController = PlayerController.Instance;
		}

		protected override void OnRangeEnter(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			_playerController.AddLives(TempAddLivesValue);
			Destroy(gameObject);
		}
	}
}