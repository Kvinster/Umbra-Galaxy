using System;

namespace STP.Utils.BehaviourTree.Tasks {
	public class CustomActionTask : BaseTask {
		readonly Action _task;

		public CustomActionTask(Action task) : this(nameof(CustomActionTask), task) { }

		public CustomActionTask(string name, Action task) : base(name) {
			_task = task;
		}

		protected override TaskStatus ExecuteInternal() {
			_task();
			return TaskStatus.Success;
		}
	}
}