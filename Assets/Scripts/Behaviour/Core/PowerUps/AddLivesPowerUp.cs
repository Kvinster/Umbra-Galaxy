using STP.Behaviour.Starter;
using STP.Core;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class AddLivesPowerUp : BasePickupable {
		PlayerController _playerController;

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);

			_playerController = starter.PlayerController;
		}

		protected override bool OnPlayerEnter() {
			_playerController.AddLife();
			return true;
		}
	}
}