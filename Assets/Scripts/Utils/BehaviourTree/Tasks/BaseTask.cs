using UnityEngine.Assertions;

using System.Collections.Generic;

namespace STP.Utils.BehaviourTree.Tasks {
	public enum TaskStatus {
		Success,
		Failure,
		Continue,
	}
	
	public abstract class BaseTask {
		public List<BaseTask> SubTasks = new List<BaseTask>();
	
		protected Blackboard Blackboard;

		public virtual void SetBlackboard(Blackboard blackboard) {
			Assert.IsNotNull(blackboard);
			Blackboard = blackboard;
		}

		public abstract TaskStatus Execute();
	}
}
