using System.Text;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class LevelText : GameComponent {
		const string LivesTextFormat          = "Lives: {0}";
		const string UnfinishedGoalTextFormat = "<color=white>Goal: {0}/{1}</color>";
		const string FinishedGoalTextFormat   = "<color=green>Goal: {0}/{1}</color>";
		const string CurScoreTextFormat       = "<color=white>Score: {0}</color>";
		const string PowerUpInfoFormat        = "<color=white>name: {0} left: {1:00}</color>";

		[NotNull] public TMP_Text Text;

		LevelGoalManager _levelGoalManager;
		PlayerManager    _playerManager;
		PlayerController _playerController;
		ScoreController     _scoreController;

		readonly StringBuilder _stringBuilder = new StringBuilder();

		bool IsInit => (_levelGoalManager != null);

		public void Init(LevelGoalManager levelGoalManager, PlayerManager playerManager,
			PlayerController playerController, ScoreController scoreController) {
			_levelGoalManager = levelGoalManager;
			_playerManager    = playerManager;
			_playerController = playerController;
			_scoreController  = scoreController;

			_levelGoalManager.OnCurLevelGoalProgressChanged += OnCurLevelGoalProgressChanged;
			_playerController.OnCurLivesChanged             += OnCurPlayerLivesChanged;
			_scoreController.Score.OnValueChanged                 += ScoreAmountChanged;

			UpdateText();
		}

		void OnDestroy() {
			if ( _levelGoalManager != null) {
				_levelGoalManager.OnCurLevelGoalProgressChanged -= OnCurLevelGoalProgressChanged;
			}
			if ( _playerController != null) {
				_playerController.OnCurLivesChanged -= OnCurPlayerLivesChanged;
			}
			if ( _scoreController != null) {
				_scoreController.Score.OnValueChanged -= ScoreAmountChanged;
			}
		}

		void Update() {
			UpdateText();
		}

		void ScoreAmountChanged(int xpAmount) {
			UpdateText();
		}

		void OnCurLevelGoalProgressChanged(int curProgress) {
			UpdateText();
		}

		void OnCurPlayerLivesChanged(int curLives) {
			UpdateText();
		}

		void UpdateText() {
			if ( !IsInit ) {
				return;
			}
			var curProgress = _levelGoalManager.CurLevelGoalProgress;
			_stringBuilder.Clear()
				.AppendLine(string.Format(LivesTextFormat, _playerController.CurLives))
				.AppendLine(string.Format(
					(curProgress >= _levelGoalManager.LevelGoal) ? FinishedGoalTextFormat : UnfinishedGoalTextFormat,
					curProgress, _levelGoalManager.LevelGoal))
				.AppendLine(string.Format(CurScoreTextFormat, _scoreController.Score.Value));
			var allPowerUps = _playerManager.GetAllActivePowerUpStates();
			if ( allPowerUps.Count > 0 ) {
				_stringBuilder.AppendLine("PowerUps info:");
			}
			foreach ( var powerUp in _playerManager.GetAllActivePowerUpStates() ) {
				_stringBuilder.AppendLine(string.Format(PowerUpInfoFormat, powerUp.Type, powerUp.TimeLeft));
			};
			Text.text = _stringBuilder.ToString();
		}
	}
}
