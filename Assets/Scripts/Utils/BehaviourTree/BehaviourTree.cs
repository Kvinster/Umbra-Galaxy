using UnityEngine.Assertions;

using System.Threading;

using Cysharp.Threading.Tasks;

namespace STP.Utils.BehaviourTree {
	public sealed class BehaviourTree {
		public readonly Blackboard Blackboard = new Blackboard();

		readonly BaseTask                _root;
		readonly CancellationTokenSource _cancellationTokenSource;

		public bool IsActive { get; private set; }

		public BehaviourTree(BaseTask root) {
			Assert.IsNull(_root);
			Assert.IsNotNull(root);

			_root                    = root;
			_cancellationTokenSource = new CancellationTokenSource();

			_root.SetBlackboard(Blackboard);
		}

		public async UniTask Execute() {
			Assert.IsNotNull(_root);

			IsActive = true;

			while ( !_cancellationTokenSource.IsCancellationRequested ) {
				Assert.IsTrue(_root.CanExecute);
				await _root.Execute(_cancellationTokenSource.Token);
			}

			IsActive = false;
		}

		public void Stop() {
			_cancellationTokenSource.Cancel();
		}
	}
}
