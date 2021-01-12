using UnityEngine;
using UnityEngine.Assertions;

using System;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		const string DefaultLevelName = "Level1";

		readonly LevelState   _levelState;
		readonly LevelsConfig _levelsConfig;

		public string NextLevelName => _levelState.NextLevelName;

		public LevelController(GameState gameState) : base(gameState) {
			_levelState   = gameState.LevelState;
			_levelsConfig = LoadConfig();
			Assert.IsNotNull(_levelsConfig);
		}

		public void OnLevelWon() {
			foreach ( var levelInfo in _levelsConfig.Levels ) {
				if ( levelInfo.LevelName == NextLevelName ) {
					_levelState.NextLevelName = levelInfo.NextLevelName;
					return;
				}
			}
			Debug.LogErrorFormat("Can't find level info for '{0}'", NextLevelName);
		}

		public LevelInfo GetCurLevelConfig() {
			return _levelsConfig.GetLevelConfig(NextLevelName);
		}

		static LevelsConfig LoadConfig() {
			return Resources.Load<LevelsConfig>("AllLevels");
		}

		public static LevelInfo GetLevelConfigInEditor(string levelName = DefaultLevelName) {
			var levelsConfig = LoadConfig();
			if ( levelsConfig ) {
				return levelsConfig.GetLevelConfig(levelName);
			}
			Debug.LogError("Can't load LevelsConfig in editor");
			return null;
		}
	}
}