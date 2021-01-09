using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public class X2XpPowerUp : BasePowerUp {
		const int TempAddPowerUpTimeSec = 10;

		protected override void OnPlayerEnter() {
			PlayerManager.AddTimeToPowerUp(PowerUpNames.X2Xp, TempAddPowerUpTimeSec);
		}
	}
}