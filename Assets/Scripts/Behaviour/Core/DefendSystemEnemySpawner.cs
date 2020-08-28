using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace STP.Behaviour.Core {
    public sealed class DefendSystemEnemySpawner : CoreComponent {
        [Serializable]
        public sealed class WaveInfo {
            public float Delay;
            public int   EnemyCount;
            [NotNullOrEmpty]
            public List<Transform>  SpawnPoints  = new List<Transform>();
            [NotNullOrEmpty]
            public List<GameObject> EnemyPrefabs = new List<GameObject>();

            public void CheckDescription(Object context) {
                GameComponentUtils.CheckAttributes(this, context);
            }
        }

        public bool StartSpawnOnInit;
        [NotNull]
        public Transform EnemyParent;
        [NotNullOrEmpty]
        public List<WaveInfo> WaveInfos = new List<WaveInfo>();

        readonly Timer _timer = new Timer();

        CoreStarter _starter;

        int  _curWaveIndex;

        public bool IsSpawnActive { get; private set; }

        public event Action<GameObject> OnEnemySpawned;

        protected override void CheckDescription() {
            foreach ( var waveInfo in WaveInfos ) {
                waveInfo.CheckDescription(this);
            }
        }

        void Update() {
            if ( !IsSpawnActive ) {
                return;
            }
            if ( _timer.DeltaTick() ) {
                Spawn();
                NextWave();
            }
        }

        public override void Init(CoreStarter starter) {
            _starter = starter;

            if ( StartSpawnOnInit ) {
                StartSpawn();
            }
        }

        public void TryStartSpawn() {
            if ( IsSpawnActive ) {
                return;
            }
            StartSpawn();
        }

        public void StartSpawn() {
            if ( IsSpawnActive ) {
                Debug.LogError("Can't start spawn — spawn is already active");
                return;
            }
            _curWaveIndex = 0;
            var waveInfo = WaveInfos[_curWaveIndex];
            _timer.Start(waveInfo.Delay);
            IsSpawnActive = true;
        }

        public void PauseSpawn() {
            if ( !IsSpawnActive ) {
                Debug.LogError("Can't pause spawn — spawn is not active");
                return;
            }
            IsSpawnActive = false;
        }

        public void StopSpawn() {
            if ( !IsSpawnActive ) {
                Debug.LogError("Can't stop spawn — spawn is not active");
                return;
            }
            _timer.Stop();
            _curWaveIndex  = 0;
            IsSpawnActive = false;
        }

        void Spawn() {
            if ( !IsSpawnActive ) {
                Debug.LogError("Can't spawn — spawn is not active");
                return;
            }
            var waveInfo = WaveInfos[_curWaveIndex];
            for ( var i = 0; i < waveInfo.EnemyCount; ++i ) {
                var enemyPrefab = waveInfo.EnemyPrefabs[Random.Range(0, waveInfo.EnemyPrefabs.Count)];
                var spawnPoint  = waveInfo.SpawnPoints[Random.Range(0, waveInfo.SpawnPoints.Count)];
                var enemyGo     = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, EnemyParent);
                // TODO: init enemy
                foreach ( var comp in enemyGo.GetComponentsInChildren<CoreComponent>() ) {
                    comp.Init(_starter);
                }
                OnEnemySpawned?.Invoke(enemyGo);
            }
        }

        void NextWave() {
            ++_curWaveIndex;
            if ( _curWaveIndex >= WaveInfos.Count ) {
                StopSpawn();
            } else {
                var waveInfo = WaveInfos[_curWaveIndex];
                _timer.Start(waveInfo.Delay);
            }
        }
    }
}
