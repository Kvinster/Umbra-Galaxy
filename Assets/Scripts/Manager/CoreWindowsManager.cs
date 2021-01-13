﻿using UnityEngine;

using STP.Behaviour.Core.UI;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public DeathWindow DeathWindow;

		PauseManager     _pauseManager;
		PlayerController _playerController;

		public void Init(PauseManager pauseManager, PlayerManager playerManager, LevelGoalManager levelGoalManager,
			PlayerController playerController) {
			_pauseManager     = pauseManager;
			_playerController = playerController;

			DeathWindow.CommonInit(playerManager, levelGoalManager);
		}

		public void ShowDeathWindow() {
			_pauseManager.Pause(this);
			DeathWindow.Show(_playerController.CurLives)
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => _pauseManager.Unpause(this));
		}
	}
}
