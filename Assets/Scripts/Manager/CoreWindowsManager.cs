using UnityEngine;

using STP.Behaviour.Core.UI;
using STP.Controller;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public DeathWindow DeathWindow;

		PlayerController _playerController;

		public void Init(PlayerManager playerManager, LevelGoalManager levelGoalManager,
			PlayerController playerController) {
			_playerController = playerController;

			DeathWindow.CommonInit(playerManager, levelGoalManager);
		}

		public void ShowDeathWindow() {
			Time.timeScale = 0f;
			DeathWindow.Show(_playerController.CurLives)
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => Time.timeScale = 1f);
		}
	}
}
