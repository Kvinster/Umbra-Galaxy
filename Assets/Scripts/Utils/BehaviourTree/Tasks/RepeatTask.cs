namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class RepeatTask: BaseTask {
		int _rounds;
		int _counter;
		
		public RepeatTask(int rounds, BaseTask task) : base($"{nameof(RepeatTask)} {rounds} rounds ") {
			_rounds = rounds;
			SubTasks.Add(task);
		}

		public override void ResetStatus() {
			base.ResetStatus();
			_counter = 0;
		}

		protected override TaskStatus ExecuteInternal() {
			if ( _counter == _rounds ) {
				return TaskStatus.Success;
			}
			var res = SubTasks[0].Execute();
			if (res == TaskStatus.Continue) {
				return TaskStatus.Continue;
			}
			if (res == TaskStatus.Success) {
				SubTasks[0].ResetStatus();
				_counter++;
			}
			if ( res == TaskStatus.Failure ) {
				return TaskStatus.Failure;
			}
			return (_counter == _rounds) ? TaskStatus.Success : TaskStatus.Continue;
		}
	}
}