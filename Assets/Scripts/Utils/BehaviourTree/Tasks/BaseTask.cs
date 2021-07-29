using UnityEngine.Assertions;

using System.Collections.Generic;

namespace STP.Utils.BehaviourTree.Tasks {
	public enum TaskStatus {
		Success,
		Failure,
		Continue,
		Unknown
	}
	
	public abstract class BaseTask {
		public List<BaseTask> SubTasks = new List<BaseTask>();

		public TaskStatus LastStatus = TaskStatus.Unknown;
		
		protected Blackboard Blackboard;

		public virtual void SetBlackboard(Blackboard blackboard) {
			Assert.IsNotNull(blackboard);
			Blackboard = blackboard;
		}

		public TaskStatus Execute() {
			var res = ExecuteInternal();
			LastStatus = res;
			return res;
		}
		
		protected abstract TaskStatus ExecuteInternal();
	}
}
