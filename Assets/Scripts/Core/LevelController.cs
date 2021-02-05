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

		public int GetLevelInfosCount() {
			return _levelsConfig.Levels.Count;
		}

		public int GetCurLevelGeneratorIncValue() {
			return CurLevelIndex / _levelsConfig.Levels.Count * _levelsConfig.GeneratorsCountIncCount;
		}

		public LevelInfo GetCurLevelConfig() {
			Assert.IsTrue(CurLevelIndex >= 0);
			return _levelsConfig.GetLevelConfig(CurLevelIndex);
		}

		static LevelsConfig LoadConfig() {
			return Resources.Load<LevelsConfig>("AllLevels");
		}
	}
}