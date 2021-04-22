using UnityEngine;

using STP.Behaviour.Starter;
using STP.Config;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;

namespace STP.Behaviour.Core.Enemy.Spawners {
    public abstract class BaseSpawner : BaseCoreComponent {
        protected Player          Player;
        protected CoreSpawnHelper SpawnHelper;

        readonly Timer _spawnTimer = new Timer();

        float      _spawnPeriod;
        float      _spawnRange;
        GameObject _prefab;

        bool _isStopped;

        void OnDestroy() {
            if ( Player ) {
                Player.OnPlayerDied -= OnPlayerDied;
            }
        }

        void Update() {
            if ( !_isStopped && _spawnTimer.DeltaTick() ) {
                Spawn();
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            EventManager.Subscribe<PlayerEnteredSafeArea>(OnPlayerEnteredSafeArea);
            EventManager.Subscribe<PlayerLeftSafeArea>(OnPlayerLeftSafeArea);
        }

        protected override void OnDisable() {
            base.OnDisable();
            EventManager.Unsubscribe<PlayerEnteredSafeArea>(OnPlayerEnteredSafeArea);
            EventManager.Unsubscribe<PlayerLeftSafeArea>(OnPlayerLeftSafeArea);
        }

        protected override void InitInternal(CoreStarter starter) {
            var settings = InitSettings(starter);
            if ( !settings.Enabled ) {
                _isStopped = true;
                EventManager.Unsubscribe<PlayerEnteredSafeArea>(OnPlayerEnteredSafeArea);
                EventManager.Unsubscribe<PlayerLeftSafeArea>(OnPlayerLeftSafeArea);
                return;
            }
            _spawnPeriod = settings.SpawnPeriod;
            _spawnRange  = settings.SpawnRange;
            _prefab      = settings.Prefab;

            SpawnHelper = starter.SpawnHelper;
            _spawnTimer.Start(_spawnPeriod);
            Player              =  starter.Player;
            Player.OnPlayerDied += OnPlayerDied;
        }

        protected abstract BaseSpawnerSettings InitSettings(CoreStarter starter);

        protected virtual void InitItem(GameObject go) { }

        void OnPlayerEnteredSafeArea(PlayerEnteredSafeArea e) {
            _isStopped = true;
        }

        void OnPlayerLeftSafeArea(PlayerLeftSafeArea e) {
            _isStopped = false;
        }

        void OnPlayerDied() {
            Player = null;
        }

        void Spawn() {
            if ( !Player ) {
                return;
            }
            var randPos = Random.insideUnitCircle.normalized;
            randPos = (randPos == Vector2.zero) ? Vector2.right : randPos;
            var pos = (Vector3) randPos * _spawnRange + Player.transform.position;
            var go  = Instantiate(_prefab, pos, Quaternion.identity, SpawnHelper.TempObjRoot);
            InitItem(go);
            // Init mini icon
            SpawnHelper.TryInitSpawnedObject(go);
        }
    }
}