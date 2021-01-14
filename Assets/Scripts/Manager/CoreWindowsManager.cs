using UnityEngine;

using STP.Behaviour.Core.UI;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public DeathWindow DeathWindow;
		[NotNull] public WinWindow   WinWindow;

		PauseManager     _pauseManager;
		PlayerController _playerController;

		public void Init(PauseManager pauseManager, LevelManager levelManager, PlayerManager playerManager,
			PlayerController playerController, XpController xpController) {
			_pauseManager     = pauseManager;
			_playerController = playerController;

			DeathWindow.CommonInit(levelManager, playerManager);
			WinWindow.CommonInit(playerController, xpController);
		}

		public void ShowDeathWindow() {
			_pauseManager.Pause(this);
			DeathWindow.Show(_playerController.CurLives)
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => _pauseManager.Unpause(this));
		}

		public void ShowWinWindow() {
			_pauseManager.Pause(this);
			WinWindow.Show()
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => _pauseManager.Unpause(this));
		}
	}
}
