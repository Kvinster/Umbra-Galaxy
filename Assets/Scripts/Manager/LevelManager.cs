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
		readonly CoreWindowsManager        _windowsManager;

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
		public event Action       OnLevelWinStarted;
		public event Action       OnLastLevelWon;

		public LevelManager(Player player, SceneTransitionController sceneTransitionController,
			PauseManager pauseManager, LevelController levelController, CoreWindowsManager windowsManager) {
			_player                    = player;
			_playerTransform           = player.transform;
			_sceneTransitionController = sceneTransitionController;
			_pauseManager              = pauseManager;
			_levelController           = levelController;
			_windowsManager            = windowsManager;

			CurLevelIndex = _levelController.CurLevelIndex;
		}

		public void Deinit() { }

		void GoToNextLevel() {
			_pauseManager.Pause(this);
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
			_levelController.FinishLevel();
			SceneService.LoadMainMenu();
		}

		public void StartLevelWin() {
			OnLevelWinStarted?.Invoke();
		}

		public void FinishLevelWin() {
			if ( CurLevelIndex < LevelsConfig.Instance.TotalLevelsCount - 1 ) {
				_levelController.FinishLevel();
				GoToNextLevel();
			} else {
				OnLastLevelWon?.Invoke();
			}
		}

		public void ActivateLevel() {
			IsLevelActive = true;
		}

		async UniTaskVoid SceneTransition() {
			_pauseManager.Pause(this);

			await _sceneTransitionController.PlayHideAnim(_playerTransform.position);

			SceneService.ReloadScene();
		}
	}
}
