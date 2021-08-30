using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossSpawnSubsystem {
		List<Spawner> _spawns;

		Spawner _selectedSpawn;
		
		public bool HasSpawners => _spawns.Count > 0;

		public BaseTask BehaviourTree { get; private set; }
		
		public void Init(List<Spawner> spawns, CoreStarter starter, SpawnParams spawnParams) {
			_spawns      = spawns;
			foreach ( var spawn in spawns ) {
				spawn.Init(starter.SpawnHelper);
				spawn.OnDiedEvent += OnSpawnerDestroyed;
			}

			BehaviourTree = new RepeatTask(spawnParams.EnemiesInWaveCount, new SequenceTask(
				new ConditionTask("Is spawner selected", TrySelectSpawner),
				new CustomActionTask("Spawn", () => _selectedSpawn.Spawn()),
				new WaitTask(spawnParams.SpawnWaitTime)
			));
		}

		public void Deinit() {
			foreach ( var spawn in _spawns ) {
				spawn.OnDiedEvent -= OnSpawnerDestroyed;
			}
		}

		void OnSpawnerDestroyed(DestructiblePart destroyedGun) {
			_spawns.RemoveAll(x => x == destroyedGun);
			if ( _selectedSpawn == destroyedGun ) {
				BehaviourTree.InstantFinishTask(TaskStatus.Failure);
			}
		}

		bool TrySelectSpawner() {
			if ( _spawns.Count == 0 ) {
				return false;
			}
			_selectedSpawn = RandomUtils.GetRandomElement(_spawns);
			return true;
		}
	}
}