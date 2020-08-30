using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public abstract class BaseAiShipMovementController : GameComponent {
        [NotNull]
        public Rigidbody2D Rigidbody;
        public Transform   OverrideMoveRoot;

        protected float Speed;

        public virtual bool IsActive { get; set; }

        protected bool IsCommonInit { get; private set; }

        protected virtual bool CanMove => (IsCommonInit && IsActive);

        protected Transform MoveRoot => OverrideMoveRoot ? OverrideMoveRoot : transform;

        protected virtual void Reset() {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        protected void CommonInit(float speed) {
            if ( IsCommonInit ) {
                Debug.LogWarningFormat(this, "Movement controller is already common init");
                return;
            }
            Speed = speed;

            IsCommonInit = true;
        }

        protected void SetVelocityInDirection(Vector2 direction) {
            direction = direction.normalized;
            MoveUtils.ApplyMovingVector(Rigidbody, Speed * direction);
        }

        protected void Stop() {
            Rigidbody.velocity = Vector2.zero;
        }

        protected void SetViewRotation(Vector2 viewDirection) {
            MoveUtils.ApplyViewVector(MoveRoot, viewDirection);
        }
    }
}
