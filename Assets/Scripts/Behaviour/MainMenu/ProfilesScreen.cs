using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using STP.Utils.Xml;

namespace STP.Behaviour.MainMenu {
	public sealed class ProfilesScreen : BaseMainMenuComponent {
		const string ExistingStateFormat = "{0}: Level {1}";
		const string NonExistingState    = "Create profile";

		static readonly string[] SaveNames = { "first", "second", "third" };

		[NotNullOrEmpty] [Count(3)]
		public List<ProfileButton> ProfileButtons = new List<ProfileButton>();

		MainMenuManager _mainMenuManager;

		void Reset() {
			GetComponentsInChildren(ProfileButtons);
		}

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;
		}

		public void Show() {
			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			foreach ( var profileButton in ProfileButtons ) {
				profileButton.Deinit();
			}
			gameObject.SetActive(false);
		}

		void UpdateView() {
			if ( ProfileController.IsActiveInstanceExists ) {
				Debug.LogError("Active profile controller instance exists");
				ProfileController.ReleaseActiveInstance();
			}

			for ( var i = 0; i < SaveNames.Length; ++i ) {
				var profileButton = ProfileButtons[i];
				var stateName     = SaveNames[i];

				profileButton.Deinit();

				string text;
				Action onMainClick;
				Action onRemoveClick = null;

				var          isExists     = XmlUtils.IsGameStateDocumentExists(stateName);
				var          canInitState = isExists;
				ProfileState ps           = null;
				if ( isExists ) {
					ps = ProfileState.LoadGameState(stateName);
					if ( ps == null ) {
						Debug.LogErrorFormat("Can't load profile state '{0}'", stateName);
						canInitState = false;
					} else if ( string.IsNullOrEmpty(ps.ProfileName) ) {
						ProfileState.TryRemoveSave(ps.StateName);
						canInitState = false;
					}
				}

				if ( canInitState ) {
					text = string.Format(ExistingStateFormat, ps.ProfileName, ps.LevelState.LastLevelIndex + 1);
					onMainClick   = () => LoadState(stateName);
					onRemoveClick = () => RemoveState(stateName);
				} else {
					text        = NonExistingState;
					onMainClick = () => CreateState(stateName);
				}
				profileButton.Init(text, onMainClick, onRemoveClick, isExists);
			}
		}

		void CreateState(string stateName) {
			_mainMenuManager.ShowProfileNameScreen(stateName);
		}

		void LoadState(string stateName) {
			var ps = ProfileState.LoadGameState(stateName);
			if ( ps != null ) {
				ProfileController.CreateNewActiveInstance(ps);
				_mainMenuManager.ShowMain();
			}
		}

		void RemoveState(string stateName) {
			if ( ProfileState.TryRemoveSave(stateName) ) {
				UpdateView();
			}
		}
	}
}
