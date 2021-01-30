using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class MainScreen : BaseMainMenuComponent {
		const string TitleFormat = "Welcome, {0}";

		[NotNull] public TMP_Text TitleText;
		[NotNull] public Button   PlayButton;
		[NotNull] public Button   ShowLeaderboardWindowButton;
		[NotNull] public Button   ChangeProfileButton;
		[NotNull] public Button   SettingsButton;
		[NotNull] public Button   ExitButton;

		MainMenuManager _mainMenuManager;

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			PlayButton.onClick.AddListener(Play);
			SettingsButton.onClick.AddListener(ShowSettingsWindow);
			ShowLeaderboardWindowButton.onClick.AddListener(ShowLeaderboardWindow);
			ChangeProfileButton.onClick.AddListener(ChangeProfile);
			ExitButton.onClick.AddListener(Exit);
		}

		public void Show() {
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.LogError("No active game state instance");
				return;
			}
			gameObject.SetActive(true);

			TitleText.text = string.Format(TitleFormat, GameState.ActiveInstance.ProfileName);
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

		void ChangeProfile() {
			GameState.ReleaseActiveInstance();
			_mainMenuManager.ShowProfiles();
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
