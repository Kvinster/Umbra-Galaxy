using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class GameController {
		public static GameController Instance { get; private set; }

		public static bool IsActiveInstanceExists => (Instance != null);
		
		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly GameState _gameState;

		public LeaderboardController LeaderboardController { get; }
		public SettingsController    SettingsController    { get; }

		public LevelController   LevelController   { get; }
		public PlayerController  PlayerController  { get; }
		public XpController      XpController      { get; }
		public PrefabsController PrefabsController { get; }

		public static void CreateGameController(GameState gameState) {
			Instance = new GameController(gameState);
		}

		public void Deinit() {
			foreach ( var controller in _controllers ) {
				controller.Deinit();
			}
		}
		
		GameController(GameState gameState) {
			_gameState            = gameState;
			LevelController       = AddController(new LevelController());
			PlayerController      = AddController(new PlayerController());
			XpController          = AddController(new XpController());
			PrefabsController     = AddController(new PrefabsController());
			LeaderboardController = AddController(new LeaderboardController(gameState));
			SettingsController    = AddController(new SettingsController(gameState));
			if (Instance == null) {
				Instance = this;
			}
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}
	}
}
