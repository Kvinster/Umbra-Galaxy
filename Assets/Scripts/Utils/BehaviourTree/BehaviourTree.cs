﻿using System;

using STP.Utils.BehaviourTree.Tasks;

namespace STP.Utils.BehaviourTree {
	public sealed class BehaviourTree {
		public readonly Blackboard Blackboard = new Blackboard();

		public readonly BaseTask Root;

		public event Action<BehaviourTree> OnBehaviourTreeUpdated; 

		public BehaviourTree(BaseTask root) {
			Root = root;
			
			Root.SetBlackboard(Blackboard);
			
		}

		public void Tick() {
			Root.Execute();
			OnBehaviourTreeUpdated?.Invoke(this);
		}
	}
}
