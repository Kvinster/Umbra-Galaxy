using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Core.Minimap;
using STP.Behaviour.Core.UI;
using STP.Behaviour.Core.UI.WinWindow;
using STP.Core;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public GameObject  UiRoot;
		[Space]
		[NotNull] public BossHealthBar BossHealthBar;
		[NotNull] public MinimapController MinimapController;
		[NotNull] public LevelText         LevelText;
		[NotNull] public PlayerLivesUi     LivesUi;
		[NotNull] public DangerScreen      DangerScreen;
		[Space]
		[NotNull] public DeathWindow DeathWindow;
		[NotNull] public WinWindow   WinWindow;
		[NotNull] public PauseWindow PauseWindow;
		[NotNull] public Button      PauseButton;

		PauseManager     _pauseManager;
		PlayerController _playerController;

		List<BaseCoreWindow> _windows;

		bool IsAnyWindowShown => _windows.Any(x => x.IsShown);

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Escape) && !IsAnyWindowShown ) {
				ShowPauseWindow();
			}
		}

		public void Init(PauseManager pauseManager, LevelManager levelManager, LevelGoalManager levelGoalManager,
			PlayerManager playerManager, MinimapManager minimapManager, LevelController levelController,
			PlayerController playerController, ScoreController scoreController,
			LeaderboardController leaderboardController) {
			_pauseManager     = pauseManager;
			_playerController = playerController;

			_windows = new List<BaseCoreWindow> {
				DeathWindow,
				WinWindow,
				PauseWindow,
			};

			BossHealthBar.Init(levelController);
			MinimapController.Init(minimapManager);
			LevelText.Init(levelGoalManager, playerManager, playerController, scoreController);
			LivesUi.Init(playerController);

			DeathWindow.CommonInit(levelManager, playerManager, _playerController);
			WinWindow.CommonInit(scoreController, leaderboardController, levelManager);
			PauseWindow.CommonInit(levelManager, levelGoalManager, scoreController, playerController);
			PauseButton.onClick.AddListener(ShowPauseWindow);

			DangerScreen.Init();

			levelManager.OnLastLevelWon += LastLevelWon;
		}

		public void HideLevelUi() {
			UiRoot.SetActive(false);
		}

		public void ShowLevelUi() {
			UiRoot.SetActive(true);
		}

		public void ShowDeathWindow() {
			ShowWindow(DeathWindow, true);
		}

		void ShowPauseWindow() {
			ShowWindow(PauseWindow, false);
		}

		void LastLevelWon() {
			ShowWindow(WinWindow);
		}

		void ShowWindow<T>(T window, bool hideUi = true) where T : BaseCoreWindow {
			if ( hideUi ) {
				HideLevelUi();
			}
			_pauseManager.Pause(this);
			window.Show()
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => {
					_pauseManager.Unpause(this);
					if ( hideUi ) {
						ShowLevelUi();
					}
				});
		}
	}
}
