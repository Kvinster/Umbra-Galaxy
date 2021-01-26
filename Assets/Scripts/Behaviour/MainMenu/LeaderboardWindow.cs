using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class LeaderboardWindow : BaseMainMenuComponent {
		[NotNull]
		public GameObject HaveLeaderboardEntriesRoot;
		[NotNull]
		public GameObject NoLeaderboardEntriesRoot;
		[NotNull]
		public ScrollRect ScrollRect;
		[NotNull]
		public Button ClearButton;
		[NotNull]
		public Button BackButton;
		[NotNullOrEmpty]
		public List<LeaderboardEntryView> Entries = new List<LeaderboardEntryView>();

		MainMenuManager       _mainMenuManager;
		LeaderboardController _leaderboardController;

		void Reset() {
			ScrollRect = GetComponentInChildren<ScrollRect>();
			GetComponentsInChildren(Entries);
		}

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager       = starter.MainMenuManager;
			_leaderboardController = starter.LeaderboardController;

			ClearButton.onClick.AddListener(ClearLeaderboard);
			BackButton.onClick.AddListener(ShowMain);
		}

		public void Show() {
			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			ResetEntries();
			gameObject.SetActive(false);
		}

		void UpdateView() {
			ResetEntries();

			var entries        = _leaderboardController.Entries;
			var haveSavedGames = (entries.Count > 0);
			var entryIndex     = 0;
			foreach ( var entry in entries ) {
				if ( entryIndex >= Entries.Count ) {
					Debug.LogError("Not enough LeaderboardEntryViews");
					break;
				}
				var view = Entries[entryIndex++];
				view.Init(entry);
				view.gameObject.SetActive(true);
			}

			ScrollRect.verticalNormalizedPosition = 1f;

			HaveLeaderboardEntriesRoot.SetActive(haveSavedGames);
			NoLeaderboardEntriesRoot.SetActive(!haveSavedGames);
		}

		void ShowMain() {
			_mainMenuManager.ShowMain();
		}

		void ClearLeaderboard() {
			_leaderboardController.Clear();
			UpdateView();
		}

		void ResetEntries() {
			foreach ( var entry in Entries ) {
				entry.Deinit();
				entry.gameObject.SetActive(false);
			}
		}
	}
}
