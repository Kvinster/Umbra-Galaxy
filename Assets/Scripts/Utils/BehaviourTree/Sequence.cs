using UnityEngine.Assertions;

using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

namespace STP.Utils.BehaviourTree {
	public class Sequence : BaseTask {
		readonly List<BaseTask> _tasks;

		public sealed override bool CanExecute => AllTasksCanExecute && ExecuteCondition;

		protected virtual bool ExecuteCondition => true;

		bool AllTasksCanExecute {
			get {
				foreach ( var task in _tasks ) {
					if ( !task.CanExecute ) {
						return false;
					}
				}
				return true;
			}
		}

		public Sequence(params BaseTask[] tasks) {
			Assert.IsNotNull(tasks);
			Assert.IsTrue(tasks.Length > 0);
			_tasks = tasks.ToList();
			_tasks.TrimExcess();
		}

		public override void SetBlackboard(Blackboard blackboard) {
			base.SetBlackboard(blackboard);
			foreach ( var task in _tasks ) {
				task.SetBlackboard(blackboard);
			}
		}

		protected sealed override async UniTask Execute() {
			Assert.IsTrue(CanExecute);
			OnStartedExecution();
			foreach ( var task in _tasks ) {
				await task.Execute(CancellationToken);
				if ( CancellationToken.IsCancellationRequested ) {
					OnFinishedExecution(true);
					return;
				}
			}
			OnFinishedExecution(CancellationToken.IsCancellationRequested);
		}

		protected virtual void OnStartedExecution() { }

		protected virtual void OnFinishedExecution(bool interrupted) { }
	}
}
