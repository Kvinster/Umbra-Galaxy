using STP.Core.State;

namespace STP.Core {
	public abstract class BaseStateController {
		protected readonly GameState GameState;

		protected BaseStateController(GameState gameState) {
			GameState = gameState;
		}

		public virtual void Deinit() { }
	}
}
