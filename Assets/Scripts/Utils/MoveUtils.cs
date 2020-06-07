using UnityEngine;

namespace STP.Utils {
    public class MoveUtils {
        public static void ApplyMovingVector(Rigidbody2D rigidbody2D, Transform transform, Vector2 movingVector) {
            var movingVectorN    = movingVector.normalized;
            var dstAngle         = Mathf.Atan2(-movingVectorN.x, movingVectorN.y) / Mathf.PI * 180;
            transform.rotation   = Quaternion.AngleAxis(dstAngle, Vector3.forward);
            rigidbody2D.velocity = movingVector;
        }
    }
}