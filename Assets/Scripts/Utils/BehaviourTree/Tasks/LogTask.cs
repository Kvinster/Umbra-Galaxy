using UnityEngine;

namespace STP.Utils.BehaviourTree.Tasks {
	public class LogTask : BaseTask {
		protected override TaskStatus ExecuteInternal() {
			Debug.Log("Log");
			return TaskStatus.Success;
		}
	}
}