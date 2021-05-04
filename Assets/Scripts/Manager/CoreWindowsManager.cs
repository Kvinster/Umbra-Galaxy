using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Core.UI;
using STP.Behaviour.Core.UI.LevelUpWindow;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Manager {
	public sealed class CoreWindowsManager : GameComponent {
		[NotNull] public GameObject  UiRoot;
		[Space]
		[NotNull] public DeathWindow    DeathWindow;
		[NotNull] public WinWindow      WinWindow;
		[NotNull] public PauseWindow    PauseWindow;
		[NotNull] public LevelUpWindow  LevelUpWindow;
		[NotNull] public Button         PauseButton;
		[NotNull] public GetReadyWindow GetReadyWindow;

		PauseManager     _pauseManager;
		PlayerController _playerController;
		XpController     _xpController;

		List<BaseCoreWindow> _windows;

		bool IsAnyWindowShown => _windows.Any(x => x.IsShown); // :uuu: LINQ :uuu:

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Escape) && !IsAnyWindowShown ) {
				ShowPauseWindow();
			}

			if ( Input.GetKeyDown(KeyCode.Q) && (_xpController.LevelUpsCount > 0) ) {
				ShowLevelUpWindow();
			}
			
			if ( Input.GetKeyDown(KeyCode.E) ) {
				_xpController.AddLevelXp(_xpController.LevelXpCap+1);
			}
		}

		public void Init(CoreStarter starter, PauseManager pauseManager, LevelManager levelManager, LevelGoalManager levelGoalManager,
			PlayerManager playerManager, PlayerController playerController, XpController xpController, PrefabsController prefabsController) {
			_pauseManager     = pauseManager;
			_playerController = playerController;
			_xpController     = xpController;

			_windows = new List<BaseCoreWindow> {
				DeathWindow,
				WinWindow,
				PauseWindow,
				LevelUpWindow,
				GetReadyWindow
			};

			DeathWindow.CommonInit(levelManager, playerManager, _playerController);
			WinWindow.CommonInit(levelManager, playerController, xpController);
			PauseWindow.CommonInit(levelManager, levelGoalManager, xpController, playerController);
			LevelUpWindow.CommonInit(prefabsController, xpController, starter);
			PauseButton.onClick.AddListener(ShowPauseWindow);

			levelGoalManager.OnLevelWon    += OnLevelWon;
			levelGoalManager.OnPlayerDeath += OnPlayerDied;
		}

		public void ShowGetReadyWindow() {
			ShowWindow(GetReadyWindow);
		}

		void ShowLevelUpWindow() {
			ShowWindow(LevelUpWindow);
		}

		void ShowPauseWindow() {
			ShowWindow(PauseWindow, false);
		}

		void OnLevelWon() {
			ShowWindow(WinWindow);
		}

		void OnPlayerDied() {
			ShowWindow(DeathWindow);
		}

		void ShowWindow<T>(T window, bool hideUi = true) where T : BaseCoreWindow {
			if ( hideUi ) {
				UiRoot.SetActive(false);
			}
			_pauseManager.Pause(this);
			window.Show()
				.Catch(ex => { Debug.LogError(ex.Message); })
				.Finally(() => {
					_pauseManager.Unpause(this);
					if ( hideUi ) {
						UiRoot.SetActive(true);
					}
				});
		}
	}
}
