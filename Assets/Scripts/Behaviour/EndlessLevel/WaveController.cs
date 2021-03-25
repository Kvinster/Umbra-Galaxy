using UnityEngine;

using System.Collections.Generic;
using STP.Behaviour.EndlessLevel.Enemies;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel {
	public class WaveController : BaseEndlessLevelComponent {
		[NotNullOrEmpty] public List<WaveInfo> Waves = new List<WaveInfo>();

		readonly Timer _timer = new Timer();
		
		int _curWaveIndex;

		EndlessLevelStarter _starter;
		
		void Update() {
			if ( _timer.Tick(Time.deltaTime) ) {
				RunNextWave();
			}
		}
		
		protected override void InitInternal(EndlessLevelStarter starter) {
			_curWaveIndex = 0;
			_starter      = starter;
			RunWave(_curWaveIndex);
		}

		void RunNextWave() {
			_curWaveIndex = (_curWaveIndex+1) % Waves.Count;
			RunWave(_curWaveIndex);
		}

		void RunWave(int index) {
			var curWave = Waves[index];
			_timer.Reset(curWave.MaxTimeUntilNextWave);
			SpawnEnemies(curWave);
			Debug.Log($"started wave {_curWaveIndex}");
		}

		void SpawnEnemies(WaveInfo wave) {
			var spawnPointIndex = 0;
			foreach ( var enemy in wave.EnemiesToSpawn ) {
				for ( var i = 0; i < enemy.Count; i++) {
					var spawnPoint = wave.SelectedSpawnPoints[spawnPointIndex];
					var go = Instantiate(enemy.Prefab, spawnPoint.position, Quaternion.identity);
					var enemyComp = go.GetComponent<BaseEnemy>();
					if ( enemyComp ) {
						enemyComp.Init(_starter);
					} else {
						Debug.LogError($"Enemy doesn't have an {nameof(BaseEnemy)} comp");	
					}
					spawnPointIndex = (spawnPointIndex + 1) % wave.SelectedSpawnPoints.Count;
				}
			}
		}
	}
}