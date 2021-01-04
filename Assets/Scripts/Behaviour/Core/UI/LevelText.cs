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

		[NotNull] public TMP_Text Text;

		LevelGoalManager _levelGoalManager;
		PlayerController _playerController;

		readonly StringBuilder _stringBuilder = new StringBuilder();

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;
			_playerController = PlayerController.Instance;

			_levelGoalManager.OnCurLevelGoalProgressChanged += OnCurLevelGoalProgressChanged;
			_playerController.OnCurLivesChanged             += OnCurPlayerLivesChanged;

			UpdateText(_playerController.CurLives, _levelGoalManager.CurLevelGoalProgress);
		}

		void OnCurLevelGoalProgressChanged(int curProgress) {
			UpdateText(_playerController.CurLives, curProgress);
		}

		void OnCurPlayerLivesChanged(int curLives) {
			UpdateText(curLives, _levelGoalManager.CurLevelGoalProgress);
		}

		void UpdateText(int curLives, int curProgress) {
			_stringBuilder.Clear()
				.AppendLine(string.Format(LivesTextFormat, curLives))
				.AppendLine(string.Format(
					(curProgress >= _levelGoalManager.LevelGoal) ? FinishedGoalTextFormat : UnfinishedGoalTextFormat,
					curProgress, _levelGoalManager.LevelGoal));
			Text.text = _stringBuilder.ToString();
		}
	}
}
