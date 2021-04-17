using UnityEngine;

using System;

using STP.Config;
using STP.Core;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly PlayerManager         _playerManager;
		readonly LevelManager          _levelManager;
		readonly LevelController       _levelController;
		readonly XpController          _xpController;
		readonly LeaderboardController _leaderboardController;
		readonly ProfileController     _profileController;

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
			XpController xpController, LeaderboardController leaderboardController,
			ProfileController profileController) {
			_playerManager         = playerManager;
			_levelManager          = levelManager;
			_levelController       = levelController;
			_xpController          = xpController;
			_leaderboardController = leaderboardController;
			_profileController     = profileController;

			var curLevelInfo = _levelController.GetCurLevelConfig();
			if ( !(curLevelInfo is RegularLevelInfo regularLevelInfo) ) {
				Debug.LogErrorFormat("{0}: unsupported level info type '{1}'", nameof(LevelGoalManager),
					curLevelInfo.GetType().Name);
				return;
			}

			LevelGoal = regularLevelInfo.GeneratorsCount;

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

			OnPlayerDeath?.Invoke();
		}

		void TryWinLevel() {
			if ( !CanWinLevel ) {
				return;
			}
			if ( !_levelManager.IsLevelActive ) {
				Debug.LogError("Can't win level — level is not active");
				return;
			}
			_levelController.FinishLevel(true);
			if ( _levelController.HasNextLevel ) {
				_profileController.Save();
				_levelManager.TryReloadLevel();
				return;
			}
			_leaderboardController.AddEntry(_profileController.ProfileName, _xpController.Xp.Value);
			OnLevelWon?.Invoke();
		}
	}
}
