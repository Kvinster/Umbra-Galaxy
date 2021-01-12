﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections.Generic;
using System.IO;

using STP.Behaviour.Starter;
using STP.Core.State;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class LoadWindow : BaseMainMenuComponent {
		[NotNull]
		public ScrollRect ScrollRect;
		[NotNull]
		public Button BackButton;
		[NotNullOrEmpty]
		public List<LoadEntry> Entries = new List<LoadEntry>();

		MainMenuManager _mainMenuManager;

		void Reset() {
			ScrollRect = GetComponentInChildren<ScrollRect>();
			GetComponentsInChildren(Entries);
		}

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			BackButton.onClick.AddListener(ShowMain);
		}

		public void Show() {
			gameObject.SetActive(true);
			ResetEntries();

			var di = new DirectoryInfo(GameState.SavesPath);
			if ( !di.Exists ) {
				Debug.LogError("Saves directory does not exist");
				return;
			}
			var entryIndex = 0;
			foreach ( var saveFile in di.EnumerateFiles("*.stpsave") ) {
				if ( entryIndex >= Entries.Count ) {
					Debug.LogError("Not enough LoadWindow Entries");
					break;
				}
				var gs = GameState.LoadGameState(Path.GetFileNameWithoutExtension(saveFile.Name));
				if ( gs != null ) {
					var entry = Entries[entryIndex++];
					entry.Init(gs, x => {
						GameState.SetActiveInstance(x);
						SceneManager.LoadScene("Scenes/TestRoom");
					});
					entry.gameObject.SetActive(true);
				}
			}

			ScrollRect.verticalNormalizedPosition = 1f;
		}

		public void Hide() {
			ResetEntries();
			gameObject.SetActive(false);
		}

		void ShowMain() {
			_mainMenuManager.ShowMain();
		}

		void ResetEntries() {
			foreach ( var entry in Entries ) {
				entry.Deinit();
				entry.gameObject.SetActive(false);
			}
		}
	}
}