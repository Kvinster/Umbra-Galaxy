using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public class X2XpPowerUp : BasePowerUp {
		const int TempAddPowerUpTimeSec = 10;

		protected override void OnPlayerEnter() {
			PlayerManager.AddTimeToPowerUp(PowerUpType.X2Xp, TempAddPowerUpTimeSec);
		}
	}
}