﻿using UnityEngine;

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
        const float ShipSpeed       = 150f;
        const int   Hp              = 2;

        public ChaseMovementController MovementController;

        EnemyState State { get; set; } = EnemyState.None;

        public override bool CanShoot => (State == EnemyState.Chase);

        void Update() {
            UpdateWeaponControlState();
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            State = EnemyState.Patrolling;
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
            MovementController.Init(ShipSpeed, x => x.GetComponentInParent<PlayerShip>() ? 100 : -1, true);
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
                case PatrolMovementController patrolMovementController: {
                    patrolMovementController.Init(ShipSpeed);
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
    }
}