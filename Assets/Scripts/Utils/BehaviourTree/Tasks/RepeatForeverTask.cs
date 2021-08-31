namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class RepeatForeverTask : BaseTask {
		
		public RepeatForeverTask(BaseTask task) : base(nameof(RepeatForeverTask)) {
			SubTasks.Add(task);
		}

		protected override TaskStatus ExecuteInternal() {
			var res = SubTasks[0].Execute();
			if ( res != TaskStatus.Continue ) {
				SubTasks[0].ResetStatus();
			}
			return TaskStatus.Continue;
		}
	}
}