namespace STP.Utils.BehaviourTree.Tasks {
	public sealed class ParallelTask : BaseTask {
		public ParallelTask(params BaseTask[] task) : base(nameof(ParallelTask)) {
			SubTasks.AddRange(task);
		}
		
		protected override TaskStatus ExecuteInternal() {
			var hasRunningTasks = false;
			foreach ( var task in SubTasks ) {
				var res = task.Execute();
				if ( res == TaskStatus.Failure ) {
					return TaskStatus.Failure;
				}
				if ( res == TaskStatus.Continue ) {
					hasRunningTasks = true;
				}
			}
			return (hasRunningTasks) ? TaskStatus.Continue : TaskStatus.Success;
		}
	}
}