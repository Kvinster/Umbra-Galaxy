using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class GameController {
		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly GameState _gameState;

		public LeaderboardController LeaderboardController { get; }
		public SettingsController    SettingsController    { get; }

		public GameController(GameState gameState) {
			_gameState = gameState;

			LeaderboardController = AddController(new LeaderboardController(gameState));
			SettingsController    = AddController(new SettingsController(gameState));
		}

		public void Deinit() {
			foreach ( var controller in _controllers ) {
				controller.Deinit();
			}
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}
	}
}
