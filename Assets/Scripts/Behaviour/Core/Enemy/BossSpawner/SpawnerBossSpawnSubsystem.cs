using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossSpawnSubsystem {
		List<Spawner> _spawns;

		Spawner     _selectedSpawn;
		SpawnParams _spawnParams;
		
		public BaseTask SpawnTask =>
			new SequenceTask(
				new RepeatTask(_spawnParams.EnemiesInWaveCount,
					new SequenceTask(
						new ConditionTask("Is spawner selected", TrySelectSpawner),
						new CustomActionTask("Spawn", () => _selectedSpawn.Spawn()),
						new WaitTask(_spawnParams.SpawnWaitTime)
					)
				)
			);

		public void Init(List<Spawner> spawns, CoreStarter starter, SpawnParams spawnParams, BossMovementSubsystem movementSubsystem) {
			_spawns      = spawns;
			_spawnParams = spawnParams;
			foreach ( var spawn in spawns ) {
				spawn.Init(starter.SpawnHelper);
			}
		}

		bool TrySelectSpawner() {
			_selectedSpawn = RandomUtils.GetRandomElement(_spawns);
			return _selectedSpawn;
		}
	}
}