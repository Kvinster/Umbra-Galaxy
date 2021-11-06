using System;
using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace STP.Behaviour.Core.UI.WinWindow {
	public class LeaderboardEntryView : GameComponent {
		[NotNull] public TMP_Text       PlaceText;
		[NotNull] public TMP_Text       ScoreText;
		[NotNull] public TMP_InputField PlayerNameText;

		public event Action<string> OnEndNameEdition;
		
		public void Reset() {
			PlayerNameText.onEndEdit.RemoveAllListeners();
			SetTextColor(Color.white);
		}
		
		public void ShowEntry(Score score) {
			PlaceText.text      = score.Rank.ToString();
			ScoreText.text      = score.ScoreValue.ToString();
			PlayerNameText.text = score.UserName;
			gameObject.SetActive(true);
		}
		
		public void SetAsCurrentPlayerView() {
			SetTextColor(Color.yellow);
			PlayerNameText.readOnly = false;
			PlayerNameText.Select();
			PlayerNameText.onEndEdit.AddListener(OnEndTextEdition);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
		
		void SetTextColor(Color color) {
			PlaceText.color                    = color;
			ScoreText.color                    = color;
			PlayerNameText.textComponent.color = color;
		}

		void OnEndTextEdition(string value) {
			if ( !Input.GetKey(KeyCode.Return) ) {
				PlayerNameText.Select();
				return;
			}
			PlayerNameText.readOnly = true;
			OnEndNameEdition?.Invoke(PlayerNameText.text);
		}
	}
}