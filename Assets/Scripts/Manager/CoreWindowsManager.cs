using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Core.UI;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public DeathWindow DeathWindow;
		[NotNull] public WinWindow   WinWindow;
		[NotNull] public PauseWindow PauseWindow;
		[NotNull] public Button      PauseButton;

		PauseManager     _pauseManager;
		PlayerController _playerController;

		List<BaseCoreWindow> _windows;

		bool IsAnyWindowShown => _windows.Any(x => x.IsShown); // :uuu: LINQ :uuu:

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Escape) && !IsAnyWindowShown ) {
				ShowPauseWindow();
			}
		}

		public void Init(PauseManager pauseManager, LevelManager levelManager, LevelGoalManager levelGoalManager,
			PlayerManager playerManager, PlayerController playerController, XpController xpController) {
			_pauseManager     = pauseManager;
			_playerController = playerController;

			_windows = new List<BaseCoreWindow> {
				DeathWindow,
				WinWindow,
				PauseWindow
			};

			DeathWindow.CommonInit(levelManager, playerManager, _playerController);
			WinWindow.CommonInit(levelManager, playerController, xpController);
			PauseWindow.CommonInit(levelManager, levelGoalManager, xpController, playerController);

			PauseButton.onClick.AddListener(ShowPauseWindow);

			levelGoalManager.OnLevelWon    += OnLevelWon;
			levelGoalManager.OnPlayerDeath += OnPlayerDied;
		}

		void ShowPauseWindow() {
			ShowWindow(PauseWindow);
		}

		void OnLevelWon() {
			ShowWindow(WinWindow);
		}

		void OnPlayerDied() {
			ShowWindow(DeathWindow);
		}

		void ShowWindow<T>(T window) where T : BaseCoreWindow {
			_pauseManager.Pause(this);
			window.Show()
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => _pauseManager.Unpause(this));
		}
	}
}
