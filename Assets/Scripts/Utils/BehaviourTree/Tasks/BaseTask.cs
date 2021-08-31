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
		public string     TaskName;
		
		protected Blackboard Blackboard;

		protected BaseTask(string name) {
			TaskName = name;
		}
		
		public virtual void SetBlackboard(Blackboard blackboard) {
			Assert.IsNotNull(blackboard);
			Blackboard = blackboard;
		}

		public virtual void ResetStatus() {
			LastStatus = TaskStatus.Unknown;
			foreach ( var subTask in SubTasks ) {
				subTask.ResetStatus();
			}
		}

		public TaskStatus Execute() {
			if ( LastStatus == TaskStatus.Success || LastStatus == TaskStatus.Failure ) {
				return LastStatus;
			}
			LastStatus = ExecuteInternal();
			return LastStatus;
		}
		
		protected abstract TaskStatus ExecuteInternal();
	}
}
