using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using STP.Core;
using STP.Core.Leaderboards;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using IPromise = RSG.IPromise;

namespace STP.Behaviour.Core.UI.WinWindow {
	public sealed class WinWindow : BaseCoreWindow {
		const int    MaxRecordsCount         = 10;
		const string LeaderboardRecordFormat = "{0} - {1} : {2}";
		
		[NotNull] public TMP_Text   StatsText;
		[NotNull] public Button     ContinueButton;

		[NotNull] public TMP_InputField InputField;

		LeaderboardController _leaderboardController;
		LevelManager          _levelManager;
		XpController          _xpController;

		List<Score> _scores;

		bool _isUsernamePassed;
		
		public void CommonInit(XpController xpController, LeaderboardController leaderboardController, LevelManager levelManager) {
			_xpController          = xpController;
			_leaderboardController = leaderboardController;
			_levelManager          = levelManager;
			ContinueButton.onClick.AddListener(OnContinueClick);
			InputField.onValueChanged.AddListener(GenerateText);
			InputField.onEndEdit.AddListener(FinishUserNameInput);
		}

		public override IPromise Show() {
			var promise = base.Show();
			InputField.Select();
			UniTask.Void(UpdateLeaderboard);
			return promise;
		}

		void FinishUserNameInput(string lastResult) {
			// submit name only on enter
			if ( !Input.GetKey(KeyCode.Return) ) {
				return;
			}
			_isUsernamePassed = true;
			UniTask.Create(() => _leaderboardController.UpdateUserName(lastResult));
			GenerateText(lastResult);
		}

		async UniTaskVoid UpdateLeaderboard() {
			await _leaderboardController.PublishScoreAsync(_xpController.Xp);
			await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);
			_scores = await _leaderboardController.GetScoresAroundPlayerAsync(MaxRecordsCount);
			GenerateText(InputField.text);
		}
		
		void GenerateText(string playerName) {
			if ( _scores == null ) {
				return;
			}
			var sb = new StringBuilder();
			foreach ( var score in _scores ) {
				var userName = score.UserName;
				if ( score.Id == _leaderboardController.PlayerId ) {
					playerName = (_isUsernamePassed) ? playerName : (playerName + "|");
					userName   = $"<color=#FFFF00>{playerName}</color>";
				}
				sb.AppendFormat(LeaderboardRecordFormat, score.Rank, userName, score.ScoreValue)
					.AppendLine();
			}
			StatsText.text = sb.ToString();
		}

		void OnContinueClick() {
			Hide();
			_isUsernamePassed = false;
			_levelManager.QuitToMenu();
			_leaderboardController.ResetUserId();
		}
	}
}
