using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using STP.Behaviour.Core;
using STP.Config;
using STP.Controller;

using Object = UnityEngine.Object;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly Transform       _playerTransform;
		readonly LevelController _levelController;

		readonly LevelInfo _curLevelInfo;

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

		public LevelGoalManager(Transform playerTransform, LevelController levelController) {
			_playerTransform = playerTransform;
			_levelController = levelController;

			_curLevelInfo = _levelController.GetCurLevelConfig();

			LevelGoal = _curLevelInfo.GeneratorsCount;

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
			Time.timeScale = 0f;
			// TODO: properly restart level
			SceneTransitionController.Instance.Transition(SceneManager.GetActiveScene().name, _playerTransform.position,
					() => {
						var player = Object.FindObjectOfType<Player>();
						return player ? player.transform.position : Vector3.zero;
					})
				.Then(() => { Time.timeScale = 1f; });
		}

		bool TryWinLevel() {
			if ( !CanWinLevel ) {
				return false;
			}
			Time.timeScale = 0f;
			_levelController.ChangeLevel(_curLevelInfo.NextLevelName);
			SceneTransitionController.Instance.Transition(SceneManager.GetActiveScene().name, _playerTransform.position,
					() => {
						var player = Object.FindObjectOfType<Player>();
						return player ? player.transform.position : Vector3.zero;
					})
				.Then(() => { Time.timeScale = 1f; });
			return true;
		}
	}
}
