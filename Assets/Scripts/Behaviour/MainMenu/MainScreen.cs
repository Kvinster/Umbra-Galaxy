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
		[NotNull] public Button   ExitButton;

		MainMenuManager _mainMenuManager;

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			PlayButton.onClick.AddListener(Play);
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

		void Play() {
			_mainMenuManager.ShowLevelsScreen();
		}

		void ShowLeaderboardWindow() {
			Hide();
			_mainMenuManager.ShowLeaderboard();
		}

		void ChangeProfile() {
			GameState.ReleaseActiveInstance();
			Hide();
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
