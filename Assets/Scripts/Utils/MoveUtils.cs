using UnityEngine;

namespace STP.Utils {
    public static class MoveUtils {
        public static void ApplyMovingVector(Rigidbody2D rigidbody2D, Vector2 movingVector, float maxSpeed, float maxAccel) {
            var accelVector = maxAccel * movingVector;
            var exceededSpeed = rigidbody2D.velocity.magnitude - maxSpeed;
            if (exceededSpeed > 0) {
                var reverseVector = -rigidbody2D.velocity.normalized * exceededSpeed;
                rigidbody2D.AddForce(reverseVector, ForceMode2D.Impulse);
            }
            rigidbody2D.AddForce(accelVector, ForceMode2D.Impulse);
        }

        public static void ApplyViewVector(Rigidbody2D rigidbody2D, Vector2 viewDirection) {
            var viewDirectionN   = viewDirection.normalized;
            var dstAngle         = Mathf.Atan2(-viewDirectionN.x, viewDirectionN.y) / Mathf.PI * 180;
            rigidbody2D.rotation = dstAngle;
        }
    }
}