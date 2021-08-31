﻿using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossController : BaseCoreComponent {
		public BehaviourTree Tree;
		
		public SpawnParams   SpawnParams;

		public List<BossGun> Guns;
		public List<Spawner> Spawners;

		SpawnerBossGunsSubsystem  _gunsSubsystem;
		SpawnerBossSpawnSubsystem _spawnSubsystem;

		void Update() {
			Tree.Tick();
		}

		void OnDestroy() {
			_gunsSubsystem?.Deinit();
			_spawnSubsystem?.Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			_gunsSubsystem = new SpawnerBossGunsSubsystem();
			_gunsSubsystem.Init(Guns, starter);

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			_spawnSubsystem.Init(Spawners, starter, SpawnParams);

			Tree = new BehaviourTree(
				new SequenceTask(
					new AlwaysSuccessDecorator(_gunsSubsystem.BehaviourTree),
					new AlwaysSuccessDecorator(_spawnSubsystem.BehaviourTree),
					new SequenceTask(
						new ConditionTask("Is everything destroyed", () => !_gunsSubsystem.HasGuns && !_spawnSubsystem.HasSpawners),
						new CustomActionTask("destroy boss object", () => Destroy(gameObject))
					)
				)
			);
		}
	}
}