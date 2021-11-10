using UnityEngine;

using System;

using STP.Behaviour.Core;
using STP.Config;
using STP.Core;
using STP.Service;
using STP.Utils;

using Cysharp.Threading.Tasks;

namespace STP.Manager {
	public sealed class LevelManager {
		const float LevelWinDelay = 2f;

		readonly Player                    _player;
		readonly Transform                 _playerTransform;
		readonly SceneTransitionController _sceneTransitionController;
		readonly PauseManager              _pauseManager;
		readonly LevelController           _levelController;

		bool _isLevelActive;

		public int CurLevelIndex { get; }

		public bool IsLevelActive {
			get => _isLevelActive;
			private set {
				if ( _isLevelActive == value ) {
					return;
				}
				_isLevelActive = value;
				OnIsLevelActiveChanged?.Invoke(_isLevelActive);
			}
		}

		public event Action<bool> OnIsLevelActiveChanged;
		public event Action       OnLastLevelWon;

		public LevelManager(Player player, SceneTransitionController sceneTransitionController,
			PauseManager pauseManager, LevelController levelController) {
			_player                    = player;
			_playerTransform           = player.transform;
			_sceneTransitionController = sceneTransitionController;
			_pauseManager              = pauseManager;
			_levelController           = levelController;

			CurLevelIndex = _levelController.CurLevelIndex;

			IsLevelActive = true;
		}

		public void Deinit() { }

		public void GoToNextLevel() {
			_levelController.StartLevel(CurLevelIndex + 1);
			SceneService.LoadLevel(CurLevelIndex + 1);
		}

		public bool TryReloadLevel() {
			if ( !IsLevelActive ) {
				Debug.LogError("Can't reload level — level not active");
				return false;
			}
			IsLevelActive = false;
			UniTask.Void(SceneTransition);
			return true;
		}

		public void QuitToMenu() {
			if ( !IsLevelActive ) {
				Debug.LogError("Can't quit level — level not active");
				return;
			}
			IsLevelActive = false;
			SceneService.LoadMainMenu();
		}

		public void StartLevelWin() {
			_player.OnLevelWin();
			AsyncUtils.DelayedAction(StartLevelWinInternal, LevelWinDelay);
		}

		void StartLevelWinInternal() {
			_levelController.FinishLevel();
			if ( CurLevelIndex < LevelsConfig.Instance.TotalLevelsCount - 1 ) {
				GoToNextLevel();
			} else {
				OnLastLevelWon?.Invoke();
			}
		}

		async UniTaskVoid SceneTransition() {
			_pauseManager.Pause(this);

			await _sceneTransitionController.PlayHideAnim(_playerTransform.position);

			SceneService.ReloadScene();
		}
	}
}
