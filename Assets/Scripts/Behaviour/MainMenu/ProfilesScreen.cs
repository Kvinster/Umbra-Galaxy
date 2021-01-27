using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;
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
			if ( GameState.IsActiveInstanceExists ) {
				Debug.LogError("Active game state instance exists");
				GameState.ReleaseActiveInstance();
			}

			for ( var i = 0; i < SaveNames.Length; ++i ) {
				var profileButton = ProfileButtons[i];
				var stateName     = SaveNames[i];

				profileButton.Deinit();

				string text;
				Action onMainClick;
				Action onRemoveClick = null;

				var       isExists     = XmlUtils.IsGameStateDocumentExists(stateName);
				var       canInitState = isExists;
				GameState gs           = null;
				if ( isExists ) {
					gs = GameState.LoadGameState(stateName);
					if ( gs == null ) {
						Debug.LogErrorFormat("Can't load game state '{0}'", stateName);
						canInitState = false;
					} else if ( string.IsNullOrEmpty(gs.ProfileName) ) {
						GameState.TryRemoveSave(gs.StateName);
						canInitState = false;
					}
				}

				if ( canInitState ) {
					text = string.Format(ExistingStateFormat, gs.ProfileName,
						gs.LevelState.NextLevelIndex + 1);
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
			var gs = GameState.LoadGameState(stateName);
			if ( gs != null ) {
				GameState.SetActiveInstance(gs);
				_mainMenuManager.ShowMain();
			}
		}

		void RemoveState(string stateName) {
			if ( GameState.TryRemoveSave(stateName) ) {
				UpdateView();
			}
		}
	}
}
