using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Manager;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class X2DamagePowerUp : BasePowerUp {
		const int TempAddPowerUpTimeSec = 10;

		PlayerManager _playerManager;

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_playerManager = starter.PlayerManager;
		}

		protected override void OnRangeEnter(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			_playerManager.AddTimeToPowerUp(PowerUpNames.X2Damage, TempAddPowerUpTimeSec);
			Destroy(gameObject);
		}
	}
}
