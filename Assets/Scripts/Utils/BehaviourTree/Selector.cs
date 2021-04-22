using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

namespace STP.Utils.BehaviourTree {
	public sealed class Selector : BaseTask {
		readonly List<BaseTask> _tasks;

		public override bool CanExecute {
			get {
				foreach ( var task in _tasks ) {
					if ( task.CanExecute ) {
						return true;
					}
				}
				return false;
			}
		}

		public Selector(params BaseTask[] tasks) {
			Assert.IsNotNull(tasks);
			Assert.IsTrue(tasks.Length > 0);
			_tasks = tasks.ToList();
			_tasks.TrimExcess();
		}

		public override void SetBlackboard(Blackboard blackboard) {
			base.SetBlackboard(blackboard);
			foreach ( var task in _tasks ) {
				task.SetBlackboard(blackboard);
			}
		}

		protected override async UniTask Execute() {
			Assert.IsTrue(CanExecute);
			foreach ( var task in _tasks ) {
				if ( task.CanExecute ) {
					await task.Execute(CancellationToken);
					return;
				}
			}
			Debug.LogError("Neither task can execute");
		}
	}
}
