using UnityEngine.Assertions;

using System.Threading;

using Cysharp.Threading.Tasks;

namespace STP.Utils.BehaviourTree {
	public abstract class BaseTask {
		public virtual bool CanExecute => true;

		public bool IsRunning => (CancellationToken != default);

		protected Blackboard        Blackboard;
		protected CancellationToken CancellationToken;

		public virtual void SetBlackboard(Blackboard blackboard) {
			Assert.IsNotNull(blackboard);
			Blackboard = blackboard;
		}

		public async UniTask Execute(CancellationToken cancellationToken) {
			CancellationToken = cancellationToken;
			await Execute();
			FinishExecution();
		}

		protected abstract UniTask Execute();

		void FinishExecution() {
			CancellationToken = default;
		}
	}
}
