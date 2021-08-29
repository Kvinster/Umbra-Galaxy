using System;

namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class RepeatUntilSuccess : BaseTask {
		public RepeatUntilSuccess(BaseTask task) : base(nameof(RepeatUntilSuccess)) {
			SubTasks.Add(task);
		}
		
		protected override TaskStatus ExecuteInternal() {
			var res = SubTasks[0].Execute();
			if (res == TaskStatus.Continue) {
				return TaskStatus.Continue;
			}
			if (res == TaskStatus.Success) {
				return TaskStatus.Success;
			}
			if (res == TaskStatus.Failure) {
				SubTasks[0].ResetStatus();
			}
			return TaskStatus.Continue;
		}
	}
}