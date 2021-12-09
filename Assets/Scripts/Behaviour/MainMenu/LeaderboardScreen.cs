using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using STP.Behaviour.Core.UI.WinWindow;
using STP.Behaviour.Starter;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class LeaderboardScreen : GameComponent, IScreen {
		[Header("Leaderboard stats")]
		[NotNull] public GameObject HaveLeaderboardEntriesRoot;
		[NotNull] public GameObject NoLeaderboardEntriesRoot;
		[NotNull] public Button     BackButton;

		[Header("Loading leaderboard")]
		[NotNull] public RotationEffect LoadingEffect;

		[NotNullOrEmpty] public List<LeaderboardEntryView> Entries = new List<LeaderboardEntryView>();

		IScreenShower         _screenShower;
		LeaderboardController _leaderboardController;

		void Reset() {
			GetComponentsInChildren(Entries);
		}

		public void Init(MainMenuStarter starter) {
			_screenShower          = starter.ScreensViewController;
			_leaderboardController = starter.GameController.LeaderboardController;

			BackButton.onClick.AddListener(() => _screenShower.Show<MainScreen>());
		}

		public void Show() {
			gameObject.SetActive(true);
			LoadingEffect.ShowEffect();
			UpdateView().Forget();
		}

		public void Hide() {
			ResetEntries();
			gameObject.SetActive(false);
		}

		async UniTask UpdateView() {
			ResetEntries();

			var entries    = await _leaderboardController.GetTopScores(Entries.Count);
			var entryIndex = 0;
			foreach ( var entry in entries ) {
				if ( entryIndex >= Entries.Count ) {
					Debug.LogError("Not enough LeaderboardEntryViews");
					break;
				}
				var view = Entries[entryIndex++];
				view.ShowEntry(entry);
				view.gameObject.SetActive(true);
			}


			var haveScores = (entries.Count > 0);
			HaveLeaderboardEntriesRoot.SetActive(haveScores);
			NoLeaderboardEntriesRoot.SetActive(!haveScores);

			// End loading effect
			LoadingEffect.HideEffect();
		}

		void ResetEntries() {
			foreach ( var entry in Entries ) {
				entry.gameObject.SetActive(false);
			}
		}
	}
}
