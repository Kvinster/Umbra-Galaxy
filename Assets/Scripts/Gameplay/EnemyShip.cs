using UnityEngine;

using System.Collections.Generic;

using STP.State;
using STP.State.Core;

namespace STP.Gameplay {
    public enum EnemyState {
        None,
        Chase,
        Patrolling
    }
    
    public class EnemyShip : BaseShip {
        const float CloseRadius     = 10f;
        const float ChaseRadius     = 350f;
        const float OutChaseRadius  = 500;
        
        const int   BulletSpeed     = 400;
        const float BulletPeriod    = 1f;
        
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;
        
        public List<Transform> Route;
        public int             StartRoutePointIndex;
        int   _nextRoutePoint;
        
        PlayerShipState _playerShipState;
        MaterialCreator _materialCreator;
        
        EnemyState _state = EnemyState.None;
        
        Vector3 NextPoint       => Route[_nextRoutePoint].position;
        Vector2 MovingVector    => (NextPoint - transform.position);
        Vector2 MovingDirection => MovingVector.normalized;
        bool    CloseToPoint    => MovingVector.magnitude < CloseRadius;
        
        
        public override void Init(CoreStarter starter) {
            _materialCreator   = starter.MaterialCreator;
            _playerShipState   = starter.CoreManager.PlayerShipState;
            transform.position = Route[StartRoutePointIndex].position;
            _nextRoutePoint    = (StartRoutePointIndex + 1) % Route.Count;
            _state             = EnemyState.Patrolling;
            InternalInit(starter, new WeaponInfo(BulletNames.EnemyBullet, BulletPeriod, BulletSpeed), new ShipInfo(Hp, ShipSpeed));
        }

        protected override void Update() {
            base.Update();
            OnShipState();
        }

        protected override void OnShipDestroy() {
            _materialCreator.CreateRandomMaterial(transform.position);
            Destroy(gameObject);
        }

        void OnShipState() {
            switch ( _state ) {
                case EnemyState.Patrolling:
                    OnPatrolling();
                    break;
                case EnemyState.Chase:
                    OnChase();
                    break;
                default:
                    Debug.LogError(string.Format("Invalid state {0} Ignored.", _state), this);
                    break;
            }
        }

        void OnPatrolling() {
            Move(MovingDirection);
            if ( CloseToPoint ) {
                _nextRoutePoint  = (_nextRoutePoint + 1) % Route.Count;
            }
            var chasingVector     = _playerShipState.Position - (Vector2) transform.position;
            var distanceToPlayer  = chasingVector.magnitude;
            if ( distanceToPlayer < ChaseRadius) {
                _state = EnemyState.Chase;    
            }
        }

        void OnChase() {
            var chasingVector    =  _playerShipState.Position - (Vector2) transform.position;
            var chasingDirection = chasingVector.normalized;
            var distanceToPlayer = chasingVector.magnitude;
            Move(chasingDirection);
            TryShoot();
            if ( distanceToPlayer >= OutChaseRadius ) {
                _state = EnemyState.Patrolling;    
            }
        }
        
        void OnDrawGizmos(){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ChaseRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, OutChaseRadius);
        }
    }
}