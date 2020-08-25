using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Gameplay.Weapon.Common;
using STP.State.Core;

namespace STP.Behaviour.Core.Objects {
    public enum EnemyState {
        None,
        Chase,
        Patrolling
    }

    public class EnemyShip : RoutedShip {
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;

        public float ChaseRadius;
        public float OutChaseRadius;

        CoreShipState   _coreShipState;
        CoreItemCreator _materialCreator;

        public List<Transform> DropItemsOnDeath;

        public EnemyState State {get; private set;} = EnemyState.None;
        public override ConflictSide CurrentSide => ConflictSide.Aliens;

        public override void Init(CoreStarter starter) {
            base.Init(starter);
            _materialCreator   = starter.CoreItemCreator;
            _coreShipState   = starter.CoreManager.CorePlayerShipState;
            State              = EnemyState.Patrolling;
            WeaponControl      = starter.WeaponCreator.GetAIWeaponController(WeaponType.Laser, this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl.GetControlledWeapon());
            foreach ( var dropItem in DropItemsOnDeath ) {
                dropItem.gameObject.SetActive(false);
            }
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
            OnShipState();
        }

        protected void Update() {
            OnShipState();
        }


        protected override void OnShipDestroy() {
            _materialCreator.CreateRandomMaterial(transform.position);
            foreach ( var dropItem in DropItemsOnDeath ) {
                _materialCreator.SetParentForItem(dropItem);
                dropItem.rotation = Quaternion.identity;
                dropItem.gameObject.SetActive(true);
            }
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
            var chasingVector     = _coreShipState.Position - (Vector2) transform.position;
            var distanceToPlayer  = chasingVector.magnitude;
            if ( distanceToPlayer < ChaseRadius) {
                State = EnemyState.Chase;
            }
        }

        void OnChase() {
            var chasingVector    =  _coreShipState.Position - (Vector2) transform.position;
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