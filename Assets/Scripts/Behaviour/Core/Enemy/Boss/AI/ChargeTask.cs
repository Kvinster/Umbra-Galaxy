using System;

using STP.Utils.BehaviourTree;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Enemy.Boss.AI {
	public sealed class ChargeTask : BaseTask {
		readonly float _chargeTime;

		public ChargeTask(float chargeTime) {
			_chargeTime = chargeTime;
		}

		protected override async UniTask Execute() {
			await UniTask.Delay(TimeSpan.FromSeconds(_chargeTime), false, PlayerLoopTiming.Update, CancellationToken);
		}
	}
}
