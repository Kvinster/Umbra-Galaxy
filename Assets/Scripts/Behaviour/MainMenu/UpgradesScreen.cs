using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class UpgradesScreen : BaseMainMenuComponent {
		const string UpgradePointsTextTemplate = "Upgrade points: {0}";

		[NotNullOrEmpty]
		public List<CharacteristicPanel> CharacteristicPanels = new List<CharacteristicPanel>();
		[NotNull]
		public Button BackButton;
		[NotNull]
		public TMP_Text UpgradePointsText;

		MainMenuManager    _mainMenuManager;
		UpgradesController _upgradesController;

		void Reset() {
			GetComponentsInChildren(CharacteristicPanels);
		}

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Q) ) {
				_upgradesController.CheatAddUpgradePoints(1);
			}
		}

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager    = starter.MainMenuManager;
			_upgradesController = starter.UpgradesController;

			foreach ( var characteristicPanel in CharacteristicPanels ) {
				characteristicPanel.Init(starter.UpgradesController);
			}
			BackButton.onClick.AddListener(OnBackClick);
		}

		public void Show() {
			gameObject.SetActive(true);

			_upgradesController.OnUpgradePointsChanged += OnUpgradePointsChanged;
			OnUpgradePointsChanged(_upgradesController.UpgradePoints);
		}

		public void Hide() {
			gameObject.SetActive(false);

			_upgradesController.OnUpgradePointsChanged -= OnUpgradePointsChanged;
		}

		void OnUpgradePointsChanged(int upgradePoints) {
			UpgradePointsText.text = string.Format(UpgradePointsTextTemplate, upgradePoints);
		}

		void OnBackClick() {
			_mainMenuManager.ShowLevelsScreen();
		}
	}
}
