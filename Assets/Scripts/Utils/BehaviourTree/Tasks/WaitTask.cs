namespace STP.Utils.BehaviourTree.Tasks {
	public class WaitTask : BaseTask {
		Timer _timer = new Timer();
		
		public WaitTask(float time) {
			_timer.Reset(time);			
		}
		
		protected override TaskStatus ExecuteInternal() {
			if ( _timer.DeltaTick() ) {
				return TaskStatus.Success;
			}
			return TaskStatus.Continue;
		}
	}
}