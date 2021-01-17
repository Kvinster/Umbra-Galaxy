using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class HealPowerUp : BasePowerUp {
		const int TempAddPowerUpTimeSec = 10;

		protected override void OnPlayerEnter() {
			PlayerManager.AddTimeToPowerUp(PowerUpType.Heal, TempAddPowerUpTimeSec);
		}
	}
}