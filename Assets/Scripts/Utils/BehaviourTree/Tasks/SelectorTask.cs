using UnityEngine.Assertions;

namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class SelectorTask : BaseTask {

		public SelectorTask(params BaseTask[] tasks) {
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
				if ( (res == TaskStatus.Continue) || (res == TaskStatus.Success) ) {
					return res;
				}
			}
			return TaskStatus.Failure;
		}
	}
}
