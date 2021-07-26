using XNode;

namespace STP.Config {
	public class BehaviourTreeNode : Node {
		[Input(connectionType = ConnectionType.Override)]  public BehaviourTreeNode Parent;
		[Output(connectionType = ConnectionType.Multiple)] public BehaviourTreeNode ThisNode;

		public override object GetValue(NodePort port) {
			return null;
		}
	}
}