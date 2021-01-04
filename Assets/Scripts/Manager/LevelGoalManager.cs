using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using STP.Behaviour.Core;

using Object = UnityEngine.Object;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		const int LevelGoalTmp = 5;

		public readonly int LevelGoal;

		readonly Transform _playerTransform;

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

		public bool CanEndLevel { get; private set; }

		public event Action<int> OnCurLevelGoalProgressChanged;

		public LevelGoalManager(Transform playerTransform) {
			_playerTransform = playerTransform;

			// TODO: replace with level config of some sort
			LevelGoal = LevelGoalTmp;

			CurLevelGoalProgress = 0;
		}

		public void Advance(int goalAdd = 1) {
			CurLevelGoalProgress += goalAdd;

			if ( CurLevelGoalProgress >= LevelGoal ) {
				CanEndLevel = true;
				TryEndLevel();
			}
		}

		bool TryEndLevel() {
			if ( !CanEndLevel ) {
				return false;
			}
			Time.timeScale = 0f;
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
