using UnityEngine;

using System.Collections.Generic;

using STP.Gameplay.Weapon.Common;
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
        
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;
        
        public List<Transform> Route;
        public int             StartRoutePointIndex;
        int   _nextRoutePoint;
        
        PlayerShipState _playerShipState;
        MaterialCreator _materialCreator;
        
        public EnemyState State {get; private set;} = EnemyState.None;
        
        Vector3 NextPoint       => Route[_nextRoutePoint].position;
        Vector2 MovingVector    => (NextPoint - transform.position);
        Vector2 MovingDirection => MovingVector.normalized;
        bool    CloseToPoint    => MovingVector.magnitude < CloseRadius;
        
        
        public override void Init(CoreStarter starter) {
            _materialCreator   = starter.MaterialCreator;
            _playerShipState   = starter.CoreManager.PlayerShipState;
            transform.position = Route[StartRoutePointIndex].position;
            _nextRoutePoint    = (StartRoutePointIndex + 1) % Route.Count;
            State              = EnemyState.Patrolling;
            WeaponControl      = starter.WeaponCreator.GetAIWeaponController(Weapons.Laser, this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl.GetControlledWeapon());
            InternalInit(new ShipInfo(Hp, ShipSpeed));
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
            switch ( State ) {
                case EnemyState.Patrolling:
                    OnPatrolling();
                    break;
                case EnemyState.Chase:
                    OnChase();
                    break;
                default:
                    Debug.LogError(string.Format("Invalid state {0} Ignored.", State), this);
                    break;
            }
            UpdateWeaponControlState();
        }

        void OnPatrolling() {
            Move(MovingDirection);
            Rotate(MovingDirection);
            if ( CloseToPoint ) {
                _nextRoutePoint  = (_nextRoutePoint + 1) % Route.Count;
            }
            var chasingVector     = _playerShipState.Position - (Vector2) transform.position;
            var distanceToPlayer  = chasingVector.magnitude;
            if ( distanceToPlayer < ChaseRadius) {
                State = EnemyState.Chase;    
            }
        }

        void OnChase() {
            var chasingVector    =  _playerShipState.Position - (Vector2) transform.position;
            var chasingDirection = chasingVector.normalized;
            var distanceToPlayer = chasingVector.magnitude;
            Move(chasingDirection);
            Rotate(chasingDirection);
            if ( distanceToPlayer >= OutChaseRadius ) {
                State = EnemyState.Patrolling;    
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