using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class IncreasedFireRatePowerUp : BasePowerUp {
		const float TmpDuration = 10f;

		protected override void OnPlayerEnter() {
			PlayerManager.AddTimeToPowerUp(PowerUpType.IncFireRate, TmpDuration);
		}
	}
}
