using System;
using STP.Utils.BehaviourTree.Tasks;
using UnityEngine;
using XNode;

namespace STP.Config {
	public class BehaviourTreeNode : Node {
		[Input(connectionType = ConnectionType.Override)]  public BehaviourTreeNode Parent;
		[Output(connectionType = ConnectionType.Multiple)] public BehaviourTreeNode ThisNode;

		public BaseTask Task;

		// Only for editor
		public Color StatusColor;
		
		public void UpdateValues() {
			if ( Task?.LastStatus == null ) {
				StatusColor = Color.black;
				return;
			}
			switch ( Task.LastStatus ) {
				case TaskStatus.Success: {
					StatusColor = Color.green;
					break;
				}
				case TaskStatus.Continue: {
					StatusColor = Color.yellow;
					break;
				}
				case TaskStatus.Failure: {
					StatusColor = Color.red;
					break;
				}
				case TaskStatus.Unknown: {
					StatusColor = Color.gray;
					break;
				}
				default: {
					Debug.LogError("unknown task status");
					break;
				}
				
			}
		}
		
		public override object GetValue(NodePort port) {
			return null;
		}
	}
}