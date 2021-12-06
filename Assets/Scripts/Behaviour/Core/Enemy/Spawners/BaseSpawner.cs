using UnityEngine;

using STP.Behaviour.Starter;
using STP.Config;
using STP.Manager;
using STP.Utils;

namespace STP.Behaviour.Core.Enemy.Spawners {
    public abstract class BaseSpawner : BaseCoreComponent {
        protected Player          Player;
        protected CoreSpawnHelper SpawnHelper;

        readonly Timer _spawnTimer = new Timer();

        protected abstract BaseSpawnerSettings Settings { get; }

        LevelManager     _levelManager;
        LevelGoalManager _levelGoalManager;

        bool _isStopped;

        bool _isInited;
        
        bool IsLevelActiveAndNotWon => _isInited && _levelManager.IsLevelActive && !_levelGoalManager.IsLevelWon;
        
        void OnDestroy() {
            if ( Player ) {
                Player.OnPlayerDied -= OnPlayerDied;
            }
            _isInited = false;
        }

        void Update() {
            if ( !IsLevelActiveAndNotWon ) {
                return;
            }
            if ( !_isStopped && _spawnTimer.DeltaTick() ) {
                Spawn();
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            if ( !Settings.Enabled ) {
                _isStopped = true;
                return;
            }

            _isInited = true;

            _levelManager     = starter.LevelManager;
            _levelGoalManager = starter.LevelGoalManager;

            SpawnHelper = starter.SpawnHelper;
            _spawnTimer.Start(Settings.SpawnPeriod);
            Player              =  starter.Player;
            Player.OnPlayerDied += OnPlayerDied;
        }

        protected virtual void InitItem(GameObject go) { }

        void OnPlayerDied() {
            Player = null;
        }

        void Spawn() {
            if ( !Player ) {
                return;
            }
            var randPos = Random.insideUnitCircle.normalized;
            randPos = (randPos == Vector2.zero) ? Vector2.right : randPos;
            var pos = (Vector3) randPos * Settings.SpawnRange + Player.transform.position;
            var go  = Instantiate(Settings.Prefab, pos, Quaternion.identity, SpawnHelper.TempObjRoot);
            InitItem(go);
            // Init mini icon
            SpawnHelper.TryInitSpawnedObject(go);
        }
    }
}