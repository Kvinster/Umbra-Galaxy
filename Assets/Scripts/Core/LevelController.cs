using UnityEngine;
using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelState   _levelState;
		readonly LevelsConfig _levelsConfig;

		public bool HasNextLevel => (LastLevelIndex != _levelsConfig.Levels.Count);

		public int CurLevelIndex  => _levelState.CurLevelIndex;

		int LastLevelIndex => _levelState.LastLevelIndex;

		public LevelController(ProfileState profileState) {
			_levelState   = profileState.LevelState;
			_levelsConfig = LoadConfig();

			Assert.IsTrue(_levelsConfig);
			Assert.AreEqual(CurLevelIndex, -1);
		}

		public void StartLevel(int levelIndex) {
			Assert.AreEqual(CurLevelIndex, -1);
			_levelState.CurLevelIndex = levelIndex;
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

		public BaseLevelInfo GetCurLevelConfig() {
			Assert.IsTrue(CurLevelIndex >= 0);
			var levelInfo = _levelsConfig.GetLevelConfig(CurLevelIndex);
			Assert.IsNotNull(levelInfo);
			return levelInfo;
		}

		static LevelsConfig LoadConfig() {
			return Resources.Load<LevelsConfig>("AllLevels");
		}
	}
}