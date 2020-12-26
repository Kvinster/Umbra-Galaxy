using UnityEngine;

namespace STP.Utils {
    public static class MathUtils {
        public static float GetSmoothRotationAngleOffset(Vector2 up, Vector2 viewVector, float rotationSpeed) {
            var angleDiff = Angle360(up, viewVector);
            return LerpFloat(0, angleDiff, rotationSpeed);
        }
        
        public static float LerpFloat(float a, float b, float coeff) {
            coeff = Mathf.Clamp01(coeff);
            return (b - a) * coeff + a;
        }

        public static float Angle360(Vector2 one, Vector2 other) {
            var res = Vector2.SignedAngle(one, other);
            if ( res < 0f ) {
                res += 360f;
            }
            return res;
        }
    }
}