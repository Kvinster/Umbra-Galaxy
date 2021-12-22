using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using STP.Behaviour.Core;
using STP.Behaviour.Core.Enemy;
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
		[NotNull] public PlayerScoreUi          ScoreUi;
		[NotNull] public PlayerLivesUi          LivesUi;
		[NotNull] public DangerScreen           DangerScreen;
		[NotNull] public MinimapGrid            MinimapGrid;
		[NotNull] public DamageScreen           DamageScreen;
		[NotNull] public PowerUpTimerUisManager PowerUpTimerUisManager;
		[NotNull] public BossLevelDialog        BossLevelDialog;
 		[Space]
		[NotNull] public DeathWindow DeathWindow;
		[NotNull] public WinWindow   WinWindow;
		[NotNull] public PauseWindow PauseWindow;

		[NotNull] public CanvasGroup UiCanvasGroup;

		PauseManager _pauseManager;

		List<BaseCoreWindow> _windows;

		Tween _tween;

		bool IsAnyWindowShown => _windows.Any(x => x.IsShown);

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Escape) && !IsAnyWindowShown ) {
				ShowPauseWindow();
			}
		}

		void OnDisable() {
			ScoreUi.Deinit();
			LivesUi.Deinit();
			MinimapGrid.Deinit();
			DamageScreen.Deinit();
			BossLevelDialog.Deinit();
			PowerUpTimerUisManager.Deinit();
			_tween?.Kill(true);
		}

		public void Init(PauseManager pauseManager, LevelManager levelManager,
			PlayerManager playerManager, MinimapManager minimapManager, LevelController levelController,
			PlayerController playerController, ScoreController scoreController,
			LeaderboardController leaderboardController, Player player, BaseBoss boss) {
			_pauseManager = pauseManager;

			_windows = new List<BaseCoreWindow> {
				DeathWindow,
				WinWindow,
				PauseWindow,
			};

			BossHealthBar.Init(boss);
			ScoreUi.Init(scoreController);
			LivesUi.Init(playerController);
			MinimapGrid.Init(player, minimapManager);
			DamageScreen.Init(playerController);
			PowerUpTimerUisManager.Init(playerManager);
			BossLevelDialog.Init(levelController);

			DeathWindow.CommonInit(levelManager);
			WinWindow.CommonInit(scoreController, leaderboardController, levelManager);
			PauseWindow.CommonInit(levelManager);

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
			ShowWindow(DeathWindow);
		}

		public void FadeInUi() {
			UiCanvasGroup.alpha = 0f;
			_tween = UiCanvasGroup.DOFade(1f, 0.5f);
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