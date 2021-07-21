using UnityEngine.Assertions;

namespace STP.Utils.BehaviourTree.Tasks {
	public enum TaskStatus {
		Success,
		Failure,
		Continue,
	}
	
	public abstract class BaseTask {
		protected Blackboard Blackboard;

		public virtual void SetBlackboard(Blackboard blackboard) {
			Assert.IsNotNull(blackboard);
			Blackboard = blackboard;
		}

		public abstract TaskStatus Execute();
	}
}
