using System.Collections.Generic;
using STP.Core.Achievements;
using STP.Core.Leaderboards;
using STP.Core.State;

namespace STP.Core {
	public sealed class GameController {
		public static GameController Instance { get; private set; }

		public static bool IsActiveInstanceExists => (Instance != null);

		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly GameState _gameState;

		public SettingsController    SettingsController    { get; }

		public LevelController   LevelController   { get; }
		public PlayerController  PlayerController  { get; }
		public ScoreController      ScoreController      { get; }
		public PrefabsController PrefabsController { get; }
		public LeaderboardController LeaderboardController { get; }
		
		public AchievementsController AchievementsController { get; }

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
			LevelController       = AddController(new LevelController(gameState));
			ScoreController          = AddController(new ScoreController());
			PlayerController      = AddController(new PlayerController());
			PrefabsController     = AddController(new PrefabsController());
			SettingsController    = AddController(new SettingsController(gameState));
			LeaderboardController = AddController(new LeaderboardController());
			AchievementsController = AddController(new AchievementsController(ScoreController));
			if ( Instance == null ) {
				Instance = this;
			}
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}
	}
}
