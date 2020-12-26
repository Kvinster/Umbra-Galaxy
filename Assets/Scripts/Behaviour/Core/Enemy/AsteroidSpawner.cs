﻿using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class AsteroidSpawner : BaseStarterCoreComponent {
        [NotNull] public GameObject AsteroidPrefab;

        public float SpawnPeriod   = 1f;
        public float AsteroidRange = 1000f;
        public float AsteroidForce = 1000f;

        Player _player;

        readonly Timer _spawnTimer = new Timer();

        protected override void InitInternal(CoreStarter starter) {
            _spawnTimer.Start(SpawnPeriod);
            _player = starter.Player;
            _player.PlayerDied += OnPlayerDied;
        }

        void OnDestroy() {
            if ( _player ) {
                _player.PlayerDied -= OnPlayerDied;
            }
        }

        void Update() {
            if ( _spawnTimer.DeltaTick() ) {
                SpawnAsteroid();
            }
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
        }
    }
}