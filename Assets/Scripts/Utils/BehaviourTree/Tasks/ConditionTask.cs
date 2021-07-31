using System;

namespace STP.Utils.BehaviourTree.Tasks {
	public class ConditionTask : BaseTask {
		readonly Func<bool> _conditionChecker;
		
		public ConditionTask(Func<bool> conditionChecker) {
			_conditionChecker = conditionChecker;
		}

		protected override TaskStatus ExecuteInternal() {
			return _conditionChecker() ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}