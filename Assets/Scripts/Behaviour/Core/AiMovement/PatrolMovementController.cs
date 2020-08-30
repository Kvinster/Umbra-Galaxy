using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Utils.GameComponentAttributes;

using NaughtyAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public sealed class PatrolMovementController : BaseAiShipMovementController {
        public float ApproachTolerance = 10f;
        public bool  IsCycledRoute;
        [NotNullOrEmpty(checkPrefab: false)]
        public List<Transform> PatrolRoute = new List<Transform>();
        [Space]
        public bool SnapOnInit;
        [ShowIf("SnapOnInit")]
        public int  StartRoutePointIndex;

        int _nextRoutePointIndex;

        protected override bool CanMove =>
            (base.CanMove && (_nextRoutePointIndex >= 0) && (_nextRoutePointIndex < PatrolRoute.Count));

        Vector3 NextPoint      => PatrolRoute[_nextRoutePointIndex].position;
        Vector2 MoveVector     => (NextPoint - MoveRoot.position);
        Vector2 MoveDirection  => MoveVector.normalized;
        bool    IsCloseToPoint => (MoveVector.magnitude < ApproachTolerance);

        public event Action OnFinishedPatrol;

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
                if ( StartRoutePointIndex >= PatrolRoute.Count ) {
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
            SetVelocityInDirection(MoveDirection);
            SetViewRotation(MoveDirection);
            if ( IsCloseToPoint ) {
                _nextRoutePointIndex = (_nextRoutePointIndex + 1) % PatrolRoute.Count;
                if ( !IsCycledRoute && (_nextRoutePointIndex == 0) ) {
                    Rigidbody.velocity = Vector2.zero;
                    _nextRoutePointIndex = -1;
                    OnFinishedPatrol?.Invoke();
                }
            }
        }

        public void Init(float maxSpeed, bool activeOnInit = false) {
            CommonInit(maxSpeed);
            if ( SnapOnInit ) {
                MoveRoot.position    = PatrolRoute[StartRoutePointIndex].position;
                _nextRoutePointIndex = (StartRoutePointIndex + 1) % PatrolRoute.Count;
            }
            IsActive = activeOnInit;
        }

        void OnDrawGizmos() {
            if ( PatrolRoute.Any(point => !point) ) {
                return;
            }
            // TODO: use different colors based on GetInstanceID()
            for ( var i = 0; i < PatrolRoute.Count; i++ ) {
                var point  = PatrolRoute[i];
                var point2 = PatrolRoute[(i + 1) % PatrolRoute.Count];
                Gizmos.DrawWireSphere(point.position, 10f);
                if ( !IsCycledRoute && (i == (PatrolRoute.Count - 1)) ) {
                    break;
                }
                Gizmos.DrawLine(point.position, point2.position);
            }
        }
    }
}
