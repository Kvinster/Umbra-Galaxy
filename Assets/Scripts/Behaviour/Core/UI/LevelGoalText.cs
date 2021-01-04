using UnityEngine;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class LevelGoalText : BaseCoreComponent {
		const string TextFormat = "Goal: {0}/{1}";

		static readonly Color UnfinishedColor = Color.white;
		static readonly Color FinishedColor   = Color.green;

		[NotNull] public TMP_Text Text;

		LevelGoalManager _levelGoalManager;

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;

			_levelGoalManager.OnCurLevelGoalProgressChanged += OnCurLevelGoalProgressChanged;
			OnCurLevelGoalProgressChanged(_levelGoalManager.CurLevelGoalProgress);
		}

		void OnCurLevelGoalProgressChanged(int curProgress) {
			Text.text  = string.Format(TextFormat, curProgress, _levelGoalManager.LevelGoal);
			Text.color = (curProgress >= _levelGoalManager.LevelGoal) ? FinishedColor : UnfinishedColor;
		}
	}
}
