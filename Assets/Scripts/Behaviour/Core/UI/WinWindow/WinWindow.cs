using System.Text;
using Cysharp.Threading.Tasks;
using STP.Core;
using STP.Core.Leaderboards;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using TMPro;
using UnityEngine.UI;
using IPromise = RSG.IPromise;

namespace STP.Behaviour.Core.UI.WinWindow {
	public sealed class WinWindow : BaseCoreWindow {
		const int    MaxRecordsCount         = 10;
		const string LeaderboardRecordFormat = "{0} : {1}";
		
		[NotNull] public TMP_Text   StatsText;
		[NotNull] public Button     ContinueButton;

		LeaderboardController _leaderboardController;
		LevelManager          _levelManager;
		XpController          _xpController;

		public void CommonInit(XpController xpController, LeaderboardController leaderboardController, LevelManager levelManager) {
			_xpController          = xpController;
			_leaderboardController = leaderboardController;
			_levelManager          = levelManager;
			ContinueButton.onClick.AddListener(OnContinueClick);
		}

		public override IPromise Show() {
			UniTask.Void(ShowInternal);
			return base.Show();
		}

		async UniTaskVoid ShowInternal() {
			await _leaderboardController.PublishScoreAsync(_xpController.Xp);
			await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);
			await GenerateText();
		}

		async UniTask GenerateText() {
			var sb     = new StringBuilder();
			var scores = await _leaderboardController.GetScoresAroundPlayerAsync(MaxRecordsCount);
			foreach ( var score in scores ) {
				sb.AppendFormat(LeaderboardRecordFormat, score.UserName, score.ScoreValue)
					.AppendLine();
			}
			StatsText.text = sb.ToString();
		}

		void OnContinueClick() {
			Hide();
			_levelManager.QuitToMenu();
		}
	}
}
