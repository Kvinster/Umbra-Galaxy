using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossController : BaseCoreComponent, IDestructible {
		int _spawnCount;
		
		public BehaviourTree Tree;

		public List<BossGun>          Guns;
		public List<DestructiblePart> Spawners;

		BossSpawnerGunsSubsystem _gunsSubsystem;

		bool HasSpawners => Spawners.Count > 0;

		void Update() {
			Tree.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			_gunsSubsystem = new BossSpawnerGunsSubsystem();
			_gunsSubsystem.Init(Guns, starter);
			
			Tree = new BehaviourTree(
				new SelectorTask(
					new SequenceTask(
						new ConditionTask(() => (_gunsSubsystem.HasGuns && (_gunsSubsystem.FireCount == _spawnCount)) || !HasSpawners),
						_gunsSubsystem.GunSubBt
					),
					new SequenceTask(
						new ConditionTask(() => (HasSpawners && (_spawnCount < _gunsSubsystem.FireCount)) || !_gunsSubsystem.HasGuns),
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