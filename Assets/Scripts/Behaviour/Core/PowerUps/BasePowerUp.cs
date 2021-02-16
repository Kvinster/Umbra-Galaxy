using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
using STP.Manager;

namespace STP.Behaviour.Core.PowerUps {
	public abstract class BasePowerUp : BasePickupable {
		protected PlayerManager    PlayerManager;
		protected PlayerController PlayerController;

		protected abstract PowerUpType PowerUpType { get; }

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			PlayerManager    = starter.PlayerManager;
			PlayerController = starter.PlayerController;
		}

		protected override bool OnPlayerEnter() {
			return PlayerManager.TryPickupPowerUp(PowerUpType);
		}
	}
}