using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using STP.Behaviour.Core;
using STP.Core;

using Object = UnityEngine.Object;

namespace STP.Manager {
	public sealed class LevelManager {
		readonly Transform       _playerTransform;
		readonly PauseManager    _pauseManager;
		readonly LevelController _levelController;

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

		public LevelManager(Transform playerTransform, PauseManager pauseManager, LevelController levelController) {
			_playerTransform = playerTransform;
			_pauseManager    = pauseManager;
			_levelController = levelController;

			IsLevelActive = true;
		}

		public bool TryReloadLevel() {
			if ( !IsLevelActive ) {
				Debug.LogError("Can't reload level — level not active");
				return false;
			}
			IsLevelActive = false;
			SceneTransition();
			return true;
		}

		public void QuitToMenu() {
			if ( !IsLevelActive ) {
				Debug.LogError("Can't quit level — level not active");
				return;
			}
			IsLevelActive = false;
			_levelController.FinishLevel(false);
			ProfileController.ReleaseActiveInstance();
			SceneManager.LoadScene("MainMenu");
		}

		void SceneTransition() {
			_pauseManager.Pause(this);
			SceneTransitionController.Instance.Transition(SceneManager.GetActiveScene().name, _playerTransform.position,
				() => {
					var player = Object.FindObjectOfType<Player>();
					return player ? player.transform.position : Vector3.zero;
				});
		}
	}
}
