using UnityEngine;

using STP.Behaviour.Starter;
using STP.Core.ShootingsSystems;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    [SelectionBase]
    public sealed class Generator : BaseEnemy, IDestructible {
        public ShootingSystemParams ShootingParams;
        [Space]
        public Connector Connector;
        public bool IsMainGenerator;
        public float StartHp = 100;
        [NotNull]
        public Transform ViewTransform;
        [NotNull]
        public ProgressBar HealthBar;
        [NotNull]
        public VfxRunner ExplosionEffect;
        [Header("Sound")]
        [NotNull]
        public BaseSimpleSoundPlayer ShotSoundPlayer;

        ShootingSystem _shootingSystem;
        
        LevelGoalManager _levelGoalManager;

        Transform _target;

        float _curHp;

        float CurHp {
            get => _curHp;
            set {
                _curHp = value;
                HealthBar.Progress = _curHp / StartHp;
            }
        }

        protected override void OnDisable() {
            base.OnDisable();
            GeneratorsWatcher.RemoveGenerator(this);
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
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            GeneratorsWatcher.TryAddGenerator(this);
            if ( !IsMainGenerator ) {
                Connector.OnOutOfLinks += DieFromGenerator;
            }

            _shootingSystem = new ShootingSystem(starter.SpawnHelper, ShootingParams);
            _levelGoalManager = starter.LevelGoalManager;

            CurHp = StartHp;
        }

        public void TakeDamage(float damage) {
            CurHp = Mathf.Max(CurHp - damage, 0);
            if ( CurHp == 0 ) {
                Die();
            }
        }

        protected override void Die(bool fromPlayer = true) {
            Die(fromConnector: false, fromPlayer);
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
                        Connector.ForceDestroy();
                    }
                } else {
                    Connector.DestroyConnector();
                }
            }
            Destroy(gameObject);
            // detach VFX on death
            ExplosionEffect.transform.SetParent(transform.parent);
            if ( fromConnector ) {
                ExplosionEffect.ScheduleVfx(0.5f, true);
            } else {
                ExplosionEffect.RunVfx(true);
            }
        }

        float GetViewAngleToTarget() {
            var dirToPlayer = _target.transform.position - transform.position;
            return MathUtils.GetSmoothRotationAngleOffset(Vector2.up, dirToPlayer.normalized, 1f);
        }
    }
}