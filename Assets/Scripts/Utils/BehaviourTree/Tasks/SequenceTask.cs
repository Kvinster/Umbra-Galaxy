using UnityEngine.Assertions;

namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class SequenceTask : BaseTask {
		public SequenceTask(params BaseTask[] tasks) {
			Assert.IsTrue(tasks.Length > 0);
			SubTasks.AddRange(tasks);
		}

		public override void SetBlackboard(Blackboard blackboard) {
			base.SetBlackboard(blackboard);
			foreach ( var task in SubTasks ) {
				task.SetBlackboard(blackboard);
			}
		}

		protected override TaskStatus ExecuteInternal() {
			foreach ( var task in SubTasks ) {
				var res = task.Execute();
				if ( (res == TaskStatus.Failure) || (res == TaskStatus.Continue) ) {
					return res;
				}
			}
			return TaskStatus.Success;
		}
	}
}