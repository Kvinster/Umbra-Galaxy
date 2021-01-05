using System;
using System.Text;

using STP.Behaviour.Starter;
using STP.Controller;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class LevelText : BaseCoreComponent {
		const string LivesTextFormat          = "Lives: {0}";
		const string UnfinishedGoalTextFormat = "<color=white>Goal: {0}/{1}</color>";
		const string FinishedGoalTextFormat   = "<color=green>Goal: {0}/{1}</color>";
		const string CurXpTextFormat          = "<color=white>Xp: {0}</color>";

		[NotNull] public TMP_Text Text;

		LevelGoalManager _levelGoalManager;
		PlayerController _playerController;
		XpController     _xpController;

		readonly StringBuilder _stringBuilder = new StringBuilder();

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;
			_playerController = PlayerController.Instance;
			_xpController     = XpController.Instance;

			_levelGoalManager.OnCurLevelGoalProgressChanged += OnCurLevelGoalProgressChanged;
			_playerController.OnCurLivesChanged             += OnCurPlayerLivesChanged;
			_xpController.OnXpChanged                       += OnXpAmountChanged;

			UpdateText(_playerController.CurLives, _levelGoalManager.CurLevelGoalProgress, _xpController.CurXp);
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

		void OnXpAmountChanged(int xpAmount) {
			UpdateText(_playerController.CurLives, _levelGoalManager.LevelGoal, xpAmount);
		}		

		void OnCurLevelGoalProgressChanged(int curProgress) {
			UpdateText(_playerController.CurLives, curProgress, _xpController.CurXp);
		}

		void OnCurPlayerLivesChanged(int curLives) {
			UpdateText(curLives, _levelGoalManager.CurLevelGoalProgress, _xpController.CurXp);
		}

		void UpdateText(int curLives, int curProgress, int curXp) {
			_stringBuilder.Clear()
				.AppendLine(string.Format(LivesTextFormat, curLives))
				.AppendLine(string.Format(
					(curProgress >= _levelGoalManager.LevelGoal) ? FinishedGoalTextFormat : UnfinishedGoalTextFormat,
					curProgress, _levelGoalManager.LevelGoal))
				.AppendLine(string.Format(CurXpTextFormat, curXp));
			Text.text = _stringBuilder.ToString();
		}
	}
}
