namespace STP.Behaviour.Core.PowerUps {
	public sealed class AddLivesPowerUp : BasePowerUp {
		const int TempAddLivesValue = 1;

		protected override void OnPlayerEnter() {
			PlayerController.AddLives(TempAddLivesValue);
		}
	}
}