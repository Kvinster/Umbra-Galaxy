using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Controller;
using STP.Manager;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class ShieldPowerUp : BasePowerUp {
		public const float TmpShieldDuration = 10f;

		PlayerManager    _playerManager;
		PlayerController _playerController;

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_playerManager          =  starter.PlayerManager;
			_playerController       =  PlayerController.Instance;
		}

		protected override void OnRangeEnter(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			_playerController.IsInvincible = true;
			_playerManager.AddTimeToPowerUp(PowerUpNames.Shield, TmpShieldDuration);
			Destroy(gameObject);
		}
	}
}
