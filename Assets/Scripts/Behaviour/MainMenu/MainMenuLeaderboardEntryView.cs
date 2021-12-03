using System.Collections.Generic;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.MainMenu {
	public sealed class MainMenuLeaderboardEntryView : GameComponent {
		const string DescFormat = "{0}: {1} XP";

		[NotNullOrEmpty] public List<Sprite> FirstPlacesImages;

		// Place
		[NotNull] public TMP_Text PlayerPlaceText;
		[NotNull] public Image    PlayerPlaceImage;
		
		[NotNull] public TMP_Text PlayerName;
		[NotNull] public TMP_Text PlayerScore;

		public void Init(Score leaderboardEntry) {
			SetPlace(leaderboardEntry.Rank);
			PlayerName.text  = leaderboardEntry.UserName;
			PlayerScore.text = leaderboardEntry.ScoreValue.ToString();
		}
		
		void SetPlace(int leaderboardEntryRank) {
			if ( leaderboardEntryRank <= FirstPlacesImages.Count ) {
				PlayerPlaceImage.gameObject.SetActive(true);
				PlayerPlaceText.gameObject.SetActive(false);
				PlayerPlaceImage.sprite = FirstPlacesImages[leaderboardEntryRank-1];
			} else {
				PlayerPlaceImage.gameObject.SetActive(false);
				PlayerPlaceText.gameObject.SetActive(true);
				PlayerPlaceText.text = leaderboardEntryRank.ToString();
			}
		}
	}
}
