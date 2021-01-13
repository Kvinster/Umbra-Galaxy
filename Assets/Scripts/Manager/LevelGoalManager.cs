using UnityEngine;

using System;

using STP.Config;
using STP.Core;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly LevelManager    _levelManager;
		readonly LevelController _levelController;
		readonly XpController    _xpController;

		int _curLevelGoalProgress;

		public int CurLevelGoalProgress {
			get => _curLevelGoalProgress;
			private set {
				if ( _curLevelGoalProgress == value ) {
					return;
				}
				_curLevelGoalProgress = value;
				OnCurLevelGoalProgressChanged?.Invoke(_curLevelGoalProgress);
			}
		}

		public bool CanWinLevel { get; private set; }

		public event Action<int> OnCurLevelGoalProgressChanged;

		public LevelGoalManager(LevelManager levelManager, LevelController levelController, XpController xpController) {
			_levelManager    = levelManager;
			_levelController = levelController;
			_xpController    = xpController;

			var curLevelInfo = _levelController.GetCurLevelConfig();

			LevelGoal = curLevelInfo.GeneratorsCount;

			CurLevelGoalProgress = 0;
		}

		public void Advance(int goalAdd = 1) {
			CurLevelGoalProgress += goalAdd;

			if ( CurLevelGoalProgress >= LevelGoal ) {
				CanWinLevel = true;
				TryWinLevel();
			}
		}

		public void LoseLevel() {
			if ( !_levelManager.IsLevelActive ) {
				Debug.LogError("Can't lose level — level is not active");
				return;
			}
			_xpController.ResetXp();
			_levelManager.TryReloadLevel();
		}

		bool TryWinLevel() {
			if ( !CanWinLevel ) {
				return false;
			}
			if ( !_levelManager.IsLevelActive ) {
				Debug.LogError("Can't win level — level is not active");
				return false;
			}
			_levelController.OnLevelWon();
			_xpController.OnLevelWon();
			if ( _levelController.HasNextLevel ) {
				return _levelManager.TryReloadLevel();
			} else {
				// TODO: win the game instead of the following
				Debug.Log("You win!");
				_levelController.TmpPreviousLevel();
				return _levelManager.TryReloadLevel();
			}
		}
	}
}
