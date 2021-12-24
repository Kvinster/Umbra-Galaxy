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
			SettingsButton.onClick.AddListener(_screenShower.Show<SettingsScreen>);
			ShowLeaderboardWindowButton.onClick.AddListener(_screenShower.Show<LeaderboardScreen>);
			ExitButton.onClick.AddListener(_screenShower.ShowWithoutHiding<QuitScreen>);
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void Play() {
			GameController.Instance.LevelController.StartLevel(0);
			SceneService.LoadLevel(GameController.Instance.LevelController.CurLevelConfig.SceneName);
		}
	}
}