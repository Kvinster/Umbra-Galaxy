using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Behaviour.Core.Enemy.Boss.AI;
using STP.Manager;
using STP.Utils.BehaviourTree;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Enemy.Boss {
	public sealed class BossController : BaseCoreComponent, IDestructible {
		[Header("Parameters")]
		public float StartHp = 100;
		[Header("Dependencies")]
		[NotNull] public BossMoveAgent             MoveAgent;
		[NotNull] public BossGunController         GunController;
		[NotNull] public BossGunRotationController GunRotationController;

		float _horizontal;
		float _vertical;

		LevelGoalManager _levelGoalManager;

		BehaviourTree _tree;

		float _curHp;

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;

			_tree = new BehaviourTree(
				new Selector(
					new GunChargeTask(MoveAgent, GunController),
					new IdleTask(1f)
				)
			);

			_curHp = StartHp;

			MoveAgent.SetTarget(starter.Player.transform);
			GunRotationController.SetTarget(starter.Player.transform);
			GunRotationController.IsActive = true;

			try {
				UniTask.Create(_tree.Execute);
			} catch ( Exception e ) {
				Debug.LogError(e.Message);
			}
		}

		public void TakeDamage(float damage) {
			_curHp = Mathf.Max(_curHp - damage, 0f);

			if ( Mathf.Approximately(_curHp, 0f) ) {
				Die();
			}
		}

		void Die() {
			_tree.Stop();
			_levelGoalManager.Advance();
			Destroy(gameObject);
		}
	}
}
