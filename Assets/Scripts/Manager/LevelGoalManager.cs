using UnityEngine;

using System;

using STP.Core;
using STP.Core.State;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly PlayerManager         _playerManager;
		readonly LevelManager          _levelManager;
		readonly LevelController       _levelController;
		readonly XpController          _xpController;
		readonly LeaderboardController _leaderboardController;
		readonly ProfileState          _profileState;

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
		public event Action      OnPlayerDeath;
		public event Action      OnLevelWon;

		public LevelGoalManager(PlayerManager playerManager, LevelManager levelManager, LevelController levelController,
			XpController xpController, LeaderboardController leaderboardController, ProfileState profileState) {
			_playerManager         = playerManager;
			_levelManager          = levelManager;
			_levelController       = levelController;
			_xpController          = xpController;
			_leaderboardController = leaderboardController;
			_profileState          = profileState;

			var curLevelInfo = _levelController.GetCurLevelConfig();

			LevelGoal = curLevelInfo.GeneratorsCount;

			CurLevelGoalProgress = 0;
		}

		public void Deinit() { }

		public void Advance(int goalAdd = 1) {
			CurLevelGoalProgress += goalAdd;

			if ( CurLevelGoalProgress >= LevelGoal ) {
				CanWinLevel = true;
				TryWinLevel();
			}
		}

		public void OnPlayerDied() {
			if ( !_playerManager.OnPlayerDied() ) {
				_xpController.ResetXp();
			}

			OnPlayerDeath?.Invoke();
		}

		bool TryWinLevel() {
			if ( !CanWinLevel ) {
				return false;
			}
			if ( !_levelManager.IsLevelActive ) {
				Debug.LogError("Can't win level — level is not active");
				return false;
			}
			_levelController.FinishLevel(true);
			_xpController.OnLevelWon();
			if ( _levelController.HasNextLevel ) {
				_profileState.Save();
				return _levelManager.TryReloadLevel();
			}
			_leaderboardController.AddEntry(_profileState.ProfileName, _xpController.CurTotalXp);
			OnLevelWon?.Invoke();
			return true;
		}
	}
}
