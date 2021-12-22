using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;
using STP.Events.Analytics;
using STP.Service;
using UnityEngine.SceneManagement;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelControllerState _state;

		public int CurLevelIndex => _state.CurLevelIndex;

		public LevelType CurLevelType => CurLevelConfig.LevelType;

		public BaseLevelInfo CurLevelConfig => LevelsConfig.Instance.GetLevelConfig(CurLevelIndex);

		public LevelController(GameState state) {
			_state = state.LevelControllerState;
		}

		public void StartLevel(int levelIndex) {
			Assert.AreEqual(CurLevelIndex, -1);
			_state.CurLevelIndex = levelIndex;
			Assert.IsTrue(CurLevelConfig);
			AnalyticsService.LogEvent(new LevelStartedEvent(levelIndex));
		}

		#if UNITY_EDITOR
		public void StartLevelFromEditor() {
			var activeScene = SceneManager.GetActiveScene().name;
			var levelIndex = LevelsConfig.Instance.FindLevelIndexByName(activeScene);
			StartLevel(levelIndex);
		}
		#endif

		public void FinishLevel() {
			Assert.AreNotEqual(CurLevelIndex, -1);
			AnalyticsService.LogEvent(new LevelEndedEvent(CurLevelIndex));
			_state.CurLevelIndex = -1;
		}
	}
}