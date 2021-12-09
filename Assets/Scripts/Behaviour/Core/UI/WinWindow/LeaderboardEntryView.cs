using System;
using System.Collections.Generic;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.Core.UI.WinWindow {
	public class LeaderboardEntryView : GameComponent {
		[NotNullOrEmpty] public List<Sprite> FirstPlacesImages;

		// Place
		[NotNull] public TMP_Text       PlayerPlaceText;
		[NotNull] public Image          PlayerPlaceImage;
		
		[NotNull] public TMP_Text       ScoreText;
		
		[NotNull] public TMP_InputField PlayerNameText;
		[NotNull] public TMP_Text       PlaceholderText;
		
		public event Action<string> OnEndNameEdition;
		
		public void Reset() {
			PlayerNameText.readOnly = true;
			PlayerNameText.onEndEdit.RemoveAllListeners();
			SetTextColor(Color.white);
		}
		
		public void ShowEntry(Score score) {
			ScoreText.text       = score.ScoreValue.ToString();
			PlayerNameText.text  = score.UserName;
			PlaceholderText.text = score.UserName;
			SetPlace(score.Rank);
			gameObject.SetActive(true);
		}
		
		public void SetAsCurrentPlayerView() {
			SetTextColor(Color.yellow);
			PlayerNameText.text     = string.Empty;
			PlayerNameText.readOnly = false;
			PlayerNameText.Select();
			PlayerNameText.onEndEdit.AddListener(OnEndTextEdition);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
		
		void SetTextColor(Color color) {
			PlayerPlaceText.color              = color;
			ScoreText.color                    = color;
			PlayerNameText.textComponent.color = color;
		}

		void OnEndTextEdition(string value) {
			if ( !Input.GetKey(KeyCode.Return) ) {
				PlayerNameText.Select();
				return;
			}
			PlayerNameText.readOnly = true;
			PlayerNameText.text     = string.IsNullOrEmpty(PlayerNameText.text) ? PlaceholderText.text : PlayerNameText.text;
			OnEndNameEdition?.Invoke(PlayerNameText.text);
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