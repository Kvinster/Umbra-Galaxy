using UnityEngine;

using System;

using NaughtyAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public abstract class BasePatrolMovementController : BaseAiShipMovementController {
        public float ApproachTolerance = 10f;
        public bool  IsCycledRoute;

        [Space]
        public bool SnapOnInit;
        [ShowIf("SnapOnInit")]
        public int  StartRoutePointIndex;

        int _nextRoutePointIndex;

        protected override bool CanMove =>
            (base.CanMove && (_nextRoutePointIndex >= 0) && (_nextRoutePointIndex < PointsCount));

        Vector3 NextPoint      => GetPoint(_nextRoutePointIndex);
        Vector2 MoveVector     => (NextPoint - MoveRoot.position);
        Vector2 MoveDirection  => MoveVector.normalized;
        bool    IsCloseToPoint => (MoveVector.magnitude < ApproachTolerance);

        public event Action OnFinishedPatrol;

        protected abstract int PointsCount { get; }

        protected abstract bool CanDrawDizmo();
        protected abstract Vector2 GetPoint(int index);

        protected override void CheckDescription() {
            base.CheckDescription();
            if ( ApproachTolerance <= 0f ) {
                Debug.LogError("PatrolMovementController: ApproachTolerance must be more than zero", this);
            }
            var isPrefab = false;
            #if UNITY_EDITOR
            isPrefab = string.IsNullOrEmpty(gameObject.scene.name) ||
                       (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null);
            #endif
            if ( !isPrefab && SnapOnInit ) {
                if ( StartRoutePointIndex < 0 ) {
                    Debug.LogError("PatrolMovementController: StartRoutePointIndex mustn't be negative", this);
                }
                if ( StartRoutePointIndex >= PointsCount ) {
                    Debug.LogError("PatrolMovementController: StartRoutePointIndex must be less than PatrolRoute.Count",
                        this);
                }
            }
        }

        void Update() {
            if ( !CanMove ) {
                if ( IsActive ) {
                    Stop();
                }
                return;
            }
            Move(MoveDirection);
            SetViewRotation(MoveDirection);
            if ( IsCloseToPoint ) {
                _nextRoutePointIndex = (_nextRoutePointIndex + 1) % PointsCount;
                if ( !IsCycledRoute && (_nextRoutePointIndex == 0) ) {
                    Rigidbody.velocity = Vector2.zero;
                    _nextRoutePointIndex = -1;
                    OnFinishedPatrol?.Invoke();
                }
            }
        }

        public void Init(float maxSpeed, float maxAccel, bool activeOnInit = false) {
            CommonInit(maxSpeed, maxAccel);
            if ( SnapOnInit ) {
                MoveRoot.position    = GetPoint(StartRoutePointIndex);
                _nextRoutePointIndex = (StartRoutePointIndex + 1) % PointsCount;
            }
            IsActive = activeOnInit;
        }

        void OnDrawGizmos() {
            if ( !CanDrawDizmo() ) {
                return;
            }
            // TODO: use different colors based on GetInstanceID()
            for ( var i = 0; i < PointsCount; i++ ) {
                var point  = GetPoint(i);
                var point2 = GetPoint((i + 1) % PointsCount);
                Gizmos.DrawWireSphere(point, 10f);
                if ( !IsCycledRoute && (i == (PointsCount - 1)) ) {
                    break;
                }
                Gizmos.DrawLine(point, point2);
            }
        }
    }
}
