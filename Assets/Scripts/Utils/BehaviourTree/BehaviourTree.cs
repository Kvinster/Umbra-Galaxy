using STP.Utils.BehaviourTree.Tasks;

namespace STP.Utils.BehaviourTree {
	public sealed class BehaviourTree {
		public readonly Blackboard Blackboard = new Blackboard();

		public readonly BaseTask Root;
		
		public BehaviourTree(BaseTask root) {
			Root = root;
			
			Root.SetBlackboard(Blackboard);
		}

		public void Tick() {
			Root.Execute();
		}
	}
}
