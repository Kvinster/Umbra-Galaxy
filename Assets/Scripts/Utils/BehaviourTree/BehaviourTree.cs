using STP.Utils.BehaviourTree.Tasks;

namespace STP.Utils.BehaviourTree {
	public sealed class BehaviourTree {
		public readonly Blackboard Blackboard = new Blackboard();

		readonly BaseTask _root;

		public BehaviourTree(BaseTask root) {
			_root = root;
			
			_root.SetBlackboard(Blackboard);
		}

		public void Tick() {
			_root.Execute();
		}
	}
}
