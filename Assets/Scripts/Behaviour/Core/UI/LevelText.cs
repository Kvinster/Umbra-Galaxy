using System;
using System.Text;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class LevelText : BaseCoreComponent {
		const string LivesTextFormat          = "Lives: {0}";
		const string UnfinishedGoalTextFormat = "<color=white>Goal: {0}/{1}</color>";
		const string FinishedGoalTextFormat   = "<color=green>Goal: {0}/{1}</color>";
		const string CurXpTextFormat          = "<color=white>Xp: {0}</color>";
		const string PowerUpInfoFormat        = "<color=white>name: {0} left: {1:00}</color>";

		[NotNull] public TMP_Text Text;

		LevelGoalManager _levelGoalManager;
		PlayerManager    _playerManager;
		PlayerController _playerController;
		XpController     _xpController;

		readonly StringBuilder _stringBuilder = new StringBuilder();

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;
			_playerManager    = starter.PlayerManager;
			_playerController = starter.PlayerController;
			_xpController     = starter.XpController;

			_levelGoalManager.OnCurLevelGoalProgressChanged += OnCurLevelGoalProgressChanged;
			_playerController.OnCurLivesChanged             += OnCurPlayerLivesChanged;
			_xpController.OnXpChanged                       += OnXpAmountChanged;

			UpdateText();
		}

		void OnDestroy() {
			if ( _levelGoalManager != null) {
				_levelGoalManager.OnCurLevelGoalProgressChanged -= OnCurLevelGoalProgressChanged;
			}
			if ( _playerController != null) {
				_playerController.OnCurLivesChanged -= OnCurPlayerLivesChanged;
			}
			if ( _xpController != null) {
				_xpController.OnXpChanged -= OnXpAmountChanged;
			}
		}

		void Update() {
			UpdateText();
		}

		void OnXpAmountChanged(int xpAmount) {
			UpdateText();
		}

		void OnCurLevelGoalProgressChanged(int curProgress) {
			UpdateText();
		}

		void OnCurPlayerLivesChanged(int curLives) {
			UpdateText();
		}

		void UpdateText() {
			var curProgress = _levelGoalManager.CurLevelGoalProgress;
			_stringBuilder.Clear()
				.AppendLine(string.Format(LivesTextFormat, _playerController.CurLives))
				.AppendLine(string.Format(
					(curProgress >= _levelGoalManager.LevelGoal) ? FinishedGoalTextFormat : UnfinishedGoalTextFormat,
					curProgress, _levelGoalManager.LevelGoal))
				.AppendLine(string.Format(CurXpTextFormat, _xpController.CurXp));
			var allPowerUps = _playerManager.GetAllActivePowerUpStates();
			if ( allPowerUps.Count > 0 ) {
				_stringBuilder.AppendLine("PowerUps info:");
			}
			foreach ( var powerUp in _playerManager.GetAllActivePowerUpStates() ) {
				_stringBuilder.AppendLine(string.Format(PowerUpInfoFormat, powerUp.Name, powerUp.TimeLeft));
			};
			Text.text = _stringBuilder.ToString();
		}
	}
}
