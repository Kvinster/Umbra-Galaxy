using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class LeaderboardWindow : GameComponent, IScreen {
		[NotNull]
		public GameObject HaveLeaderboardEntriesRoot;
		[NotNull]
		public GameObject NoLeaderboardEntriesRoot;
		[NotNull]
		public ScrollRect ScrollRect;
		[NotNull]
		public Button BackButton;
		[NotNullOrEmpty]
		public List<LeaderboardEntryView> Entries = new List<LeaderboardEntryView>();

		IScreenShower         _screenShower;
		LeaderboardController _leaderboardController;

		void Reset() {
			ScrollRect = GetComponentInChildren<ScrollRect>();
			GetComponentsInChildren(Entries);
		}

		public void Init(MainMenuStarter starter) {
			_screenShower          = starter.ScreensViewController;
			_leaderboardController = starter.GameController.LeaderboardController;

			BackButton.onClick.AddListener(() => _screenShower.Show<MainScreen>());
		}

		public void Show() {
			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			ResetEntries();
			gameObject.SetActive(false);
		}

		async void UpdateView() {
			ResetEntries();

			var entries    = await _leaderboardController.GetTopScores(Entries.Count);
			var entryIndex = 0;
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

			var haveScores = (entries.Count > 0);
			HaveLeaderboardEntriesRoot.SetActive(haveScores);
			NoLeaderboardEntriesRoot.SetActive(!haveScores);
		}

		void ResetEntries() {
			foreach ( var entry in Entries ) {
				entry.Deinit();
				entry.gameObject.SetActive(false);
			}
		}
	}
}
