using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class MainScreen : BaseMainMenuComponent {
		const string TitleStr = "Welcome!";

		[NotNull] public TMP_Text TitleText;
		[NotNull] public Button   PlayButton;
		[NotNull] public Button   ShowLeaderboardWindowButton;
		[NotNull] public Button   SettingsButton;
		[NotNull] public Button   ExitButton;

		MainMenuManager _mainMenuManager;

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			PlayButton.onClick.AddListener(Play);
			SettingsButton.onClick.AddListener(ShowSettingsWindow);
			ShowLeaderboardWindowButton.onClick.AddListener(ShowLeaderboardWindow);
			ExitButton.onClick.AddListener(Exit);
		}

		public void Show() {
			gameObject.SetActive(true);

			TitleText.text = TitleStr;
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void ShowSettingsWindow() {
			_mainMenuManager.ShowSettingsScreen();
		}

		void Play() {
			_mainMenuManager.ShowLevelsScreen();
		}

		void ShowLeaderboardWindow() {
			_mainMenuManager.ShowLeaderboard();
		}

		void Exit() {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
