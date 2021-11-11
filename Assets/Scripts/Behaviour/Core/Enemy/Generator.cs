using UnityEngine;

using STP.Behaviour.Starter;
using STP.Behaviour.Utils.ProgressBar;
using STP.Core.ShootingsSystems.Generator;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    [SelectionBase]
    public sealed class Generator : BaseEnemy, IDestructible {
        static int _instancesCount;

        public GeneratorShootingSystemParams ShootingParams;
        [Space]
        public Connector Connector;
        public bool IsMainGenerator;
        [NotNull]
        public Transform ViewTransform;
        [NotNull]
        public BaseProgressBar HealthBar;
        [Header("Sound")]
        [NotNull]
        public BaseSimpleSoundPlayer ShotSoundPlayer;

        GeneratorShootingSystem _shootingSystem;

        LevelManager     _levelManager;
        LevelGoalManager _levelGoalManager;

        Transform _target;

        protected override void OnEnable() {
            base.OnEnable();
            ++_instancesCount;
        }

        protected override void OnDisable() {
            base.OnDisable();
            GeneratorsWatcher.RemoveGenerator(this);
            --_instancesCount;
            if ( (_instancesCount == 0) && _levelManager.IsLevelActive && _levelGoalManager.IsLevelWon ) {
                _levelManager.StartLevelWin();
            }
        }

        void Update() {
            if ( !IsInit ) {
                return;
            }
            _shootingSystem.DeltaTick();
            if ( !_target ) {
                return;
            }
            ViewTransform.rotation = Quaternion.Euler(0, 0, GetViewAngleToTarget());
            if ( _shootingSystem.TryShoot() ) {
                ShotSoundPlayer.Play();
            }
        }

        public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
            SetTarget(playerTransform);
        }

        public override void OnBecomeInvisibleForPlayer() {
            SetTarget(null);
        }

        public override void SetTarget(Transform target) {
            _target = target;
            _shootingSystem.SetTarget(target);
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);

            _levelManager     = starter.LevelManager;
            _levelGoalManager = starter.LevelGoalManager;
            _shootingSystem   = new GeneratorShootingSystem(starter.SpawnHelper, ShootingParams, transform);

            GeneratorsWatcher.TryAddGenerator(this);
            if ( !IsMainGenerator ) {
                Connector.OnOutOfLinks += DieFromGenerator;
            }

            HpSystem.OnDied += DieFromPlayer;
            HpSystem.OnHpChanged += OnHpChanged;
        }

        public void TakeDamage(float damage) {
            HpSystem.TakeDamage(damage);
        }

        public override void Die(bool fromPlayer = true) {
            Die(fromConnector: false, fromPlayer);
        }

        void DieFromPlayer() {
            Die();
        }

        void OnHpChanged(float newHp) {
            HealthBar.Progress = newHp / StartHp;
        }

        void DieFromGenerator() {
            Die(fromConnector: true, fromPlayer: true);
        }

        void Die(bool fromConnector, bool fromPlayer) {
            base.Die(fromPlayer);
            if ( !fromConnector ) {
                if ( IsMainGenerator ) {
                    _levelGoalManager.Advance();
                    if ( Connector ) {
                        Connector.StartForceDestroyConnector(false);
                    }
                } else {
                    Connector.StartDestroyConnector(false);
                }
            }
            Destroy(gameObject);
        }

        float GetViewAngleToTarget() {
            var dirToPlayer = _target.transform.position - transform.position;
            return MathUtils.GetSmoothRotationAngleOffset(Vector2.up, dirToPlayer.normalized, 1f);
        }
    }
}