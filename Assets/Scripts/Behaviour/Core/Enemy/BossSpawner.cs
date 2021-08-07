﻿using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;


namespace STP.Behaviour.Core.Enemy {
	public class BossSpawner : BaseCoreComponent, IDestructible {
		BehaviourTree _tree;
		
		void Update() {
			_tree.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			_tree = new BehaviourTree(
				new SequenceTask(
					new WaitTask(1),
					new LogTask("log")
				)
		);
	}
		
		public void TakeDamage(float damage) {
			
		}
		
		// int _firedCount;
		// int _spawnedCount;
		//
		// bool NeedToFire() {
		// 	return _firedCount < 2;
		// }
		//
		// bool NeedToSpawn() {
		// 	return _spawnedCount < 2;
		// }

		// TaskStatus Spawn() {
		// 	_spawnedCount++;
		// 	_firedCount = 0;
		// 	return TaskStatus.Success;
		// }
		//
		// TaskStatus Fire() {
		// 	_firedCount++;
		// 	_spawnedCount = 0;
		// 	return TaskStatus.Success;
		// }
		//
		// TaskStatus Charge() {
		// 	return TaskStatus.Success;
		// }
		//
		// TaskStatus RotateToPlayer() {
		// 	return TaskStatus.Continue;
		// }
	}
}