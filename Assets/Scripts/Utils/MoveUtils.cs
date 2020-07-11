using UnityEngine;

namespace STP.Utils {
    public class MoveUtils {
        public static void ApplyMovingVector(Rigidbody2D rigidbody2D,Vector2 movingVector) {
            rigidbody2D.velocity = movingVector;
        }

        public static void ApplyViewVector(Transform transform, Vector2 viewDirection) {
            var viewDirectionN   = viewDirection.normalized;
            var dstAngle         = Mathf.Atan2(-viewDirectionN.x, viewDirectionN.y) / Mathf.PI * 180;
            transform.rotation   = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(dstAngle, Vector3.forward), 0.5f);
        }
    }
}