using System;

namespace STP.Utils.BehaviourTree.Tasks {
	public class CustomActionTask : BaseTask {
		readonly Func<TaskStatus> _task;
		
		public CustomActionTask(Func<TaskStatus> task) {
			_task = task;
		}

		public CustomActionTask(Action task) {
			_task = () => {
				task();
				return TaskStatus.Success;
			};
		}

		protected override TaskStatus ExecuteInternal() {
			return _task();
		}
	}
}