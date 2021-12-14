using STP.Behaviour.Starter;
using STP.Common;
using STP.Manager;

namespace STP.Behaviour.Core.PowerUps {
	public abstract class BasePowerUp : BasePickupable {
		PlayerManager _playerManager;

		protected abstract PowerUpType PowerUpType { get; }

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_playerManager = starter.PlayerManager;
		}

		protected override bool OnPlayerEnter() {
			return _playerManager.TryUsePowerUp(PowerUpType);
		}
	}
}