using System;

using STP.Utils.BehaviourTree;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Enemy.Boss.AI {
	public sealed class IdleTask : BaseTask {
		readonly float _idleDuration;

		public override bool CanExecute => Blackboard.GetParameterOrDefault("action_spent", false);

		public IdleTask(float idleDuration) {
			_idleDuration = idleDuration;
		}

		protected override async UniTask Execute() {
			await UniTask.Delay(TimeSpan.FromSeconds(_idleDuration), false, PlayerLoopTiming.Update, CancellationToken);
			Blackboard.SetParameter("action_spent", false);
		}
	}
}
