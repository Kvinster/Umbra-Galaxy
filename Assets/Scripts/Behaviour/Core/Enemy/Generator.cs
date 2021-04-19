using UnityEngine;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    [SelectionBase]
    public sealed class Generator : BaseEnemy, IDestructible {
        [Space]
        public Connector Connector;

        public bool  IsMainGenerator;
        public float StartHp = 100;
        [NotNull]
        public Transform ViewTransform;
        [NotNull]
        public Collider2D  Collider;
        [NotNull]
        public ProgressBar HealthBar;
        [NotNull]
        public VfxRunner   ExplosionEffect;
        [Header("Turret")]
        [NotNull]
        public TriggerNotifier FireTrigger;
        public float           ReloadDuration;
        [Header("Bullet")]
        [NotNull]
        public Transform BulletOrigin;
        [NotNull]
        public GameObject BulletPrefab;
        public float      BulletStartSpeed;
        [Header("Sound")]
        [NotNull]
        public BaseSimpleSoundPlayer ShotSoundPlayer;

        CoreSpawnHelper  _spawnHelper;
        LevelGoalManager _levelGoalManager;

        readonly Timer _fireTimer = new Timer();

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
            if ( !_target ) {
                return;
            }
            ViewTransform.rotation = Quaternion.Euler(0, 0, GetViewAngleToTarget());
            if ( _fireTimer.DeltaTick() ) {
                Fire();
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
            if ( _target ) {
                _fireTimer.Start(ReloadDuration);
            } else {
              _fireTimer.Stop();  
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            GeneratorsWatcher.TryAddGenerator(this);
            FireTrigger.OnTriggerEnter += OnFireRangeEnter;
            FireTrigger.OnTriggerExit  += OnFireRangeExit;
            if ( !IsMainGenerator ) {
                Connector.OnOutOfLinks += DieFromGenerator;
            }

            _spawnHelper      = starter.SpawnHelper;
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


        void OnFireRangeEnter(GameObject other) {
            var playerComp = other.GetComponent<Player>();
            if ( playerComp ) {
                _target = other.transform;
                _fireTimer.Start(ReloadDuration);
            }
        }

        void OnFireRangeExit(GameObject other) {
            if ( _target == other.transform ) {
                _target = null;
                _fireTimer.Stop();
            }
        }

        void Fire() {
            if ( !_target ) {
                Debug.LogError("Can't fire. Target not found.");
                return;
            }
            var go = Instantiate(BulletPrefab, BulletOrigin.position, Quaternion.Euler(0, 0, GetViewAngleToTarget()), _spawnHelper.TempObjRoot);
            InitCreatedObject(go);
            ShotSoundPlayer.Play();
        }

        void InitCreatedObject(GameObject go) {
            var bulletComp = go.GetComponent<IBullet>();
            bulletComp?.Init(10f, BulletStartSpeed, GetViewAngleToTarget(), Collider);
            _spawnHelper.TryInitSpawnedObject(go);
        }

        float GetViewAngleToTarget() {
            var dirToPlayer = _target.transform.position - transform.position;
            return MathUtils.GetSmoothRotationAngleOffset(Vector2.up, dirToPlayer.normalized, 1f);
        }
    }
}