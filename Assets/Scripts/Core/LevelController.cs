using UnityEngine;
using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelState   _levelState;
		readonly LevelsConfig _levelsConfig;

		public bool HasNextLevel => (LastLevelIndex != _levelsConfig.Levels.Count);

		int CurLevelIndex  => _levelState.CurLevelIndex;
		int LastLevelIndex => _levelState.LastLevelIndex;

		public LevelController(ProfileState profileState) {
			_levelState   = profileState.LevelState;
			_levelsConfig = LoadConfig();

			Assert.AreNotEqual(CurLevelIndex, -1);
			Assert.IsTrue(_levelsConfig);
		}

		public void FinishLevel(bool win) {
			Assert.IsTrue(CurLevelIndex >= 0);

			if ( win ) {
				if ( CurLevelIndex == LastLevelIndex ) {
					_levelState.LastLevelIndex++;
				}
				++_levelState.CurLevelIndex;
			} else {
				_levelState.CurLevelIndex = -1;
			}
		}

		public LevelInfo GetCurLevelConfig() {
			Assert.IsTrue(CurLevelIndex >= 0);
			return _levelsConfig.GetLevelConfig(CurLevelIndex);
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