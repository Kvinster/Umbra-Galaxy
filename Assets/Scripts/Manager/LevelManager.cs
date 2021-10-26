using UnityEngine;

using System;

using STP.Behaviour.Core;
using STP.Core;
using STP.Events;
using STP.Service;
using STP.Utils.Events;

using Cysharp.Threading.Tasks;

namespace STP.Manager {
	public sealed class LevelManager {
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

		public LevelManager(Transform playerTransform, SceneTransitionController sceneTransitionController,
			PauseManager pauseManager, LevelController levelController) {
			_playerTransform           = playerTransform;
			_sceneTransitionController = sceneTransitionController;
			_pauseManager              = pauseManager;
			_levelController           = levelController;

			CurLevelIndex = _levelController.CurLevelIndex;

			IsLevelActive = true;
		}

		public void Deinit() { }

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

		async UniTaskVoid SceneTransition() {
			_pauseManager.Pause(this);

			await _sceneTransitionController.PlayHideAnim(_playerTransform.position);

			SceneService.ReloadScene();
		}
	}
}
