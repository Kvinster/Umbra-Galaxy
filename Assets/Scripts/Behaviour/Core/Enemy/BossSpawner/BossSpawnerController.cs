using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossSpawnerController : BaseCoreComponent, IDestructible {
		int _spawnCount;
		
		public BehaviourTree Tree;

		public List<DestructiblePart> Spawners;

		public BossSpawnerGunsSubsystem GunsSubsystem;

		bool HasSpawners => Spawners.Count > 0;

		void Update() {
			Tree.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			GunsSubsystem.Init();
			
			Tree = new BehaviourTree(
				new SelectorTask(
					new SequenceTask(
						new ConditionTask(() => (GunsSubsystem.HasGuns && (GunsSubsystem.FireCount == _spawnCount)) || !HasSpawners),
						GunsSubsystem.GunSubBt
					),
					new SequenceTask(
						new ConditionTask(() => (HasSpawners && (_spawnCount < GunsSubsystem.FireCount)) || !GunsSubsystem.HasGuns),
						new WaitTask(3f),
						new CustomActionTask(() => {
							_spawnCount++;
						})
					)
				)
			);
		}

		public void TakeDamage(float damage) {
			
		}
	}
}