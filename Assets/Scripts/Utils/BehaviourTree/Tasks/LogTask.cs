using UnityEngine;

namespace STP.Utils.BehaviourTree.Tasks {
	public class LogTask : BaseTask {
		public override TaskStatus Execute() {
			Debug.Log("Log");
			return TaskStatus.Success;
		}
	}
}