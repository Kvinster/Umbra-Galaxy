using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class GameController {
		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly GameState _gameState;

		public ChunkController  ChunkController  { get; }
		public LevelController  LevelController  { get; }
		public PlayerController PlayerController { get; }
		public XpController     XpController     { get; }

		public GameController(GameState gameState) {
			_gameState = gameState;

			ChunkController  = AddController(new ChunkController(_gameState));
			LevelController  = AddController(new LevelController(_gameState));
			PlayerController = AddController(new PlayerController(_gameState));
			XpController     = AddController(new XpController(_gameState));
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
