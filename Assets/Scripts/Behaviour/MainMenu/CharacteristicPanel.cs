using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Common;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class CharacteristicPanel : GameComponent {
		public PlayerCharacteristicType CharacteristicType = PlayerCharacteristicType.Unknown;

		[NotNullOrEmpty][Count(5)]
		public List<GameObject> Bars = new List<GameObject>();
		[NotNull]
		public Button UpgradeButton;
		[NotNull]
		public TMP_Text CharacteristicTypeText;

		UpgradesController _upgradesController;

		bool _subscribed;

		void OnEnable() {
			TrySubscribe();
		}

		void OnDisable() {
			_upgradesController.OnCharacteristicLevelChanged -= OnCharacteristicLevelChanged;
			_upgradesController.OnUpgradePointsChanged       -= OnUpgradePointsChanged;

			_subscribed = false;
		}

		public void Init(UpgradesController upgradesController) {
			_upgradesController = upgradesController;

			var curLevel = _upgradesController.GetCurCharacteristicLevel(CharacteristicType);
			OnCharacteristicLevelChanged(curLevel);

			UpgradeButton.onClick.AddListener(OnUpgradeClick);

			CharacteristicTypeText.text = CharacteristicType.ToString();

			TrySubscribe();
		}

		void TrySubscribe() {
			if ( (_upgradesController == null) || _subscribed ) {
				return;
			}
			_upgradesController.OnCharacteristicLevelChanged += OnCharacteristicLevelChanged;
			_upgradesController.OnUpgradePointsChanged       += OnUpgradePointsChanged;

			_subscribed = true;
		}

		void OnUpgradeClick() {
			if ( !_upgradesController.TryUpgrade(CharacteristicType) ) {
				Debug.LogErrorFormat("CharacteristicPanel.OnUpgradeClick: can't upgrade {0}, but the button is active",
					CharacteristicType);
			}
		}

		void UpdateUpgradeButton() {
			UpgradeButton.interactable = _upgradesController.CanUpgrade(CharacteristicType);
		}

		void OnUpgradePointsChanged(int _) {
			UpdateUpgradeButton();
		}

		void OnCharacteristicLevelChanged(PlayerCharacteristicType characteristicType, int level) {
			if ( characteristicType == CharacteristicType ) {
				OnCharacteristicLevelChanged(level);
			}
		}

		void OnCharacteristicLevelChanged(int level) {
			int i;
			for ( i = 0; i < level; ++i ) {
				Bars[i].gameObject.SetActive(true);
			}
			for ( ; i < Bars.Count; ++i ) {
				Bars[i].gameObject.SetActive(false);
			}

			UpdateUpgradeButton();
		}
	}
}
