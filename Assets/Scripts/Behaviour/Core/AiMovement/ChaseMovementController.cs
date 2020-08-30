using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public sealed class ChaseMovementController : BaseAiShipMovementController {
        public float ChaseToleranceDistance;
        [Space]
        [NotNull] public TriggerNotifier InnerTriggerNotifier;
        [NotNull] public TriggerNotifier OuterTriggerNotifier;
        [Space]
        public BaseAiShipMovementController FallbackMovementController;
        [Space]
        public Transform InitFallbackChaseTarget;

        Transform _chaseTarget;
        Transform _fallbackChaseTarget;

        int _curTargetPriority;

        Func<GameObject, int> _targetRecognition;

        readonly Dictionary<Transform, int> _potentialTargets = new Dictionary<Transform, int>();

        public Transform CurChaseTarget => (CanChase ? (ChaseTarget ? ChaseTarget : FallbackChaseTarget) : null);

        public Transform FallbackChaseTarget {
            get => _fallbackChaseTarget;
            set {
                _fallbackChaseTarget = value;
                OnChaseTargetsUpdate();
            }
        }

        Transform ChaseTarget {
            get => _chaseTarget;
            set {
                _chaseTarget = value;
                OnChaseTargetsUpdate();
            }
        }

        bool CanChase => ChaseTarget || FallbackChaseTarget;

        float TargetDistance =>
            CurChaseTarget ? Vector2.Distance(MoveRoot.position, CurChaseTarget.position) : float.MaxValue;

        public event Action<Transform> OnCurChaseTargetChanged;

        protected override void CheckDescription() {
            if ( ChaseToleranceDistance <= 0f ) {
                Debug.LogError("ChaseMovementController: ChaseToleranceDistance must be more than zero", this);
            }
        }

        protected override void Reset() {
            base.Reset();
            foreach ( var movementController in GetComponents<BaseAiShipMovementController>() ) {
                if ( movementController != this ) {
                    FallbackMovementController = movementController;
                    break;
                }
            }
        }

        void Update() {
            if ( !CanMove ) {
                return;
            }
            if ( CanChase ) {
                var chaseTarget      = CurChaseTarget;
                var chasingVector    = (Vector2)(chaseTarget.transform.position - MoveRoot.position);
                var chasingDirection = chasingVector.normalized;
                SetVelocityInDirection((TargetDistance <= ChaseToleranceDistance) ? Vector2.zero : chasingDirection);
                SetViewRotation(chasingDirection);
            } else {
                if ( FallbackMovementController ) {
                    FallbackMovementController.IsActive = true;
                }
            }
        }

        void OnDestroy() {
            InnerTriggerNotifier.OnTriggerEnter -= OnInnerTriggerEnter;
            OuterTriggerNotifier.OnTriggerExit  -= OnOuterTriggerExit;
        }

        /// <summary>
        /// ChaseMovementController initialization
        /// </summary>
        /// <param name="maxSpeed">Max speed</param>
        /// <param name="targetRecognition">"Potential target => priority" function. Must return -1 to ignore potential targets.</param>
        /// <param name="activeOnInit">Specifies whether or not the movement controller must become active after the Init is called</param>
        public void Init(float maxSpeed, Func<GameObject, int> targetRecognition, bool activeOnInit = false) {
            CommonInit(maxSpeed);

            _targetRecognition = targetRecognition;

            if ( InitFallbackChaseTarget ) {
                FallbackChaseTarget = InitFallbackChaseTarget;
            }

            InnerTriggerNotifier.Exceptions.Add(gameObject);
            OuterTriggerNotifier.Exceptions.Add(gameObject);
            InnerTriggerNotifier.OnTriggerEnter += OnInnerTriggerEnter;
            OuterTriggerNotifier.OnTriggerExit  += OnOuterTriggerExit;

            IsActive = activeOnInit;
            if ( IsActive ) {
                OnChaseTargetsUpdate();
            }
        }

        void OnChaseTargetsUpdate() {
            if ( !IsActive ) {
                return;
            }
            if ( ChaseTarget || FallbackChaseTarget ) {
                OnChaseTargetAppear();
            } else {
                OnChaseTargetDisappear();
            }
            OnCurChaseTargetChanged?.Invoke(CurChaseTarget);
        }

        void OnChaseTargetAppear() {
            if ( FallbackMovementController ) {
                FallbackMovementController.IsActive = false;
            }
        }

        void OnChaseTargetDisappear() {
            if ( FallbackMovementController ) {
                FallbackMovementController.IsActive = true;
            } else {
                Stop();
            }
        }

        void OnInnerTriggerEnter(GameObject other) {
            var targetPriority = _targetRecognition.Invoke(other.gameObject);
            if ( targetPriority == -1 ) {
                return;
            }
            if ( !ChaseTarget || (targetPriority > _curTargetPriority) ) {
                if ( ChaseTarget ) {
                    _potentialTargets.Add(ChaseTarget, _curTargetPriority);
                }
                _curTargetPriority = targetPriority;
                ChaseTarget        = other.transform;
            } else if ( !_potentialTargets.ContainsKey(other.transform) ) {
                _potentialTargets.Add(other.transform, targetPriority);
            }
        }

        void OnOuterTriggerExit(GameObject other) {
            if ( _potentialTargets.ContainsKey(other.transform) ) {
                _potentialTargets.Remove(other.transform);
            }
            if ( !ChaseTarget || (other.transform != ChaseTarget) ) {
                return;
            }
            var       minPriority = int.MaxValue;
            Transform minTarget   = null;
            foreach ( var pair in _potentialTargets ) {
                var priority = pair.Value;
                if ( !minTarget || (priority < minPriority) ||
                     ((priority == minPriority) && (Vector2.Distance(MoveRoot.position, pair.Key.position) <
                                                    Vector2.Distance(MoveRoot.position, minTarget.position))) ) {
                    minTarget   = pair.Key;
                    minPriority = priority;
                }
            }
            if ( minTarget ) {
                _curTargetPriority = minPriority;
                ChaseTarget        = minTarget;
                _potentialTargets.Remove(minTarget);
            } else {
                _curTargetPriority = -1;
                ChaseTarget        = null;
            }
        }
    }
}
