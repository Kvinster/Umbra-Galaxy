using UnityEngine.Assertions;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class ProfileNameScreen : BaseMainMenuComponent {
		[NotNull] public TMP_InputField InputField;
		[NotNull] public Button         ContinueButton;
		[NotNull] public Button         CancelButton;

		MainMenuManager _mainMenuManager;

		string _stateName;

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			ContinueButton.onClick.AddListener(OnContinueClick);
			CancelButton.onClick.AddListener(OnCancelClick);

			InputField.onValueChanged.AddListener(OnValueChanged);
			OnValueChanged(InputField.text);
		}

		public void Show(string stateName) {
			_stateName      = stateName;
			InputField.text = "";

			Assert.IsFalse(GameState.IsActiveInstanceExists);

			gameObject.SetActive(true);
		}

		public void Hide() {
			_stateName      = null;
			InputField.text = "";

			gameObject.SetActive(false);
		}

		void OnValueChanged(string text) {
			ContinueButton.interactable = !string.IsNullOrEmpty(text);
		}

		void OnContinueClick() {
			var profileName = InputField.text;
			if ( string.IsNullOrEmpty(profileName) ) {
				return;
			}
			if ( GameState.CreateNewActiveGameState(_stateName, profileName) != null ) {
				_mainMenuManager.ShowMain();
			}
		}

		void OnCancelClick() {
			_mainMenuManager.ShowProfiles();
		}
	}
}
