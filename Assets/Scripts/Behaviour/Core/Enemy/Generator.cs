using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class Generator : BaseStarterCoreComponent, IDestructible {
                  public float       StartHp = 100;
        [NotNull] public Collider2D  Collider;
        [NotNull] public ProgressBar HealthBar;
        [Header("Turret")]
        [NotNull] public TriggerNotifier FireTrigger;
                  public float           ReloadDuration;
        [Header("Bullet")]
        [NotNull] public GameObject BulletPrefab;
                  public float      BulletRunForce;
        [Header("SubGenerators")]
        public List<Generator> SubGenerators;
        public GameObject      ConnectorPrefab;

        CoreStarter _starter;
        
        Generator _rootGenerator;
        
        readonly List<Connector> _connectors = new List<Connector>();
        
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
            _starter = starter;
            CurHp = StartHp;
            ConnectToSubGenerators();
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
            CurHp = Mathf.Max(CurHp - damage, 0);
            if ( CurHp == 0 ) {
                DestroyGenerator();
            }
        }

        void DestroyGenerator() {
            if ( _rootGenerator ) {
                _rootGenerator.OnSubGeneratorDestroyed(this);
            }
            DestroySubGenerators();
            Destroy(gameObject);
        }

        public void OnSubGeneratorDestroyed(Generator generator) {
            SubGenerators.Remove(generator);
            var connectorToSubGenerator = _connectors.Find(x => x.Other == generator);
            if ( !connectorToSubGenerator ) {
                Debug.LogError($"Can't find connection to the sub generator {generator.gameObject.name}");
                return;
            }
            _connectors.Remove(connectorToSubGenerator);
            connectorToSubGenerator.DestroyConnector();
        }
        
        void ConnectToSubGenerators() {
            foreach ( var subGenerator in SubGenerators ) {
                var go = Instantiate(ConnectorPrefab, Vector3.zero, Quaternion.identity, transform);
                var connector = go.GetComponent<Connector>();
                connector.Init(this, subGenerator);
                subGenerator.SetRootGenerator(this);
                _connectors.Add(connector);
            }
        }

        void SetRootGenerator(Generator rootGenerator) {
            _rootGenerator = rootGenerator;
        }
        
        void DestroySubGenerators() {
            var generators = new List<Generator>(SubGenerators);
            foreach ( var generator in generators ) {
                generator.DestroyGenerator();
            }
            foreach ( var connector in _connectors ) {
                connector.DestroyConnector();
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

        void InitCreatedObject(GameObject go) {
            var bulletComp = go.GetComponent<Bullet>();
            if ( bulletComp ) {
                bulletComp.Init(Collider, Vector2.up * BulletRunForce, GetViewAngleToTarget());
                return;
            }
            var coreComps = go.GetComponentsInChildren<BaseStarterCoreComponent>();
            foreach ( var coreComp in coreComps ) {
                coreComp.Init(_starter);
            }
            if ( coreComps.Length != 0 ) {
                return;
            }
            Debug.LogError("Can't init spawned object");
        }

        float GetViewAngleToTarget() {
            var dirToPlayer = _target.transform.position - transform.position;
            return MathUtils.GetSmoothRotationAngleOffset(Vector2.up, dirToPlayer.normalized, 1f);
        }
    }
}