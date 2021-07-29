using STP.Utils.BehaviourTree.Tasks;

using XNode;

namespace STP.Config {
	public class BehaviourTreeNode : Node {
		[Input(connectionType = ConnectionType.Override)]  public BehaviourTreeNode Parent;
		[Output(connectionType = ConnectionType.Multiple)] public BehaviourTreeNode ThisNode;

		public BaseTask Task;

		public TaskStatus Status;

		public void UpdateValues() {
			Status = Task?.LastStatus ?? TaskStatus.Unknown;
		}
		
		public override object GetValue(NodePort port) {
			return null;
		}
	}
}