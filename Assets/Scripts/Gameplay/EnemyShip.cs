using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.State.Core;

namespace STP.Gameplay {
    public enum EnemyState {
        None,
        Chase,
        Patrolling
    }
    
    public class EnemyShip : RoutedShip {
        const float ChaseRadius     = 350f;
        const float OutChaseRadius  = 500;
        
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;
        
        PlayerShipState _playerShipState;
        MaterialCreator _materialCreator;
        
        public EnemyState State {get; private set;} = EnemyState.None;
        public override ConflictSide CurrentSide => ConflictSide.Aliens;

        public override void Init(CoreStarter starter) {
            base.Init(starter);
            _materialCreator   = starter.MaterialCreator;
            _playerShipState   = starter.CoreManager.PlayerShipState;
            State              = EnemyState.Patrolling;
            WeaponControl      = starter.WeaponCreator.GetAIWeaponController(WeaponType.Laser, this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl.GetControlledWeapon());
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
        }

        protected void Update() {
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
            Move();
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
    }
}