using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using STP.Behaviour.Core;
using STP.Core;
using STP.Events;
using STP.Utils.Events;

using Cysharp.Threading.Tasks;

namespace STP.Manager {
	public sealed class LevelManager {
		Transform _playerTransform;
		readonly SceneTransitionController _sceneTransitionController;
		readonly PauseManager              _pauseManager;
		readonly LevelController           _levelController;

		bool _isLevelActive;

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

			IsLevelActive = true;
			EventManager.Subscribe<PlayerShipChanged>(UpdatePlayerComp);
		}

		public void Deinit() {
			EventManager.Unsubscribe<PlayerShipChanged>(UpdatePlayerComp);
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
			_levelController.FinishLevel(false);
			SceneManager.LoadScene("MainMenu");
		}

		async UniTaskVoid SceneTransition() {
			_pauseManager.Pause(this);

			await _sceneTransitionController.PlayHideAnim(_playerTransform.position);

			var clm = CoreLoadingManager.Create();
			if ( clm != null ) {
				UniTask.Void(clm.LoadCore);
			}
		}

		void UpdatePlayerComp(PlayerShipChanged ship) {
			_playerTransform = ship.NewPlayer.transform;
		}
	}
}
