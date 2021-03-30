using STP.Behaviour.Core;

namespace STP.Events {
	public struct PlayerShipChanged {
		public Player NewPlayer;

		public PlayerShipChanged(Player newPlayer) {
			NewPlayer = newPlayer;
		}
	}
}