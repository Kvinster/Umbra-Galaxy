using UnityEngine;

namespace STP.Utils.BehaviourTree.Tasks {
	public class LogTask : BaseTask {
		string _message;

		public LogTask(string message) : base(nameof(LogTask)) {
			_message = message;
		}
		
		protected override TaskStatus ExecuteInternal() {
			Debug.Log(_message);
			return TaskStatus.Success;
		}
	}
}