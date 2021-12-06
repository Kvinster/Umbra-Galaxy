using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Service;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class MainScreen : GameComponent, IScreen {
		[NotNull] public Button   PlayButton;
		[NotNull] public Button   ShowLeaderboardWindowButton;
		[NotNull] public Button   SettingsButton;
		[NotNull] public Button   ExitButton;

		IScreenShower _screenShower;

		public void Init(IScreenShower screenShower) {
			_screenShower = screenShower;
			PlayButton.onClick.AddListener(Play);
			SettingsButton.onClick.AddListener(ShowSettingsWindow);
			ShowLeaderboardWindowButton.onClick.AddListener(ShowLeaderboardWindow);
			ExitButton.onClick.AddListener(Exit);
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void ShowSettingsWindow() {
			_screenShower.Show<SettingsScreen>();
		}

		void Play() {
			GameController.Instance.LevelController.StartLevel(0);
			SceneService.LoadLevel(0);
		}

		void ShowLeaderboardWindow() {
			_screenShower.Show<LeaderboardScreen>();
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
