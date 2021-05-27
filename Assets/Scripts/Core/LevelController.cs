using UnityEngine;
using UnityEngine.Assertions;

using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		readonly LevelsGraph _allLevels;

		readonly LevelControllerState _state;

		public StartLevelNode StartLevelNode => _allLevels.nodes.Find(x => x is StartLevelNode) as StartLevelNode;
		
		public LevelType CurLevelType => CurLevelConfig.LevelType;

		public BaseLevelInfo CurLevelConfig => _state.CurStartedLevel.Config;
		
		LevelControllerState.CompletedMainLevelInfo LastCompletedMainBranchLevel => HasCompletedLevels ? _state.CompletedLevels[_state.CompletedLevels.Count - 1] : null;

		bool HasCompletedLevels => _state.CompletedLevels.Count > 0;

		public LevelController(GameState state) {
			_state     = state.LevelControllerState;
			_allLevels = LoadGraph();
		}
		
		public void StartLevel(LevelNode level) {
			_state.CurStartedLevel = level;
			Assert.IsTrue(CurLevelConfig);
		}

		public LevelNode GetNodeFromConfig(BaseLevelInfo levelInfo) {
			return _allLevels.nodes.Find(x => {
				var node = x as LevelNode;
				return (node.Config == levelInfo);
			}) as LevelNode;
		}  

		public void FinishLevel(bool win) {
			if ( win ) {
				// Mark level as completed
				if ( !HasCompletedLevels || LastCompletedMainBranchLevel.IsNextMainLevel(_state.CurStartedLevel) ) {
					_state.CompletedLevels.Add(new LevelControllerState.CompletedMainLevelInfo{ MainLevel = _state.CurStartedLevel});
				} else {
					LastCompletedMainBranchLevel.SetOptionalLevelAsCompleted(_state.CurStartedLevel);
				}
			} else {
				// Reset all progress
				_state.CompletedLevels.Clear();
			}
			_state.CurStartedLevel = null;
		}

		public bool IsLevelCompleted(LevelNode node) {
			return _state.CompletedLevels.Exists(x => (x.MainLevel == node) || x.IsOptionalLevelCompleted(node));
		}
		
		public bool IsLevelAvailableToRun(LevelNode node) {
			if ( !HasCompletedLevels ) {
				return node == StartLevelNode;
			}
			return LastCompletedMainBranchLevel.IsNextMainLevel(node) || 
			       (LastCompletedMainBranchLevel.IsOptionalLevel(node) && !LastCompletedMainBranchLevel.IsOptionalLevelCompleted(node));
		}

		LevelsGraph LoadGraph() {
			return Resources.Load<LevelsGraph>("LevelsGraph");
		}
	}
}