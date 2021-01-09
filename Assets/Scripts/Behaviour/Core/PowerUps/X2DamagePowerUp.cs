﻿using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class X2DamagePowerUp : BasePowerUp {
		const int TempAddPowerUpTimeSec = 10;

		protected override void OnPlayerEnter() {
			PlayerManager.AddTimeToPowerUp(PowerUpNames.X2Damage, TempAddPowerUpTimeSec);
		}
	}
}