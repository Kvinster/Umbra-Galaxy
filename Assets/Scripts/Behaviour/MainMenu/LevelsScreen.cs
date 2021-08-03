using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class LevelsScreen : BaseMainMenuComponent {
		[NotNull] public Button     BackButton;
		[NotNull] public Button     UpgradesButton;
		[NotNull] public ScrollRect LevelButtonsScrollRect;

		[NotNull] public GameObject LevelButtonPrefab;
		[NotNull] public GameObject LayerPrefab;

		LevelController _levelController;

		MainMenuManager _mainMenuManager;

		readonly LevelGraphDrawer _graphDrawer = new LevelGraphDrawer();

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;
			_levelController = starter.GameController.LevelController;
			_graphDrawer.InitGraph(_levelController, LayerPrefab, LevelButtonPrefab, _levelController.StartLevelNode, LevelButtonsScrollRect.content);
			BackButton.onClick.AddListener(OnBackClick);
			UpgradesButton.onClick.AddListener(OnUpgradesClick);
		}

		public void Show() {
			_graphDrawer.DrawGraph();
			gameObject.SetActive(true);
			LevelButtonsScrollRect.horizontalNormalizedPosition = 0f;
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void OnBackClick() {
			_mainMenuManager?.ShowMain();
		}

		void OnUpgradesClick() {
			_mainMenuManager?.ShowUpgradesScreen();
		}
	}
}
