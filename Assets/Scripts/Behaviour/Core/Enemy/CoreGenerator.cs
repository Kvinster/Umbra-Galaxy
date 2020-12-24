using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class CoreGenerator : BaseStarterCoreComponent, IDestructible {
                  public float       StartHp = 100;
        [NotNull] public Collider2D  Collider;
        [NotNull] public ProgressBar HealthBar;
        [Header("Turret")]
        [NotNull] public TriggerNotifier FireTrigger;
                  public float           ReloadDuration;
        [Header("Bullet")]
        [NotNull] public GameObject BulletPrefab;
                  public float      BulletRunForce;
        
        readonly Timer _fireTimer = new Timer();
        
        GameObject _target;

        float _curHp;
        
        float CurHp {
            get => _curHp;
            set {
                _curHp = value;
                HealthBar.Progress = _curHp / StartHp;
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            FireTrigger.OnTriggerEnter += OnFireRangeEnter;
            FireTrigger.OnTriggerExit  += OnFireRangeExit;
            CurHp = StartHp;
        }
        
        void Update() {
            if ( !_target ) {
                return;
            }
            if ( _fireTimer.DeltaTick() ) {
                Fire();
            }
        }

        public void TakeDamage(float damage) {
            CurHp -= damage;
            if ( CurHp <= 0 ) {
                Destroy(gameObject);
            }
        }
        
        void OnFireRangeEnter(GameObject other) {
            var playerComp = other.GetComponent<Player>();
            if ( playerComp ) {
                _target = other;
                _fireTimer.Start(ReloadDuration);
            }
        }
        
        void OnFireRangeExit(GameObject other) {
            if ( _target == other ) {
                _target = null;
                _fireTimer.Stop();
            }
        }

        void Fire() {
            if ( !_target ) {
                Debug.LogError("Can't fire. Target not found.");
                return;
            }
            var go = Instantiate(BulletPrefab, transform.position, Quaternion.Euler(0, 0, GetViewAngleToTarget()));
            InitCreatedObject(go);
        }

        void InitCreatedObject(GameObject bullet) {
            var bulletComp = bullet.GetComponent<Bullet>();
            if ( bulletComp ) {
                bulletComp.Init(Collider, Vector2.up * BulletRunForce, GetViewAngleToTarget());
            }
        }

        float GetViewAngleToTarget() {
            var dirToPlayer = _target.transform.position - transform.position;
            return MathUtils.GetSmoothRotationAngleOffset(Vector2.up, dirToPlayer.normalized, 1f);
        }
    }
}