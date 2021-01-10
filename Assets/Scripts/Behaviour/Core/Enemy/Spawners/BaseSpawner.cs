using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
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

        void OnDestroy() {
            if ( Player ) {
                Player.OnPlayerDied -= OnPlayerDied;
            }
        }

        void Update() {
            if ( _spawnTimer.DeltaTick() ) {
                SpawnAsteroid();
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            SpawnHelper = starter.SpawnHelper;
            _spawnTimer.Start(SpawnPeriod);
            Player = starter.Player;
            Player.OnPlayerDied += OnPlayerDied;
        }

        protected virtual void InitItem(GameObject go) {}

        void OnPlayerDied() {
            Player = null;
        }
        
        void SpawnAsteroid() {
            if ( !Player ) {
                return;
            }
            var randPos = Random.insideUnitCircle.normalized;
            randPos = (randPos == Vector2.zero) ? Vector2.right : randPos;
            var pos = (Vector3) randPos * SpawnRange + Player.transform.position;
            var go  = Instantiate(Prefab, pos, Quaternion.identity);
            InitItem(go);
            //Init mini icon
            SpawnHelper.TryInitSpawnedObject(go);
        }
    }
}