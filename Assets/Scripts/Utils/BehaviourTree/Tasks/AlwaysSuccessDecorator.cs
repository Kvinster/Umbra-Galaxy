namespace STP.Utils.BehaviourTree.Tasks {
	public class AlwaysSuccessDecorator : BaseTask {
		public AlwaysSuccessDecorator(BaseTask task) : base(nameof(AlwaysSuccessDecorator)) {
			SubTasks.Add(task);
		}
		
		protected override TaskStatus ExecuteInternal() {
			var res = SubTasks[0].Execute();
			if (res == TaskStatus.Continue) {
				return TaskStatus.Continue;
			}
			return TaskStatus.Success;
		}
	}
}