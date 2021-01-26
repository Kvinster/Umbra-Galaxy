using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;

using STP.Behaviour.Starter;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class MainScreen : BaseMainMenuComponent {
		[NotNull] public Button StartNewGameButton;
		[NotNull] public Button ShowLoadWindowButton;
		[NotNull] public Button ShowLeaderboardWindowButton;

		MainMenuManager _mainMenuManager;

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			StartNewGameButton.onClick.AddListener(StartNewGame);
			ShowLoadWindowButton.onClick.AddListener(ShowLoadWindow);
			ShowLeaderboardWindowButton.onClick.AddListener(ShowLeaderboardWindow);
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void StartNewGame() {
			GameState.CreateNewActiveGameState(Guid.NewGuid().ToString());
			SceneManager.LoadScene("TestRoom");
		}

		void ShowLoadWindow() {
			Hide();
			_mainMenuManager.ShowLoad();
		}

		void ShowLeaderboardWindow() {
			Hide();
			_mainMenuManager.ShowLeaderboard();
		}
	}
}
