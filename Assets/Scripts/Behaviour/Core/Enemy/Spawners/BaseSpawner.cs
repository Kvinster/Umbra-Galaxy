using UnityEngine;

using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.Spawners {
    public abstract class BaseSpawner : BaseCoreComponent {
        [NotNull]
        public GameObject Prefab;

        public float SpawnPeriod = 1f;
        public float SpawnRange  = 1000f;

        protected Player          Player;
        protected CoreSpawnHelper SpawnHelper;

        readonly Timer _spawnTimer = new Timer();

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
            SpawnHelper = starter.SpawnHelper;
            _spawnTimer.Start(SpawnPeriod);
            Player              =  starter.Player;
            Player.OnPlayerDied += OnPlayerDied;
        }

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
            var pos = (Vector3) randPos * SpawnRange + Player.transform.position;
            var go  = Instantiate(Prefab, pos, Quaternion.identity, SpawnHelper.TempObjRoot);
            InitItem(go);
            // Init mini icon
            SpawnHelper.TryInitSpawnedObject(go);
        }
    }
}