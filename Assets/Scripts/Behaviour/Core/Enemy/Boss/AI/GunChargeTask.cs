using STP.Utils.BehaviourTree;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Enemy.Boss.AI {
	public sealed class GunChargeTask : BaseTask {
		readonly BossMoveAgent     _moveAgent;
		readonly BossGunController _gunController;

		public override bool CanExecute => !Blackboard.GetParameterOrDefault("action_spent", false);

		public GunChargeTask(BossMoveAgent moveAgent, BossGunController gunController) {
			_moveAgent     = moveAgent;
			_gunController = gunController;
		}

		protected override async UniTask Execute() {
			_moveAgent.IsActive = true;
			_gunController.StartCharging();

			while ( !_gunController.IsCharged && !CancellationToken.IsCancellationRequested ) {
				await UniTask.Yield();
			}

			if ( CancellationToken.IsCancellationRequested ) {
				return;
			}

			_moveAgent.IsActive = false;
			_gunController.Shoot();

			Blackboard.SetParameter("action_spent", true);
		}
	}
}
