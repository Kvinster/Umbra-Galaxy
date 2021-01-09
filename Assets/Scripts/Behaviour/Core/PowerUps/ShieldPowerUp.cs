using STP.Common;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class ShieldPowerUp : BasePowerUp {
		public const float TmpShieldDuration = 10f;

		protected override void OnPlayerEnter() {
			PlayerController.IsInvincible = true;
			PlayerManager.AddTimeToPowerUp(PowerUpNames.Shield, TmpShieldDuration);
		}
	}
}
