using UnityEngine;
using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelState   _levelState;
		readonly LevelsConfig _levelsConfig;

		public int NextLevelIndex => _levelState.NextLevelIndex;

		public bool HasNextLevel => (NextLevelIndex != _levelsConfig.Levels.Count);

		public LevelController(GameState gameState) : base(gameState) {
			_levelState   = gameState.LevelState;
			_levelsConfig = LoadConfig();
			Assert.IsNotNull(_levelsConfig);
		}

		public void OnLevelWon() {
			_levelState.NextLevelIndex++;
		}

		public LevelInfo GetCurLevelConfig() {
			return _levelsConfig.GetLevelConfig(NextLevelIndex);
		}

		static LevelsConfig LoadConfig() {
			return Resources.Load<LevelsConfig>("AllLevels");
		}

		public static LevelInfo GetLevelConfigInEditor(int levelIndex = 0) {
			var levelsConfig = LoadConfig();
			if ( levelsConfig ) {
				return levelsConfig.GetLevelConfig(levelIndex);
			}
			Debug.LogError("Can't load LevelsConfig in editor");
			return null;
		}
	}
}