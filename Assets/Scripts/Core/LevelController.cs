using UnityEngine;
using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelState   _levelState;
		readonly LevelsConfig _levelsConfig;

		public bool HasNextLevel => (LastLevelIndex != _levelsConfig.Levels.Count);

		public LevelType CurLevelType { get; private set; } = LevelType.Unknown;

		public BaseLevelInfo CurLevelConfig { get; private set; }

		public int CurLevelIndex => _levelState.CurLevelIndex;

		int LastLevelIndex => _levelState.LastLevelIndex;

		public LevelController(LevelState levelState) {
			_levelState   = levelState;
			_levelsConfig = LoadConfig();

			Assert.IsTrue(_levelsConfig);
			Assert.AreEqual(CurLevelIndex, -1);
		}

		public void StartLevel(int levelIndex) {
			Assert.AreEqual(CurLevelIndex, -1);
			_levelState.CurLevelIndex = levelIndex;

			CurLevelConfig = GetCurLevelConfig();
			Assert.IsTrue(CurLevelConfig);
			CurLevelType = CurLevelConfig.LevelType;
		}

		public void FinishLevel(bool win) {
			Assert.IsTrue(CurLevelIndex >= 0);

			if ( win ) {
				if ( CurLevelIndex == LastLevelIndex ) {
					_levelState.LastLevelIndex++;
				}
			} else {
				_levelState.ResetState();
			}

			_levelState.CurLevelIndex = -1;
		

			CurLevelType   = LevelType.Unknown;
			CurLevelConfig = null;
		}

		BaseLevelInfo GetCurLevelConfig() {
			Assert.IsTrue(CurLevelIndex >= 0);
			var levelInfo = _levelsConfig.GetLevelConfig(CurLevelIndex);
			Assert.IsTrue(levelInfo);
			return levelInfo;
		}

		static LevelsConfig LoadConfig() {
			return Resources.Load<LevelsConfig>("AllLevels");
		}
	}
}