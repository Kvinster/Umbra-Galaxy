using UnityEngine;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		const string DefaultLevelName = "Level1";

		readonly LevelsConfig _levelsConfig;

		public string CurLevelName { get; private set; }

		public LevelController(GameState gameState) : base(gameState) {
			CurLevelName = DefaultLevelName;
			_levelsConfig = LoadConfig();
			Debug.Assert(_levelsConfig);
		}

		public void ChangeLevel(string newLevelName) {
			CurLevelName = newLevelName;
		}

		public LevelInfo GetCurLevelConfig() {
			return _levelsConfig.GetLevelConfig(CurLevelName);
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