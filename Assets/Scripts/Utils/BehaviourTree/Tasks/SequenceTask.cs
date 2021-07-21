using UnityEngine.Assertions;

namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class SequenceTask : BaseTask {
		readonly BaseTask[] _tasks;

		public SequenceTask(params BaseTask[] tasks) {
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
				if ( (res == TaskStatus.Failure) || (res == TaskStatus.Continue) ) {
					return res;
				}
			}
			return TaskStatus.Success;
		}
	}
}