using UnityEngine;

using System;
using System.Linq;

using STP.Behaviour.Core.Enemy;
using STP.Config;
using STP.Core;

using Object = UnityEngine.Object;

namespace STP.Manager {
	public sealed class LevelGoalManager {
		public readonly int LevelGoal;

		readonly PlayerManager _playerManager;
		readonly LevelManager  _levelManager;

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
		public bool IsLevelWon  { get; private set; }

		public event Action<int> OnCurLevelGoalProgressChanged;
		public event Action      OnPlayerDeath;

		public LevelGoalManager(PlayerManager playerManager, LevelManager levelManager, LevelController levelController) {
			_playerManager = playerManager;
			_levelManager  = levelManager;

			var curLevelInfo = levelController.CurLevelConfig;
			switch ( curLevelInfo ) {
				case RegularLevelInfo regularLevelInfo: {
					LevelGoal = Object.FindObjectsOfType<Generator>().Count(x => x.IsMainGenerator); // TODO: <– not that
					break;
				}
				case BossLevelInfo bossLevelInfo: {
					LevelGoal = 1; // TODO: boss count?
					break;
				}
				default: {
					Debug.LogErrorFormat("{0}: unsupported level info type '{1}'", nameof(LevelGoalManager),
						curLevelInfo.GetType().Name);
					return;
				}
			}

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
			_playerManager.SubLife();
			OnPlayerDeath?.Invoke();
		}

		void TryWinLevel() {
			if ( !CanWinLevel || IsLevelWon ) {
				return;
			}
			if ( !_levelManager.IsLevelActive ) {
				Debug.LogError("Can't win level — level is not active");
				return;
			}
			IsLevelWon = true;
		}
	}
}
