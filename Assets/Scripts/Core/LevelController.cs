using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;

using STP.Config;

namespace STP.Core {
	public sealed class LevelController : BaseStateController {
		public class CompletedMainLevelInfo {
			public   LevelNode       MainLevel;
			
			readonly List<LevelNode> _completedOptionalLevels = new List<LevelNode>();

			public bool IsNextMainLevel(LevelNode node) {
				return MainLevel.NextLevels.Contains(node);
			}

			public bool IsOptionalLevel(LevelNode node) {
				return MainLevel.OptionalLevels.Contains(node);
			}
			
			public bool IsOptionalLevelCompleted(LevelNode node) {
				return _completedOptionalLevels.Contains(node);
			}

			public void SetOptionalLevelAsCompleted(LevelNode node) {
				_completedOptionalLevels.Add(node);
			} 
		}
		
		readonly LevelsGraph _allLevels;
		
		readonly List<CompletedMainLevelInfo> _completedLevels = new List<CompletedMainLevelInfo>();

		LevelNode _curStartedLevel;

		public StartLevelNode StartLevelNode => _allLevels.nodes.Find(x => x is StartLevelNode) as StartLevelNode;
		
		public LevelType CurLevelType => CurLevelConfig.LevelType;

		public BaseLevelInfo CurLevelConfig => _curStartedLevel.Config;
		
		CompletedMainLevelInfo LastCompletedMainBranchLevel => HasCompletedLevels ? _completedLevels[_completedLevels.Count - 1] : null;

		bool HasCompletedLevels => _completedLevels.Count > 0;

		public LevelController() {
			_allLevels = LoadGraph();
			Assert.IsTrue(_allLevels);
		}
		
		public void StartLevel(LevelNode level) {
			_curStartedLevel = level;
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
				if ( !HasCompletedLevels || LastCompletedMainBranchLevel.IsNextMainLevel(_curStartedLevel) ) {
					_completedLevels.Add(new CompletedMainLevelInfo{ MainLevel = _curStartedLevel});
				} else {
					LastCompletedMainBranchLevel.SetOptionalLevelAsCompleted(_curStartedLevel);
				}
			} else {
				// Reset all progress
				_completedLevels.Clear();
			}
			_curStartedLevel = null;
		}

		public bool IsLevelCompleted(LevelNode node) {
			return _completedLevels.Exists(x => (x.MainLevel == node) || x.IsOptionalLevelCompleted(node));
		}
		
		public bool IsLevelAvailableToRun(LevelNode node) {
			if ( !HasCompletedLevels ) {
				return node == StartLevelNode;
			}
			return LastCompletedMainBranchLevel.IsNextMainLevel(node) || LastCompletedMainBranchLevel.IsOptionalLevel(node);
		}

		LevelsGraph LoadGraph() {
			return Resources.Load<LevelsGraph>("LevelsGraph");
		}
	}
}