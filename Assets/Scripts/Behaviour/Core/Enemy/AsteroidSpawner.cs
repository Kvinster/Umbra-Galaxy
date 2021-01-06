using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class AsteroidSpawner : BaseCoreComponent {
        [NotNull]
        public GameObject AsteroidPrefab;

        public float SpawnPeriod   = 1f;
        public float AsteroidRange = 1000f;
        public float AsteroidForce = 1000f;

        Player          _player;
        CoreSpawnHelper _spawnHelper;

        readonly Timer _spawnTimer = new Timer();

        void OnDestroy() {
            if ( _player ) {
                _player.OnPlayerDied -= OnPlayerDied;
            }
        }

        void Update() {
            if ( _spawnTimer.DeltaTick() ) {
                SpawnAsteroid();
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            _spawnHelper = starter.SpawnHelper;

            _spawnTimer.Start(SpawnPeriod);
            _player = starter.Player;
            _player.OnPlayerDied += OnPlayerDied;
        }

        void OnPlayerDied() {
            _player = null;
        }

        void SpawnAsteroid() {
            if ( !_player ) {
                return;
            }
            var randPos = Random.insideUnitCircle.normalized;
            randPos = (randPos == Vector2.zero) ? Vector2.right : randPos;
            var pos = (Vector3) randPos * AsteroidRange + _player.transform.position;
            var go  = Instantiate(AsteroidPrefab, pos, Quaternion.identity);
            var asteroid = go.GetComponent<Asteroid>();
            if ( !asteroid ) {
                Debug.LogError("Can't init asteroid");
                return;
            }
            var dirToPlayer = _player.transform.position - pos;
            asteroid.Init(dirToPlayer.normalized * AsteroidForce);

            _spawnHelper.TryInitSpawnedObject(go);
        }
    }
}