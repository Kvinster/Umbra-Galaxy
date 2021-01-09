namespace STP.Behaviour.Core.PowerUps {
	public sealed class RestoreHpPowerUp : BasePowerUp {
		protected override void OnPlayerEnter() {
			PlayerController.RestoreHp();
		}
	}
}