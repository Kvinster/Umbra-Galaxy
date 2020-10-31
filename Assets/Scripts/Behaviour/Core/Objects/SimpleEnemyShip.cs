using UnityEngine;

using STP.Behaviour.Core.AiMovement;
using STP.Behaviour.Starter;
using STP.Gameplay;

namespace STP.Behaviour.Core.Objects {
    public enum EnemyState {
        None,
        Chase,
        Patrolling
    }

    public class SimpleEnemyShip : BaseEnemyShip {
        const float ShipSpeed        = 150f;
        const float ShipAccel        = 35f;
        const int   Hp               = 2;

        const int   PhysicsLayers    = 1 << 10 | //Aliens
                                       1 << 8;   //Player

        public ChaseMovementController MovementController;

        EnemyState State { get; set; } = EnemyState.None;

        public override bool CanShoot {
            get {
                if ( State != EnemyState.Chase ) {
                    return false;
                }
                var distance = (transform.position - MovementController.CurChaseTarget.position).magnitude;
                var hit = Physics2D.Raycast(transform.position, transform.rotation * Vector3.up, distance, PhysicsLayers);
                if ( !hit.collider ) {
                    return false;
                }
                return !hit.collider.gameObject.GetComponent<BaseEnemyShip>();
            }
        }

        void Update() {
            UpdateWeaponControlState();
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            State = EnemyState.Patrolling;
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
            MovementController.Init(ShipSpeed, ShipAccel, x => x.GetComponentInParent<PlayerShip>() ? 100 : -1, true);
            if ( MovementController.FallbackMovementController ) {
                TryInitFallbackMovementController(MovementController.FallbackMovementController);
            }
            MovementController.OnCurChaseTargetChanged += OnChaseTargetChanged;
            OnChaseTargetChanged(MovementController.CurChaseTarget);
            UpdateWeaponControlState();
        }

        void TryInitFallbackMovementController(BaseAiShipMovementController fallbackMovementController) {
            if ( !fallbackMovementController ) {
                Debug.LogError("Fallback movement controller is null", this);
                return;
            }
            switch ( fallbackMovementController ) {
                case TransformPatrolMovementController patrolMovementController: {
                    patrolMovementController.Init(ShipSpeed, ShipAccel);
                    break;
                }
                case VectorPatrolMovementController patrolMovementController: {
                    patrolMovementController.Init(ShipSpeed, ShipAccel);
                    break;
                }
                default: {
                    Debug.LogErrorFormat(this, "Unsupported ai ship movement controller type '{0}'",
                        fallbackMovementController.GetType().Name);
                    break;
                }
            }
        }

        void OnChaseTargetChanged(Transform chaseTarget) {
            State = chaseTarget ? EnemyState.Chase : EnemyState.Patrolling;
        }

        void OnDrawGizmos() {
            Gizmos.DrawRay(transform.position, transform.rotation * Vector3.up * 10000);
        }
    }
}