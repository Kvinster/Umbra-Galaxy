using UnityEngine;

using System;

using STP.Behaviour.Sound;
using STP.Core;
using STP.Core.State;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly PlayerManager         _playerManager;
		readonly LevelManager          _levelManager;
		readonly PlayerController      _playerController;
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

		public LevelGoalManager(PlayerManager playerManager, LevelManager levelManager,
			PlayerController playerController, LevelController levelController, XpController xpController,
			LeaderboardController leaderboardController, ProfileState profileState) {
			_playerManager         = playerManager;
			_levelManager          = levelManager;
			_playerController      = playerController;
			_levelController       = levelController;
			_xpController          = xpController;
			_leaderboardController = leaderboardController;
			_profileState          = profileState;

			var curLevelInfo = _levelController.GetCurLevelConfig();

			LevelGoal = curLevelInfo.GeneratorsCount;

			CurLevelGoalProgress = 0;

			_playerController.OnIsAliveChanged += OnPlayerIsAliveChanged;
		}

		public void Deinit() {
			if ( _playerController != null ) {
				_playerController.OnIsAliveChanged -= OnPlayerIsAliveChanged;
			}
		}

		public void Advance(int goalAdd = 1) {
			CurLevelGoalProgress += goalAdd;

			if ( CurLevelGoalProgress >= LevelGoal ) {
				CanWinLevel = true;
				TryWinLevel();
			}
		}

		void OnPlayerIsAliveChanged(bool isPlayerAlive) {
			if ( !isPlayerAlive ) {
				OnPlayerDied();
			}
		}

		void OnPlayerDied() {
			if ( !_playerManager.OnPlayerDied() ) {
				_xpController.ResetXp();
			}

			UniTask.Void(StartPlayerDeath);
		}

		async UniTaskVoid StartPlayerDeath() {
			var progress = 0f;
			Time.timeScale = 0.5f;
			var anim = DOTween.To(() => progress, x => {
				progress = x;
				PersistentAudioPlayer.Instance.SetPitch(1f - x);
			}, 1f, 2f).SetUpdate(true).SetEase(Ease.OutQuad);
			await anim;
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
