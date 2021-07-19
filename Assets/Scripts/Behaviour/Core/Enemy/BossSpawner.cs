using STP.Behaviour.Starter;

using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;

namespace STP.Behaviour.Core.Enemy {
	public class BossSpawner : BaseCoreComponent, IDestructible {
		public BehaviorTree Tree;
		
		void Update() {
			Tree?.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			Tree = CreateBt();
		}
		
		public void TakeDamage(float damage) {
			
		}

		BehaviorTree CreateBt() {
			var treeBuilder = new BehaviorTreeBuilder(gameObject);
			
			return treeBuilder
				.Selector()
					.Parallel()
						.Do("rotate to player", RotateToPlayer)
						.RepeatUntilFailure()
							.Condition("need to fire", NeedToFire)
						.End()
						.Sequence()
							.Do("charge", Charge)
							.WaitTime(3f)
							.Do("fire", Fire)
							.End()
						.End()
					.Parallel()
						.Do("rotate to player", RotateToPlayer)
						.RepeatUntilFailure()
							.Condition("need to spawn", NeedToSpawn)
						.End()
						.Sequence()
							.WaitTime(3f)
							.Do("spawn", Spawn)
							.End()
						.End()
				.End().Build();
		}

		int _firedCount;
		int _spawnedCount;
		
		bool NeedToFire() {
			return _firedCount == 0;
		}
		
		bool NeedToSpawn() {
			return _spawnedCount == 0;
		}

		TaskStatus Spawn() {
			_spawnedCount++;
			_firedCount = 0;
			return TaskStatus.Success;
		}
		
		TaskStatus Fire() {
			_firedCount++;
			_spawnedCount = 0;
			return TaskStatus.Success;
		}
		
		TaskStatus Charge() {
			return TaskStatus.Success;
		}

		TaskStatus RotateToPlayer() {
			return TaskStatus.Continue;
		}
	}
}