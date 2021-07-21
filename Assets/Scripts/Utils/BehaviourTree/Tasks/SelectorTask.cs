using UnityEngine.Assertions;

namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class SelectorTask : BaseTask {
		readonly BaseTask[] _tasks;

		public SelectorTask(params BaseTask[] tasks) {
			Assert.IsTrue(tasks.Length > 0);
			_tasks = tasks;
		}

		public override void SetBlackboard(Blackboard blackboard) {
			base.SetBlackboard(blackboard);
			foreach ( var task in _tasks ) {
				task.SetBlackboard(blackboard);
			}
		}

		public override TaskStatus Execute() {
			foreach ( var task in _tasks ) {
				var res = task.Execute();
				if ( (res == TaskStatus.Continue) || (res == TaskStatus.Success) ) {
					return res;
				}
			}
			return TaskStatus.Failure;
		}
	}
}
